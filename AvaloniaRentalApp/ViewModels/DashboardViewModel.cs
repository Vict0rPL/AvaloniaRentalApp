using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;
    private readonly PricingService _pricing = new();
    private readonly User _currentUser;

    // Number of days ahead within which an expiring document raises an alert
    private const int ExpiryWarningDays = 30;

    public string Greeting => $"Dzień dobry, {_currentUser.FullName}!";

    public ObservableCollection<Rental> RecentRentals { get; } = new();
    public ObservableCollection<AlertItem> Alerts { get; } = new();
    public ObservableCollection<FleetStatusRow> FleetStatus { get; } = new();

    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    public DashboardViewModel(User currentUser)
    {
        _currentUser = currentUser;
        _databaseService = new DatabaseService();

        RefreshCommand = ReactiveCommand.CreateFromTask(Initialize);

        _ = Initialize();
    }

    private int _availableCars;
    public int AvailableCars
    {
        get => _availableCars;
        set => this.RaiseAndSetIfChanged(ref _availableCars, value);
    }

    private int _totalCars;
    public int TotalCars
    {
        get => _totalCars;
        set => this.RaiseAndSetIfChanged(ref _totalCars, value);
    }

    private int _activeRentals;
    public int ActiveRentals
    {
        get => _activeRentals;
        set => this.RaiseAndSetIfChanged(ref _activeRentals, value);
    }

    private decimal _monthRevenue;
    public decimal MonthRevenue   // gross paid income this month
    {
        get => _monthRevenue;
        set => this.RaiseAndSetIfChanged(ref _monthRevenue, value);
    }

    private decimal _monthNetRevenue;
    public decimal MonthNetRevenue   // paid income − fuel − maintenance this month
    {
        get => _monthNetRevenue;
        set => this.RaiseAndSetIfChanged(ref _monthNetRevenue, value);
    }

    private int _customersCount;
    public int CustomersCount
    {
        get => _customersCount;
        set => this.RaiseAndSetIfChanged(ref _customersCount, value);
    }

    private bool _hasAlerts;
    public bool HasAlerts
    {
        get => _hasAlerts;
        set => this.RaiseAndSetIfChanged(ref _hasAlerts, value);
    }

    private async Task Initialize()
    {
        var cars = await _databaseService.GetCarsAsync();
        var rentals = await _databaseService.GetRentalsAsync();
        var customers = await _databaseService.GetCustomersAsync();
        var fleetStats = await _databaseService.GetFleetStatsAsync();
        var fuelPrices = await _databaseService.GetFuelPricesAsync();
        var maintenance = await _databaseService.GetAllMaintenanceAsync();

        // Stat cards
        TotalCars = cars.Count(c => c.IsActive);
        AvailableCars = cars.Count(c => c.IsActive && c.Status == "dostepny");
        ActiveRentals = rentals.Count(r => r.Status == "aktywna");

        var now = DateTime.Now;

        // Revenue: paid rentals only, current month. Net subtracts fuel + maintenance.
        var paidThisMonth = rentals
            .Where(r => r.PaymentStatus == "oplacona"
                        && r.DateStart.Month == now.Month && r.DateStart.Year == now.Year)
            .ToList();
        var maintenanceThisMonth = maintenance
            .Where(m => m.Date.Month == now.Month && m.Date.Year == now.Year)
            .ToList();
        var priceMap = PricingService.BuildPriceMap(fuelPrices);

        MonthRevenue = paidThisMonth.Sum(r => r.TotalCost);
        MonthNetRevenue = _pricing.NetRevenue(paidThisMonth, maintenanceThisMonth, priceMap);

        CustomersCount = customers.Count(c => c.IsActive);

        // Recent rentals (newest first)
        RecentRentals.Clear();
        foreach (var rental in rentals.OrderByDescending(r => r.DateStart).Take(5))
            RecentRentals.Add(rental);

        // Fleet status by category
        FleetStatus.Clear();
        foreach (var row in fleetStats)
            FleetStatus.Add(row);

        BuildAlerts(cars);
        HasAlerts = Alerts.Count > 0;
    }

    private void BuildAlerts(System.Collections.Generic.List<Car> cars)
    {
        Alerts.Clear();
        var today = DateTime.Now.Date;

        foreach (var car in cars.Where(c => c.IsActive))
        {
            if (car.Status == "serwis")
            {
                Alerts.Add(new AlertItem
                {
                    Type = "warning",
                    Title = $"{car.FullName} ({car.Registration})",
                    Message = "Pojazd w serwisie",
                    Time = ""
                });
            }

            if (car.InsuranceExpiry is { } ins)
            {
                var days = (ins.Date - today).Days;
                if (days >= 0 && days <= ExpiryWarningDays)
                    Alerts.Add(new AlertItem
                    {
                        Type = "danger",
                        Title = $"Polisa OC — {car.FullName}",
                        Message = $"Ubezpieczenie wygasa za {days} dni",
                        Time = car.Registration
                    });
            }

            if (car.InspectionExpiry is { } insp)
            {
                var days = (insp.Date - today).Days;
                if (days >= 0 && days <= ExpiryWarningDays)
                    Alerts.Add(new AlertItem
                    {
                        Type = "danger",
                        Title = $"Przegląd — {car.FullName}",
                        Message = $"Przegląd techniczny wygasa za {days} dni",
                        Time = car.Registration
                    });
            }
        }
    }
}

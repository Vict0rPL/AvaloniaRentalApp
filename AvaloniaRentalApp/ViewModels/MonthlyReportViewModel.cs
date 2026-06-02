using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

/// <summary>Monthly report: rentals, revenue and fleet statistics for the selected month.</summary>
public class MonthlyReportViewModel : PeriodReportViewModel
{
    private readonly ReportService _reports = new();

    public ObservableCollection<CountAmountRow> ByStatus { get; } = new();
    public ObservableCollection<CountAmountRow> ByCategory { get; } = new();

    private int _rentalCount;
    public int RentalCount
    {
        get => _rentalCount;
        set => this.RaiseAndSetIfChanged(ref _rentalCount, value);
    }

    private int _completedReturns;
    public int CompletedReturns
    {
        get => _completedReturns;
        set => this.RaiseAndSetIfChanged(ref _completedReturns, value);
    }

    private int _newCustomers;
    public int NewCustomers
    {
        get => _newCustomers;
        set => this.RaiseAndSetIfChanged(ref _newCustomers, value);
    }

    private decimal _grossRevenue;
    public decimal GrossRevenue
    {
        get => _grossRevenue;
        set => this.RaiseAndSetIfChanged(ref _grossRevenue, value);
    }

    private decimal _netRevenue;
    public decimal NetRevenue
    {
        get => _netRevenue;
        set => this.RaiseAndSetIfChanged(ref _netRevenue, value);
    }

    private decimal _fuelCost;
    public decimal FuelCost
    {
        get => _fuelCost;
        set => this.RaiseAndSetIfChanged(ref _fuelCost, value);
    }

    private decimal _maintenanceCost;
    public decimal MaintenanceCost
    {
        get => _maintenanceCost;
        set => this.RaiseAndSetIfChanged(ref _maintenanceCost, value);
    }

    protected override async Task LoadAsync()
    {
        var rentals = await Db.GetRentalsAsync();
        var maintenance = await Db.GetAllMaintenanceAsync();
        var fuelPrices = await Db.GetFuelPricesAsync();
        var customers = await Db.GetCustomersAsync();
        var cars = await Db.GetCarsAsync();

        var r = _reports.MonthlySummary(rentals, maintenance, fuelPrices, customers, cars, Year, Month);

        RentalCount = r.RentalCount;
        CompletedReturns = r.CompletedReturns;
        NewCustomers = r.NewCustomers;
        GrossRevenue = r.Revenue.Gross;
        NetRevenue = r.Revenue.Net;
        FuelCost = r.Revenue.Fuel;
        MaintenanceCost = r.Revenue.Maintenance;

        ByStatus.Clear();
        foreach (var row in r.ByStatus)
            ByStatus.Add(row);

        ByCategory.Clear();
        foreach (var row in r.ByCategory)
            ByCategory.Add(row);
    }
}

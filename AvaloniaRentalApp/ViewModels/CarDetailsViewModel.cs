using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class CarDetailsViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;

    public Car Car { get; }

    public ObservableCollection<Rental> Rentals { get; } = new();
    public ObservableCollection<MaintenanceRecord> Maintenance { get; } = new();

    public Interaction<MaintenanceEditViewModel, MaintenanceRecord?> ShowMaintenanceDialog { get; } = new();

    private bool _isLoadingRentals;
    public bool IsLoadingRentals
    {
        get => _isLoadingRentals;
        set => this.RaiseAndSetIfChanged(ref _isLoadingRentals, value);
    }

    private decimal _maintenanceTotal;
    public decimal MaintenanceTotal
    {
        get => _maintenanceTotal;
        set => this.RaiseAndSetIfChanged(ref _maintenanceTotal, value);
    }

    public ReactiveCommand<Unit, Unit> AddMaintenanceCommand { get; }

    public string InsuranceBadge => GetExpiryBadge(Car.InsuranceExpiry);
    public string InsuranceStatusText => GetExpiryStatusText(Car.InsuranceExpiry);
    public string InspectionBadge => GetExpiryBadge(Car.InspectionExpiry);
    public string InspectionStatusText => GetExpiryStatusText(Car.InspectionExpiry);

    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    public CarDetailsViewModel(Car car)
    {
        Car = car;
        _databaseService = new DatabaseService();
        CloseCommand = ReactiveCommand.Create(() => { });
        AddMaintenanceCommand = ReactiveCommand.CreateFromTask(AddMaintenanceAsync);

        _ = LoadRentalsAsync();
        _ = LoadMaintenanceAsync();
    }

    private async Task LoadMaintenanceAsync()
    {
        Maintenance.Clear();
        var records = await _databaseService.GetMaintenanceByCarIdAsync(Car.CarId);
        foreach (var r in records)
            Maintenance.Add(r);
        MaintenanceTotal = Maintenance.Sum(m => m.Cost);
    }

    private async Task AddMaintenanceAsync()
    {
        var vm = new MaintenanceEditViewModel(Car.CarId);
        var result = await ShowMaintenanceDialog.Handle(vm);
        if (result != null)
            await LoadMaintenanceAsync();
    }

    private async Task LoadRentalsAsync()
    {
        IsLoadingRentals = true;
        try
        {
            var rentals = await _databaseService.GetRentalsByCarIdAsync(Car.CarId);
            foreach (var r in rentals)
            {
                Rentals.Add(r);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading car rentals: {ex.Message}");
        }
        finally
        {
            IsLoadingRentals = false;
        }
    }

    private static string GetExpiryBadge(DateTime? expiry)
    {
        if (expiry == null) return "red";
        var daysLeft = (expiry.Value - DateTime.Today).Days;
        if (daysLeft < 0) return "red";
        if (daysLeft <= 30) return "orange";
        return "green";
    }

    private static string GetExpiryStatusText(DateTime? expiry)
    {
        if (expiry == null) return "Brak danych";
        var daysLeft = (expiry.Value - DateTime.Today).Days;
        if (daysLeft < 0) return "Wygasło";
        if (daysLeft <= 30) return $"Wygasa za {daysLeft} dni";
        return "Ważne";
    }
}

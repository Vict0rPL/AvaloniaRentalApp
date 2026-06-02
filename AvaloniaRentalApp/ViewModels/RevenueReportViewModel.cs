using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

/// <summary>Revenue Raport: revenue, payment methods, arrears and a 6-month trend.</summary>
public class RevenueReportViewModel : PeriodReportViewModel
{
    private readonly ReportService _reports = new();

    public ObservableCollection<PaymentMethodRow> ByMethod { get; } = new();
    public ObservableCollection<ArrearsRow> Arrears { get; } = new();
    public ObservableCollection<CountAmountRow> ByCategory { get; } = new();
    public ObservableCollection<CountAmountRow> ByEmployee { get; } = new();
    public ObservableCollection<RevenueTrendPoint> Trend { get; } = new();

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

    private decimal _arrearsTotal;
    public decimal ArrearsTotal
    {
        get => _arrearsTotal;
        set => this.RaiseAndSetIfChanged(ref _arrearsTotal, value);
    }

    private bool _hasArrears;
    public bool HasArrears
    {
        get => _hasArrears;
        set => this.RaiseAndSetIfChanged(ref _hasArrears, value);
    }

    protected override async Task LoadAsync()
    {
        var rentals = await Db.GetRentalsAsync();
        var maintenance = await Db.GetAllMaintenanceAsync();
        var fuelPrices = await Db.GetFuelPricesAsync();
        var cars = await Db.GetCarsAsync();

        var r = _reports.RevenueReport(rentals, maintenance, fuelPrices, cars, Year, Month);

        GrossRevenue = r.Revenue.Gross;
        NetRevenue = r.Revenue.Net;
        FuelCost = r.Revenue.Fuel;
        MaintenanceCost = r.Revenue.Maintenance;
        ArrearsTotal = r.ArrearsTotal;

        ByMethod.Clear();
        foreach (var row in r.ByMethod)
            ByMethod.Add(row);

        Arrears.Clear();
        foreach (var row in r.Arrears)
            Arrears.Add(row);
        HasArrears = Arrears.Count > 0;

        ByCategory.Clear();
        foreach (var row in r.ByCategory)
            ByCategory.Add(row);

        ByEmployee.Clear();
        foreach (var row in r.ByEmployee)
            ByEmployee.Add(row);

        Trend.Clear();
        foreach (var row in r.Trend)
            Trend.Add(row);
    }
}

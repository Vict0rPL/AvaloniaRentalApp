using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

/// <summary>Fleet utilization: vehicle occupancy, idle time and most popular categories.</summary>
public class FleetUtilizationReportViewModel : PeriodReportViewModel
{
    private readonly ReportService _reports = new();

    /// <summary>Shown as a caption: utilization is estimated, not exact (no status history is stored).</summary>
    public string EstimateNote =>
        "Obłożenie i postój szacowane są na podstawie terminów wypożyczeń (brak historii statusów pojazdów).";

    public ObservableCollection<FleetUtilizationRow> Cars { get; } = new();
    public ObservableCollection<PopularCategoryRow> PopularCategories { get; } = new();

    private int _periodDays;
    public int PeriodDays
    {
        get => _periodDays;
        set => this.RaiseAndSetIfChanged(ref _periodDays, value);
    }

    private double _averageUtilization;
    public double AverageUtilization
    {
        get => _averageUtilization;
        set => this.RaiseAndSetIfChanged(ref _averageUtilization, value);
    }

    protected override async Task LoadAsync()
    {
        var cars = await Db.GetCarsAsync();
        var rentals = await Db.GetRentalsAsync();
        var maintenance = await Db.GetAllMaintenanceAsync();

        var r = _reports.FleetUtilization(cars, rentals, maintenance, Year, Month);

        PeriodDays = r.PeriodDays;
        AverageUtilization = r.AverageUtilization;

        Cars.Clear();
        foreach (var row in r.Cars)
            Cars.Add(row);

        PopularCategories.Clear();
        foreach (var row in r.PopularCategories)
            PopularCategories.Add(row);
    }
}

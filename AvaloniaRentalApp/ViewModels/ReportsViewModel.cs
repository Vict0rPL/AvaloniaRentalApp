using System.Reactive;
using AvaloniaRentalApp.Models;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

/// <summary>
/// Reports landing page. Shows the three report cards; opening one swaps <see cref="CurrentReport"/>
/// </summary>
public class ReportsViewModel : ViewModelBase
{
    private readonly User _currentUser;

    public ReportsViewModel(User currentUser)
    {
        _currentUser = currentUser;

        OpenMonthlyCommand = ReactiveCommand.Create(() => CurrentReport = new MonthlyReportViewModel());
        OpenFleetCommand   = ReactiveCommand.Create(() => CurrentReport = new FleetUtilizationReportViewModel());
        OpenRevenueCommand = ReactiveCommand.Create(() => CurrentReport = new RevenueReportViewModel());
        BackCommand        = ReactiveCommand.Create(() => CurrentReport = null);
    }

    private ViewModelBase? _currentReport;
    public ViewModelBase? CurrentReport
    {
        get => _currentReport;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentReport, value);
            this.RaisePropertyChanged(nameof(IsLanding));
        }
    }

    /// <summary>True while the landing grid is shown (no report opened).</summary>
    public bool IsLanding => _currentReport is null;

    public ReactiveCommand<Unit, ViewModelBase> OpenMonthlyCommand { get; }
    public ReactiveCommand<Unit, ViewModelBase> OpenFleetCommand   { get; }
    public ReactiveCommand<Unit, ViewModelBase> OpenRevenueCommand { get; }
    public ReactiveCommand<Unit, ViewModelBase?> BackCommand       { get; }
}

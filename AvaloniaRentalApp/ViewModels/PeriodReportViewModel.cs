using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

/// <summary>A selectable month in the period picker.</summary>
public record MonthItem(int Number, string Name)
{
    public override string ToString() => Name;
}

/// <summary>
/// Base for the month-scoped report view models
/// </summary>
public abstract class PeriodReportViewModel : ViewModelBase
{
    protected readonly DatabaseService Db = new();

    public ObservableCollection<int> Years { get; } = new();
    public ObservableCollection<MonthItem> Months { get; } = new();

    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    protected PeriodReportViewModel()
    {
        var now = DateTime.Now;
        for (var y = now.Year - 4; y <= now.Year; y++)
            Years.Add(y);
        for (var m = 1; m <= 12; m++)
            Months.Add(new MonthItem(m, ReportService.MonthNames[m]));

        _selectedYear = now.Year;
        _selectedMonth = Months[now.Month - 1];

        RefreshCommand = ReactiveCommand.CreateFromTask(LoadAsync);

        // Reload when the period changes
        this.WhenAnyValue(x => x.SelectedYear, x => x.SelectedMonth, (y, m) => (y, m))
            .Skip(1)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(period => { _ = LoadAsync(); });

        _ = LoadAsync();
    }

    private int _selectedYear;
    public int SelectedYear
    {
        get => _selectedYear;
        set => this.RaiseAndSetIfChanged(ref _selectedYear, value);
    }

    private MonthItem _selectedMonth;
    public MonthItem SelectedMonth
    {
        get => _selectedMonth;
        set => this.RaiseAndSetIfChanged(ref _selectedMonth, value);
    }

    protected int Month => SelectedMonth.Number;
    protected int Year => SelectedYear;

    protected abstract Task LoadAsync();
}

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace AvaloniaRentalApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {

        NavigateDatabaseCommand = ReactiveCommand.Create(() => NavigateTo("Database"));
        NavigateFleetCommand = ReactiveCommand.Create(() => NavigateTo("Fleet"));

        NavigateTo("Database");
    }

    private ViewModelBase _currentView = null!;
    public ViewModelBase CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    private string _currentSection = string.Empty;
    public string CurrentSection
    {
        get => _currentSection;
        set => this.RaiseAndSetIfChanged(ref _currentSection, value);
    }


    public ReactiveCommand<Unit, Unit> NavigateDatabaseCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateFleetCommand { get; }

    private void NavigateTo(string section)
    {
        CurrentSection = section;
        CurrentView = section switch
        {
            "Database" => new DatabaseViewModel(),
            "Fleet" => new FleetViewModel(),
            _ => new DatabaseViewModel()
        };
    }
}

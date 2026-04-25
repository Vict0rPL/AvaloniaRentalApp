
using AvaloniaRentalApp.Models;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;
using System.Reactive;

namespace AvaloniaRentalApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        NavigateDatabaseCommand = ReactiveCommand.Create(() => NavigateTo("Database"));
        NavigateCustomersCommand = ReactiveCommand.Create(() => NavigateTo("Customers"));
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
    public ReactiveCommand<Unit, Unit> NavigateCustomersCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateFleetCommand { get; }

    private void NavigateTo(string section)
    {
        CurrentSection = section;
        CurrentView = section switch
        {
            "Database"  => new DatabaseViewModel(),
            "Customers" => new CustomersViewModel(GetCurrentUser()),
            "Fleet" => new FleetViewModel(),
            _ => new DatabaseViewModel()
        };
    }

    // for now , we return a placeholder user until we implement authentication
    private static User GetCurrentUser() => new()
    {
        UserId   = 0,
        Username = "placeholder",
        FullName = "Placeholder User",
        Role     = "admin",
        IsActive = true
    };
}

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
        NavigateRentalsCommand = ReactiveCommand.Create(() => NavigateTo("Rentals"));

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
    public ReactiveCommand<Unit, Unit> NavigateRentalsCommand { get; }

    private void NavigateTo(string section)
    {
        CurrentSection = section;
        CurrentView = section switch
        {
            "Database"  => new DatabaseViewModel(),
            "Customers" => new CustomersViewModel(GetCurrentUser()),
            "Fleet"     => new FleetViewModel(),
            "Rentals"   => new RentalsViewModel(GetCurrentUser()),
            _ => new DatabaseViewModel()
        };
    }

    // placeholder until authentication is implemented; UserId=1 matches seed data admin user
    private static User GetCurrentUser() => new()
    {
        UserId   = 1,
        Username = "admin",
        FullName = "Administrator Systemu",
        Role     = "admin",
        IsActive = true
    };
}

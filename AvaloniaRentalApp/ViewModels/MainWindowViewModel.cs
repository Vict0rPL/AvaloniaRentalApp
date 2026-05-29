using System;
using System.Reactive;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Action _onLogout;

    public User CurrentUser { get; }
    public string UserInitials    => CurrentUser.Initials;
    public string UserFullName    => CurrentUser.FullName;
    public string UserRoleDisplay => CurrentUser.RoleDisplay;
    public bool   IsAdmin         => CurrentUser.IsAdmin;

    public MainWindowViewModel(User currentUser, Action onLogout)
    {
        CurrentUser = currentUser;
        _onLogout   = onLogout;

        NavigateDatabaseCommand  = ReactiveCommand.Create(() => NavigateTo("Database"));
        NavigateCustomersCommand = ReactiveCommand.Create(() => NavigateTo("Customers"));
        NavigateFleetCommand     = ReactiveCommand.Create(() => NavigateTo("Fleet"));
        NavigateRentalsCommand   = ReactiveCommand.Create(() => NavigateTo("Rentals"));
        NavigateSettingsCommand  = ReactiveCommand.Create(() => { });
        LogoutCommand            = ReactiveCommand.Create(_onLogout);

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

    public ReactiveCommand<Unit, Unit> NavigateDatabaseCommand  { get; }
    public ReactiveCommand<Unit, Unit> NavigateCustomersCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateFleetCommand     { get; }
    public ReactiveCommand<Unit, Unit> NavigateRentalsCommand   { get; }
    public ReactiveCommand<Unit, Unit> NavigateSettingsCommand  { get; }
    public ReactiveCommand<Unit, Unit> LogoutCommand            { get; }

    private void NavigateTo(string section)
    {
        CurrentSection = section;
        CurrentView = section switch
        {
            "Database"  => new DashboardViewModel(CurrentUser),
            "Customers" => new CustomersViewModel(CurrentUser),
            "Fleet"     => new FleetViewModel(CurrentUser),
            "Rentals"   => new RentalsViewModel(CurrentUser),
            _           => new DashboardViewModel(CurrentUser)
        };
    }
}

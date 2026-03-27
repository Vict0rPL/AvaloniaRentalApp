using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class DatabaseViewModel : ViewModelBase
{
    private string _connectionStatus = "Unknown";
    private readonly DatabaseService _databaseService;

    public ObservableCollection<Car> Cars { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<User> Users { get; } = new();
    public ObservableCollection<Rental> Rentals { get; } = new();

    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => this.RaiseAndSetIfChanged(ref _connectionStatus, value);
    }

    public string Greeting => "Welcome to Car Rental App!";

    public DatabaseViewModel()
    {
        _databaseService = new DatabaseService();
        _ = Initialize();
    }

    private async Task Initialize()
    {
        bool isConnected = await _databaseService.TestConnectionAsync();
        ConnectionStatus = isConnected ? "Connected to MariaDB" : "Connection Failed";

        if (isConnected)
        {
            var carsList = await _databaseService.GetCarsAsync();
            foreach (var car in carsList) Cars.Add(car);

            var categoriesList = await _databaseService.GetCategoriesAsync();
            foreach (var category in categoriesList) Categories.Add(category);

            var customersList = await _databaseService.GetCustomersAsync();
            foreach (var customer in customersList) Customers.Add(customer);

            var usersList = await _databaseService.GetUsersAsync();
            foreach (var user in usersList) Users.Add(user);

            var rentalsList = await _databaseService.GetRentalsAsync();
            foreach (var rental in rentalsList) Rentals.Add(rental);
        }
    }
}

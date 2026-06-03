using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;
    private readonly MessageBoxService _messageBox = new();

    public ObservableCollection<FuelPrice> FuelPrices { get; } = new();

    // ===== Database connection fields =====
    private string _dbHost = "";
    public string DbHost
    {
        get => _dbHost;
        set => this.RaiseAndSetIfChanged(ref _dbHost, value);
    }

    private string _dbPort = "3306";
    public string DbPort
    {
        get => _dbPort;
        set => this.RaiseAndSetIfChanged(ref _dbPort, value);
    }

    private string _dbDatabase = "";
    public string DbDatabase
    {
        get => _dbDatabase;
        set => this.RaiseAndSetIfChanged(ref _dbDatabase, value);
    }

    private string _dbUser = "";
    public string DbUser
    {
        get => _dbUser;
        set => this.RaiseAndSetIfChanged(ref _dbUser, value);
    }

    private string _dbPassword = "";
    public string DbPassword
    {
        get => _dbPassword;
        set => this.RaiseAndSetIfChanged(ref _dbPassword, value);
    }

    // ===== Application info (read-only) =====
    public string AppVersion { get; }
    public string Framework => "Avalonia UI 11.2 / .NET 8";
    public string DatabaseInfo => "MariaDB 10.6+";
    public string OrmInfo => "Dapper + MySqlConnector";
    public string MvvmInfo => "ReactiveUI";
    public string License => "Wewnętrzna — użytek firmowy";

    public ReactiveCommand<Unit, Unit> LoadCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> TestConnectionCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveConnectionCommand { get; }

    public SettingsViewModel()
    {
        _databaseService = new DatabaseService();

        AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0-beta";

        LoadConnectionFields();

        LoadCommand = ReactiveCommand.CreateFromTask(LoadAsync);
        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
        TestConnectionCommand = ReactiveCommand.CreateFromTask(TestConnectionAsync);
        SaveConnectionCommand = ReactiveCommand.CreateFromTask(SaveConnectionAsync);

        _ = LoadAsync();
    }

    private void LoadConnectionFields()
    {
        try
        {
            var (host, port, database, user, password) =
                DatabaseService.ParseConnectionString(_databaseService.ConnectionString);
            DbHost = host;
            DbPort = port.ToString();
            DbDatabase = database;
            DbUser = user;
            DbPassword = password;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing connection string: {ex.Message}");
        }
    }

    private async Task LoadAsync()
    {
        FuelPrices.Clear();
        var prices = await _databaseService.GetFuelPricesAsync();
        foreach (var p in prices)
            FuelPrices.Add(p);
    }

    private async Task SaveAsync()
    {
        bool allOk = true;
        foreach (var price in FuelPrices)
        {
            if (!await _databaseService.UpdateFuelPriceAsync(price))
                allOk = false;
        }

        if (allOk)
            await _messageBox.ShowInfoMessageAsync("Ustawienia", "Ceny paliw zostały zapisane.");
        else
            await _messageBox.ShowErrorMessageAsync("Ustawienia", "Nie udało się zapisać niektórych cen paliw.");
    }

    private bool TryBuildConnectionString(out string connectionString)
    {
        connectionString = "";
        if (!uint.TryParse(DbPort, out var port))
            return false;

        connectionString = DatabaseService.BuildConnectionString(
            _databaseService.ConnectionString, DbHost, port, DbDatabase, DbUser, DbPassword);
        return true;
    }

    private async Task TestConnectionAsync()
    {
        if (!TryBuildConnectionString(out var connStr))
        {
            await _messageBox.ShowErrorMessageAsync("Ustawienia", "Port musi być liczbą.");
            return;
        }

        if (await DatabaseService.TestConnectionAsync(connStr))
            await _messageBox.ShowInfoMessageAsync("Ustawienia", "Połączenie nawiązane poprawnie.");
        else
            await _messageBox.ShowErrorMessageAsync("Ustawienia", "Nie udało się połączyć z bazą danych.");
    }

    private async Task SaveConnectionAsync()
    {
        if (!TryBuildConnectionString(out var connStr))
        {
            await _messageBox.ShowErrorMessageAsync("Ustawienia", "Port musi być liczbą.");
            return;
        }

        try
        {
            AppConfigService.SaveConnectionString(connStr);
            await _messageBox.ShowInfoMessageAsync(
                "Ustawienia",
                "Konfiguracja zapisana. Zmiany zostaną zastosowane po ponownym uruchomieniu aplikacji.");
        }
        catch (Exception ex)
        {
            await _messageBox.ShowErrorMessageAsync("Ustawienia", $"Nie udało się zapisać konfiguracji: {ex.Message}");
        }
    }
}

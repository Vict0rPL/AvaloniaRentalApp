using System.Collections.ObjectModel;
using System.Reactive;
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

    public ReactiveCommand<Unit, Unit> LoadCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public SettingsViewModel()
    {
        _databaseService = new DatabaseService();

        LoadCommand = ReactiveCommand.CreateFromTask(LoadAsync);
        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);

        _ = LoadAsync();
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
}

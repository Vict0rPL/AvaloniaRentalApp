using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class MaintenanceEditViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;
    private readonly int _carId;

    public MaintenanceRecord Record { get; }

    public List<string> Types { get; } = new() { "serwis", "przeglad", "naprawa" };

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public ReactiveCommand<Unit, MaintenanceRecord?> SaveCommand { get; }
    public ReactiveCommand<Unit, MaintenanceRecord?> CancelCommand { get; }

    public MaintenanceEditViewModel(int carId)
    {
        _databaseService = new DatabaseService();
        _carId = carId;
        Record = new MaintenanceRecord { CarId = carId, Date = DateTime.Today };

        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
        CancelCommand = ReactiveCommand.Create(() => (MaintenanceRecord?)null);
    }

    private async Task<MaintenanceRecord?> SaveAsync()
    {
        ErrorMessage = null;

        if (Record.Cost < 0)
        {
            ErrorMessage = "Koszt nie może być ujemny.";
            return null;
        }

        var newId = await _databaseService.AddMaintenanceAsync(Record);
        if (newId > 0)
        {
            Record.MaintenanceId = newId;
            return Record;
        }

        ErrorMessage = "Błąd zapisu wpisu serwisowego.";
        return null;
    }
}

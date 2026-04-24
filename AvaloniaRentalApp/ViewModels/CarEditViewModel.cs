using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class CarEditViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;
    
    public Car EditingCar { get; }
    public bool IsNew { get; }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public ObservableCollection<Category> Categories { get; } = new();

    private Category? _selectedCategory;
    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set 
        {
            this.RaiseAndSetIfChanged(ref _selectedCategory, value);
            if (value != null) EditingCar.CategoryId = value.CategoryId;
        }
    }

    public ObservableCollection<string> FuelTypes { get; } = new() { "benzyna", "diesel", "LPG", "elektryczny", "hybryda" };
    public ObservableCollection<string> TransmissionTypes { get; } = new() { "manualna", "automatyczna" };
    public ObservableCollection<string> Statuses { get; } = new() { "dostepny", "wypozyczony", "serwis", "wycofany" };

    public ReactiveCommand<Unit, Car?> SaveCommand { get; }
    public ReactiveCommand<Unit, Car?> CancelCommand { get; }

    public CarEditViewModel(Car? carToEdit = null)
    {
        _databaseService = new DatabaseService();
        IsNew = carToEdit == null;
        EditingCar = carToEdit ?? new Car();

        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
        CancelCommand = ReactiveCommand.Create(() => (Car?)null);

        _ = LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _databaseService.GetCategoriesAsync();
        foreach (var cat in categories)
        {
            Categories.Add(cat);
            if (!IsNew && cat.CategoryId == EditingCar.CategoryId)
            {
                SelectedCategory = cat;
            }
        }
        
        if (IsNew && Categories.Count > 0)
        {
            SelectedCategory = Categories[0];
        }
    }

    private async Task<Car?> SaveAsync()
    {
        ErrorMessage = null;

        // Basic validation
        if (string.IsNullOrWhiteSpace(EditingCar.Brand) || 
            string.IsNullOrWhiteSpace(EditingCar.Model) || 
            string.IsNullOrWhiteSpace(EditingCar.Registration) ||
            string.IsNullOrWhiteSpace(EditingCar.Vin))
        {
            ErrorMessage = "Marka, Model, Rejestracja i VIN są wymagane!";
            return null;
        }

        if (IsNew)
        {
            var newId = await _databaseService.AddCarAsync(EditingCar);
            if (newId > 0)
            {
                EditingCar.CarId = newId;
                return EditingCar;
            }
            else
            {
                ErrorMessage = "Błąd zapisu. Prawdopodobnie duplikat numeru rejestracyjnego lub VIN.";
                return null;
            }
        }
        else
        {
            var success = await _databaseService.UpdateCarAsync(EditingCar);
            if (success) return EditingCar;
            
            ErrorMessage = "Błąd aktualizacji danych pojazdu.";
            return null;
        }
    }
}

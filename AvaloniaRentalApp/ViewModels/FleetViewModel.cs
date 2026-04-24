using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class FleetViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;

    public ObservableCollection<Car> Cars { get; } = new();
    
    private Car? _selectedCar;
    public Car? SelectedCar
    {
        get => _selectedCar;
        set => this.RaiseAndSetIfChanged(ref _selectedCar, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private bool _showArchived;
    public bool ShowArchived
    {
        get => _showArchived;
        set => this.RaiseAndSetIfChanged(ref _showArchived, value);
    }

    public ReactiveCommand<Unit, Unit> LoadCarsCommand { get; }
    public ReactiveCommand<Unit, Unit> AddCarCommand { get; }
    public ReactiveCommand<Unit, Unit> EditCarCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCarCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowDetailsCommand { get; }
    public ReactiveCommand<Unit, Unit> RestoreCarCommand { get; }

    public FleetViewModel()
    {
        _databaseService = new DatabaseService();

        LoadCarsCommand = ReactiveCommand.CreateFromTask(LoadCarsAsync);
        
        AddCarCommand = ReactiveCommand.CreateFromTask(AddCarAsync);
        
        var canEditOrDelete = this.WhenAnyValue(x => x.SelectedCar)
            .Select(selected => selected != null);

        var canRestore = this.WhenAnyValue(x => x.SelectedCar)
            .Select(selected => selected != null && !selected.IsActive);

        EditCarCommand = ReactiveCommand.CreateFromTask(EditCarAsync, canEditOrDelete);
        DeleteCarCommand = ReactiveCommand.CreateFromTask(DeleteCarAsync, canEditOrDelete);
        ShowDetailsCommand = ReactiveCommand.CreateFromTask(ShowDetailsAsync, canEditOrDelete);
        RestoreCarCommand = ReactiveCommand.CreateFromTask(RestoreCarAsync, canRestore);

        // Reload when ShowArchived changes
        this.WhenAnyValue(x => x.ShowArchived)
            .Select(_ => Unit.Default)
            .InvokeCommand(LoadCarsCommand);

        // Initial load is handled by the WhenAnyValue subscription above
    }

    private async Task LoadCarsAsync()
    {
        IsLoading = true;
        try
        {
            Cars.Clear();
            var cars = await _databaseService.GetCarsAsync();
            foreach (var car in cars)
            {
                // Filter based on ShowArchived
                if (car.IsActive == !ShowArchived)
                {
                    Cars.Add(car);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading cars: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public Interaction<CarEditViewModel, Car?> ShowDialog { get; } = new();
    public Interaction<CarDetailsViewModel, Unit> ShowDetailsDialog { get; } = new();

    private async Task AddCarAsync()
    {
        var vm = new CarEditViewModel();
        var result = await ShowDialog.Handle(vm);
        
        if (result != null)
        {
            await LoadCarsAsync();
        }
    }

    private async Task EditCarAsync()
    {
        if (SelectedCar == null) return;
        
        // Pass a copy or the reference. Passing reference might update UI before save, but it's simpler for now.
        // For a real app, copy the model, and copy back on save.
        var vm = new CarEditViewModel(SelectedCar);
        var result = await ShowDialog.Handle(vm);
        
        if (result != null)
        {
            // If reference was passed, it's already updated. But let's trigger property changed if needed.
            // A simple way to refresh the grid is to replace the item or reload.
            // For now, reloading is safest.
            await LoadCarsAsync();
        }
    }

    private async Task ShowDetailsAsync()
    {
        if (SelectedCar == null) return;
        
        var vm = new CarDetailsViewModel(SelectedCar);
        await ShowDetailsDialog.Handle(vm);
    }

    private async Task DeleteCarAsync()
    {
        if (SelectedCar == null) return;
        
        IsLoading = true;
        var success = await _databaseService.DeleteCarAsync(SelectedCar.CarId);
        if (success)
        {
            Cars.Remove(SelectedCar);
            SelectedCar = null;
        }
        IsLoading = false;
    }

    private async Task RestoreCarAsync()
    {
        if (SelectedCar == null) return;
        
        IsLoading = true;
        var success = await _databaseService.RestoreCarAsync(SelectedCar.CarId);
        if (success)
        {
            Cars.Remove(SelectedCar);
            SelectedCar = null;
        }
        IsLoading = false;
    }
}

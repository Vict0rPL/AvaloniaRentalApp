using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class RentalsViewModel : ViewModelBase
{
    private readonly DatabaseService _dbService;
    private readonly User _currentUser;
    private readonly List<Rental> _allRentals = new();

    public ObservableCollection<Rental> Rentals { get; } = new();

    private Rental? _selectedRental;
    public Rental? SelectedRental
    {
        get => _selectedRental;
        set => this.RaiseAndSetIfChanged(ref _selectedRental, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    private int _selectedStatusIndex;
    public int SelectedStatusIndex
    {
        get => _selectedStatusIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedStatusIndex, value);
    }

    public Interaction<AddRentalViewModel, Rental?> ShowAddRentalDialog { get; } = new();
    public Interaction<ReturnRentalViewModel, Rental?> ShowReturnRentalDialog { get; } = new();
    public Interaction<ChangePaymentStatusViewModel, Rental?> ShowChangePaymentDialog { get; } = new();

    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<Unit, Unit> AddRentalCommand { get; }
    public ReactiveCommand<Unit, Unit> ReturnRentalCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelRentalCommand { get; }
    public ReactiveCommand<Unit, Unit> ChangePaymentStatusCommand { get; }

    public RentalsViewModel(User currentUser)
    {
        _dbService = new DatabaseService();
        _currentUser = currentUser;

        RefreshCommand   = ReactiveCommand.CreateFromTask(LoadDataAsync);
        AddRentalCommand = ReactiveCommand.CreateFromTask(AddRentalAsync);

        var canReturn = this.WhenAnyValue(
            x => x.SelectedRental,
            r => r != null && r.Status == "aktywna"
        );
        ReturnRentalCommand = ReactiveCommand.CreateFromTask(ReturnRentalAsync, canReturn);

        var canCancel = this.WhenAnyValue(
            x => x.SelectedRental,
            r => r != null && r.Status == "aktywna"
        );
        CancelRentalCommand = ReactiveCommand.CreateFromTask(CancelRentalAsync, canCancel);

        var canChangePayment = this.WhenAnyValue(
            x => x.SelectedRental)
            .Select(r => r != null
        );
        ChangePaymentStatusCommand = ReactiveCommand.CreateFromTask(ChangePaymentStatusAsync, canChangePayment);

        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ApplyFilter());

        this.WhenAnyValue(x => x.SelectedStatusIndex)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ApplyFilter());

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            await _dbService.MarkOverdueRentalsAsync();
            var rentals = await _dbService.GetRentalsAsync();
            _allRentals.Clear();
            _allRentals.AddRange(rentals);
            ApplyFilter();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading rentals: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilter()
    {
        var filtered = _allRentals.AsEnumerable();

        filtered = SelectedStatusIndex switch
        {
            1 => filtered.Where(r => r.Status == "aktywna"),
            2 => filtered.Where(r => r.Status == "zakonczona"),
            3 => filtered.Where(r => r.Status == "anulowana"),
            4 => filtered.Where(r => r.Status == "przeterminowana"),
            _ => filtered
        };

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var t = SearchText;
            filtered = filtered.Where(r =>
                r.RentalNumber.Contains(t, StringComparison.OrdinalIgnoreCase) ||
                r.CustomerName.Contains(t, StringComparison.OrdinalIgnoreCase) ||
                r.CarName.Contains(t, StringComparison.OrdinalIgnoreCase)      ||
                r.CarRegistration.Contains(t, StringComparison.OrdinalIgnoreCase));
        }

        Rentals.Clear();
        foreach (var r in filtered) Rentals.Add(r);
    }

    private async Task AddRentalAsync()
    {
        var vm = new AddRentalViewModel(_currentUser);
        var result = await ShowAddRentalDialog.Handle(vm);
        if (result != null)
            await LoadDataAsync();
    }

    private async Task ReturnRentalAsync()
    {
        if (SelectedRental == null) return;

        var vm = new ReturnRentalViewModel(SelectedRental);
        var result = await ShowReturnRentalDialog.Handle(vm);
        if (result != null)
            await LoadDataAsync();
    }

    private async Task CancelRentalAsync()
    {
        if (SelectedRental == null) return;
        bool success = await _dbService.CancelRentalAsync(SelectedRental.RentalId, SelectedRental.CarId);
        if (success)
            await LoadDataAsync();
    }

    private async Task ChangePaymentStatusAsync()
    {
        if (SelectedRental == null) return;
        var vm = new ChangePaymentStatusViewModel(SelectedRental);
        var result = await ShowChangePaymentDialog.Handle(vm);
        if (result != null)
            await LoadDataAsync();
    }
}

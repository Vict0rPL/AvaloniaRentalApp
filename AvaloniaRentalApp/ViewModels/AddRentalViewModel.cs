using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class AddRentalViewModel : ViewModelBase
{
    private readonly DatabaseService _dbService;
    private readonly User _currentUser;

    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<Car> AvailableCars { get; } = new();
    public List<string> PaymentMethods { get; } = ["gotowka", "karta", "przelew"];

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set => this.RaiseAndSetIfChanged(ref _selectedCustomer, value);
    }

    private Car? _selectedCar;
    public Car? SelectedCar
    {
        get => _selectedCar;
        set => this.RaiseAndSetIfChanged(ref _selectedCar, value);
    }

    private string? _selectedPaymentMethod = "gotowka";
    public string? SelectedPaymentMethod
    {
        get => _selectedPaymentMethod;
        set => this.RaiseAndSetIfChanged(ref _selectedPaymentMethod, value);
    }

    private DateTime? _dateStart = DateTime.Now;
    public DateTime? DateStart
    {
        get => _dateStart;
        set => this.RaiseAndSetIfChanged(ref _dateStart, value);
    }

    private DateTime? _dateEndPlanned = DateTime.Now.AddDays(1);
    public DateTime? DateEndPlanned
    {
        get => _dateEndPlanned;
        set => this.RaiseAndSetIfChanged(ref _dateEndPlanned, value);
    }

    private int _mileageStart;
    public int MileageStart
    {
        get => _mileageStart;
        set => this.RaiseAndSetIfChanged(ref _mileageStart, value);
    }

    private decimal _dailyRate;
    public decimal DailyRate
    {
        get => _dailyRate;
        set => this.RaiseAndSetIfChanged(ref _dailyRate, value);
    }

    private decimal _depositPaid;
    public decimal DepositPaid
    {
        get => _depositPaid;
        set => this.RaiseAndSetIfChanged(ref _depositPaid, value);
    }

    private string? _notes;
    public string? Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    public int TotalDays
    {
        get
        {
            if (DateStart == null || DateEndPlanned == null) return 1;
            return Math.Max(1, (DateEndPlanned.Value - DateStart.Value).Days);
        }
    }

    public decimal BaseCost => TotalDays * DailyRate;
    public decimal TotalCost => BaseCost;

    private string? _customerError;
    public string? CustomerError
    {
        get => _customerError;
        private set => this.RaiseAndSetIfChanged(ref _customerError, value);
    }

    private string? _carError;
    public string? CarError
    {
        get => _carError;
        private set => this.RaiseAndSetIfChanged(ref _carError, value);
    }

    private string? _dateError;
    public string? DateError
    {
        get => _dateError;
        private set => this.RaiseAndSetIfChanged(ref _dateError, value);
    }

    private string? _mileageError;
    public string? MileageError
    {
        get => _mileageError;
        private set => this.RaiseAndSetIfChanged(ref _mileageError, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public ReactiveCommand<Unit, Rental?> ConfirmCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public AddRentalViewModel(User currentUser)
    {
        _dbService = new DatabaseService();
        _currentUser = currentUser;

        ConfirmCommand = ReactiveCommand.CreateFromTask<Rental?>(SaveAsync);
        CancelCommand = ReactiveCommand.Create(() => { });

        this.WhenAnyValue(x => x.SelectedCar)
            .Where(c => c != null)
            .Subscribe(car =>
            {
                DailyRate = car!.DailyRate;
                MileageStart = car.MileageKm;
                RecalculateCosts();
            });

        this.WhenAnyValue(x => x.DateStart, x => x.DateEndPlanned, x => x.DailyRate)
            .Subscribe(_ => RecalculateCosts());

        this.WhenAnyValue(x => x.SelectedCustomer)
            .Subscribe(_ => { if (SelectedCustomer != null) CustomerError = null; });
        this.WhenAnyValue(x => x.SelectedCar)
            .Subscribe(_ => { if (SelectedCar != null) CarError = null; });
        this.WhenAnyValue(x => x.DateStart, x => x.DateEndPlanned)
            .Subscribe(_ =>
            {
                if (DateStart.HasValue && DateEndPlanned.HasValue && DateEndPlanned.Value > DateStart.Value)
                    DateError = null;
            });

        _ = LoadDataAsync();
    }

    private void RecalculateCosts()
    {
        this.RaisePropertyChanged(nameof(TotalDays));
        this.RaisePropertyChanged(nameof(BaseCost));
        this.RaisePropertyChanged(nameof(TotalCost));
    }

    private async Task LoadDataAsync()
    {
        var customers = await _dbService.GetCustomersAsync();
        foreach (var c in customers)
        {
            if (c.IsActive && !c.IsBlacklisted)
                Customers.Add(c);
        }

        var cars = await _dbService.GetAvailableCarsAsync();
        foreach (var car in cars)
            AvailableCars.Add(car);
    }

    private async Task<Rental?> SaveAsync()
    {
        if (!Validate()) return null;

        bool hasOverlap = await _dbService.CheckRentalOverlapAsync(
            SelectedCar!.CarId, DateStart!.Value, DateEndPlanned!.Value);
        if (hasOverlap)
        {
            ErrorMessage = "Ten pojazd ma już aktywne wypożyczenie w wybranym terminie.";
            return null;
        }

        var rental = new Rental
        {
            CustomerId     = SelectedCustomer!.CustomerId,
            CarId          = SelectedCar!.CarId,
            UserId         = _currentUser.UserId,
            DateStart      = DateStart!.Value,
            DateEndPlanned = DateEndPlanned!.Value,
            MileageStart   = MileageStart,
            DailyRate      = DailyRate,
            TotalDays      = TotalDays,
            BaseCost       = BaseCost,
            TotalCost      = TotalCost,
            DepositPaid    = DepositPaid,
            PaymentMethod  = SelectedPaymentMethod,
            Notes          = Notes
        };

        var newId = await _dbService.AddRentalAsync(rental);
        if (newId > 0)
        {
            rental.RentalId = newId;
            return rental;
        }

        ErrorMessage = "Błąd zapisu. Sprawdź dane i spróbuj ponownie.";
        var msgService = new MessageBoxService();
        await msgService.ShowErrorMessageAsync("Błąd Zapisu", "Nie udało się zapisać wypożyczenia do bazy danych. Brak połączenia lub błąd SQL.");
        return null;
    }

    private bool Validate()
    {
        CustomerError = SelectedCustomer == null ? "Wybierz klienta." : null;
        CarError      = SelectedCar == null      ? "Wybierz pojazd." : null;

        if (!DateStart.HasValue || !DateEndPlanned.HasValue)
            DateError = "Podaj daty wypożyczenia.";
        else if (DateEndPlanned.Value <= DateStart.Value)
            DateError = "Data zakończenia musi być późniejsza niż data rozpoczęcia.";
        else
            DateError = null;

        MileageError = MileageStart < 0 ? "Przebieg nie może być ujemny." : null;

        return CustomerError == null && CarError == null &&
               DateError == null && MileageError == null;
    }
}

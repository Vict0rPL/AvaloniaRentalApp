using System;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels
{
    public class ReturnRentalViewModel : ViewModelBase
    {
        private readonly DatabaseService _dbService;
        public Rental Rental { get; }

        private DateTimeOffset _returnDate = DateTimeOffset.Now;
        public DateTimeOffset ReturnDate
        {
            get => _returnDate;
            set 
            {
                this.RaiseAndSetIfChanged(ref _returnDate, value);
                UpdateCosts();
            }
        }

        private int _returnMileage;
        public int ReturnMileage
        {
            get => _returnMileage;
            set => this.RaiseAndSetIfChanged(ref _returnMileage, value);
        }

        private decimal _damageCost;
        public decimal DamageCost
        {
            get => _damageCost;
            set 
            {
                this.RaiseAndSetIfChanged(ref _damageCost, value);
                UpdateCosts();
            }
        }

        private string? _notes;
        public string? Notes
        {
            get => _notes;
            set => this.RaiseAndSetIfChanged(ref _notes, value);
        }

        private decimal _lateReturnCost;
        public decimal LateReturnCost
        {
            get => _lateReturnCost;
            private set => this.RaiseAndSetIfChanged(ref _lateReturnCost, value);
        }

        private decimal _totalCost;
        public decimal TotalCost
        {
            get => _totalCost;
            private set => this.RaiseAndSetIfChanged(ref _totalCost, value);
        }

        public ReactiveCommand<Unit, Rental?> ConfirmCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ReturnRentalViewModel(Rental rental)
        {
            _dbService = new DatabaseService();
            Rental = rental;
            ReturnMileage = rental.MileageStart;
            
            UpdateCosts();

            var canConfirm = this.WhenAnyValue(
                x => x.ReturnMileage,
                m => m >= rental.MileageStart);

            ConfirmCommand = ReactiveCommand.CreateFromTask(ConfirmReturnAsync, canConfirm);
            CancelCommand = ReactiveCommand.Create(() => { });
        }

        private void UpdateCosts()
        {
            LateReturnCost = RentalCalculator.CalculateLateReturnCost(
                Rental.DateEndPlanned, 
                ReturnDate.DateTime, 
                Rental.DailyRate);

            Rental.DateEndActual = ReturnDate.DateTime;
            Rental.LateReturnCost = LateReturnCost;
            Rental.DamageCost = DamageCost;
            
            TotalCost = RentalCalculator.CalculateTotalCost(Rental);
        }

        private async Task<Rental?> ConfirmReturnAsync()
        {
            Rental.Status = "zakonczona";
            Rental.DateEndActual = ReturnDate.DateTime;
            Rental.MileageEnd = ReturnMileage;
            Rental.LateReturnCost = LateReturnCost;
            Rental.DamageCost = DamageCost;
            Rental.TotalCost = TotalCost;
            Rental.Notes = Notes;

            bool success = await _dbService.ReturnRentalAsync(Rental);
            return success ? Rental : null;
        }
    }
}

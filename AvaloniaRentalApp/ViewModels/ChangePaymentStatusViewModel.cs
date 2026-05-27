using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class ChangePaymentStatusViewModel : ViewModelBase
{
    private readonly DatabaseService _dbService;
    public Rental Rental { get; }

    public List<string> PaymentStatuses { get; } = new()
        { "oczekuje", "oplacona", "czesciowa", "zalegla" };

    private string _selectedPaymentStatus;
    public string SelectedPaymentStatus
    {
        get => _selectedPaymentStatus;
        set => this.RaiseAndSetIfChanged(ref _selectedPaymentStatus, value);
    }

    public ReactiveCommand<Unit, Rental?> ConfirmCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public ChangePaymentStatusViewModel(Rental rental)
    {
        _dbService = new DatabaseService();
        Rental = rental;
        _selectedPaymentStatus = rental.PaymentStatus;

        ConfirmCommand = ReactiveCommand.CreateFromTask(ConfirmAsync);
        CancelCommand = ReactiveCommand.Create(() => { });
    }

    private async Task<Rental?> ConfirmAsync()
    {
        bool success = await _dbService.UpdatePaymentStatusAsync(
            Rental.RentalId, SelectedPaymentStatus);
        if (success)
        {
            Rental.PaymentStatus = SelectedPaymentStatus;
            return Rental;
        }
        return null;
    }
}

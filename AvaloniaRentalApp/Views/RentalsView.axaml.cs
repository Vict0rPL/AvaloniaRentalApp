using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.Views;

public partial class RentalsView : ReactiveUserControl<RentalsViewModel>
{
    public RentalsView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel != null)
            {
                d(ViewModel.ShowAddRentalDialog.RegisterHandler(
                    async interaction => await DoShowAddRentalDialogAsync(interaction)));

                d(ViewModel.ShowReturnRentalDialog.RegisterHandler(
                    async interaction => await DoShowReturnRentalDialogAsync(interaction)));

                d(ViewModel.ShowChangePaymentDialog.RegisterHandler(
                    async interaction => await DoShowChangePaymentDialogAsync(interaction)));
            }
        });
    }

    private async Task DoShowAddRentalDialogAsync(
        IInteractionContext<AddRentalViewModel, Rental?> interaction)
    {
        var dialog = new AddRentalWindow { DataContext = interaction.Input };
        var window = (Window)this.GetVisualRoot()!;
        var result = await dialog.ShowDialog<Rental?>(window);
        interaction.SetOutput(result);
    }

    private async Task DoShowReturnRentalDialogAsync(
        IInteractionContext<ReturnRentalViewModel, Rental?> interaction)
    {
        var dialog = new ReturnRentalWindow { DataContext = interaction.Input };
        var window = (Window)this.GetVisualRoot()!;
        var result = await dialog.ShowDialog<Rental?>(window);
        interaction.SetOutput(result);
    }

    private async Task DoShowChangePaymentDialogAsync(
        IInteractionContext<ChangePaymentStatusViewModel, Rental?> interaction)
    {
        var dialog = new ChangePaymentStatusWindow { DataContext = interaction.Input };
        var window = (Window)this.GetVisualRoot()!;
        var result = await dialog.ShowDialog<Rental?>(window);
        interaction.SetOutput(result);
    }
}

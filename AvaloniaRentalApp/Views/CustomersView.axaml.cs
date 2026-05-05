using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.Views;

public partial class CustomersView : ReactiveUserControl<CustomersViewModel>
{
    public CustomersView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel != null)
            {
                d(ViewModel.ShowAddCustomerDialog.RegisterHandler(async interaction =>
                    await DoShowAddDialogAsync(interaction)));
                d(ViewModel.ShowEditCustomerDialog.RegisterHandler(async interaction =>
                    await DoShowEditDialogAsync(interaction)));
                d(ViewModel.ShowCustomerDetailsDialog.RegisterHandler(async interaction =>
                    await DoShowDetailsDialogAsync(interaction)));
            }
        });
    }

    private async Task DoShowAddDialogAsync(
        IInteractionContext<AddCustomerViewModel, Customer?> interaction)
    {
        var dialog = new AddCustomerWindow { DataContext = interaction.Input };
        var window = (Window)this.GetVisualRoot()!;
        var result = await dialog.ShowDialog<Customer?>(window);
        interaction.SetOutput(result);
    }

    private async Task DoShowEditDialogAsync(
        IInteractionContext<CustomerEditViewModel, Customer?> interaction)
    {
        var dialog = new CustomerEditWindow { DataContext = interaction.Input };
        var window = (Window)this.GetVisualRoot()!;
        var result = await dialog.ShowDialog<Customer?>(window);
        interaction.SetOutput(result);
    }

    private async Task DoShowDetailsDialogAsync(
        IInteractionContext<CustomerDetailsViewModel, System.Reactive.Unit> interaction)
    {
        var dialog = new CustomerDetailsWindow { DataContext = interaction.Input };
        var window = (Window)this.GetVisualRoot()!;
        await dialog.ShowDialog(window);
        interaction.SetOutput(System.Reactive.Unit.Default);
    }
}

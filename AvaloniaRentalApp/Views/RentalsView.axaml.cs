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
                d(ViewModel.ShowAddRentalDialog.RegisterHandler(
                    async interaction => await DoShowAddRentalDialogAsync(interaction)));
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
}

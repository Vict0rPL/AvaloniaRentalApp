using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.Views;

public partial class FleetView : ReactiveUserControl<FleetViewModel>
{
    public FleetView()
    {
        InitializeComponent();
        
        this.WhenActivated(d =>
        {
            if (ViewModel != null)
            {
                d(ViewModel.ShowDialog.RegisterHandler(async interaction => await DoShowDialogAsync(interaction)));
                d(ViewModel.ShowDetailsDialog.RegisterHandler(async interaction => await DoShowDetailsDialogAsync(interaction)));
            }
        });
    }

    private async Task DoShowDialogAsync(IInteractionContext<CarEditViewModel, Models.Car?> interaction)
    {
        var dialog = new CarEditWindow
        {
            DataContext = interaction.Input
        };

        var window = (Window)this.GetVisualRoot()!;
        var result = await dialog.ShowDialog<Models.Car?>(window);
        interaction.SetOutput(result);
    }

    private async Task DoShowDetailsDialogAsync(IInteractionContext<CarDetailsViewModel, System.Reactive.Unit> interaction)
    {
        var dialog = new CarDetailsWindow
        {
            DataContext = interaction.Input
        };

        var window = (Window)this.GetVisualRoot()!;
        await dialog.ShowDialog(window);
        interaction.SetOutput(System.Reactive.Unit.Default);
    }
}

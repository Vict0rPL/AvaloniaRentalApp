using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.Views;

public partial class CarDetailsWindow : ReactiveWindow<CarDetailsViewModel>
{
    public CarDetailsWindow()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel != null)
            {
                d(ViewModel.CloseCommand.Subscribe(_ => Close()));
                d(ViewModel.ShowMaintenanceDialog.RegisterHandler(
                    async interaction => await DoShowMaintenanceDialogAsync(interaction)));
            }
        });
    }

    private async Task DoShowMaintenanceDialogAsync(
        IInteractionContext<MaintenanceEditViewModel, MaintenanceRecord?> interaction)
    {
        var dialog = new MaintenanceEditWindow { DataContext = interaction.Input };
        var window = (Window)this.GetVisualRoot()!;
        var result = await dialog.ShowDialog<MaintenanceRecord?>(window);
        interaction.SetOutput(result);
    }
}

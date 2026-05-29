using System;
using Avalonia.ReactiveUI;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.Views;

public partial class MaintenanceEditWindow : ReactiveWindow<MaintenanceEditViewModel>
{
    public MaintenanceEditWindow()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            d(ViewModel!.SaveCommand.Subscribe(result => Close(result)));
            d(ViewModel!.CancelCommand.Subscribe(_ => Close(null)));
        });
    }
}

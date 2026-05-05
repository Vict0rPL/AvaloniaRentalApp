using System;
using Avalonia.ReactiveUI;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.Views;

public partial class AddRentalWindow : ReactiveWindow<AddRentalViewModel>
{
    public AddRentalWindow()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel != null)
            {
                d(ViewModel.ConfirmCommand.Subscribe(result =>
                {
                    if (result != null) Close(result);
                }));

                d(ViewModel.CancelCommand.Subscribe(_ => Close((Rental?)null)));
            }
        });
    }
}

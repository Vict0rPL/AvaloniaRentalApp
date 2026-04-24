using System;
using Avalonia.ReactiveUI;
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
            }
        });
    }
}

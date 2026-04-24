using System;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.Views;

public partial class CarEditWindow : ReactiveWindow<CarEditViewModel>
{
    public CarEditWindow()
    {
        InitializeComponent();
        
        this.WhenActivated(d => 
        {
            if (ViewModel != null)
            {
                // Only close if we have a result (successful save)
                d(ViewModel.SaveCommand.Subscribe(result => 
                {
                    if (result != null) Close(result);
                }));
                
                // Cancel always closes with null
                d(ViewModel.CancelCommand.Subscribe(result => Close(result)));
            }
        });
    }
}

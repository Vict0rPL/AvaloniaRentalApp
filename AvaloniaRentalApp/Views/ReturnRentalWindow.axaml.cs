using System;
using Avalonia.ReactiveUI;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.Views
{
    public partial class ReturnRentalWindow : ReactiveWindow<ReturnRentalViewModel>
    {
        public ReturnRentalWindow()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                d(ViewModel!.ConfirmCommand.Subscribe(result => Close(result)));
                d(ViewModel!.CancelCommand.Subscribe(_ => Close(null)));
            });
        }
    }
}

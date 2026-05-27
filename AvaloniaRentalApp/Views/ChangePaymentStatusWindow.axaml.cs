using System;
using Avalonia.ReactiveUI;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.Views
{
    public partial class ChangePaymentStatusWindow : ReactiveWindow<ChangePaymentStatusViewModel>
    {
        public ChangePaymentStatusWindow()
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

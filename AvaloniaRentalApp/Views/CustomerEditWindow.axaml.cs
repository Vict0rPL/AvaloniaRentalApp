using System;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;

namespace AvaloniaRentalApp.Views;

public partial class CustomerEditWindow : Window
{
    private IDisposable? _confirmSub;
    private IDisposable? _cancelSub;

    public CustomerEditWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        _confirmSub?.Dispose();
        _cancelSub?.Dispose();

        if (DataContext is not CustomerEditViewModel vm) return;

        _confirmSub = vm.ConfirmCommand.Subscribe(customer =>
        {
            if (customer is not null)
                Dispatcher.UIThread.Post(() => Close(customer), DispatcherPriority.Background);
        });

        _cancelSub = vm.CancelCommand.Subscribe(_ =>
            Dispatcher.UIThread.Post(() => Close(null), DispatcherPriority.Background));
    }

    protected override void OnClosed(EventArgs e)
    {
        _confirmSub?.Dispose();
        _cancelSub?.Dispose();
        base.OnClosed(e);
    }
}

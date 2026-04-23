using System;
using Avalonia.Controls;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;

namespace AvaloniaRentalApp.Views;

public partial class AddCustomerWindow : Window
{
    private IDisposable? _confirmSub;
    private IDisposable? _cancelSub;

    public AddCustomerWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        _confirmSub?.Dispose();
        _cancelSub?.Dispose();

        if (DataContext is not AddCustomerViewModel vm) return;

        _confirmSub = vm.ConfirmCommand.Subscribe(customer => Close(customer));
        _cancelSub  = vm.CancelCommand.Subscribe(_ => Close(null));
    }

    protected override void OnClosed(EventArgs e)
    {
        _confirmSub?.Dispose();
        _cancelSub?.Dispose();
        base.OnClosed(e);
    }
}

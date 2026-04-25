using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;

namespace AvaloniaRentalApp.Views;

public partial class CustomersView : UserControl
{
    private System.IDisposable? _interactionSubscription;

    public CustomersView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        _interactionSubscription?.Dispose();
        if (DataContext is not CustomersViewModel vm) return;

        _interactionSubscription = vm.ShowAddCustomerDialog.RegisterHandler(async ctx =>
        {
            var dialog = new AddCustomerWindow { DataContext = ctx.Input };
            var parentWindow = (Avalonia.Application.Current?.ApplicationLifetime
                as IClassicDesktopStyleApplicationLifetime)?.MainWindow
                ?? throw new InvalidOperationException("No main window available.");
            var result = await dialog.ShowDialog<Customer?>(parentWindow);
            ctx.SetOutput(result);
        });
    }
}

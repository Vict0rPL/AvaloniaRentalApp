using System;
using Avalonia.Controls;
using AvaloniaRentalApp.ViewModels;

namespace AvaloniaRentalApp.Views;

public partial class CustomerDetailsWindow : Window
{
    private IDisposable? _closeSub;

    public CustomerDetailsWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        _closeSub?.Dispose();

        if (DataContext is not CustomerDetailsViewModel vm) return;

        _closeSub = vm.CloseCommand.Subscribe(_ => Close());
    }

    protected override void OnClosed(EventArgs e)
    {
        _closeSub?.Dispose();
        base.OnClosed(e);
    }
}

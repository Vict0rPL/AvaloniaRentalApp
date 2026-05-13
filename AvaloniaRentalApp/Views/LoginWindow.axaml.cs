using Avalonia.ReactiveUI;
using AvaloniaRentalApp.ViewModels;

namespace AvaloniaRentalApp.Views;

public partial class LoginWindow : ReactiveWindow<LoginViewModel>
{
    public LoginWindow()
    {
        InitializeComponent();
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using AvaloniaRentalApp.ViewModels;
using AvaloniaRentalApp.Views;

namespace AvaloniaRentalApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var db = new DatabaseService();
            var auth = new AuthService(db);
            ShowLogin(desktop, auth);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ShowLogin(IClassicDesktopStyleApplicationLifetime desktop, AuthService auth)
    {
        var oldWindow = desktop.MainWindow;

        // closedByAuth prevents Shutdown when we programmatically close on successful login
        bool closedByAuth = false;

        var login = new LoginWindow();
        login.DataContext = new LoginViewModel(auth, user =>
        {
            closedByAuth = true;
            ShowMain(desktop, auth, user, login);
        });

        login.Closing += (_, _) =>
        {
            if (!closedByAuth)
                desktop.Shutdown();
        };

        desktop.MainWindow = login;
        login.Show();
        oldWindow?.Close();
    }

    private void ShowMain(IClassicDesktopStyleApplicationLifetime desktop, AuthService auth, User user, Window oldWindow)
    {
        var main = new MainWindow
        {
            DataContext = new MainWindowViewModel(user, () => ShowLogin(desktop, auth))
        };

        desktop.MainWindow = main;
        main.Show();
        oldWindow.Close();
    }
}

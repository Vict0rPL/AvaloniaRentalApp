using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly Action<User> _onAuthenticated;

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    public LoginViewModel(AuthService authService, Action<User> onAuthenticated)
    {
        _authService = authService;
        _onAuthenticated = onAuthenticated;

        var canLogin = this.WhenAnyValue(
            x => x.Username,
            x => x.Password,
            x => x.IsBusy,
            (u, p, busy) => !busy && !string.IsNullOrWhiteSpace(u) && !string.IsNullOrEmpty(p));

        LoginCommand = ReactiveCommand.CreateFromTask(DoLoginAsync, canLogin);

        LoginCommand.ThrownExceptions
            .Subscribe(ex => ErrorMessage = $"Błąd połączenia: {ex.Message}");

        this.WhenAnyValue(x => x.ErrorMessage)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(HasError)));
    }

    private async Task DoLoginAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        var pwd = Password;
        Password = string.Empty;

        try
        {
            var result = await _authService.LoginAsync(Username, pwd);

            switch (result.Status)
            {
                case LoginStatus.Success:
                    _onAuthenticated(result.User!);
                    break;
                case LoginStatus.InvalidCredentials:
                    ErrorMessage = "Nieprawidłowy login lub hasło.";
                    break;
                case LoginStatus.Locked:
                    var localTime = result.LockedUntil.HasValue
                        ? result.LockedUntil.Value.ToLocalTime().ToString("HH:mm")
                        : "?";
                    ErrorMessage = $"Konto zablokowane do {localTime}. Zbyt wiele nieudanych prób.";
                    break;
                case LoginStatus.Inactive:
                    ErrorMessage = "Konto jest nieaktywne. Skontaktuj się z administratorem.";
                    break;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}

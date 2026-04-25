using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text.RegularExpressions;
using AvaloniaRentalApp.Models;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class AddCustomerViewModel : ViewModelBase
{
    // Required fields

    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
    }

    private string _phone = string.Empty;
    public string Phone
    {
        get => _phone;
        set => this.RaiseAndSetIfChanged(ref _phone, value);
    }

    private string _idDocument = string.Empty;
    public string IdDocument
    {
        get => _idDocument;
        set => this.RaiseAndSetIfChanged(ref _idDocument, value);
    }

    private string _idType = "dowod";
    public string IdType
    {
        get => _idType;
        set => this.RaiseAndSetIfChanged(ref _idType, value);
    }

    private string _licenseNumber = string.Empty;
    public string LicenseNumber
    {
        get => _licenseNumber;
        set => this.RaiseAndSetIfChanged(ref _licenseNumber, value);
    }

    // Optional fields

    private string? _email;
    public string? Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    private string? _pesel;
    public string? Pesel
    {
        get => _pesel;
        set => this.RaiseAndSetIfChanged(ref _pesel, value);
    }

    private DateTimeOffset? _licenseExpiry;
    public DateTimeOffset? LicenseExpiry
    {
        get => _licenseExpiry;
        set => this.RaiseAndSetIfChanged(ref _licenseExpiry, value);
    }

    private DateTimeOffset? _dateOfBirth;
    public DateTimeOffset? DateOfBirth
    {
        get => _dateOfBirth;
        set => this.RaiseAndSetIfChanged(ref _dateOfBirth, value);
    }

    private string? _addressStreet;
    public string? AddressStreet
    {
        get => _addressStreet;
        set => this.RaiseAndSetIfChanged(ref _addressStreet, value);
    }

    private string? _addressCity;
    public string? AddressCity
    {
        get => _addressCity;
        set => this.RaiseAndSetIfChanged(ref _addressCity, value);
    }

    private string? _addressZip;
    public string? AddressZip
    {
        get => _addressZip;
        set => this.RaiseAndSetIfChanged(ref _addressZip, value);
    }

    private string? _notes;
    public string? Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    // Error properties 

    private string? _firstNameError;
    public string? FirstNameError
    {
        get => _firstNameError;
        private set => this.RaiseAndSetIfChanged(ref _firstNameError, value);
    }

    private string? _lastNameError;
    public string? LastNameError
    {
        get => _lastNameError;
        private set => this.RaiseAndSetIfChanged(ref _lastNameError, value);
    }

    private string? _phoneError;
    public string? PhoneError
    {
        get => _phoneError;
        private set => this.RaiseAndSetIfChanged(ref _phoneError, value);
    }

    private string? _emailError;
    public string? EmailError
    {
        get => _emailError;
        private set => this.RaiseAndSetIfChanged(ref _emailError, value);
    }

    private string? _peselError;
    public string? PeselError
    {
        get => _peselError;
        private set => this.RaiseAndSetIfChanged(ref _peselError, value);
    }

    private string? _idDocumentError;
    public string? IdDocumentError
    {
        get => _idDocumentError;
        private set => this.RaiseAndSetIfChanged(ref _idDocumentError, value);
    }

    private string? _licenseNumberError;
    public string? LicenseNumberError
    {
        get => _licenseNumberError;
        private set => this.RaiseAndSetIfChanged(ref _licenseNumberError, value);
    }

    // Options

    public List<string> IdTypeOptions { get; } = ["dowod", "paszport", "prawo_jazdy"];

    // Commands

    public ReactiveCommand<Unit, Customer?> ConfirmCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public AddCustomerViewModel()
    {
        ConfirmCommand = ReactiveCommand.Create<Customer?>(() =>
            Validate() ? BuildCustomer() : null);

        CancelCommand = ReactiveCommand.Create(() => { });

        // Clear required field errors as soon as the user fills them in
        this.WhenAnyValue(x => x.FirstName)
            .Subscribe(_ => { if (!string.IsNullOrWhiteSpace(FirstName)) FirstNameError = null; });
        this.WhenAnyValue(x => x.LastName)
            .Subscribe(_ => { if (!string.IsNullOrWhiteSpace(LastName)) LastNameError = null; });
        this.WhenAnyValue(x => x.IdDocument)
            .Subscribe(_ => { if (!string.IsNullOrWhiteSpace(IdDocument)) IdDocumentError = null; });
        this.WhenAnyValue(x => x.LicenseNumber)
            .Subscribe(_ => { if (!string.IsNullOrWhiteSpace(LicenseNumber)) LicenseNumberError = null; });

        // Realtime format validation while the user types
        this.WhenAnyValue(x => x.Phone)
            .Subscribe(_ =>
            {
                if (string.IsNullOrWhiteSpace(Phone)) PhoneError = null;
                else ValidatePhoneFormat();
            });

        this.WhenAnyValue(x => x.Email)
            .Subscribe(_ => ValidateEmailFormat());

        this.WhenAnyValue(x => x.Pesel)
            .Subscribe(_ => ValidatePeselFormat());
    }

    // Validation

    private bool Validate()
    {
        FirstNameError     = string.IsNullOrWhiteSpace(FirstName)     ? "Imię jest wymagane."                   : null;
        LastNameError      = string.IsNullOrWhiteSpace(LastName)      ? "Nazwisko jest wymagane."               : null;
        IdDocumentError    = string.IsNullOrWhiteSpace(IdDocument)    ? "Dokument tożsamości jest wymagany."    : null;
        LicenseNumberError = string.IsNullOrWhiteSpace(LicenseNumber) ? "Nr prawa jazdy jest wymagany."        : null;

        if (string.IsNullOrWhiteSpace(Phone))
            PhoneError = "Telefon jest wymagany.";
        else
            ValidatePhoneFormat();

        ValidateEmailFormat();
        ValidatePeselFormat();

        return FirstNameError     == null &&
               LastNameError      == null &&
               PhoneError         == null &&
               IdDocumentError    == null &&
               LicenseNumberError == null &&
               EmailError         == null &&
               PeselError         == null;
    }

    private void ValidatePhoneFormat()
    {
        if (string.IsNullOrWhiteSpace(Phone)) return;
        PhoneError = Regex.IsMatch(Phone.Trim(), @"^[\d\s\-\+\(\)]{7,20}$")
            ? null
            : "Numer powinien zawierać 7–20 znaków (cyfry, spacje, myślniki).";
    }

    private void ValidateEmailFormat()
    {
        if (string.IsNullOrWhiteSpace(Email)) { EmailError = null; return; }
        var at = Email.IndexOf('@');
        EmailError = at > 0 && Email.IndexOf('.', at) > at + 1
            ? null
            : "Nieprawidłowy adres e-mail.";
    }

    private void ValidatePeselFormat()
    {
        if (string.IsNullOrWhiteSpace(Pesel)) { PeselError = null; return; }
        PeselError = Regex.IsMatch(Pesel, @"^\d{11}$")
            ? null
            : "PESEL musi składać się z dokładnie 11 cyfr.";
    }

    // Builder

    private Customer BuildCustomer() => new()
    {
        FirstName     = FirstName,
        LastName      = LastName,
        Phone         = Phone,
        IdDocument    = IdDocument,
        IdType        = IdType,
        LicenseNumber = LicenseNumber,
        LicenseExpiry = LicenseExpiry?.DateTime,
        Email         = NullIfBlank(Email),
        Pesel         = NullIfBlank(Pesel),
        AddressStreet = NullIfBlank(AddressStreet),
        AddressCity   = NullIfBlank(AddressCity),
        AddressZip    = NullIfBlank(AddressZip),
        DateOfBirth   = DateOfBirth?.DateTime,
        Notes         = NullIfBlank(Notes),
    };

    private static string? NullIfBlank(string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : s;
}

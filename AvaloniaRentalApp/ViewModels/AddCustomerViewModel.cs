using System;
using System.Collections.Generic;
using System.Reactive;
using AvaloniaRentalApp.Models;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class AddCustomerViewModel : ViewModelBase
{
    // Required
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

    // Optional
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

    public List<string> IdTypeOptions { get; } = ["dowod", "paszport", "prawo_jazdy"];

    public ReactiveCommand<Unit, Customer> ConfirmCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public AddCustomerViewModel()
    {
        var canConfirm = this.WhenAnyValue(
            x => x.FirstName, x => x.LastName, x => x.Phone, x => x.IdDocument, x => x.LicenseNumber,
            (fn, ln, ph, id, lic) =>
                !string.IsNullOrWhiteSpace(fn)  &&
                !string.IsNullOrWhiteSpace(ln)  &&
                !string.IsNullOrWhiteSpace(ph)  &&
                !string.IsNullOrWhiteSpace(id)  &&
                !string.IsNullOrWhiteSpace(lic));

        ConfirmCommand = ReactiveCommand.Create(BuildCustomer, canConfirm);
        CancelCommand  = ReactiveCommand.Create(() => { });
    }

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

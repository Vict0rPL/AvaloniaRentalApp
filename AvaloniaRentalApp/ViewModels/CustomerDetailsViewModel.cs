using System;
using System.Reactive;
using AvaloniaRentalApp.Models;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class CustomerDetailsViewModel : ViewModelBase
{
    public Customer Customer { get; }

    public int CustomerId       => Customer.CustomerId;
    public string FullName      => Customer.FullName;
    public string FirstName     => Customer.FirstName;
    public string LastName      => Customer.LastName;
    public string Phone         => Customer.Phone;
    public string? Email        => Customer.Email;
    public string? Pesel        => Customer.Pesel;
    public string IdDocument    => Customer.IdDocument;
    public string IdType        => Customer.IdType;
    public string LicenseNumber => Customer.LicenseNumber;
    public DateTime? LicenseExpiry  => Customer.LicenseExpiry;
    public DateTime? DateOfBirth    => Customer.DateOfBirth;
    public string? AddressStreet    => Customer.AddressStreet;
    public string? AddressCity      => Customer.AddressCity;
    public string? AddressZip       => Customer.AddressZip;
    public string? Notes            => Customer.Notes;
    public int ActiveRentals        => Customer.ActiveRentals;
    public bool IsBlacklisted       => Customer.IsBlacklisted;

    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    public CustomerDetailsViewModel(Customer customer)
    {
        Customer = customer;
        CloseCommand = ReactiveCommand.Create(() => { });
    }
}

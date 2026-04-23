using System;

namespace AvaloniaRentalApp.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Pesel { get; set; }
    public string IdDocument { get; set; } = string.Empty;
    public string IdType { get; set; } = "dowod";
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime? LicenseExpiry { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? AddressStreet { get; set; }
    public string? AddressCity { get; set; }
    public string? AddressZip { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? CompanyName { get; set; }
    public string? Nip { get; set; }
    public string? Notes { get; set; }
    public bool IsBlacklisted { get; set; }

    // computed in queries
    public int ActiveRentals { get; set; }

    // display helpers
    public string FullName => $"{FirstName} {LastName}";
    public string ShortName => $"{FirstName[0]}. {LastName}";
    public string Initials => $"{FirstName[0]}{LastName[0]}".ToUpper();
}

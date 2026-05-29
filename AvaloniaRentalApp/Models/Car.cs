using System;

namespace AvaloniaRentalApp.Models;

public class Car
{
    public int CarId { get; set; }
    public int CategoryId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public short Year { get; set; }
    public string Registration { get; set; } = string.Empty;
    public string Vin { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string FuelType { get; set; } = "benzyna";
    public string Transmission { get; set; } = "manualna";
    public byte Seats { get; set; } = 5;
    public int MileageKm { get; set; }
    public decimal? PurchasePrice { get; set; }      // market price when produced/bought
    public decimal? FuelConsumption { get; set; }    // L/100km (kWh/100km for electric)
    public string Status { get; set; } = "dostepny";
    public DateTime? InsuranceExpiry { get; set; }
    public DateTime? InspectionExpiry { get; set; }
    public string? ImagePath { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    // joined from category table
    public string CategoryName { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public decimal DepositAmount { get; set; }

    public string FullName => $"{Brand} {Model}";

    // display helpers
    public string DisplayStatus => Status switch
    {
        "dostepny" => "Dostępny",
        "wypozyczony" => "Wypożyczony",
        "serwis" => "Serwis",
        "wycofany" => "Wycofany",
        _ => Status
    };
    public string StatusBadge => Status switch
    {
        "dostepny" => "green",
        "wypozyczony" => "blue",
        "serwis" => "orange",
        "wycofany" => "red",
        _ => "gray"
    };
}

using System;

namespace AvaloniaRentalApp.Models;

public class MaintenanceRecord
{
    public int MaintenanceId { get; set; }
    public int CarId { get; set; }
    public string Type { get; set; } = "serwis";
    public DateTime Date { get; set; } = DateTime.Today;
    public decimal Cost { get; set; }
    public int? OdometerKm { get; set; }
    public string? Description { get; set; }

    // display helpers
    public string DisplayType => Type switch
    {
        "serwis"   => "Serwis",
        "przeglad" => "Przegląd",
        "naprawa"  => "Naprawa",
        _          => Type
    };

    public string TypeBadge => Type switch
    {
        "serwis"   => "blue",
        "przeglad" => "green",
        "naprawa"  => "orange",
        _          => "gray"
    };
}

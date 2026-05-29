using System;

namespace AvaloniaRentalApp.Models;

public class Rental
{
    public int RentalId { get; set; }
    public string RentalNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int CarId { get; set; }
    public int UserId { get; set; }

    public DateTime DateStart { get; set; }
    public DateTime DateEndPlanned { get; set; }
    public DateTime? DateEndActual { get; set; }

    public int MileageStart { get; set; }
    public int? MileageEnd { get; set; }

    public decimal DailyRate { get; set; }
    public int TotalDays { get; set; }
    public decimal BaseCost { get; set; }
    public decimal ExtraKmCost { get; set; }
    public decimal LateReturnCost { get; set; }
    public decimal DamageCost { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TotalCost { get; set; }
    public decimal DepositPaid { get; set; }
    public bool DepositReturned { get; set; }

    public string Status { get; set; } = "aktywna";
    public string PaymentStatus { get; set; } = "oczekuje";
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }

    // joined fields
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CarName { get; set; } = string.Empty;
    public string CarRegistration { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string CarFuelType { get; set; } = string.Empty;
    public decimal? CarFuelConsumption { get; set; }

    // km driven during this rental (0 until the car is returned)
    public int KmDriven => MileageEnd.HasValue ? MileageEnd.Value - MileageStart : 0;

    // display helpers
    public string DisplayStatus => Status switch
    {
        "aktywna" => "Aktywna",
        "zakonczona" => "Zakończona",
        "anulowana" => "Anulowana",
        "przeterminowana" => "Przeterminowana",
        _ => Status
    };

    public string DisplayPayment => PaymentStatus switch
    {
        "oczekuje" => "Oczekuje",
        "oplacona" => "Opłacona",
        "czesciowa" => "Częściowa",
        "zalegla" => "Zaległa",
        _ => PaymentStatus
    };

    public string StatusBadge => Status switch
    {
        "aktywna" => "green",
        "zakonczona" => "gray",
        "anulowana" => "red",
        "przeterminowana" => "orange",
        _ => "gray"
    };

    public string PaymentBadge => PaymentStatus switch
    {
        "oplacona" => "green",
        "oczekuje" => "blue",
        "czesciowa" => "orange",
        "zalegla" => "red",
        _ => "gray"
    };

    public int DaysRemaining => (DateEndPlanned - DateTime.Now).Days;
    public string DateRange => $"{DateStart:dd.MM} – {DateEndPlanned:dd.MM.yyyy}";
    public string CarDisplay => $"{CarName} ({CarRegistration})";
}

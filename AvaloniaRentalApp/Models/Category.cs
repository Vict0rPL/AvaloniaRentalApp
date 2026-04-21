namespace AvaloniaRentalApp.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DailyRate { get; set; }
    public decimal? WeekendRate { get; set; }
    public decimal? WeeklyRate { get; set; }
    public decimal DepositAmount { get; set; }
    public int? MileageLimit { get; set; }
    public decimal? ExtraKmRate { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FleetStatusRow
{
    public string CategoryName { get; set; } = string.Empty;
    public int TotalCars { get; set; }
    public int Available { get; set; }
    public int Rented { get; set; }
    public int InService { get; set; }
    public decimal DailyRate { get; set; }
}

public class AlertItem
{
    public string Type { get; set; } = "info"; // info, warning, danger
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;

    public string Icon => Type switch
    {
        "danger" => "⛔",
        "warning" => "⚠",
        _ => "ℹ"
    };
}

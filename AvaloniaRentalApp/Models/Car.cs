using System;

namespace AvaloniaRentalApp.Models
{
    public class Car
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Vin { get; set; } = string.Empty;
        public int ProductionYear { get; set; }
        public int Mileage { get; set; }
        public string Status { get; set; } = "Available";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

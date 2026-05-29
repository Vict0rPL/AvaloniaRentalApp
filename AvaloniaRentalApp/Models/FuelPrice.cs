namespace AvaloniaRentalApp.Models;

public class FuelPrice
{
    public string FuelType { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }   // PLN per litre (per kWh for 'elektryczny')
    public string Unit { get; set; } = "l";

    // display helper
    public string DisplayType => FuelType switch
    {
        "benzyna"     => "Benzyna",
        "diesel"      => "Diesel",
        "LPG"         => "LPG",
        "hybryda"     => "Hybryda",
        "elektryczny" => "Elektryczny",
        _             => FuelType
    };
}

using System.Collections.Generic;
using System.Linq;
using AvaloniaRentalApp.Models;

namespace AvaloniaRentalApp.Services;

/// <summary>
/// Pure-calculation helpers for the cost-aware revenue model. No database access here,
/// callers pass already-loaded data so the algorithms stay unit-testable.
/// </summary>
public class PricingService
{
    /// <summary>Fuel price lookup keyed by fuel type (e.g. "benzyna" → 6.50).</summary>
    public static IReadOnlyDictionary<string, decimal> BuildPriceMap(IEnumerable<FuelPrice> prices)
        => prices.ToDictionary(p => p.FuelType, p => p.PricePerUnit);

    /// <summary>
    /// Fuel cost for a trip: (km / 100) × consumption(per 100 km) × price-per-unit for the fuel type.
    /// Returns 0 when consumption is unknown/zero or no price is configured.
    /// </summary>
    public decimal FuelCost(string fuelType, decimal? consumptionPer100Km, int kmDriven,
                            IReadOnlyDictionary<string, decimal> prices)
    {
        if (consumptionPer100Km is null or <= 0 || kmDriven <= 0)
            return 0m;

        var price = prices.TryGetValue(fuelType, out var p) ? p : 0m;
        return (kmDriven / 100m) * consumptionPer100Km.Value * price;
    }

    /// <summary>Fuel cost for a single rental, using its joined car fuel data and km driven.</summary>
    public decimal RentalFuelCost(Rental rental, IReadOnlyDictionary<string, decimal> prices)
        => FuelCost(rental.CarFuelType, rental.CarFuelConsumption, rental.KmDriven, prices);

    /// <summary>
    /// The components behind net revenue: gross paid income, fuel cost of those rentals,
    /// and maintenance/inspection costs in the period. Net = Gross − Fuel − Maintenance.
    /// </summary>
    public record RevenueBreakdown(decimal Gross, decimal Fuel, decimal Maintenance)
    {
        public decimal Net => Gross - Fuel - Maintenance;
    }

    /// <summary>
    /// Breaks period revenue into gross income, fuel cost of those rentals, and maintenance
    /// costs incurred in the period — so the dashboard can show why net differs from gross.
    /// </summary>
    public RevenueBreakdown Breakdown(IEnumerable<Rental> paidRentals,
                                      IEnumerable<MaintenanceRecord> maintenanceInPeriod,
                                      IReadOnlyDictionary<string, decimal> prices)
    {
        var paid = paidRentals.ToList();
        return new RevenueBreakdown(
            paid.Sum(r => r.TotalCost),
            paid.Sum(r => RentalFuelCost(r, prices)),
            maintenanceInPeriod.Sum(m => m.Cost));
    }

    /// <summary>
    /// Net ("real") revenue for a period = paid rental income − fuel cost of those rentals
    /// − maintenance/inspection costs incurred in the period.
    /// </summary>
    public decimal NetRevenue(IEnumerable<Rental> paidRentals,
                              IEnumerable<MaintenanceRecord> maintenanceInPeriod,
                              IReadOnlyDictionary<string, decimal> prices)
        => Breakdown(paidRentals, maintenanceInPeriod, prices).Net;

}

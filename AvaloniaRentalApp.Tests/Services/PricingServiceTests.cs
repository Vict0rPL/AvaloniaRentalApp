using System.Collections.Generic;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;

namespace AvaloniaRentalApp.Tests.Services;

[TestFixture]
public class PricingServiceTests
{
    private PricingService _pricing = null!;
    private IReadOnlyDictionary<string, decimal> _prices = null!;

    [SetUp]
    public void SetUp()
    {
        _pricing = new PricingService();
        _prices = PricingService.BuildPriceMap(new[]
        {
            new FuelPrice { FuelType = "benzyna", PricePerUnit = 6.50m, Unit = "l" },
            new FuelPrice { FuelType = "diesel",  PricePerUnit = 6.70m, Unit = "l" },
        });
    }

    [Test]
    public void FuelCost_ComputesKmTimesConsumptionTimesPrice()
    {
        // 200 km, 7.5 L/100km, 6.50 zł/L  ->  (200/100)*7.5*6.50 = 97.50
        var cost = _pricing.FuelCost("benzyna", 7.5m, 200, _prices);
        Assert.That(cost, Is.EqualTo(97.50m));
    }

    [Test]
    public void FuelCost_ZeroWhenConsumptionNull()
    {
        Assert.That(_pricing.FuelCost("benzyna", null, 200, _prices), Is.EqualTo(0m));
    }

    [Test]
    public void FuelCost_ZeroWhenNoKmDriven()
    {
        Assert.That(_pricing.FuelCost("benzyna", 7.5m, 0, _prices), Is.EqualTo(0m));
    }

    [Test]
    public void FuelCost_ZeroWhenFuelTypeHasNoConfiguredPrice()
    {
        Assert.That(_pricing.FuelCost("LPG", 9.5m, 100, _prices), Is.EqualTo(0m));
    }

    [Test]
    public void RentalFuelCost_UsesJoinedCarDataAndKmDriven()
    {
        var rental = new Rental
        {
            CarFuelType = "diesel",
            CarFuelConsumption = 6.0m,
            MileageStart = 10_000,
            MileageEnd = 10_500   // 500 km driven
        };
        // (500/100)*6.0*6.70 = 201.00
        Assert.That(_pricing.RentalFuelCost(rental, _prices), Is.EqualTo(201.00m));
    }

    [Test]
    public void NetRevenue_SubtractsFuelAndMaintenanceFromGross()
    {
        var paid = new List<Rental>
        {
            new()
            {
                TotalCost = 1000m,
                CarFuelType = "benzyna",
                CarFuelConsumption = 7.5m,
                MileageStart = 0,
                MileageEnd = 200   // fuel = 97.50
            }
        };
        var maintenance = new List<MaintenanceRecord>
        {
            new() { Cost = 150m }
        };

        // 1000 - 97.50 - 150 = 752.50
        var net = _pricing.NetRevenue(paid, maintenance, _prices);
        Assert.That(net, Is.EqualTo(752.50m));
    }
}

using System;
using AvaloniaRentalApp.Models;

namespace AvaloniaRentalApp.Tests.Models;

[TestFixture]
public class CarTests
{
    [Test]
    public void FullName_CombinesBrandAndModel()
    {
        var car = new Car { Brand = "Toyota", Model = "Corolla" };
        Assert.That(car.FullName, Is.EqualTo("Toyota Corolla"));
    }

    [TestCase("dostepny", "Dostępny")]
    [TestCase("wypozyczony", "Wypożyczony")]
    [TestCase("serwis", "Serwis")]
    [TestCase("wycofany", "Wycofany")]
    [TestCase("unknown", "unknown")]
    public void DisplayStatus_MapsStatusToPolishLabel(string status, string expected)
    {
        var car = new Car { Status = status };
        Assert.That(car.DisplayStatus, Is.EqualTo(expected));
    }

    [TestCase("dostepny", "green")]
    [TestCase("wypozyczony", "blue")]
    [TestCase("serwis", "orange")]
    [TestCase("wycofany", "red")]
    [TestCase("unknown", "gray")]
    public void StatusBadge_MapsStatusToColor(string status, string expected)
    {
        var car = new Car { Status = status };
        Assert.That(car.StatusBadge, Is.EqualTo(expected));
    }

    [Test]
    public void IsInsuranceExpired_TrueWhenDateInPast()
    {
        var car = new Car { InsuranceExpiry = DateTime.Now.Date.AddDays(-1) };
        Assert.That(car.IsInsuranceExpired, Is.True);
    }

    [Test]
    public void IsInsuranceExpired_FalseWhenValidOrNull()
    {
        Assert.That(new Car { InsuranceExpiry = DateTime.Now.Date.AddDays(10) }.IsInsuranceExpired, Is.False);
        Assert.That(new Car { InsuranceExpiry = null }.IsInsuranceExpired, Is.False);
    }

    [Test]
    public void IsInspectionExpired_TrueWhenDateInPast()
    {
        var car = new Car { InspectionExpiry = DateTime.Now.Date.AddDays(-1) };
        Assert.That(car.IsInspectionExpired, Is.True);
    }

    [Test]
    public void IsRentBlocked_TrueWhenEitherDocumentExpired()
    {
        Assert.That(new Car { InsuranceExpiry = DateTime.Now.Date.AddDays(-1) }.IsRentBlocked, Is.True);
        Assert.That(new Car { InspectionExpiry = DateTime.Now.Date.AddDays(-1) }.IsRentBlocked, Is.True);
    }

    [Test]
    public void IsRentBlocked_FalseWhenBothValid()
    {
        var car = new Car
        {
            InsuranceExpiry = DateTime.Now.Date.AddDays(30),
            InspectionExpiry = DateTime.Now.Date.AddDays(30)
        };
        Assert.That(car.IsRentBlocked, Is.False);
        Assert.That(car.RentBlockReason, Is.Null);
    }

    [Test]
    public void RentBlockReason_NamesExpiredDocuments()
    {
        var past = DateTime.Now.Date.AddDays(-1);
        Assert.That(new Car { InsuranceExpiry = past }.RentBlockReason, Does.Contain("OC"));
        Assert.That(new Car { InspectionExpiry = past }.RentBlockReason, Does.Contain("przegląd"));
        Assert.That(new Car { InsuranceExpiry = past, InspectionExpiry = past }.RentBlockReason,
            Does.Contain("OC").And.Contain("przegląd"));
    }
}

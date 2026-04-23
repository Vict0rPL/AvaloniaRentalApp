using AvaloniaRentalApp.Models;

namespace AvaloniaRentalApp.Tests.Models;

[TestFixture]
public class RentalTests
{
    [TestCase("aktywna", "Aktywna")]
    [TestCase("zakonczona", "Zakończona")]
    [TestCase("anulowana", "Anulowana")]
    [TestCase("przeterminowana", "Przeterminowana")]
    [TestCase("unknown", "unknown")]
    public void DisplayStatus_MapsStatusToPolishLabel(string status, string expected)
    {
        var rental = new Rental { Status = status };
        Assert.That(rental.DisplayStatus, Is.EqualTo(expected));
    }

    [TestCase("oczekuje", "Oczekuje")]
    [TestCase("oplacona", "Opłacona")]
    [TestCase("czesciowa", "Częściowa")]
    [TestCase("zalegla", "Zaległa")]
    [TestCase("unknown", "unknown")]
    public void DisplayPayment_MapsPaymentStatusToPolishLabel(string paymentStatus, string expected)
    {
        var rental = new Rental { PaymentStatus = paymentStatus };
        Assert.That(rental.DisplayPayment, Is.EqualTo(expected));
    }

    [TestCase("aktywna", "green")]
    [TestCase("zakonczona", "gray")]
    [TestCase("anulowana", "red")]
    [TestCase("przeterminowana", "orange")]
    [TestCase("unknown", "gray")]
    public void StatusBadge_MapsStatusToColor(string status, string expected)
    {
        var rental = new Rental { Status = status };
        Assert.That(rental.StatusBadge, Is.EqualTo(expected));
    }

    [TestCase("oplacona", "green")]
    [TestCase("oczekuje", "blue")]
    [TestCase("czesciowa", "orange")]
    [TestCase("zalegla", "red")]
    [TestCase("unknown", "gray")]
    public void PaymentBadge_MapsPaymentStatusToColor(string paymentStatus, string expected)
    {
        var rental = new Rental { PaymentStatus = paymentStatus };
        Assert.That(rental.PaymentBadge, Is.EqualTo(expected));
    }

    [Test]
    public void DateRange_FormatsStartAndEndDates()
    {
        var rental = new Rental
        {
            DateStart = new DateTime(2025, 1, 1),
            DateEndPlanned = new DateTime(2025, 1, 10)
        };
        Assert.That(rental.DateRange, Is.EqualTo("01.01 – 10.01.2025"));
    }

    [Test]
    public void CarDisplay_CombinesCarNameAndRegistration()
    {
        var rental = new Rental { CarName = "Toyota Corolla", CarRegistration = "WA12345" };
        Assert.That(rental.CarDisplay, Is.EqualTo("Toyota Corolla (WA12345)"));
    }

    [Test]
    public void DaysRemaining_IsPositiveForFutureDate()
    {
        var rental = new Rental { DateEndPlanned = DateTime.Now.AddDays(5) };
        Assert.That(rental.DaysRemaining, Is.GreaterThan(0));
    }

    [Test]
    public void DaysRemaining_IsNegativeForPastDate()
    {
        var rental = new Rental { DateEndPlanned = DateTime.Now.AddDays(-3) };
        Assert.That(rental.DaysRemaining, Is.LessThan(0));
    }
}

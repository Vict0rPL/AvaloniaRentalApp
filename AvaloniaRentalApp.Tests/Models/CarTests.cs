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
}

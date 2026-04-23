using AvaloniaRentalApp.Models;

namespace AvaloniaRentalApp.Tests.Models;

[TestFixture]
public class CustomerTests
{
    private Customer _customer = null!;

    [SetUp]
    public void SetUp() => _customer = new Customer { FirstName = "Jan", LastName = "Kowalski" };

    [Test]
    public void FullName_CombinesFirstAndLastName()
    {
        Assert.That(_customer.FullName, Is.EqualTo("Jan Kowalski"));
    }

    [Test]
    public void ShortName_UsesFirstInitialAndLastName()
    {
        Assert.That(_customer.ShortName, Is.EqualTo("J. Kowalski"));
    }

    [Test]
    public void Initials_ReturnsUppercasedFirstLettersOfBothNames()
    {
        Assert.That(_customer.Initials, Is.EqualTo("JK"));
    }

    [Test]
    public void Initials_IsAlwaysUppercase()
    {
        var customer = new Customer { FirstName = "anna", LastName = "nowak" };
        Assert.That(_customer.Initials, Is.EqualTo(_customer.Initials.ToUpper()));
    }
}

using AvaloniaRentalApp.Models;

namespace AvaloniaRentalApp.Tests.Models;

[TestFixture]
public class UserTests
{
    [Test]
    public void IsAdmin_ReturnsTrueForAdminRole()
    {
        var user = new User { Role = "admin" };
        Assert.That(user.IsAdmin, Is.True);
    }

    [Test]
    public void IsAdmin_ReturnsFalseForEmployeeRole()
    {
        var user = new User { Role = "employee" };
        Assert.That(user.IsAdmin, Is.False);
    }

    [TestCase("admin", "Administrator")]
    [TestCase("employee", "Pracownik")]
    [TestCase("other", "Pracownik")]
    public void RoleDisplay_MapsRoleToPolishLabel(string role, string expected)
    {
        var user = new User { Role = role };
        Assert.That(user.RoleDisplay, Is.EqualTo(expected));
    }

    [TestCase("Jan Kowalski", "JK")]
    [TestCase("Anna Nowak", "AN")]
    [TestCase("Jan Krzysztof Kowalski", "JK")]
    public void Initials_ReturnsFirstTwoWordInitialsUppercased(string fullName, string expected)
    {
        var user = new User { FullName = fullName };
        Assert.That(user.Initials, Is.EqualTo(expected));
    }

    [Test]
    public void Initials_SingleNameReturnsFirstLetter()
    {
        var user = new User { FullName = "Anna" };
        Assert.That(user.Initials, Is.EqualTo("A"));
    }
}

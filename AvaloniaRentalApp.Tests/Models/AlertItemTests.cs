using AvaloniaRentalApp.Models;

namespace AvaloniaRentalApp.Tests.Models;

[TestFixture]
public class AlertItemTests
{
    [TestCase("danger", "⛔")]
    [TestCase("warning", "⚠")]
    [TestCase("info", "ℹ")]
    [TestCase("unknown", "ℹ")]
    public void Icon_MapsTypeToEmoji(string type, string expected)
    {
        var alert = new AlertItem { Type = type };
        Assert.That(alert.Icon, Is.EqualTo(expected));
    }
}

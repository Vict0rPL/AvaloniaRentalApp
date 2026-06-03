using AvaloniaRentalApp.Services;

namespace AvaloniaRentalApp.Tests.Services;

[TestFixture]
public class ConnectionStringTests
{
    private const string DefaultConn =
        "Server=localhost;Database=CarRentalDB;User=root;Password=root;AllowUserVariables=True";

    [Test]
    public void ParseConnectionString_ReadsFieldsFromDefault()
    {
        var (host, port, database, user, _) = DatabaseService.ParseConnectionString(DefaultConn);

        Assert.Multiple(() =>
        {
            Assert.That(host, Is.EqualTo("localhost"));
            Assert.That(port, Is.EqualTo(3306u)); // MySqlConnector default
            Assert.That(database, Is.EqualTo("CarRentalDB"));
            Assert.That(user, Is.EqualTo("root"));
        });
    }

    [Test]
    public void BuildConnectionString_OverridesFields()
    {
        var result = DatabaseService.BuildConnectionString(
            DefaultConn, "db.example.com", 3307, "OtherDb", "appuser", "pw");

        var (host, port, database, user, password) = DatabaseService.ParseConnectionString(result);

        Assert.Multiple(() =>
        {
            Assert.That(host, Is.EqualTo("db.example.com"));
            Assert.That(port, Is.EqualTo(3307u));
            Assert.That(database, Is.EqualTo("OtherDb"));
            Assert.That(user, Is.EqualTo("appuser"));
            Assert.That(password, Is.EqualTo("pw"));
        });
    }

    [Test]
    public void BuildConnectionString_PreservesExtraOptions()
    {
        var result = DatabaseService.BuildConnectionString(
            DefaultConn, "localhost", 3306, "CarRentalDB", "root", "root");

        // The builder normalizes the key to "Allow User Variables"; the option must survive.
        var rebuilt = new MySqlConnector.MySqlConnectionStringBuilder(result);
        Assert.That(rebuilt.AllowUserVariables, Is.True);
    }

    [Test]
    public void ParseThenBuild_RoundTrips()
    {
        var (host, port, database, user, password) = DatabaseService.ParseConnectionString(DefaultConn);
        var rebuilt = DatabaseService.BuildConnectionString(DefaultConn, host, port, database, user, password);

        var (host2, port2, database2, user2, password2) = DatabaseService.ParseConnectionString(rebuilt);

        Assert.Multiple(() =>
        {
            Assert.That(host2, Is.EqualTo(host));
            Assert.That(port2, Is.EqualTo(port));
            Assert.That(database2, Is.EqualTo(database));
            Assert.That(user2, Is.EqualTo(user));
            Assert.That(password2, Is.EqualTo(password));
        });
    }
}

using System.Reactive.Concurrency;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.ViewModels;
using ReactiveUI;

namespace AvaloniaRentalApp.Tests.ViewModels;

[TestFixture]
public class AddCustomerViewModelTests
{
    private AddCustomerViewModel _vm = null!;

    [SetUp]
    public void SetUp()
    {
        // Run all Rx operations synchronously so tests don't need a UI thread
        RxApp.MainThreadScheduler = ImmediateScheduler.Instance;
        _vm = new AddCustomerViewModel();
    }

    // Helpers

    private void FillRequired()
    {
        _vm.FirstName     = "Jan";
        _vm.LastName      = "Kowalski";
        _vm.Phone         = "600100200";
        _vm.IdDocument    = "ABC 123456";
        _vm.LicenseNumber = "WI/12345/2020";
    }

    private Customer? Confirm()
    {
        Customer? result = null;
        _vm.ConfirmCommand.Execute().Subscribe(r => result = r);
        return result;
    }

    // Required field errors on empty submit

    [Test]
    public void Confirm_AllEmpty_SetsAllRequiredErrors()
    {
        Confirm();
        Assert.Multiple(() =>
        {
            Assert.That(_vm.FirstNameError,     Is.Not.Null);
            Assert.That(_vm.LastNameError,      Is.Not.Null);
            Assert.That(_vm.PhoneError,         Is.Not.Null);
            Assert.That(_vm.IdDocumentError,    Is.Not.Null);
            Assert.That(_vm.LicenseNumberError, Is.Not.Null);
        });
    }

    [Test]
    public void Confirm_AllEmpty_ReturnsNull()
    {
        Assert.That(Confirm(), Is.Null);
    }

    // Required field errors clear when field is fixed

    [TestCase(nameof(AddCustomerViewModel.FirstName),     nameof(AddCustomerViewModel.FirstNameError))]
    [TestCase(nameof(AddCustomerViewModel.LastName),      nameof(AddCustomerViewModel.LastNameError))]
    [TestCase(nameof(AddCustomerViewModel.IdDocument),    nameof(AddCustomerViewModel.IdDocumentError))]
    [TestCase(nameof(AddCustomerViewModel.LicenseNumber), nameof(AddCustomerViewModel.LicenseNumberError))]
    public void RequiredFieldError_ClearsWhenFieldFilledAfterFailedConfirm(
        string fieldName, string errorPropName)
    {
        Confirm(); // trigger errors

        var errorBefore = (string?)typeof(AddCustomerViewModel)
            .GetProperty(errorPropName)!.GetValue(_vm);
        Assert.That(errorBefore, Is.Not.Null, "error should be set after failed confirm");

        typeof(AddCustomerViewModel)
            .GetProperty(fieldName)!.SetValue(_vm, "SomeValue");

        var errorAfter = (string?)typeof(AddCustomerViewModel)
            .GetProperty(errorPropName)!.GetValue(_vm);
        Assert.That(errorAfter, Is.Null, "error should clear once field is filled");
    }

    // Phone format validation

    [TestCase("ab")]
    [TestCase("12")]
    [TestCase("abc!@#")]
    public void PhoneError_SetForInvalidFormat(string phone)
    {
        _vm.Phone = phone;
        Assert.That(_vm.PhoneError, Is.Not.Null);
    }

    [TestCase("600100200")]
    [TestCase("+48 600 100 200")]
    [TestCase("(22) 123-4567")]
    public void PhoneError_NullForValidFormat(string phone)
    {
        _vm.Phone = phone;
        Assert.That(_vm.PhoneError, Is.Null);
    }

    [Test]
    public void PhoneError_NullWhenPhoneCleared()
    {
        _vm.Phone = "!";
        _vm.Phone = string.Empty;
        Assert.That(_vm.PhoneError, Is.Null);
    }

    // Email format validation

    [TestCase("notanemail")]
    [TestCase("missing@dot")]
    [TestCase("@nodomain.com")]
    public void EmailError_SetForInvalidFormat(string email)
    {
        _vm.Email = email;
        Assert.That(_vm.EmailError, Is.Not.Null);
    }

    [TestCase("jan@email.pl")]
    [TestCase("user@domain.com")]
    public void EmailError_NullForValidFormat(string email)
    {
        _vm.Email = email;
        Assert.That(_vm.EmailError, Is.Null);
    }

    [Test]
    public void EmailError_NullWhenEmailCleared()
    {
        _vm.Email = "bad@";
        _vm.Email = null;
        Assert.That(_vm.EmailError, Is.Null);
    }

    // PESEL format validation

    [TestCase("1234567890")]    // 10 digits
    [TestCase("123456789012")]  // 12 digits
    [TestCase("abcdefghijk")]   // letters
    public void PeselError_SetForInvalidFormat(string pesel)
    {
        _vm.Pesel = pesel;
        Assert.That(_vm.PeselError, Is.Not.Null);
    }

    [Test]
    public void PeselError_NullForValid11DigitPesel()
    {
        _vm.Pesel = "85050512345";
        Assert.That(_vm.PeselError, Is.Null);
    }

    [Test]
    public void PeselError_NullWhenPeselCleared()
    {
        _vm.Pesel = "123";
        _vm.Pesel = null;
        Assert.That(_vm.PeselError, Is.Null);
    }

    // Successful confirm

    [Test]
    public void Confirm_WithValidRequiredFields_ReturnsNonNullCustomer()
    {
        FillRequired();
        Assert.That(Confirm(), Is.Not.Null);
    }

    [Test]
    public void Confirm_WithValidRequiredFields_ClearsAllErrors()
    {
        FillRequired();
        Confirm();
        Assert.Multiple(() =>
        {
            Assert.That(_vm.FirstNameError,     Is.Null);
            Assert.That(_vm.LastNameError,      Is.Null);
            Assert.That(_vm.PhoneError,         Is.Null);
            Assert.That(_vm.IdDocumentError,    Is.Null);
            Assert.That(_vm.LicenseNumberError, Is.Null);
        });
    }

    [Test]
    public void Confirm_MapsAllFieldsOntoCustomer()
    {
        FillRequired();
        _vm.Email          = "jan@email.pl";
        _vm.Pesel          = "85050512345";
        _vm.AddressCity    = "Kielce";
        _vm.AddressStreet  = "ul. Lipowa 15";
        _vm.AddressZip     = "25-001";
        _vm.IdType         = "paszport";

        var c = Confirm()!;
        Assert.Multiple(() =>
        {
            Assert.That(c.FirstName,     Is.EqualTo("Jan"));
            Assert.That(c.LastName,      Is.EqualTo("Kowalski"));
            Assert.That(c.Phone,         Is.EqualTo("600100200"));
            Assert.That(c.Email,         Is.EqualTo("jan@email.pl"));
            Assert.That(c.Pesel,         Is.EqualTo("85050512345"));
            Assert.That(c.AddressCity,   Is.EqualTo("Kielce"));
            Assert.That(c.AddressStreet, Is.EqualTo("ul. Lipowa 15"));
            Assert.That(c.AddressZip,    Is.EqualTo("25-001"));
            Assert.That(c.IdType,        Is.EqualTo("paszport"));
        });
    }

    [Test]
    public void Confirm_WhitespaceOptionalFields_AreNullInCustomer()
    {
        FillRequired();
        _vm.Email       = "   ";
        _vm.Pesel       = "  ";
        _vm.AddressCity = "\t";
        _vm.Notes       = "";

        var c = Confirm()!;
        Assert.Multiple(() =>
        {
            Assert.That(c.Email,       Is.Null);
            Assert.That(c.Pesel,       Is.Null);
            Assert.That(c.AddressCity, Is.Null);
            Assert.That(c.Notes,       Is.Null);
        });
    }

    // Invalid optional field blocks confirm

    [Test]
    public void Confirm_WithInvalidEmail_ReturnsNull()
    {
        FillRequired();
        _vm.Email = "notvalid";
        Assert.That(Confirm(), Is.Null);
    }

    [Test]
    public void Confirm_WithInvalidPesel_ReturnsNull()
    {
        FillRequired();
        _vm.Pesel = "123";
        Assert.That(Confirm(), Is.Null);
    }
}

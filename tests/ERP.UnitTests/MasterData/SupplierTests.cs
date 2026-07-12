using ERP.Domain.MasterData;
using ERP.Domain.MasterData.Events;

namespace ERP.UnitTests.MasterData;

public sealed class SupplierTests
{
    [Fact]
    public void Create_ShouldCreateActiveSupplierAndRegisterEvent_WhenDataIsValid()
    {
        var supplier = CreateSupplier();

        Assert.NotEqual(Guid.Empty, supplier.Id);
        Assert.Equal("SUP-001", supplier.Code.Value);
        Assert.True(supplier.IsActive);
        Assert.Contains(supplier.DomainEvents, domainEvent => domainEvent is SupplierCreated);
    }

    [Fact]
    public void UpdateDetails_ShouldNotChangeCode()
    {
        var supplier = CreateSupplier();

        supplier.UpdateDetails("Updated Supplier", DateTime.UtcNow);

        Assert.Equal("SUP-001", supplier.Code.Value);
        Assert.Equal("Updated Supplier", supplier.Name);
    }

    [Fact]
    public void AddContact_ShouldAddContact_WhenDataIsValid()
    {
        var supplier = CreateSupplier();

        var contact = supplier.AddContact("Ana Silva", "ana@example.com", "+244 900 000 000");

        Assert.Contains(contact, supplier.Contacts);
        Assert.Equal("Ana Silva", contact.Name);
        Assert.Equal("ana@example.com", contact.Email);
    }

    [Fact]
    public void AddContact_ShouldThrow_WhenEmailIsInvalid()
    {
        var supplier = CreateSupplier();

        Assert.Throws<ArgumentException>(() => supplier.AddContact("Ana Silva", "invalid", null));
    }

    [Fact]
    public void RemoveContact_ShouldRemoveContact_WhenContactExists()
    {
        var supplier = CreateSupplier();
        var contact = supplier.AddContact("Ana Silva", null, null);

        supplier.RemoveContact(contact.Id);

        Assert.Empty(supplier.Contacts);
    }

    [Fact]
    public void AddAddress_ShouldAddAddress_WhenDataIsValid()
    {
        var supplier = CreateSupplier();

        var address = supplier.AddAddress("Rua Principal", null, "Luanda", "1000", "Angola");

        Assert.Contains(address, supplier.Addresses);
        Assert.Equal("Luanda", address.City);
    }

    [Fact]
    public void RemoveAddress_ShouldRemoveAddress_WhenAddressExists()
    {
        var supplier = CreateSupplier();
        var address = supplier.AddAddress("Rua Principal", null, "Luanda", "1000", "Angola");

        supplier.RemoveAddress(address.Id);

        Assert.Empty(supplier.Addresses);
    }

    [Fact]
    public void Deactivate_ShouldSoftDeactivateSupplierAndRegisterEvent()
    {
        var supplier = CreateSupplier();

        supplier.Deactivate(DateTime.UtcNow);

        Assert.False(supplier.IsActive);
        Assert.NotNull(supplier.DeactivatedAtUtc);
        Assert.Contains(supplier.DomainEvents, domainEvent => domainEvent is SupplierDeactivated);
    }

    private static Supplier CreateSupplier()
    {
        return Supplier.Create(SupplierCode.Create("sup-001"), "Sample Supplier", DateTime.UtcNow);
    }
}

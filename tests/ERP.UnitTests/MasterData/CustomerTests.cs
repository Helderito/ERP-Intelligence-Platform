using ERP.Domain.MasterData;
using ERP.Domain.MasterData.Events;

namespace ERP.UnitTests.MasterData;

public sealed class CustomerTests
{
    [Fact]
    public void Create_ShouldCreateActiveCustomerAndRegisterEvent_WhenDataIsValid()
    {
        var customer = CreateCustomer();

        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("CUS-001", customer.Code.Value);
        Assert.True(customer.IsActive);
        Assert.Contains(customer.DomainEvents, domainEvent => domainEvent is CustomerCreated);
    }

    [Fact]
    public void UpdateDetails_ShouldNotChangeCode()
    {
        var customer = CreateCustomer();

        customer.UpdateDetails("Updated Customer", DateTime.UtcNow);

        Assert.Equal("CUS-001", customer.Code.Value);
        Assert.Equal("Updated Customer", customer.Name);
    }

    [Fact]
    public void AddContact_ShouldAddContact_WhenDataIsValid()
    {
        var customer = CreateCustomer();

        var contact = customer.AddContact("Ana Silva", "ana@example.com", "+244 900 000 000");

        Assert.Contains(contact, customer.Contacts);
        Assert.Equal("Ana Silva", contact.Name);
        Assert.Equal("ana@example.com", contact.Email);
    }

    [Fact]
    public void AddContact_ShouldThrow_WhenEmailIsInvalid()
    {
        var customer = CreateCustomer();

        Assert.Throws<ArgumentException>(() => customer.AddContact("Ana Silva", "invalid", null));
    }

    [Fact]
    public void RemoveContact_ShouldRemoveContact_WhenContactExists()
    {
        var customer = CreateCustomer();
        var contact = customer.AddContact("Ana Silva", null, null);

        customer.RemoveContact(contact.Id);

        Assert.Empty(customer.Contacts);
    }

    [Fact]
    public void AddAddress_ShouldAddAddress_WhenDataIsValid()
    {
        var customer = CreateCustomer();

        var address = customer.AddAddress("Rua Principal", null, "Luanda", "1000", "Angola");

        Assert.Contains(address, customer.Addresses);
        Assert.Equal("Luanda", address.City);
    }

    [Fact]
    public void RemoveAddress_ShouldRemoveAddress_WhenAddressExists()
    {
        var customer = CreateCustomer();
        var address = customer.AddAddress("Rua Principal", null, "Luanda", "1000", "Angola");

        customer.RemoveAddress(address.Id);

        Assert.Empty(customer.Addresses);
    }

    [Fact]
    public void Deactivate_ShouldSoftDeactivateCustomerAndRegisterEvent()
    {
        var customer = CreateCustomer();

        customer.Deactivate(DateTime.UtcNow);

        Assert.False(customer.IsActive);
        Assert.NotNull(customer.DeactivatedAtUtc);
        Assert.Contains(customer.DomainEvents, domainEvent => domainEvent is CustomerDeactivated);
    }

    private static Customer CreateCustomer()
    {
        return Customer.Create(CustomerCode.Create("cus-001"), "Sample Customer", DateTime.UtcNow);
    }
}

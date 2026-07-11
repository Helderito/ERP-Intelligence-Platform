using ERP.Domain.MasterData.Events;
using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class Customer : Entity<Guid>
{
    private readonly List<CustomerContact> _contacts = [];
    private readonly List<CustomerAddress> _addresses = [];

    private Customer()
        : base(Guid.Empty)
    {
        Code = null!;
        Name = string.Empty;
    }

    private Customer(
        Guid id,
        CustomerCode code,
        string name,
        DateTime createdAtUtc)
        : base(id)
    {
        Code = code;
        Name = NormalizeName(name);
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public CustomerCode Code { get; private set; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public DateTime? DeactivatedAtUtc { get; private set; }

    public IReadOnlyCollection<CustomerContact> Contacts => _contacts.AsReadOnly();

    public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

    public static Customer Create(CustomerCode code, string name, DateTime createdAtUtc)
    {
        var customer = new Customer(Guid.NewGuid(), code, name, createdAtUtc);
        customer.RaiseDomainEvent(new CustomerCreated(customer.Id, customer.Code.Value, createdAtUtc));

        return customer;
    }

    public CustomerContact AddContact(string name, string? email, string? phone)
    {
        var contact = new CustomerContact(Guid.NewGuid(), Id, name, email, phone);
        _contacts.Add(contact);

        return contact;
    }

    public void RemoveContact(Guid contactId)
    {
        var contact = _contacts.FirstOrDefault(item => item.Id == contactId);

        if (contact is not null)
        {
            _contacts.Remove(contact);
        }
    }

    public CustomerAddress AddAddress(string line1, string? line2, string city, string postalCode, string country)
    {
        var address = new CustomerAddress(Guid.NewGuid(), Id, line1, line2, city, postalCode, country);
        _addresses.Add(address);

        return address;
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(item => item.Id == addressId);

        if (address is not null)
        {
            _addresses.Remove(address);
        }
    }

    public void UpdateDetails(string name, DateTime updatedAtUtc)
    {
        Name = NormalizeName(name);
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Deactivate(DateTime deactivatedAtUtc)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        DeactivatedAtUtc = deactivatedAtUtc;
        UpdatedAtUtc = deactivatedAtUtc;
        RaiseDomainEvent(new CustomerDeactivated(Id, deactivatedAtUtc));
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Customer name is required.", nameof(name));
        }

        return name.Trim();
    }
}

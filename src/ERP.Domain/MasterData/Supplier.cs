using ERP.Domain.MasterData.Events;
using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class Supplier : Entity<Guid>
{
    private readonly List<SupplierContact> _contacts = [];
    private readonly List<SupplierAddress> _addresses = [];

    private Supplier()
        : base(Guid.Empty)
    {
        Code = null!;
        Name = string.Empty;
    }

    private Supplier(
        Guid id,
        SupplierCode code,
        string name,
        DateTime createdAtUtc)
        : base(id)
    {
        Code = code;
        Name = NormalizeName(name);
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public SupplierCode Code { get; private set; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public DateTime? DeactivatedAtUtc { get; private set; }

    public IReadOnlyCollection<SupplierContact> Contacts => _contacts.AsReadOnly();

    public IReadOnlyCollection<SupplierAddress> Addresses => _addresses.AsReadOnly();

    public static Supplier Create(SupplierCode code, string name, DateTime createdAtUtc)
    {
        var supplier = new Supplier(Guid.NewGuid(), code, name, createdAtUtc);
        supplier.RaiseDomainEvent(new SupplierCreated(supplier.Id, supplier.Code.Value, createdAtUtc));

        return supplier;
    }

    public SupplierContact AddContact(string name, string? email, string? phone)
    {
        var contact = new SupplierContact(Guid.NewGuid(), Id, name, email, phone);
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

    public SupplierAddress AddAddress(string line1, string? line2, string city, string postalCode, string country)
    {
        var address = new SupplierAddress(Guid.NewGuid(), Id, line1, line2, city, postalCode, country);
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
        RaiseDomainEvent(new SupplierDeactivated(Id, deactivatedAtUtc));
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Supplier name is required.", nameof(name));
        }

        return name.Trim();
    }
}

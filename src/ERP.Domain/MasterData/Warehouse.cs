using ERP.Domain.MasterData.Events;
using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class Warehouse : Entity<Guid>
{
    private Warehouse() : base(Guid.Empty)
    {
        Code = null!;
        Name = string.Empty;
        WarehouseType = null!;
    }

    private Warehouse(Guid id, WarehouseCode code, string name, Guid warehouseTypeId, DateTime createdAtUtc)
        : base(id)
    {
        Code = code;
        Name = NormalizeName(name);
        WarehouseTypeId = EnsureRequiredId(warehouseTypeId);
        WarehouseType = null!;
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public WarehouseCode Code { get; private set; }
    public string Name { get; private set; }
    public Guid WarehouseTypeId { get; private set; }
    public WarehouseType WarehouseType { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? DeactivatedAtUtc { get; private set; }

    public static Warehouse Create(WarehouseCode code, string name, Guid warehouseTypeId, DateTime createdAtUtc)
    {
        var warehouse = new Warehouse(Guid.NewGuid(), code, name, warehouseTypeId, createdAtUtc);
        warehouse.RaiseDomainEvent(new WarehouseCreated(warehouse.Id, warehouse.Code.Value, createdAtUtc));
        return warehouse;
    }

    public void UpdateDetails(string name, Guid warehouseTypeId, DateTime updatedAtUtc)
    {
        Name = NormalizeName(name);
        WarehouseTypeId = EnsureRequiredId(warehouseTypeId);
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
        RaiseDomainEvent(new WarehouseDeactivated(Id, deactivatedAtUtc));
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Warehouse name is required.", nameof(name));
        }

        return name.Trim();
    }

    private static Guid EnsureRequiredId(Guid warehouseTypeId)
    {
        if (warehouseTypeId == Guid.Empty)
        {
            throw new ArgumentException("Warehouse type identifier is required.", nameof(warehouseTypeId));
        }

        return warehouseTypeId;
    }
}

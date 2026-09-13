using ERP.Domain.MasterData;
using ERP.Domain.Tenancy;
using ERP.Domain.MasterData.Events;

namespace ERP.UnitTests.MasterData;

public sealed class WarehouseTests
{
    [Fact]
    public void Create_ShouldCreateActiveWarehouseAndRegisterEvent_WhenDataIsValid()
    {
        var warehouse = CreateWarehouse();

        Assert.NotEqual(Guid.Empty, warehouse.Id);
        Assert.Equal("MAIN-01", warehouse.Code.Value);
        Assert.True(warehouse.IsActive);
        Assert.Contains(warehouse.DomainEvents, domainEvent => domainEvent is WarehouseCreated);
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => Warehouse.Create(
            TenancySeed.DefaultCompanyId,
            WarehouseCode.Create("MAIN-01"),
            " ",
            Guid.NewGuid(),
            DateTime.UtcNow));
    }

    [Fact]
    public void Create_ShouldThrow_WhenWarehouseTypeIsMissing()
    {
        Assert.Throws<ArgumentException>(() => Warehouse.Create(
            TenancySeed.DefaultCompanyId,
            WarehouseCode.Create("MAIN-01"),
            "Main Warehouse",
            Guid.Empty,
            DateTime.UtcNow));
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateNameAndTypeWithoutChangingCode()
    {
        var warehouse = CreateWarehouse();
        var nextTypeId = Guid.NewGuid();

        warehouse.UpdateDetails("Transit Warehouse", nextTypeId, DateTime.UtcNow);

        Assert.Equal("MAIN-01", warehouse.Code.Value);
        Assert.Equal("Transit Warehouse", warehouse.Name);
        Assert.Equal(nextTypeId, warehouse.WarehouseTypeId);
    }

    [Fact]
    public void Deactivate_ShouldSoftDeactivateAndRegisterEvent()
    {
        var warehouse = CreateWarehouse();

        warehouse.Deactivate(DateTime.UtcNow);

        Assert.False(warehouse.IsActive);
        Assert.NotNull(warehouse.DeactivatedAtUtc);
        Assert.Contains(warehouse.DomainEvents, domainEvent => domainEvent is WarehouseDeactivated);
    }

    [Fact]
    public void Deactivate_ShouldBeIdempotent_WhenAlreadyInactive()
    {
        var warehouse = CreateWarehouse();
        warehouse.Deactivate(DateTime.UtcNow);
        var eventCount = warehouse.DomainEvents.Count;

        warehouse.Deactivate(DateTime.UtcNow.AddMinutes(1));

        Assert.Equal(eventCount, warehouse.DomainEvents.Count);
    }

    private static Warehouse CreateWarehouse()
    {
        return Warehouse.Create(
            TenancySeed.DefaultCompanyId,
            WarehouseCode.Create("main-01"),
            "Main Warehouse",
            Guid.NewGuid(),
            DateTime.UtcNow);
    }
}

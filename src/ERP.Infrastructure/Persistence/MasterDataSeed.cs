using ERP.Domain.MasterData;

namespace ERP.Infrastructure.Persistence;

public static class MasterDataSeed
{
    public static readonly Guid GeneralCategoryId = Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b01");

    public static readonly Guid UnitOfMeasureUnitId = Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b02");

    public static readonly Guid UnitOfMeasureKilogramId = Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b03");

    public static readonly Category[] Categories =
    [
        new Category(GeneralCategoryId, "GENERAL", "General")
    ];

    public static readonly UnitOfMeasure[] UnitsOfMeasure =
    [
        new UnitOfMeasure(UnitOfMeasureUnitId, "UNIT", "Unit"),
        new UnitOfMeasure(UnitOfMeasureKilogramId, "KG", "Kilogram")
    ];
}

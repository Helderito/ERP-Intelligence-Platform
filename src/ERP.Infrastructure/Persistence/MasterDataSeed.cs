using ERP.Domain.MasterData;

namespace ERP.Infrastructure.Persistence;

public static class MasterDataSeed
{
    public static readonly DateTime SeededAtUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid GeneralCategoryId = Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b01");

    public static readonly Guid UnitOfMeasureUnitId = Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b02");

    public static readonly Guid UnitOfMeasureKilogramId = Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b03");

    public static readonly Guid MainWarehouseTypeId = Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b04");

    public static readonly Guid TransitWarehouseTypeId = Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b05");

    public static readonly Guid VirtualWarehouseTypeId = Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b06");

    public static readonly Category[] Categories =
    [
        new Category(GeneralCategoryId, "GENERAL", "General", SeededAtUtc)
    ];

    public static readonly UnitOfMeasure[] UnitsOfMeasure =
    [
        new UnitOfMeasure(UnitOfMeasureUnitId, "UNIT", "Unit", SeededAtUtc),
        new UnitOfMeasure(UnitOfMeasureKilogramId, "KG", "Kilogram", SeededAtUtc)
    ];

    public static readonly WarehouseType[] WarehouseTypes =
    [
        new WarehouseType(MainWarehouseTypeId, "MAIN", "Main Warehouse"),
        new WarehouseType(TransitWarehouseTypeId, "TRANSIT", "Transit"),
        new WarehouseType(VirtualWarehouseTypeId, "VIRTUAL", "Virtual")
    ];

    public static readonly Country[] Countries =
    [
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c01"), "AO", "Angola"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c02"), "PT", "Portugal"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c03"), "BR", "Brazil"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c04"), "US", "United States"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c05"), "GB", "United Kingdom"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c06"), "ES", "Spain"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c07"), "FR", "France"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c08"), "DE", "Germany"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c09"), "IT", "Italy"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c10"), "NL", "Netherlands"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c11"), "BE", "Belgium"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c12"), "CH", "Switzerland"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c13"), "LU", "Luxembourg"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c14"), "CV", "Cabo Verde"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c15"), "MZ", "Mozambique"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c16"), "ST", "Sao Tome and Principe"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c17"), "GW", "Guinea-Bissau"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c18"), "TL", "Timor-Leste"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c19"), "ZA", "South Africa"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c20"), "NA", "Namibia"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c21"), "CN", "China"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c22"), "IN", "India"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c23"), "AE", "United Arab Emirates"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c24"), "CA", "Canada"),
        new Country(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c25"), "AU", "Australia")
    ];

    public static readonly Currency[] Currencies =
    [
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d01"), "AOA", "Angolan Kwanza"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d02"), "EUR", "Euro"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d03"), "USD", "US Dollar"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d04"), "BRL", "Brazilian Real"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d05"), "GBP", "Pound Sterling"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d06"), "CHF", "Swiss Franc"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d07"), "CAD", "Canadian Dollar"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d08"), "AUD", "Australian Dollar"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d09"), "CNY", "Chinese Yuan"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d10"), "INR", "Indian Rupee"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d11"), "ZAR", "South African Rand"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d12"), "NAD", "Namibian Dollar"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d13"), "MZN", "Mozambican Metical"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d14"), "CVE", "Cabo Verde Escudo"),
        new Currency(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d15"), "STN", "Sao Tome and Principe Dobra")
    ];

    public static readonly PaymentTerm[] PaymentTerms =
    [
        new PaymentTerm(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8e01"), "NET0", "Immediate", 0),
        new PaymentTerm(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8e02"), "NET15", "Net 15 days", 15),
        new PaymentTerm(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8e03"), "NET30", "Net 30 days", 30),
        new PaymentTerm(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8e04"), "NET60", "Net 60 days", 60),
        new PaymentTerm(Guid.Parse("7f6a9325-d0a1-4d3b-9d16-9f8a579a8e05"), "NET90", "Net 90 days", 90)
    ];
}

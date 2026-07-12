using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record SupplierListItemResponse(Guid Id, string Code, string Name, bool IsActive)
{
    public static SupplierListItemResponse FromDto(SupplierListItemDto supplier)
    {
        return new SupplierListItemResponse(supplier.Id, supplier.Code, supplier.Name, supplier.IsActive);
    }
}

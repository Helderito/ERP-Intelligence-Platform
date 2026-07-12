using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record ProductListItemResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsActive)
{
    public static ProductListItemResponse FromDto(ProductListItemDto product)
    {
        return new ProductListItemResponse(product.Id, product.Code, product.Name, product.IsActive);
    }
}

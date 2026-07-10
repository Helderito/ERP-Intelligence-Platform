using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record ProductResponse(
    Guid Id,
    string Code,
    string Name,
    Guid CategoryId,
    Guid UnitOfMeasureId,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeactivatedAtUtc)
{
    public static ProductResponse FromDto(ProductDto product)
    {
        return new ProductResponse(
            product.Id,
            product.Code,
            product.Name,
            product.CategoryId,
            product.UnitOfMeasureId,
            product.IsActive,
            product.CreatedAtUtc,
            product.UpdatedAtUtc,
            product.DeactivatedAtUtc);
    }
}

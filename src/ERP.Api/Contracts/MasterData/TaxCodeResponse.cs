using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record TaxCodeResponse(
    Guid Id,
    string Code,
    string Name,
    decimal Rate,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeactivatedAtUtc)
{
    public static TaxCodeResponse FromDto(TaxCodeDto taxCode)
        => new(
            taxCode.Id,
            taxCode.Code,
            taxCode.Name,
            taxCode.Rate,
            taxCode.IsActive,
            taxCode.CreatedAtUtc,
            taxCode.UpdatedAtUtc,
            taxCode.DeactivatedAtUtc);
}

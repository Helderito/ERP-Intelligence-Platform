using ERP.Application.Tenancy.Models;

namespace ERP.Api.Contracts.Tenancy;

public sealed record CreateCompanyRequest(
    string Name,
    string EstablishmentCode,
    string EstablishmentName,
    string EstablishmentNumber);

public sealed record UpdateCompanyRequest(string Name);

public sealed record AddEstablishmentRequest(string Code, string Name, string EstablishmentNumber);

public sealed record UpdateCompanyFiscalProfileRequest(string Nif, string VatRegime, string FiscalAddress);

public sealed record EstablishmentResponse(Guid Id, string Code, string Name, string EstablishmentNumber)
{
    public static EstablishmentResponse FromDto(EstablishmentDto dto)
        => new(dto.Id, dto.Code, dto.Name, dto.EstablishmentNumber);
}

public sealed record CompanyFiscalProfileResponse(string Nif, string VatRegime, string FiscalAddress)
{
    public static CompanyFiscalProfileResponse FromDto(CompanyFiscalProfileDto dto)
        => new(dto.Nif, dto.VatRegime, dto.FiscalAddress);
}

public sealed record CompanyListItemResponse(Guid Id, string Name, bool IsActive)
{
    public static CompanyListItemResponse FromDto(CompanyListItemDto dto)
        => new(dto.Id, dto.Name, dto.IsActive);
}

public sealed record CompanyResponse(
    Guid Id,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    CompanyFiscalProfileResponse? FiscalProfile,
    IReadOnlyCollection<EstablishmentResponse> Establishments)
{
    public static CompanyResponse FromDto(CompanyDto dto)
        => new(
            dto.Id,
            dto.Name,
            dto.IsActive,
            dto.CreatedAtUtc,
            dto.UpdatedAtUtc,
            dto.FiscalProfile is null ? null : CompanyFiscalProfileResponse.FromDto(dto.FiscalProfile),
            dto.Establishments.Select(EstablishmentResponse.FromDto).ToArray());
}

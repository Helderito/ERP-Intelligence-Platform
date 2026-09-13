namespace ERP.Application.Tenancy.Models;

public sealed record EstablishmentDto(Guid Id, string Code, string Name, string EstablishmentNumber);

public sealed record CompanyFiscalProfileDto(string Nif, string VatRegime, string FiscalAddress);

public sealed record CompanyListItemDto(Guid Id, string Name, bool IsActive);

public sealed record CompanyDto(
    Guid Id,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    CompanyFiscalProfileDto? FiscalProfile,
    IReadOnlyCollection<EstablishmentDto> Establishments);

namespace ERP.Application.Tenancy.Commands;

public sealed record CreateCompanyCommand(
    string Name,
    string EstablishmentCode,
    string EstablishmentName,
    string EstablishmentNumber);

public sealed record UpdateCompanyCommand(Guid CompanyId, string Name);

public sealed record AddEstablishmentCommand(
    Guid CompanyId,
    string Code,
    string Name,
    string EstablishmentNumber);

public sealed record UpdateCompanyFiscalProfileCommand(
    Guid CompanyId,
    string Nif,
    string VatRegime,
    string FiscalAddress);

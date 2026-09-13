using ERP.Application.Tenancy.Abstractions;
using ERP.Application.Tenancy.Commands;
using ERP.Application.Tenancy.Exceptions;
using ERP.Application.Tenancy.Models;
using ERP.Application.Tenancy.Queries;
using ERP.Domain.Tenancy;

namespace ERP.Application.Tenancy.Services;

public sealed class CompanyManagementService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyManagementService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IReadOnlyCollection<CompanyListItemDto>> GetCompaniesAsync(
        GetCompaniesQuery query,
        CancellationToken cancellationToken = default)
    {
        var companies = await _companyRepository.ListAsync(cancellationToken);
        return companies.Select(company => new CompanyListItemDto(company.Id, company.Name, company.IsActive)).ToArray();
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(
        GetCompanyByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdAsync(query.CompanyId, cancellationToken);
        return company is null ? null : ToDto(company);
    }

    public async Task<CompanyDto> CreateCompanyAsync(
        CreateCompanyCommand command,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var company = Company.Register(command.Name, now);
        company.AddEstablishment(
            command.EstablishmentCode,
            command.EstablishmentName,
            command.EstablishmentNumber,
            now);

        await _companyRepository.AddAsync(company, cancellationToken);
        await _companyRepository.SaveChangesAsync(cancellationToken);
        return ToDto(company);
    }

    public async Task<CompanyDto> UpdateCompanyAsync(
        UpdateCompanyCommand command,
        CancellationToken cancellationToken = default)
    {
        var company = await GetCompanyOrThrowAsync(command.CompanyId, cancellationToken);
        company.UpdateDetails(command.Name, DateTime.UtcNow);
        await _companyRepository.SaveChangesAsync(cancellationToken);
        return ToDto(company);
    }

    public async Task<CompanyDto> AddEstablishmentAsync(
        AddEstablishmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var company = await GetCompanyOrThrowAsync(command.CompanyId, cancellationToken);
        if (string.IsNullOrWhiteSpace(command.Code))
        {
            throw new ArgumentException("Establishment code is required.", nameof(command));
        }

        var normalizedCode = command.Code.Trim().ToUpperInvariant();
        if (company.Establishments.Any(item => item.Code == normalizedCode))
        {
            throw new EstablishmentCodeAlreadyExistsException(normalizedCode);
        }

        company.AddEstablishment(command.Code, command.Name, command.EstablishmentNumber, DateTime.UtcNow);
        await _companyRepository.SaveChangesAsync(cancellationToken);
        return ToDto(company);
    }

    public async Task<CompanyDto> UpdateFiscalProfileAsync(
        UpdateCompanyFiscalProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var company = await GetCompanyOrThrowAsync(command.CompanyId, cancellationToken);
        if (!Enum.TryParse<VatRegime>(command.VatRegime, true, out var vatRegime)
            || !Enum.IsDefined(vatRegime))
        {
            throw new ArgumentException("VAT regime is invalid.", nameof(command));
        }

        company.UpdateFiscalProfile(
            Nif.Create(command.Nif),
            vatRegime,
            FiscalAddress.Create(command.FiscalAddress),
            DateTime.UtcNow);

        await _companyRepository.SaveChangesAsync(cancellationToken);
        return ToDto(company);
    }

    private async Task<Company> GetCompanyOrThrowAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await _companyRepository.GetByIdAsync(companyId, cancellationToken)
            ?? throw new CompanyNotFoundException();
    }

    private static CompanyDto ToDto(Company company)
    {
        var fiscalProfile = company.FiscalProfile is null
            ? null
            : new CompanyFiscalProfileDto(
                company.FiscalProfile.Nif.Value,
                company.FiscalProfile.VatRegime.ToString(),
                company.FiscalProfile.FiscalAddress.Value);

        return new CompanyDto(
            company.Id,
            company.Name,
            company.IsActive,
            company.CreatedAtUtc,
            company.UpdatedAtUtc,
            fiscalProfile,
            company.Establishments
                .Select(item => new EstablishmentDto(item.Id, item.Code, item.Name, item.EstablishmentNumber))
                .ToArray());
    }
}

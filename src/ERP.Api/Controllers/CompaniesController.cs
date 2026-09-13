using ERP.Api.Contracts.Authentication;
using ERP.Api.Contracts.Tenancy;
using ERP.Application.Tenancy.Commands;
using ERP.Application.Tenancy.Exceptions;
using ERP.Application.Tenancy.Queries;
using ERP.Application.Tenancy.Services;
using ERP.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize(Policy = PermissionCodes.CompanyManage)]
[Route("companies")]
public sealed class CompaniesController : ControllerBase
{
    private readonly CompanyManagementService _companyManagementService;

    public CompaniesController(CompanyManagementService companyManagementService)
    {
        _companyManagementService = companyManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CompanyListItemResponse>>> GetCompanies(
        CancellationToken cancellationToken)
    {
        var companies = await _companyManagementService.GetCompaniesAsync(
            new GetCompaniesQuery(),
            cancellationToken);
        return Ok(companies.Select(CompanyListItemResponse.FromDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CompanyResponse>> GetCompany(Guid id, CancellationToken cancellationToken)
    {
        var company = await _companyManagementService.GetCompanyByIdAsync(
            new GetCompanyByIdQuery(id),
            cancellationToken);
        return company is null
            ? NotFound(new ErrorResponse("Company was not found."))
            : Ok(CompanyResponse.FromDto(company));
    }

    [HttpPost]
    public async Task<ActionResult<CompanyResponse>> CreateCompany(
        CreateCompanyRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var company = await _companyManagementService.CreateCompanyAsync(
                new CreateCompanyCommand(
                    request.Name,
                    request.EstablishmentCode,
                    request.EstablishmentName,
                    request.EstablishmentNumber),
                cancellationToken);
            return Created($"/companies/{company.Id}", CompanyResponse.FromDto(company));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ErrorResponse(exception.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CompanyResponse>> UpdateCompany(
        Guid id,
        UpdateCompanyRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var company = await _companyManagementService.UpdateCompanyAsync(
                new UpdateCompanyCommand(id, request.Name),
                cancellationToken);
            return Ok(CompanyResponse.FromDto(company));
        }
        catch (CompanyNotFoundException exception)
        {
            return NotFound(new ErrorResponse(exception.Message));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ErrorResponse(exception.Message));
        }
    }

    [HttpPost("{id:guid}/establishments")]
    public async Task<ActionResult<CompanyResponse>> AddEstablishment(
        Guid id,
        AddEstablishmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var company = await _companyManagementService.AddEstablishmentAsync(
                new AddEstablishmentCommand(id, request.Code, request.Name, request.EstablishmentNumber),
                cancellationToken);
            return Ok(CompanyResponse.FromDto(company));
        }
        catch (CompanyNotFoundException exception)
        {
            return NotFound(new ErrorResponse(exception.Message));
        }
        catch (EstablishmentCodeAlreadyExistsException exception)
        {
            return Conflict(new ErrorResponse(exception.Message));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ErrorResponse(exception.Message));
        }
    }

    [HttpPut("{id:guid}/fiscal-profile")]
    public async Task<ActionResult<CompanyResponse>> UpdateFiscalProfile(
        Guid id,
        UpdateCompanyFiscalProfileRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var company = await _companyManagementService.UpdateFiscalProfileAsync(
                new UpdateCompanyFiscalProfileCommand(
                    id,
                    request.Nif,
                    request.VatRegime,
                    request.FiscalAddress),
                cancellationToken);
            return Ok(CompanyResponse.FromDto(company));
        }
        catch (CompanyNotFoundException exception)
        {
            return NotFound(new ErrorResponse(exception.Message));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ErrorResponse(exception.Message));
        }
    }
}

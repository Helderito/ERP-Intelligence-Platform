using ERP.Api.Contracts.Authentication;
using ERP.Api.Contracts.MasterData;
using ERP.Application.MasterData.Authorization;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Queries;
using ERP.Application.MasterData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize]
[Route("tax-codes")]
public sealed class TaxCodesController : ControllerBase
{
    private readonly ReferenceDataManagementService _referenceDataManagementService;

    public TaxCodesController(ReferenceDataManagementService referenceDataManagementService)
        => _referenceDataManagementService = referenceDataManagementService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TaxCodeResponse>>> GetTaxCodes(
        [FromQuery] string? search,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var taxCodes = await _referenceDataManagementService.ListTaxCodesAsync(
            new ListReferenceItemsQuery(search, includeInactive), cancellationToken);
        return Ok(taxCodes.Select(TaxCodeResponse.FromDto).ToArray());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaxCodeResponse>> GetTaxCodeById(Guid id, CancellationToken cancellationToken)
    {
        var taxCode = await _referenceDataManagementService.GetTaxCodeByIdAsync(
            new GetTaxCodeByIdQuery(id), cancellationToken);
        return taxCode is null
            ? NotFound(new ErrorResponse("Tax code was not found."))
            : Ok(TaxCodeResponse.FromDto(taxCode));
    }

    [HttpPost]
    [Authorize(Policy = MasterDataPermissionPolicies.ReferenceManage)]
    public async Task<ActionResult<TaxCodeResponse>> CreateTaxCode(
        CreateTaxCodeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var taxCode = await _referenceDataManagementService.CreateTaxCodeAsync(
                new CreateTaxCodeCommand(request.Code, request.Name, request.Rate), cancellationToken);
            return Created($"/tax-codes/{taxCode.Id}", TaxCodeResponse.FromDto(taxCode));
        }
        catch (ReferenceCodeAlreadyExistsException ex)
        {
            return Conflict(new ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = MasterDataPermissionPolicies.ReferenceManage)]
    public async Task<ActionResult<TaxCodeResponse>> UpdateTaxCode(
        Guid id,
        UpdateTaxCodeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var taxCode = await _referenceDataManagementService.UpdateTaxCodeAsync(
                new UpdateTaxCodeCommand(id, request.Name, request.Rate), cancellationToken);
            return Ok(TaxCodeResponse.FromDto(taxCode));
        }
        catch (ReferenceItemNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = MasterDataPermissionPolicies.ReferenceManage)]
    public async Task<IActionResult> DeactivateTaxCode(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _referenceDataManagementService.DeactivateTaxCodeAsync(
                new DeactivateTaxCodeCommand(id), cancellationToken);
            return NoContent();
        }
        catch (ReferenceItemNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }
}

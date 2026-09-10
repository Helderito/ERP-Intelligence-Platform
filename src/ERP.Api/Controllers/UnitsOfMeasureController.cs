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
[Route("units-of-measure")]
public sealed class UnitsOfMeasureController : ControllerBase
{
    private readonly ReferenceDataManagementService _referenceDataManagementService;

    public UnitsOfMeasureController(ReferenceDataManagementService referenceDataManagementService)
    {
        _referenceDataManagementService = referenceDataManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<UnitOfMeasureResponse>>> GetUnitsOfMeasure(
        [FromQuery] string? search,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var units = await _referenceDataManagementService.ListUnitsOfMeasureAsync(
            new ListReferenceItemsQuery(search, includeInactive), cancellationToken);

        return Ok(units.Select(UnitOfMeasureResponse.FromDto).ToArray());
    }

    [HttpPost]
    [Authorize(Policy = MasterDataPermissionPolicies.ReferenceManage)]
    public async Task<ActionResult<UnitOfMeasureResponse>> CreateUnitOfMeasure(
        CreateReferenceDataRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var unit = await _referenceDataManagementService.CreateUnitOfMeasureAsync(
                new CreateUnitOfMeasureCommand(request.Code, request.Name), cancellationToken);
            return Created($"/units-of-measure/{unit.Id}", UnitOfMeasureResponse.FromDto(unit));
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
    public async Task<ActionResult<UnitOfMeasureResponse>> UpdateUnitOfMeasure(
        Guid id,
        UpdateReferenceDataRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var unit = await _referenceDataManagementService.UpdateUnitOfMeasureAsync(
                new UpdateUnitOfMeasureCommand(id, request.Name), cancellationToken);
            return Ok(UnitOfMeasureResponse.FromDto(unit));
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
    public async Task<IActionResult> DeactivateUnitOfMeasure(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _referenceDataManagementService.DeactivateUnitOfMeasureAsync(
                new DeactivateUnitOfMeasureCommand(id), cancellationToken);
            return NoContent();
        }
        catch (ReferenceItemNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }
}

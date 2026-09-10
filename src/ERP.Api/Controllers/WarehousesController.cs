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
[Authorize(Policy = MasterDataPermissionPolicies.WarehousesManage)]
[Route("warehouses")]
public sealed class WarehousesController : ControllerBase
{
    private readonly WarehouseManagementService _warehouseManagementService;

    public WarehousesController(WarehouseManagementService warehouseManagementService)
    {
        _warehouseManagementService = warehouseManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultResponse<WarehouseListItemResponse>>> SearchWarehouses(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _warehouseManagementService.SearchWarehousesAsync(
            new SearchWarehousesQuery(search, page, pageSize),
            cancellationToken);

        return Ok(PagedResultResponse<WarehouseListItemResponse>.FromDto(
            result,
            WarehouseListItemResponse.FromDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WarehouseResponse>> GetWarehouseById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseManagementService.GetWarehouseByIdAsync(
            new GetWarehouseByIdQuery(id),
            cancellationToken);

        return warehouse is null
            ? NotFound(new ErrorResponse("Warehouse was not found."))
            : Ok(WarehouseResponse.FromDto(warehouse));
    }

    [HttpPost]
    public async Task<ActionResult<WarehouseResponse>> CreateWarehouse(
        CreateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await _warehouseManagementService.CreateWarehouseAsync(
                new CreateWarehouseCommand(request.Code, request.Name, request.WarehouseTypeId),
                cancellationToken);

            return Created($"/warehouses/{warehouse.Id}", WarehouseResponse.FromDto(warehouse));
        }
        catch (WarehouseCodeAlreadyExistsException ex)
        {
            return Conflict(new ErrorResponse(ex.Message));
        }
        catch (MasterDataReferenceNotFoundException ex)
        {
            return UnprocessableEntity(new ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<WarehouseResponse>> UpdateWarehouse(
        Guid id,
        UpdateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await _warehouseManagementService.UpdateWarehouseAsync(
                new UpdateWarehouseCommand(id, request.Name, request.WarehouseTypeId),
                cancellationToken);

            return Ok(WarehouseResponse.FromDto(warehouse));
        }
        catch (WarehouseNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (MasterDataReferenceNotFoundException ex)
        {
            return UnprocessableEntity(new ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateWarehouse(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _warehouseManagementService.DeactivateWarehouseAsync(
                new DeactivateWarehouseCommand(id),
                cancellationToken);
            return NoContent();
        }
        catch (WarehouseNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }
}

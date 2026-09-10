using ERP.Api.Contracts.MasterData;
using ERP.Application.MasterData.Authorization;
using ERP.Application.MasterData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize(Policy = MasterDataPermissionPolicies.WarehousesManage)]
[Route("warehouse-types")]
public sealed class WarehouseTypesController : ControllerBase
{
    private readonly WarehouseManagementService _warehouseManagementService;

    public WarehouseTypesController(WarehouseManagementService warehouseManagementService)
    {
        _warehouseManagementService = warehouseManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<WarehouseTypeResponse>>> GetWarehouseTypes(
        CancellationToken cancellationToken)
    {
        var warehouseTypes = await _warehouseManagementService.GetWarehouseTypesAsync(cancellationToken);
        return Ok(warehouseTypes.Select(WarehouseTypeResponse.FromDto).ToArray());
    }
}

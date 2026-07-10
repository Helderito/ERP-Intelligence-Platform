using ERP.Api.Contracts.MasterData;
using ERP.Application.MasterData.Authorization;
using ERP.Application.MasterData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize(Policy = MasterDataPermissionPolicies.CatalogManage)]
[Route("units-of-measure")]
public sealed class UnitsOfMeasureController : ControllerBase
{
    private readonly ProductCatalogService _productCatalogService;

    public UnitsOfMeasureController(ProductCatalogService productCatalogService)
    {
        _productCatalogService = productCatalogService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<UnitOfMeasureResponse>>> GetUnitsOfMeasure(
        CancellationToken cancellationToken)
    {
        var units = await _productCatalogService.GetUnitsOfMeasureAsync(cancellationToken);

        return Ok(units.Select(UnitOfMeasureResponse.FromDto).ToArray());
    }
}

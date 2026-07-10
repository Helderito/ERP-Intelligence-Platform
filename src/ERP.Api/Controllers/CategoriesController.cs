using ERP.Api.Contracts.MasterData;
using ERP.Application.MasterData.Authorization;
using ERP.Application.MasterData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize(Policy = MasterDataPermissionPolicies.CatalogManage)]
[Route("categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ProductCatalogService _productCatalogService;

    public CategoriesController(ProductCatalogService productCatalogService)
    {
        _productCatalogService = productCatalogService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CategoryResponse>>> GetCategories(
        CancellationToken cancellationToken)
    {
        var categories = await _productCatalogService.GetCategoriesAsync(cancellationToken);

        return Ok(categories.Select(CategoryResponse.FromDto).ToArray());
    }
}

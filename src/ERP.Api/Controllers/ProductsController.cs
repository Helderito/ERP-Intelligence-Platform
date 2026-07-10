using ERP.Api.Contracts.Authentication;
using ERP.Api.Contracts.MasterData;
using ERP.Application.MasterData.Authorization;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Queries;
using ERP.Application.MasterData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize(Policy = MasterDataPermissionPolicies.CatalogManage)]
[Route("products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ProductCatalogService _productCatalogService;

    public ProductsController(ProductCatalogService productCatalogService)
    {
        _productCatalogService = productCatalogService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultResponse<ProductResponse>>> SearchProducts(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _productCatalogService.SearchProductsAsync(
            new SearchProductsQuery(search, page, pageSize),
            cancellationToken);

        return Ok(PagedResultResponse<ProductResponse>.FromDto(result, ProductResponse.FromDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productCatalogService.GetProductByIdAsync(
            new GetProductByIdQuery(id),
            cancellationToken);

        if (product is null)
        {
            return NotFound(new ErrorResponse("Produto não encontrado."));
        }

        return Ok(ProductResponse.FromDto(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productCatalogService.CreateProductAsync(
                new CreateProductCommand(
                    request.Code,
                    request.Name,
                    request.CategoryId,
                    request.UnitOfMeasureId),
                cancellationToken);

            return Created($"/products/{product.Id}", ProductResponse.FromDto(product));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Já existe", StringComparison.Ordinal))
        {
            return Conflict(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productCatalogService.UpdateProductAsync(
                new UpdateProductCommand(
                    id,
                    request.Name,
                    request.CategoryId,
                    request.UnitOfMeasureId),
                cancellationToken);

            return Ok(ProductResponse.FromDto(product));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Produto", StringComparison.Ordinal))
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateProduct(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _productCatalogService.DeactivateProductAsync(
                new DeactivateProductCommand(id),
                cancellationToken);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }
}

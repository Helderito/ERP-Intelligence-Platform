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
[Route("categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ReferenceDataManagementService _referenceDataManagementService;

    public CategoriesController(ReferenceDataManagementService referenceDataManagementService)
    {
        _referenceDataManagementService = referenceDataManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CategoryResponse>>> GetCategories(
        [FromQuery] string? search,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var categories = await _referenceDataManagementService.ListCategoriesAsync(
            new ListReferenceItemsQuery(search, includeInactive), cancellationToken);

        return Ok(categories.Select(CategoryResponse.FromDto).ToArray());
    }

    [HttpPost]
    [Authorize(Policy = MasterDataPermissionPolicies.ReferenceManage)]
    public async Task<ActionResult<CategoryResponse>> CreateCategory(
        CreateReferenceDataRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _referenceDataManagementService.CreateCategoryAsync(
                new CreateCategoryCommand(request.Code, request.Name), cancellationToken);
            return Created($"/categories/{category.Id}", CategoryResponse.FromDto(category));
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
    public async Task<ActionResult<CategoryResponse>> UpdateCategory(
        Guid id,
        UpdateReferenceDataRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _referenceDataManagementService.UpdateCategoryAsync(
                new UpdateCategoryCommand(id, request.Name), cancellationToken);
            return Ok(CategoryResponse.FromDto(category));
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
    public async Task<IActionResult> DeactivateCategory(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _referenceDataManagementService.DeactivateCategoryAsync(
                new DeactivateCategoryCommand(id), cancellationToken);
            return NoContent();
        }
        catch (ReferenceItemNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }
}

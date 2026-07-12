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
[Authorize(Policy = MasterDataPermissionPolicies.SuppliersManage)]
[Route("suppliers")]
public sealed class SuppliersController : ControllerBase
{
    private readonly SupplierManagementService _supplierManagementService;

    public SuppliersController(SupplierManagementService supplierManagementService)
    {
        _supplierManagementService = supplierManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultResponse<SupplierListItemResponse>>> SearchSuppliers(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _supplierManagementService.SearchSuppliersAsync(
            new SearchSuppliersQuery(search, page, pageSize),
            cancellationToken);

        return Ok(PagedResultResponse<SupplierListItemResponse>.FromDto(result, SupplierListItemResponse.FromDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SupplierResponse>> GetSupplierById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var supplier = await _supplierManagementService.GetSupplierByIdAsync(
            new GetSupplierByIdQuery(id),
            cancellationToken);

        if (supplier is null)
        {
            return NotFound(new ErrorResponse("Supplier was not found."));
        }

        return Ok(SupplierResponse.FromDto(supplier));
    }

    [HttpPost]
    public async Task<ActionResult<SupplierResponse>> CreateSupplier(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var supplier = await _supplierManagementService.CreateSupplierAsync(
                new CreateSupplierCommand(
                    request.Code,
                    request.Name,
                    (request.Contacts ?? []).Select(ToContactCommand).ToArray(),
                    (request.Addresses ?? []).Select(ToAddressCommand).ToArray()),
                cancellationToken);

            return Created($"/suppliers/{supplier.Id}", SupplierResponse.FromDto(supplier));
        }
        catch (SupplierCodeAlreadyExistsException ex)
        {
            return Conflict(new ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SupplierResponse>> UpdateSupplier(
        Guid id,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var supplier = await _supplierManagementService.UpdateSupplierAsync(
                new UpdateSupplierCommand(
                    id,
                    request.Name,
                    (request.Contacts ?? []).Select(ToContactCommand).ToArray(),
                    (request.Addresses ?? []).Select(ToAddressCommand).ToArray()),
                cancellationToken);

            return Ok(SupplierResponse.FromDto(supplier));
        }
        catch (SupplierNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateSupplier(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _supplierManagementService.DeactivateSupplierAsync(
                new DeactivateSupplierCommand(id),
                cancellationToken);

            return NoContent();
        }
        catch (SupplierNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }

    private static SupplierContactCommand ToContactCommand(SupplierContactRequest contact)
    {
        return new SupplierContactCommand(contact.Name, contact.Email, contact.Phone);
    }

    private static SupplierAddressCommand ToAddressCommand(SupplierAddressRequest address)
    {
        return new SupplierAddressCommand(
            address.Line1,
            address.Line2,
            address.City,
            address.PostalCode,
            address.Country);
    }
}

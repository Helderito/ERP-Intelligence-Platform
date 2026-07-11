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
[Authorize(Policy = MasterDataPermissionPolicies.CustomersManage)]
[Route("customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly CustomerManagementService _customerManagementService;

    public CustomersController(CustomerManagementService customerManagementService)
    {
        _customerManagementService = customerManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultResponse<CustomerListItemResponse>>> SearchCustomers(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _customerManagementService.SearchCustomersAsync(
            new SearchCustomersQuery(search, page, pageSize),
            cancellationToken);

        return Ok(PagedResultResponse<CustomerListItemResponse>.FromDto(result, CustomerListItemResponse.FromDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> GetCustomerById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerManagementService.GetCustomerByIdAsync(
            new GetCustomerByIdQuery(id),
            cancellationToken);

        if (customer is null)
        {
            return NotFound(new ErrorResponse("Customer was not found."));
        }

        return Ok(CustomerResponse.FromDto(customer));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> CreateCustomer(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerManagementService.CreateCustomerAsync(
                new CreateCustomerCommand(
                    request.Code,
                    request.Name,
                    (request.Contacts ?? []).Select(ToContactCommand).ToArray(),
                    (request.Addresses ?? []).Select(ToAddressCommand).ToArray()),
                cancellationToken);

            return Created($"/customers/{customer.Id}", CustomerResponse.FromDto(customer));
        }
        catch (CustomerCodeAlreadyExistsException ex)
        {
            return Conflict(new ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> UpdateCustomer(
        Guid id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerManagementService.UpdateCustomerAsync(
                new UpdateCustomerCommand(
                    id,
                    request.Name,
                    (request.Contacts ?? []).Select(ToContactCommand).ToArray(),
                    (request.Addresses ?? []).Select(ToAddressCommand).ToArray()),
                cancellationToken);

            return Ok(CustomerResponse.FromDto(customer));
        }
        catch (CustomerNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateCustomer(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _customerManagementService.DeactivateCustomerAsync(
                new DeactivateCustomerCommand(id),
                cancellationToken);

            return NoContent();
        }
        catch (CustomerNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }

    private static CustomerContactCommand ToContactCommand(CustomerContactRequest contact)
    {
        return new CustomerContactCommand(contact.Name, contact.Email, contact.Phone);
    }

    private static CustomerAddressCommand ToAddressCommand(CustomerAddressRequest address)
    {
        return new CustomerAddressCommand(
            address.Line1,
            address.Line2,
            address.City,
            address.PostalCode,
            address.Country);
    }
}

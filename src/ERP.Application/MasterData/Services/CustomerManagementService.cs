using ERP.Application.Common.Models;
using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Models;
using ERP.Application.MasterData.Queries;
using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Services;

public sealed class CustomerManagementService
{
    public const int MaximumPageSize = 100;

    private readonly ICustomerRepository _customerRepository;

    public CustomerManagementService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto> CreateCustomerAsync(
        CreateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var code = CustomerCode.Create(command.Code);
        var existingCustomer = await _customerRepository.GetByCodeAsync(code, cancellationToken);

        if (existingCustomer is not null)
        {
            throw new CustomerCodeAlreadyExistsException(code.Value);
        }

        var customer = Customer.Create(code, command.Name, DateTime.UtcNow);
        AddContacts(customer, command.Contacts);
        AddAddresses(customer, command.Addresses);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return ToCustomerDto(customer);
    }

    public async Task<CustomerDto> UpdateCustomerAsync(
        UpdateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(command.CustomerId, cancellationToken);
        customer.UpdateDetails(command.Name, DateTime.UtcNow);
        ReplaceContacts(customer, command.Contacts);
        ReplaceAddresses(customer, command.Addresses);

        await _customerRepository.SaveChangesAsync(cancellationToken);

        return ToCustomerDto(customer);
    }

    public async Task DeactivateCustomerAsync(
        DeactivateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(command.CustomerId, cancellationToken);
        customer.Deactivate(DateTime.UtcNow);

        await _customerRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(query.CustomerId, cancellationToken);

        return customer is null ? null : ToCustomerDto(customer);
    }

    public async Task<PagedResultDto<CustomerDto>> SearchCustomersAsync(
        SearchCustomersQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, MaximumPageSize);
        var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();

        var totalRecords = await _customerRepository.CountAsync(search, cancellationToken);
        var customers = await _customerRepository.SearchAsync(search, page, pageSize, cancellationToken);

        return new PagedResultDto<CustomerDto>(
            page,
            pageSize,
            totalRecords,
            customers.Select(ToCustomerDto).ToArray());
    }

    private async Task<Customer> GetCustomerOrThrowAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);

        if (customer is null)
        {
            throw new CustomerNotFoundException();
        }

        return customer;
    }

    private static void ReplaceContacts(Customer customer, IReadOnlyCollection<CustomerContactCommand> contacts)
    {
        foreach (var contactId in customer.Contacts.Select(contact => contact.Id).ToArray())
        {
            customer.RemoveContact(contactId);
        }

        AddContacts(customer, contacts);
    }

    private static void ReplaceAddresses(Customer customer, IReadOnlyCollection<CustomerAddressCommand> addresses)
    {
        foreach (var addressId in customer.Addresses.Select(address => address.Id).ToArray())
        {
            customer.RemoveAddress(addressId);
        }

        AddAddresses(customer, addresses);
    }

    private static void AddContacts(Customer customer, IReadOnlyCollection<CustomerContactCommand> contacts)
    {
        foreach (var contact in contacts)
        {
            customer.AddContact(contact.Name, contact.Email, contact.Phone);
        }
    }

    private static void AddAddresses(Customer customer, IReadOnlyCollection<CustomerAddressCommand> addresses)
    {
        foreach (var address in addresses)
        {
            customer.AddAddress(address.Line1, address.Line2, address.City, address.PostalCode, address.Country);
        }
    }

    private static CustomerDto ToCustomerDto(Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.Code.Value,
            customer.Name,
            customer.IsActive,
            customer.CreatedAtUtc,
            customer.UpdatedAtUtc,
            customer.DeactivatedAtUtc,
            customer.Contacts.Select(ToCustomerContactDto).ToArray(),
            customer.Addresses.Select(ToCustomerAddressDto).ToArray());
    }

    private static CustomerContactDto ToCustomerContactDto(CustomerContact contact)
    {
        return new CustomerContactDto(contact.Id, contact.Name, contact.Email, contact.Phone);
    }

    private static CustomerAddressDto ToCustomerAddressDto(CustomerAddress address)
    {
        return new CustomerAddressDto(
            address.Id,
            address.Line1,
            address.Line2,
            address.City,
            address.PostalCode,
            address.Country);
    }
}

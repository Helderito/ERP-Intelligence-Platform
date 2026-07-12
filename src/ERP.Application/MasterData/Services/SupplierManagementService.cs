using ERP.Application.Common.Models;
using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Models;
using ERP.Application.MasterData.Queries;
using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Services;

public sealed class SupplierManagementService
{
    public const int MaximumPageSize = 100;

    private readonly ISupplierRepository _supplierRepository;

    public SupplierManagementService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDto> CreateSupplierAsync(
        CreateSupplierCommand command,
        CancellationToken cancellationToken = default)
    {
        var code = SupplierCode.Create(command.Code);
        var existingSupplier = await _supplierRepository.GetByCodeAsync(code, cancellationToken);

        if (existingSupplier is not null)
        {
            throw new SupplierCodeAlreadyExistsException(code.Value);
        }

        var supplier = Supplier.Create(code, command.Name, DateTime.UtcNow);
        AddContacts(supplier, command.Contacts);
        AddAddresses(supplier, command.Addresses);

        await _supplierRepository.AddAsync(supplier, cancellationToken);
        await _supplierRepository.SaveChangesAsync(cancellationToken);

        return ToSupplierDto(supplier);
    }

    public async Task<SupplierDto> UpdateSupplierAsync(
        UpdateSupplierCommand command,
        CancellationToken cancellationToken = default)
    {
        var supplier = await GetSupplierOrThrowAsync(command.SupplierId, cancellationToken);
        supplier.UpdateDetails(command.Name, DateTime.UtcNow);
        ReplaceContacts(supplier, command.Contacts);
        ReplaceAddresses(supplier, command.Addresses);

        await _supplierRepository.SaveChangesAsync(cancellationToken);

        return ToSupplierDto(supplier);
    }

    public async Task DeactivateSupplierAsync(
        DeactivateSupplierCommand command,
        CancellationToken cancellationToken = default)
    {
        var supplier = await GetSupplierOrThrowAsync(command.SupplierId, cancellationToken);
        supplier.Deactivate(DateTime.UtcNow);

        await _supplierRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<SupplierDto?> GetSupplierByIdAsync(
        GetSupplierByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(query.SupplierId, cancellationToken);

        return supplier is null ? null : ToSupplierDto(supplier);
    }

    public async Task<PagedResultDto<SupplierListItemDto>> SearchSuppliersAsync(
        SearchSuppliersQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, MaximumPageSize);
        var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();

        var totalRecords = await _supplierRepository.CountAsync(search, cancellationToken);
        var suppliers = await _supplierRepository.SearchAsync(search, page, pageSize, cancellationToken);

        return new PagedResultDto<SupplierListItemDto>(
            page,
            pageSize,
            totalRecords,
            suppliers.Select(ToSupplierListItemDto).ToArray());
    }

    private async Task<Supplier> GetSupplierOrThrowAsync(Guid supplierId, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(supplierId, cancellationToken);

        if (supplier is null)
        {
            throw new SupplierNotFoundException();
        }

        return supplier;
    }

    private static void ReplaceContacts(Supplier supplier, IReadOnlyCollection<SupplierContactCommand> contacts)
    {
        foreach (var contactId in supplier.Contacts.Select(contact => contact.Id).ToArray())
        {
            supplier.RemoveContact(contactId);
        }

        AddContacts(supplier, contacts);
    }

    private static void ReplaceAddresses(Supplier supplier, IReadOnlyCollection<SupplierAddressCommand> addresses)
    {
        foreach (var addressId in supplier.Addresses.Select(address => address.Id).ToArray())
        {
            supplier.RemoveAddress(addressId);
        }

        AddAddresses(supplier, addresses);
    }

    private static void AddContacts(Supplier supplier, IReadOnlyCollection<SupplierContactCommand> contacts)
    {
        foreach (var contact in contacts)
        {
            supplier.AddContact(contact.Name, contact.Email, contact.Phone);
        }
    }

    private static void AddAddresses(Supplier supplier, IReadOnlyCollection<SupplierAddressCommand> addresses)
    {
        foreach (var address in addresses)
        {
            supplier.AddAddress(address.Line1, address.Line2, address.City, address.PostalCode, address.Country);
        }
    }

    private static SupplierListItemDto ToSupplierListItemDto(Supplier supplier)
    {
        return new SupplierListItemDto(
            supplier.Id,
            supplier.Code.Value,
            supplier.Name,
            supplier.IsActive);
    }

    private static SupplierDto ToSupplierDto(Supplier supplier)
    {
        return new SupplierDto(
            supplier.Id,
            supplier.Code.Value,
            supplier.Name,
            supplier.IsActive,
            supplier.CreatedAtUtc,
            supplier.UpdatedAtUtc,
            supplier.DeactivatedAtUtc,
            supplier.Contacts.Select(ToSupplierContactDto).ToArray(),
            supplier.Addresses.Select(ToSupplierAddressDto).ToArray());
    }

    private static SupplierContactDto ToSupplierContactDto(SupplierContact contact)
    {
        return new SupplierContactDto(contact.Id, contact.Name, contact.Email, contact.Phone);
    }

    private static SupplierAddressDto ToSupplierAddressDto(SupplierAddress address)
    {
        return new SupplierAddressDto(
            address.Id,
            address.Line1,
            address.Line2,
            address.City,
            address.PostalCode,
            address.Country);
    }
}

using ERP.Application.Common.Models;
using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Models;
using ERP.Application.MasterData.Queries;
using ERP.Domain.MasterData;
using ERP.Domain.Tenancy;

namespace ERP.Application.MasterData.Services;

public sealed class WarehouseManagementService
{
    public const int MaximumPageSize = 100;

    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWarehouseTypeRepository _warehouseTypeRepository;
    private readonly ICurrentCompanyProvider _currentCompanyProvider;

    public WarehouseManagementService(
        IWarehouseRepository warehouseRepository,
        IWarehouseTypeRepository warehouseTypeRepository,
        ICurrentCompanyProvider currentCompanyProvider)
    {
        _warehouseRepository = warehouseRepository;
        _warehouseTypeRepository = warehouseTypeRepository;
        _currentCompanyProvider = currentCompanyProvider;
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(
        CreateWarehouseCommand command,
        CancellationToken cancellationToken = default)
    {
        var warehouseType = await GetWarehouseTypeOrThrowAsync(command.WarehouseTypeId, cancellationToken);
        var code = WarehouseCode.Create(command.Code);

        if (await _warehouseRepository.GetByCodeAsync(code, cancellationToken) is not null)
        {
            throw new WarehouseCodeAlreadyExistsException(code.Value);
        }

        var warehouse = Warehouse.Create(
            _currentCompanyProvider.GetRequiredCompanyId(),
            code,
            command.Name,
            command.WarehouseTypeId,
            DateTime.UtcNow);
        await _warehouseRepository.AddAsync(warehouse, cancellationToken);
        await _warehouseRepository.SaveChangesAsync(cancellationToken);

        return ToWarehouseDto(warehouse, warehouseType.Name);
    }

    public async Task<WarehouseDto> UpdateWarehouseAsync(
        UpdateWarehouseCommand command,
        CancellationToken cancellationToken = default)
    {
        var warehouseType = await GetWarehouseTypeOrThrowAsync(command.WarehouseTypeId, cancellationToken);
        var warehouse = await GetWarehouseOrThrowAsync(command.WarehouseId, cancellationToken);

        warehouse.UpdateDetails(command.Name, command.WarehouseTypeId, DateTime.UtcNow);
        await _warehouseRepository.SaveChangesAsync(cancellationToken);

        return ToWarehouseDto(warehouse, warehouseType.Name);
    }

    public async Task DeactivateWarehouseAsync(
        DeactivateWarehouseCommand command,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await GetWarehouseOrThrowAsync(command.WarehouseId, cancellationToken);
        warehouse.Deactivate(DateTime.UtcNow);
        await _warehouseRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(
        GetWarehouseByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(query.WarehouseId, cancellationToken);
        return warehouse is null ? null : ToWarehouseDto(warehouse, warehouse.WarehouseType.Name);
    }

    public async Task<PagedResultDto<WarehouseListItemDto>> SearchWarehousesAsync(
        SearchWarehousesQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, MaximumPageSize);
        var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();
        var totalRecords = await _warehouseRepository.CountAsync(search, cancellationToken);
        var warehouses = await _warehouseRepository.SearchAsync(search, page, pageSize, cancellationToken);

        return new PagedResultDto<WarehouseListItemDto>(
            page,
            pageSize,
            totalRecords,
            warehouses.Select(ToWarehouseListItemDto).ToArray());
    }

    public async Task<IReadOnlyCollection<WarehouseTypeDto>> GetWarehouseTypesAsync(
        CancellationToken cancellationToken = default)
    {
        var warehouseTypes = await _warehouseTypeRepository.ListAsync(cancellationToken);
        return warehouseTypes.Select(type => new WarehouseTypeDto(type.Id, type.Code, type.Name)).ToArray();
    }

    private async Task<Warehouse> GetWarehouseOrThrowAsync(Guid warehouseId, CancellationToken cancellationToken)
    {
        return await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken)
            ?? throw new WarehouseNotFoundException();
    }

    private async Task<WarehouseType> GetWarehouseTypeOrThrowAsync(
        Guid warehouseTypeId,
        CancellationToken cancellationToken)
    {
        return await _warehouseTypeRepository.GetByIdAsync(warehouseTypeId, cancellationToken)
            ?? throw new MasterDataReferenceNotFoundException("Warehouse type was not found.");
    }

    private static WarehouseDto ToWarehouseDto(Warehouse warehouse, string warehouseTypeName)
    {
        return new WarehouseDto(
            warehouse.Id,
            warehouse.Code.Value,
            warehouse.Name,
            warehouse.WarehouseTypeId,
            warehouseTypeName,
            warehouse.IsActive,
            warehouse.CreatedAtUtc,
            warehouse.UpdatedAtUtc,
            warehouse.DeactivatedAtUtc);
    }

    private static WarehouseListItemDto ToWarehouseListItemDto(Warehouse warehouse)
    {
        return new WarehouseListItemDto(warehouse.Id, warehouse.Code.Value, warehouse.Name, warehouse.IsActive);
    }
}

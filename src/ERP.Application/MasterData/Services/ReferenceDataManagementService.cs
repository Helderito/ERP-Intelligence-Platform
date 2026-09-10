using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Models;
using ERP.Application.MasterData.Queries;
using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Services;

public sealed class ReferenceDataManagementService
{
    private readonly IManagedCategoryRepository _categoryRepository;
    private readonly IManagedUnitOfMeasureRepository _unitOfMeasureRepository;
    private readonly ITaxCodeRepository _taxCodeRepository;

    public ReferenceDataManagementService(
        IManagedCategoryRepository categoryRepository,
        IManagedUnitOfMeasureRepository unitOfMeasureRepository,
        ITaxCodeRepository taxCodeRepository)
    {
        _categoryRepository = categoryRepository;
        _unitOfMeasureRepository = unitOfMeasureRepository;
        _taxCodeRepository = taxCodeRepository;
    }

    public async Task<ReferenceDataItemDto> CreateCategoryAsync(
        CreateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var category = Category.Create(command.Code, command.Name, DateTime.UtcNow);
        if (await _categoryRepository.GetByCodeAsync(category.Code, cancellationToken) is not null)
        {
            throw new ReferenceCodeAlreadyExistsException("Category", category.Code);
        }

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
        return ToReferenceItemDto(category);
    }

    public async Task<ReferenceDataItemDto> UpdateCategoryAsync(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var category = await GetCategoryOrThrowAsync(command.CategoryId, cancellationToken);
        category.UpdateDetails(command.Name, DateTime.UtcNow);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
        return ToReferenceItemDto(category);
    }

    public async Task DeactivateCategoryAsync(
        DeactivateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var category = await GetCategoryOrThrowAsync(command.CategoryId, cancellationToken);
        category.Deactivate(DateTime.UtcNow);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ReferenceDataItemDto>> ListCategoriesAsync(
        ListReferenceItemsQuery query,
        CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.ListAsync(
            NormalizeSearch(query.Search), query.IncludeInactive, cancellationToken);
        return categories.Select(ToReferenceItemDto).ToArray();
    }

    public async Task<ReferenceDataItemDto> CreateUnitOfMeasureAsync(
        CreateUnitOfMeasureCommand command,
        CancellationToken cancellationToken = default)
    {
        var unit = UnitOfMeasure.Create(command.Code, command.Name, DateTime.UtcNow);
        if (await _unitOfMeasureRepository.GetByCodeAsync(unit.Code, cancellationToken) is not null)
        {
            throw new ReferenceCodeAlreadyExistsException("Unit of measure", unit.Code);
        }

        await _unitOfMeasureRepository.AddAsync(unit, cancellationToken);
        await _unitOfMeasureRepository.SaveChangesAsync(cancellationToken);
        return ToReferenceItemDto(unit);
    }

    public async Task<ReferenceDataItemDto> UpdateUnitOfMeasureAsync(
        UpdateUnitOfMeasureCommand command,
        CancellationToken cancellationToken = default)
    {
        var unit = await GetUnitOfMeasureOrThrowAsync(command.UnitOfMeasureId, cancellationToken);
        unit.UpdateDetails(command.Name, DateTime.UtcNow);
        await _unitOfMeasureRepository.SaveChangesAsync(cancellationToken);
        return ToReferenceItemDto(unit);
    }

    public async Task DeactivateUnitOfMeasureAsync(
        DeactivateUnitOfMeasureCommand command,
        CancellationToken cancellationToken = default)
    {
        var unit = await GetUnitOfMeasureOrThrowAsync(command.UnitOfMeasureId, cancellationToken);
        unit.Deactivate(DateTime.UtcNow);
        await _unitOfMeasureRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ReferenceDataItemDto>> ListUnitsOfMeasureAsync(
        ListReferenceItemsQuery query,
        CancellationToken cancellationToken = default)
    {
        var units = await _unitOfMeasureRepository.ListAsync(
            NormalizeSearch(query.Search), query.IncludeInactive, cancellationToken);
        return units.Select(ToReferenceItemDto).ToArray();
    }

    public async Task<TaxCodeDto> CreateTaxCodeAsync(
        CreateTaxCodeCommand command,
        CancellationToken cancellationToken = default)
    {
        var taxCode = TaxCode.Create(command.Code, command.Name, command.Rate, DateTime.UtcNow);
        if (await _taxCodeRepository.GetByCodeAsync(taxCode.Code, cancellationToken) is not null)
        {
            throw new ReferenceCodeAlreadyExistsException("Tax code", taxCode.Code);
        }

        await _taxCodeRepository.AddAsync(taxCode, cancellationToken);
        await _taxCodeRepository.SaveChangesAsync(cancellationToken);
        return ToTaxCodeDto(taxCode);
    }

    public async Task<TaxCodeDto> UpdateTaxCodeAsync(
        UpdateTaxCodeCommand command,
        CancellationToken cancellationToken = default)
    {
        var taxCode = await GetTaxCodeOrThrowAsync(command.TaxCodeId, cancellationToken);
        taxCode.UpdateDetails(command.Name, command.Rate, DateTime.UtcNow);
        await _taxCodeRepository.SaveChangesAsync(cancellationToken);
        return ToTaxCodeDto(taxCode);
    }

    public async Task DeactivateTaxCodeAsync(
        DeactivateTaxCodeCommand command,
        CancellationToken cancellationToken = default)
    {
        var taxCode = await GetTaxCodeOrThrowAsync(command.TaxCodeId, cancellationToken);
        taxCode.Deactivate(DateTime.UtcNow);
        await _taxCodeRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TaxCodeDto>> ListTaxCodesAsync(
        ListReferenceItemsQuery query,
        CancellationToken cancellationToken = default)
    {
        var taxCodes = await _taxCodeRepository.ListAsync(
            NormalizeSearch(query.Search), query.IncludeInactive, cancellationToken);
        return taxCodes.Select(ToTaxCodeDto).ToArray();
    }

    public async Task<TaxCodeDto?> GetTaxCodeByIdAsync(
        GetTaxCodeByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var taxCode = await _taxCodeRepository.GetByIdAsync(query.TaxCodeId, cancellationToken);
        return taxCode is null ? null : ToTaxCodeDto(taxCode);
    }

    private async Task<Category> GetCategoryOrThrowAsync(Guid id, CancellationToken cancellationToken)
        => await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ReferenceItemNotFoundException("Category");

    private async Task<UnitOfMeasure> GetUnitOfMeasureOrThrowAsync(Guid id, CancellationToken cancellationToken)
        => await _unitOfMeasureRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ReferenceItemNotFoundException("Unit of measure");

    private async Task<TaxCode> GetTaxCodeOrThrowAsync(Guid id, CancellationToken cancellationToken)
        => await _taxCodeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ReferenceItemNotFoundException("Tax code");

    private static string? NormalizeSearch(string? search)
        => string.IsNullOrWhiteSpace(search) ? null : search.Trim();

    private static ReferenceDataItemDto ToReferenceItemDto(Category category)
        => new(category.Id, category.Code, category.Name, category.IsActive);

    private static ReferenceDataItemDto ToReferenceItemDto(UnitOfMeasure unit)
        => new(unit.Id, unit.Code, unit.Name, unit.IsActive);

    private static TaxCodeDto ToTaxCodeDto(TaxCode taxCode)
        => new(
            taxCode.Id,
            taxCode.Code,
            taxCode.Name,
            taxCode.Rate,
            taxCode.IsActive,
            taxCode.CreatedAtUtc,
            taxCode.UpdatedAtUtc,
            taxCode.DeactivatedAtUtc);
}

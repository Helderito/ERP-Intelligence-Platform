using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface IUnitOfMeasureRepository
{
    Task<bool> ExistsAsync(Guid unitOfMeasureId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UnitOfMeasure>> ListAsync(CancellationToken cancellationToken = default);
}

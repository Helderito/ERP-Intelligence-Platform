using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface ICurrencyRepository
{
    Task<IReadOnlyCollection<Currency>> ListAsync(CancellationToken cancellationToken = default);
}

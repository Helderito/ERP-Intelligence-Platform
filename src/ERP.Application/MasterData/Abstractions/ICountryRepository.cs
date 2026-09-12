using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface ICountryRepository
{
    Task<IReadOnlyCollection<Country>> ListAsync(CancellationToken cancellationToken = default);
}

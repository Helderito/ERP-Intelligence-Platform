using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface IPaymentTermRepository
{
    Task<IReadOnlyCollection<PaymentTerm>> ListAsync(CancellationToken cancellationToken = default);
}

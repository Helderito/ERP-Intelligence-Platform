using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class PaymentTermRepository : IPaymentTermRepository
{
    private readonly AppDbContext _dbContext;

    public PaymentTermRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyCollection<PaymentTerm>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.PaymentTerms
            .AsNoTracking()
            .OrderBy(term => term.Code)
            .ToArrayAsync(cancellationToken);
    }
}

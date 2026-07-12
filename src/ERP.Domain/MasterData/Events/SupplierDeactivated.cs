using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record SupplierDeactivated(Guid SupplierId, DateTime OccurredAtUtc) : IDomainEvent;

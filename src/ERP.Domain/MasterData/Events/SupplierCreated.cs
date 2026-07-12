using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record SupplierCreated(Guid SupplierId, string Code, DateTime OccurredAtUtc) : IDomainEvent;

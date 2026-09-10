using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record WarehouseCreated(Guid WarehouseId, string WarehouseCode, DateTime OccurredAtUtc) : IDomainEvent;

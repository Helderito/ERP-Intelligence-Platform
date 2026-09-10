using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record WarehouseDeactivated(Guid WarehouseId, DateTime OccurredAtUtc) : IDomainEvent;

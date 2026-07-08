namespace ERP.SharedKernel;

public interface IDomainEvent
{
    DateTime OccurredAtUtc { get; }
}

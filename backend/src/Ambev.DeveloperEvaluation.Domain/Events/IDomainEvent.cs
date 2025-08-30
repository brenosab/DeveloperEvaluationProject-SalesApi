namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }

    public record SaleCreatedEvent(Guid SaleId, DateTime OccurredOn) : IDomainEvent;
    public record SaleModifiedEvent(Guid SaleId, DateTime OccurredOn) : IDomainEvent;
    public record SaleCancelledEvent(Guid SaleId, DateTime OccurredOn) : IDomainEvent;
    public record ItemCancelledEvent(Guid SaleId, Guid ItemId, DateTime OccurredOn) : IDomainEvent;
}
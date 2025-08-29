using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Events.Interfaces
{
    public interface IEventDispatcher
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent;

        void Register<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent;
        void Unregister<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent;
    }

}

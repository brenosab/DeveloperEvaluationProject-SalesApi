using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, List<object>> _handlers = [];
        private readonly ILogger<EventDispatcher> _logger;

        public EventDispatcher(ILogger<EventDispatcher> logger)
        {
            _logger = logger;
        }

        public void Register<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
                _handlers[eventType] = [];

            _handlers[eventType].Add(handler);
        }

        public void Unregister<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent
        {
            var eventType = typeof(TEvent);
            if (_handlers.ContainsKey(eventType))
                _handlers[eventType].Remove(handler);
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent
        {
            var eventType = typeof(TEvent);

            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                _logger.LogInformation("Event dispatched: {EventName} at {OccurredOn}", @event.GetType().Name, @event.OccurredOn);

                foreach (var handler in handlers.Cast<IEventHandler<TEvent>>())
                {
                    await handler.HandleAsync(@event, cancellationToken);
                }
            }
        }
    }
}

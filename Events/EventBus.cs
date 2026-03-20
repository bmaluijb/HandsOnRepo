using System.Collections.Concurrent;

namespace NNPensionPlanner.Events;

public class EventBus
{
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();
    private readonly ILogger<EventBus> _logger;

    public EventBus(ILogger<EventBus> logger)
    {
        _logger = logger;
    }

    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        var handlers = _handlers.GetOrAdd(typeof(TEvent), _ => new List<Delegate>());
        lock (handlers)
        {
            handlers.Add(handler);
        }
        _logger.LogInformation("Subscribed handler for {EventType}", typeof(TEvent).Name);
    }

    public void Publish<TEvent>(TEvent domainEvent)
    {
        _logger.LogInformation("Publishing event {EventType}", typeof(TEvent).Name);

        if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
        {
            List<Delegate> snapshot;
            lock (handlers)
            {
                snapshot = handlers.ToList();
            }

            foreach (var handler in snapshot)
            {
                try
                {
                    ((Action<TEvent>)handler)(domainEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling event {EventType}", typeof(TEvent).Name);
                }
            }
        }
    }
}

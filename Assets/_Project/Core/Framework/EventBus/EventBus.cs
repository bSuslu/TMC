using System.Collections.Generic;
using _Project.Core.Framework.EventBus.Implementations;
using _Project.Core.Framework.EventBus.Interfaces;

namespace _Project.Core.Framework.EventBus
{
    /// <summary>
    /// Static event bus for a specific event type T. Manages subscriptions and publishing of events to all subscribers.
    /// </summary>
    public static class EventBus<T> where T : IEvent
    {
        /// <summary>
        /// Stores all event bindings (subscribers) for the event type T.
        /// </summary>
        private static readonly HashSet<IEventBinding<T>> _bindings = new ();

        /// <summary>
        /// Subscribes an event binding to the event bus.
        /// </summary>
        /// <param name="binding">The event binding to add.</param>
        public static void Subscribe(EventBinding<T> binding) => _bindings.Add(binding);

        /// <summary>
        /// Unsubscribes an event binding from the event bus.
        /// </summary>
        /// <param name="binding">The event binding to remove.</param>
        public static void Unsubscribe(EventBinding<T> binding) => _bindings.Remove(binding);

        /// <summary>
        /// Publishes an event to all subscribed bindings.
        /// </summary>
        /// <param name="eventToRaise">The event instance to publish.</param>
        public static void Publish(T eventToRaise)
        {
            foreach (var binding in _bindings)
            {
                binding.OnEvent.Invoke(eventToRaise);
                binding.OnEventNoArgs.Invoke();
            }
        }

        /// <summary>
        /// Clears all event bindings (removes all subscribers).
        /// </summary>
        public static void Clear()
        {
            _bindings.Clear();
        }
    }
}
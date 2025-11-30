using System;
using _Project.Core.Framework.EventBus.Interfaces;

namespace _Project.Core.Framework.EventBus.Implementations
{
    /// <summary>
    /// Represents a binding for event handlers of type <typeparamref name="T"/>.
    /// Allows adding and removing both typed and parameterless event handlers.
    /// </summary>
    /// <typeparam name="T">The type of event handled by this binding.</typeparam>
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        /// <summary>
        /// Holds the delegate for handling events of type <typeparamref name="T"/>.
        /// Initialized to an empty delegate to prevent null reference exceptions.
        /// </summary>
        private Action<T> _onEvent = _ => { }; // to prevent null reference exceptions

        /// <summary>
        /// Holds the delegate for handling parameterless events.
        /// Initialized to an empty delegate to prevent null reference exceptions.
        /// </summary>
        private Action _onEventNoArgs = () => { }; // to prevent null reference exceptions

        /// <inheritdoc />
        Action<T> IEventBinding<T>.OnEvent { get => _onEvent; set => _onEvent = value; }
        /// <inheritdoc />
        Action IEventBinding<T>.OnEventNoArgs { get => _onEventNoArgs; set => _onEventNoArgs = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBinding{T}"/> class with a typed event handler.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is triggered.</param>
        public EventBinding(Action<T> onEvent) => _onEvent = onEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBinding{T}"/> class with a parameterless event handler.
        /// </summary>
        /// <param name="onEventNoArgs">The parameterless event handler to be invoked when the event is triggered.</param>
        public EventBinding(Action onEventNoArgs) => _onEventNoArgs = onEventNoArgs;
        
        /// <summary>
        /// Adds a typed event handler to the binding.
        /// </summary>
        /// <param name="onEvent">The event handler to add.</param>
        public void Add(Action<T> onEvent) => _onEvent += onEvent;

        /// <summary>
        /// Removes a typed event handler from the binding.
        /// </summary>
        /// <param name="onEvent">The event handler to remove.</param>
        public void Remove(Action<T> onEvent) => _onEvent -= onEvent;
        
        /// <summary>
        /// Adds a parameterless event handler to the binding.
        /// </summary>
        /// <param name="onEvent">The parameterless event handler to add.</param>
        public void Add(Action onEvent) => _onEventNoArgs += onEvent;

        /// <summary>
        /// Removes a parameterless event handler from the binding.
        /// </summary>
        /// <param name="onEvent">The parameterless event handler to remove.</param>
        public void Remove(Action onEvent) => _onEventNoArgs -= onEvent;
    }
}
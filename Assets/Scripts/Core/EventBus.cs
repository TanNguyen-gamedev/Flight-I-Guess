using System;
using System.Collections.Generic;

namespace FlightIGuess.Core
{
    /// <summary>
    /// A centralized, pure C# Event Bus for cross-domain communication.
    /// </summary>
    public static class EventBus
    {
        // Dictionary to hold delegate references based on event type
        private static readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Subscribes a listener to a specific event type.
        /// IMPORTANT: Must pair with an Unsubscribe call (e.g., in OnDestroy) to prevent memory leaks.
        /// </summary>
        public static void Subscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);

            if (_events.ContainsKey(eventType))
            {
                _events[eventType] = Delegate.Combine(_events[eventType], listener);
            }
            else
            {
                _events[eventType] = listener;
            }
        }

        /// <summary>
        /// Unsubscribes a listener from a specific event type.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);

            if (_events.ContainsKey(eventType))
            {
                Delegate currentDelegate = _events[eventType];
                currentDelegate = Delegate.Remove(currentDelegate, listener);

                if (currentDelegate == null)
                {
                    _events.Remove(eventType);
                }
                else
                {
                    _events[eventType] = currentDelegate;
                }
            }
        }

        /// <summary>
        /// Raises an event, invoking all subscribed listeners with the provided payload.
        /// </summary>
        public static void Raise<T>(T eventArgs)
        {
            Type eventType = typeof(T);

            if (_events.TryGetValue(eventType, out Delegate existingDelegate))
            {
                var callback = existingDelegate as Action<T>;
                callback?.Invoke(eventArgs);
            }
        }

        /// <summary>
        /// Clears all events. Useful for resetting state when returning to the main menu.
        /// </summary>
        public static void ClearAll()
        {
            _events.Clear();
        }
    }
}

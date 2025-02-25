using System.Collections.Generic;

namespace Script.EventBus
{
    public static class Bus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBindings<T>> Bindings = new HashSet<IEventBindings<T>>();
        
        public static void Register(EventBindings<T> binding) => Bindings.Add(binding);
        public static void Unregister(EventBindings<T> binding) => Bindings.Remove(binding);
        public static void Raise(T @event)
        {
            foreach (var binding in Bindings)
            {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }
    }
}

using System;

namespace Script.EventBus
{
    public interface IEventBindings<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
    
    public class EventBindings<T> : IEventBindings<T> where T : IEvent
    {
        private Action<T> onEvent = _ => { };
        private Action onEventNoArgs = () => { };
        
        Action<T> IEventBindings<T>.OnEvent
        {
            get => onEvent;
            set => onEvent = value;
        }
        
        Action IEventBindings<T>.OnEventNoArgs
        {
            get => onEventNoArgs;
            set => onEventNoArgs = value;
        }
        
        public EventBindings(Action<T> onEvent) => this.onEvent = onEvent;
        public EventBindings(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;
        
        public void Add(Action newEvent) => onEventNoArgs += newEvent;
        public void Remove(Action newEvent) => onEventNoArgs -= newEvent;
        
        public void Add(Action<T> newEvent) => onEvent += newEvent;
        public void Remove(Action<T> newEvent) => onEvent -= newEvent;
    }
}

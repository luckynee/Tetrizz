using UnityEngine;

namespace Script.EventBus
{
    public interface IEvent
    {
        
    }
    
    public struct OnBlockReachBottomEvent: IEvent
    {
        public Transform transform { get; }
        
        public OnBlockReachBottomEvent(Transform transform)
        {
            this.transform = transform;
        }
    }

    public struct OnDoneCheckingRow : IEvent
    {
        
    }
}

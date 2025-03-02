using UnityEngine;
using UnityEngine.UIElements;

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
    
    public struct OnBlockStored : IEvent
    {
        public BlockPoolSetting CurrentBlock;
        public BlockPoolSetting BlockStored;
        public OnBlockStored(BlockPoolSetting currentBlock = null, BlockPoolSetting blockStored = null)
        {
            CurrentBlock = currentBlock;
            BlockStored = blockStored;
        }
    }
    
    public struct OnBlockMoved:IEvent
    {
        public float xPosition;
        public OnBlockMoved(float xPosition)
        {
            this.xPosition = xPosition;
        }
    }
    
    public struct OnBlockRotated : IEvent
    {
        public float RotationAngle;

        public OnBlockRotated(float rotationAngle)
        {
            RotationAngle = rotationAngle;
        }
    }
    
    public struct OnBlockEnabled : IEvent
    {
        public BlockType blockType;
        public float xPosition;
        
        public OnBlockEnabled(BlockType blockType, float xPosition)
        {
            this.blockType = blockType;
            this.xPosition = xPosition;
        }
    }
}

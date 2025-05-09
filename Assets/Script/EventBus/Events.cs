using System.Collections.Generic;
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

    public struct OnDestroyRow : IEvent
    {
        public readonly List<int> DeletedRow;
        
        public OnDestroyRow(List<int> deletedRow)
        {
            DeletedRow = deletedRow ?? new List<int>(); // Ensures it is never null
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
    
    public struct OnQueueUpdated : IEvent
    {
        public Queue<BlockPoolSetting> UpcomingBlocks;
        
        public OnQueueUpdated(Queue<BlockPoolSetting> upcomingBlocks)
        {
            UpcomingBlocks = upcomingBlocks;
        }
    }
    
    public struct OnGameOver : IEvent
    {
        
    }
    
    public struct ShowGameOverPopUp : IEvent
    {
        public bool HasNewHighScore;
        
        public ShowGameOverPopUp(bool hasNewHighScore)
        {
            HasNewHighScore = hasNewHighScore;
        }
    }
    
    public struct OnSubmitNewHighScore : IEvent
    {
        public int NewHighScore;
        public string Username;
        
        public OnSubmitNewHighScore(int newHighScore, string username)
        {
            NewHighScore = newHighScore;
            Username = username;
        }
    }
    
    public struct ChangingTimeToNormalSpeed : IEvent
    {
        
    }
}

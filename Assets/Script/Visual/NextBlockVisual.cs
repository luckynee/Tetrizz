using System;
using System.Collections.Generic;
using Script.EventBus;
using UnityEngine;

namespace Script.Visual
{
    public class NextBlockVisual : MonoBehaviour
    {
        [Header("Next 1")]
        [SerializeField] private Transform next1;
        [SerializeField] private Block[] next1Blocks;
        
        [Header("Next 2")]
        [SerializeField] private Transform next2;
        [SerializeField] private Block[] next2Blocks;
        
        [Header("Next 3")]
        [SerializeField] private Transform next3;
        [SerializeField] private Block[] next3Blocks;
        
        private Dictionary<BlockType, GameObject> _next1BlockDictionary = new Dictionary<BlockType, GameObject>();
        private Dictionary<BlockType, GameObject> _next2BlockDictionary = new Dictionary<BlockType, GameObject>();
        private Dictionary<BlockType, GameObject> _next3BlockDictionary = new Dictionary<BlockType, GameObject>();
        
        private Queue<BlockPoolSetting> _upcomingBlocks;
        private EventBindings<OnQueueUpdated> _onQueueUpdated;
        
        private BlockType _previousBlock1Type;
        private BlockType _previousBlock2Type;
        private BlockType _previousBlock3Type;

        private void Awake()
        {
            _onQueueUpdated = new EventBindings<OnQueueUpdated>(UpdateNextBlockVisual);
        }
        
        private void OnEnable()
        {
            foreach (var block in next1Blocks)
            {
                _next1BlockDictionary.Add(block.blockType, block.block);
            }
            
            foreach (var block in next2Blocks)
            {
                _next2BlockDictionary.Add(block.blockType, block.block);
            }
            
            foreach (var block in next3Blocks)
            {
                _next3BlockDictionary.Add(block.blockType, block.block);
            }
            
            Bus<OnQueueUpdated>.Register(_onQueueUpdated);
        }
        
        private void OnDisable()
        {
            Bus<OnQueueUpdated>.Unregister(_onQueueUpdated);
        }

        private void UpdateNextBlockVisual(OnQueueUpdated obj)
        {
            
            _upcomingBlocks = obj.UpcomingBlocks;
            
            var next1Block = _upcomingBlocks.ToArray()[0];
            var next2Block = _upcomingBlocks.ToArray()[1];
            var next3Block = _upcomingBlocks.ToArray()[2];
            
            if(_previousBlock1Type != next1Block.blockType)
                _next1BlockDictionary[_previousBlock1Type].SetActive(false);
            
            if(_previousBlock2Type != next2Block.blockType)
                _next2BlockDictionary[_previousBlock2Type].SetActive(false);
            
            if(_previousBlock3Type != next3Block.blockType)
                _next3BlockDictionary[_previousBlock3Type].SetActive(false);
            
            
            _previousBlock1Type = next1Block.blockType;
            _previousBlock2Type = next2Block.blockType;
            _previousBlock3Type = next3Block.blockType;
            
            _next1BlockDictionary[next1Block.blockType].SetActive(true);
            _next2BlockDictionary[next2Block.blockType].SetActive(true);
            _next3BlockDictionary[next3Block.blockType].SetActive(true);
        }
    }
}

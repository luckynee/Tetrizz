using System;
using System.Collections.Generic;
using Script.EventBus;
using UnityEngine;

namespace Script.Visual
{
    public class StoredBlockVisual : MonoBehaviour
    {
        [SerializeField] private Block[] blocks;
        
        private readonly Dictionary<BlockType, GameObject> _blockDictionary = new Dictionary<BlockType, GameObject>();
        
        private BlockType _currentBlockType;
        private BlockType _previousBlockType;
        
        private EventBindings<OnBlockStored> _onBlockStoredEvent;
        

        private void Awake()
        {
            _onBlockStoredEvent = new EventBindings<OnBlockStored>(UpdateStoredBlockVisual);
        }

        private void OnEnable()
        {
            Bus<OnBlockStored>.Register(_onBlockStoredEvent);
            
            foreach (var block in blocks)
            {
                _blockDictionary.Add(block.blockType, block.block);
            }
        }
        
        private void OnDisable()
        {
            Bus<OnBlockStored>.Unregister(_onBlockStoredEvent);
        }
        
        
        private void UpdateStoredBlockVisual(OnBlockStored obj)
        {
            if(obj.BlockStored.blockType == _previousBlockType) return;
            _blockDictionary[_previousBlockType].SetActive(false);
            
            if(!obj.BlockStored) return;
            _currentBlockType = obj.BlockStored.blockType;
            _blockDictionary[_currentBlockType].SetActive(true);
            _previousBlockType = _currentBlockType;
        }
    }
}

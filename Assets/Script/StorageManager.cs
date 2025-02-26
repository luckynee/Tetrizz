using System;
using Script.EventBus;
using UnityEngine;

namespace Script
{
    public class StorageManager : MonoBehaviour
    {
        [SerializeField] Spawner spawner;
        
        private BlockPoolSetting _storedBlock;
        private bool _hasBlockStored = false;
        private bool _usedStore = false;
        
        public static StorageManager Instance;
        
        private EventBindings<OnDoneCheckingRow> _onDoneCheckingRow;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            _onDoneCheckingRow = new EventBindings<OnDoneCheckingRow>(ResetUsedStore);
        }
        
        private void OnEnable()
        {
            Bus<OnDoneCheckingRow>.Register(_onDoneCheckingRow);
        }
        
        private void OnDisable()
        {
            Bus<OnDoneCheckingRow>.Unregister(_onDoneCheckingRow);
        }

        public void StoreBlock(PoolHandler currentBlock)
        {
            if(_usedStore) return;
            
            var currentBlockType = currentBlock.blockPoolSetting;

            if (_hasBlockStored)
            {
                // Swap the stored block with the current block
                var temp = _storedBlock;
                _storedBlock = currentBlockType;
                
                BlockFactory.ReturnToPool(currentBlock);
                Bus<OnBlockStored>.Raise(new OnBlockStored(temp));
            }
            else
            {
                // Store the current block
                _storedBlock = currentBlockType;
                _hasBlockStored = true;
                
                BlockFactory.ReturnToPool(currentBlock);
                Bus<OnBlockStored>.Raise(new OnBlockStored());
            }
            
            _usedStore = true;
        }
        
        private void ResetUsedStore()
        {
            _usedStore = false;
        }
    }
}

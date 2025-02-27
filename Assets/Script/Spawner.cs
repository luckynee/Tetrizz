using System.Collections.Generic;
using Script.EventBus;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform blockHolder;
        [SerializeField] private BlockPoolSetting[] blockPool;
        [SerializeField] private int previewCount = 3; // Number of previews

        private readonly Queue<BlockPoolSetting> _upcomingBlocks = new Queue<BlockPoolSetting>();
        
        private EventBindings<OnDoneCheckingRow> _onDoneCheckingRow;
        private EventBindings<OnBlockStored> _onBlockStored;

        private void Awake()
        {
            _onDoneCheckingRow = new EventBindings<OnDoneCheckingRow>(SpawnRandomBlock);
            _onBlockStored = new EventBindings<OnBlockStored>(SpawnBlockAfterStored);
            
            InitializeQueue();
        }

        private void OnEnable()
        {
            Bus<OnDoneCheckingRow>.Register(_onDoneCheckingRow);
            Bus<OnBlockStored>.Register(_onBlockStored);
        }

        private void OnDisable()
        {
            Bus<OnDoneCheckingRow>.Unregister(_onDoneCheckingRow);
            Bus<OnBlockStored>.Unregister(_onBlockStored);
        }

        private void Start()
        {
            SpawnRandomBlock();
        }

        private void InitializeQueue()
        {
            // Fill the queue with initial blocks
            for (var i = 0; i < previewCount; i++)
            {
                _upcomingBlocks.Enqueue(blockPool[Random.Range(0, blockPool.Length)]);
            }

            UpdatePreviewUI(); // Update UI to reflect changes
        }
        
        private void SpawnBlockAfterStored(OnBlockStored evt)
        {
            if (!evt.CurrentBlock)
            {
                SpawnRandomBlock();
            }
            else
            {
                SpawnSpecificBlock(evt.CurrentBlock);
            }
        }

        private void SpawnRandomBlock()
        {
            if (GameGrid.Instance.IsPositionFilled(transform.position))
            {
                Debug.Log("Game Over!");
                return;
            }

            // Get the next block from the queue
            var nextBlock = _upcomingBlocks.Dequeue();

            // Spawn the block
            var go = BlockFactory.Spawn(nextBlock);
            go.transform.SetParent(blockHolder);
            go.transform.position = transform.position;

            // Add a new random block to the queue
            _upcomingBlocks.Enqueue(blockPool[Random.Range(0, blockPool.Length)]);
            
            UpdatePreviewUI(); // Update the UI
        }

        private void SpawnSpecificBlock(BlockPoolSetting blockType)
        {
            if (GameGrid.Instance.IsPositionFilled(transform.position))
            {
                Debug.Log("Game Over!");
                return;
            }
            
            var go = BlockFactory.Spawn(blockType);
            go.transform.SetParent(blockHolder);
            go.transform.position = transform.position;
        }


        private void UpdatePreviewUI()
        {
            // TODO: Implement UI logic to display upcoming blocks
        }
    }
}

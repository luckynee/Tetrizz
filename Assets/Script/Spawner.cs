using System.Collections.Generic;
using Script.EventBus;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform currentBlockHolder;
        [SerializeField] private Transform blockHolder;
        [SerializeField] private BlockPoolSetting[] blockPool;
        [SerializeField] private int previewCount = 3; // Number of previews

        private readonly Queue<BlockPoolSetting> _upcomingBlocks = new Queue<BlockPoolSetting>();
        
        private EventBindings<OnDoneCheckingRow> _onDoneCheckingRow;
        private EventBindings<OnBlockStored> _onBlockStored;
        private EventBindings<OnBlockReachBottomEvent> _onBlockReachBottomEvent;

        private void Awake()
        {
            _onDoneCheckingRow = new EventBindings<OnDoneCheckingRow>(SpawnRandomBlock);
            _onBlockStored = new EventBindings<OnBlockStored>(SpawnBlockAfterStored);
            _onBlockReachBottomEvent = new EventBindings<OnBlockReachBottomEvent>(ChangeBlockParent);
            
            InitializeQueue();
        }

        private void OnEnable()
        {
            Bus<OnDoneCheckingRow>.Register(_onDoneCheckingRow);
            Bus<OnBlockStored>.Register(_onBlockStored);
            Bus<OnBlockReachBottomEvent>.Register(_onBlockReachBottomEvent);
        }

        private void OnDisable()
        {
            Bus<OnDoneCheckingRow>.Unregister(_onDoneCheckingRow);
            Bus<OnBlockStored>.Unregister(_onBlockStored);
            Bus<OnBlockReachBottomEvent>.Unregister(_onBlockReachBottomEvent);
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
            go.transform.SetParent(currentBlockHolder);
            go.transform.position = transform.position;

            // Add a new random block to the queue
            _upcomingBlocks.Enqueue(blockPool[Random.Range(0, blockPool.Length)]);
            
            Bus<OnQueueUpdated>.Raise(new OnQueueUpdated(_upcomingBlocks));
            
            Bus<OnBlockEnabled>.Raise(new OnBlockEnabled(go.blockPoolSetting.blockType, go.transform.position.x));
        }

        private void SpawnSpecificBlock(BlockPoolSetting blockType)
        {
            if (GameGrid.Instance.IsPositionFilled(transform.position))
            {
                Debug.Log("Game Over!");
                return;
            }
            
            var go = BlockFactory.Spawn(blockType);
            go.transform.SetParent(currentBlockHolder);
            go.transform.position = transform.position;
            
            Bus<OnBlockEnabled>.Raise(new OnBlockEnabled(go.blockPoolSetting.blockType, go.transform.position.x));
            
        }

        private void ChangeBlockParent(OnBlockReachBottomEvent args)
        {
            args.transform.SetParent(blockHolder);
        }
    }
}

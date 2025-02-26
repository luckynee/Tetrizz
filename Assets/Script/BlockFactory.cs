using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace Script
{
    public enum BlockType
    {
        BlockI,
        BlockJ,
        BlockL,
        BlockO,
        BlockS,
        BlockT,
        BlockZ
    }
    
    public class BlockFactory : MonoBehaviour
    {
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        
        public static BlockFactory Instance;

        private readonly Dictionary<BlockType, IObjectPool<BlockController>> _pools = new();

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        public static BlockController Spawn(BlockPoolSetting s) => Instance.GetPoolFor(s)?.Get();
        public static void ReturnToPool(BlockController block) => Instance.GetPoolFor(block.blockPoolSetting)?.Release(block);

        private IObjectPool<BlockController> GetPoolFor(BlockPoolSetting settings)
        {
            IObjectPool<BlockController> pool;
            
            if(_pools.TryGetValue(settings.blockType, out pool)) return pool;

            pool = new ObjectPool<BlockController>(
                settings.Create,
                settings.OnGet,
                settings.OnRelease,
                settings.OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
            
            _pools.Add(settings.blockType, pool);
            return pool;
        }
        
        
    }
}

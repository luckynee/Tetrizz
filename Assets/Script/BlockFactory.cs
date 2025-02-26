using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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

        private readonly Dictionary<BlockType, IObjectPool<PoolHandler>> _pools = new();

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        public static PoolHandler Spawn(BlockPoolSetting s) => Instance.GetPoolFor(s)?.Get();
        public static void ReturnToPool(PoolHandler block) => Instance.GetPoolFor(block.blockPoolSetting)?.Release(block);

        private IObjectPool<PoolHandler> GetPoolFor(BlockPoolSetting settings)
        {
            IObjectPool<PoolHandler> pool;
            
            if(_pools.TryGetValue(settings.blockType, out pool)) return pool;

            pool = new ObjectPool<PoolHandler>(
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

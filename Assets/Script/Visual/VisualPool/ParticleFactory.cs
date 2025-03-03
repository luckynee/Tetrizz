using System;
using System.Collections.Generic;
using Script.EventBus;
using Script.Tweening;
using UnityEngine;
using UnityEngine.Pool;

namespace Script.Visual.VisualPool
{
    public enum VisualType
    {
        ClearParticle,
    }
    public class ParticleFactory : MonoBehaviour
    {
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        
        public static ParticleFactory Instance;
        
        private readonly Dictionary<VisualType, IObjectPool<ParticleTween>> _pools = new();
        
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

        }

        public static ParticleTween Spawn(VisualPoolSetting s) => Instance.GetPoolFor(s)?.Get();
        
        public static void ReturnToPool(ParticleTween particleTween) => Instance.GetPoolFor(particleTween.visualPoolSetting)?.Release(particleTween);
        
        private IObjectPool<ParticleTween> GetPoolFor(VisualPoolSetting settings)
        {
            IObjectPool<ParticleTween> pool;
            
            if(_pools.TryGetValue(settings.visualType, out pool)) return pool;

            pool = new ObjectPool<ParticleTween>(
                settings.Create,
                settings.OnGet,
                settings.OnRelease,
                settings.OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
            
            _pools.Add(settings.visualType, pool);
            return pool;
        }
        
    }
}

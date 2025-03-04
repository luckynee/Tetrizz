using System;
using Script.EventBus;
using Script.Tweening;
using UnityEngine;

namespace Script.Visual.VisualPool
{
    public class ParticleSpawner : MonoBehaviour
    {
        [SerializeField] private VisualPoolSetting destroyedBlockVisual;

        private EventBindings<OnDestroyRow> _onDoneCheckingRow;

        private void Awake()
        {
            _onDoneCheckingRow = new EventBindings<OnDestroyRow>(SpawnDestroyedBlock);
        }

        private void OnEnable()
        {
            Bus<OnDestroyRow>.Register(_onDoneCheckingRow);
        }
        
        private void OnDisable()
        {
            Bus<OnDestroyRow>.Unregister(_onDoneCheckingRow);
        }

        private void SpawnDestroyedBlock(OnDestroyRow obj)
        {
            foreach (var c in obj.DeletedRow)
            {
                var go = ParticleFactory.Spawn(destroyedBlockVisual);
                go.transform.SetParent(transform);
                var newPos = new Vector3(0.5f, c + 0.5f, 0);
                go.transform.position = newPos;
            }
        }
    }
}

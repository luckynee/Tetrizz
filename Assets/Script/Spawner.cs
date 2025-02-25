using Script.EventBus;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class Spawner : MonoBehaviour
    {
       [SerializeField] private GameObject[] blockPrefabs;
       
       private EventBindings<OnBlockReachBottomEvent> _onBlockReachBottomEvent;

       private void Awake()
       {
           _onBlockReachBottomEvent = new EventBindings<OnBlockReachBottomEvent>(SpawnBlock);
       }

       private void OnEnable()
       {
           Bus<OnBlockReachBottomEvent>.Register(_onBlockReachBottomEvent);
       }

       private void OnDisable()
       {
           Bus<OnBlockReachBottomEvent>.Unregister(_onBlockReachBottomEvent);
       }

       private void Start()
       {
           SpawnBlock();
       }
       
         private void SpawnBlock()
         {
              Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)], transform.position, Quaternion.identity);
         }
    }
}

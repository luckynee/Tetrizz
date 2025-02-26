using Script.EventBus;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class Spawner : MonoBehaviour
    {
       [SerializeField] private BlockPoolSetting[] blockPool;
       
       private EventBindings<OnDoneCheckingRow> _onDoneCheckingRow;

       private void Awake()
       {
           _onDoneCheckingRow = new EventBindings<OnDoneCheckingRow>(SpawnBlock);
       }

       private void OnEnable()
       {
           Bus<OnDoneCheckingRow>.Register(_onDoneCheckingRow);
       }

       private void OnDisable()
       {
           Bus<OnDoneCheckingRow>.Unregister(_onDoneCheckingRow);
       }

       private void Start()
       {
           SpawnBlock();
       }
       
       private void SpawnBlock()
       {
           if (GameGrid.Instance.IsPositionFilled(transform.position))
           {
               Debug.Log("Game Over!");
               //TODO -> Add Game Over Event
               return; // Stop spawning blocks
           }

           var go = BlockFactory.Spawn(blockPool[Random.Range(0, blockPool.Length)]); //TODO -> Make It into command pattern
           go.transform.position = transform.position;
       }

    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    [CreateAssetMenu(menuName = "Settings/Block Pool Setting")]
    public class BlockPoolSetting : ScriptableObject
    {
        public GameObject prefabs;
        public BlockType blockType;

        private List<Vector3> _childPositions;

        public PoolHandler Create()
        {
            var go = Instantiate(prefabs);
            var poolHandler = go.AddComponent<PoolHandler>();
            poolHandler.blockPoolSetting = this;
            
            _childPositions = new List<Vector3>();

            foreach (Transform child in go.transform)
            {
                _childPositions.Add(child.localPosition);
            }
            
            return poolHandler;
        }
        
        public void OnGet(PoolHandler poolHandler)
        {
            poolHandler.gameObject.TryGetComponent(out BlockController blockController);
            blockController.enabled = true;
            
            poolHandler.gameObject.SetActive(true);

            var index = 0;
    
            // Reactivate all child blocks
            foreach (Transform child in poolHandler.transform)
            {
                if (index < _childPositions.Count)
                {
                    child.localPosition = _childPositions[index]; // Reset position
                }
                child.gameObject.SetActive(true);
                index++;
            }
    
            poolHandler.transform.position = Vector3.zero;
            poolHandler.transform.rotation = Quaternion.identity;
        }

        
        public void OnRelease(PoolHandler poolHandler)
        {
            poolHandler.gameObject.SetActive(false);
        }
        
        public void OnDestroyPoolObject(PoolHandler poolHandler)
        {
            Destroy(poolHandler.gameObject);
        }
    }
}

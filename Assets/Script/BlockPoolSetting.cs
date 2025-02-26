using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Script
{
    [CreateAssetMenu(menuName = "Settings/Block Pool Setting")]
    public class BlockPoolSetting : ScriptableObject
    {
        public GameObject prefabs;
        public BlockType blockType;

        public BlockController Create()
        {
            var go = Instantiate(prefabs);
            go.TryGetComponent(out BlockController blockController);
            blockController.blockPoolSetting = this;
            
            return blockController;
        }
        
        public void OnGet(BlockController blockController)
        {
            blockController.gameObject.SetActive(true);
        }
        
        public void OnRelease(BlockController blockController)
        {
            blockController.gameObject.SetActive(false);
        }
        
        public void OnDestroyPoolObject(BlockController blockController)
        {
            Destroy(blockController.gameObject);
        }
    }
}

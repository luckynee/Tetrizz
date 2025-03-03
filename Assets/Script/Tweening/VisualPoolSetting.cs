using Script.Visual.VisualPool;
using UnityEngine;
using UnityEngine.Search;

namespace Script.Tweening
{
    [CreateAssetMenu(menuName = "Settings/Visual Pool Setting")]
    public class VisualPoolSetting : ScriptableObject
    {
        [SearchContext("t:prefab",SearchViewFlags.GridView | SearchViewFlags.Centered)]
        public GameObject prefabs;
        public VisualType visualType;
        
        public ParticleTween Create()
        {
            var go = Instantiate(prefabs);
            var particleTween = go.GetComponent<ParticleTween>();
            
            return particleTween;
        }
        
        public void OnGet(ParticleTween particleTween)
        {
            particleTween.gameObject.SetActive(true);
        }
        
        public void OnRelease(ParticleTween particleTween)
        {
            particleTween.gameObject.SetActive(false);
        }
        
        public void OnDestroyPoolObject(ParticleTween particleTween)
        {
            Destroy(particleTween.gameObject);
        }
    }
}
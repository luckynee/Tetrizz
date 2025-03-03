using System.Collections;
using Script.Visual.VisualPool;
using UnityEngine;

namespace Script.Tweening
{
    public class ParticleTween : MonoBehaviour
    {
        [SerializeField] private LeanTweenType easeType;
        
        [SerializeField] private GameObject[] particles;
        
        public VisualPoolSetting visualPoolSetting;

        private void OnEnable()
        {
            StartCoroutine(RotateGameObject());
        }
        
        private IEnumerator RotateGameObject()
        {
            var completeTween = 0;
            
            foreach (var child in particles)
            {
                yield return new WaitForSeconds(0.03f);
                
                LeanTween.sequence()
                    .append(() => LeanTween.rotateAround(child, Vector3.forward, 360f, 0.5f)
                        .setEase(easeType))
                    .append(() => LeanTween.scale(child, new Vector3(.5f, .5f, .5f), 0.5f)
                        .setEase(LeanTweenType.easeInBounce))
                    .append(() => LeanTween.alpha(child, 0f, .5f)
                        .setEase(easeType)
                        .setOnComplete(() =>
                        {
                            completeTween++;
                            if (completeTween == particles.Length)
                            {
                                OnComplete();
                            }
                        }));
            }
        }
        
        private void ResetChild(GameObject child)
        {
            child.transform.localScale = Vector3.one;
            child.transform.localRotation = Quaternion.identity;
            LeanTween.alpha(child, 1f, 0f);
        }

        private void OnComplete()
        {
            foreach (var child in particles)
            {
                ResetChild(child);
            }
            ParticleFactory.ReturnToPool(this);
        }

    }
}

using UnityEngine;

namespace Script.Tweening
{
    public class ButtonTween : MonoBehaviour
    {
        [SerializeField] private LeanTweenType easeType;
        private LTDescr _currentTween;

        public void ScaleButton()
        {
            // Cancel any ongoing tween on this GameObject
            LeanTween.cancel(gameObject);
            
            // Start a new scale tween with a ping-pong loop (no need for ScaleToNormal)
            _currentTween = LeanTween.scale(gameObject, new Vector3(1.3f, 1.3f, 1.3f), 0.1f).setIgnoreTimeScale(true);
            _currentTween.setOnComplete(SetScaleToNormal);
            
        }
        
        private void SetScaleToNormal()
        {
            LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.5f)
                .setEase(easeType)
                .setIgnoreTimeScale(true);
        }
    }
}
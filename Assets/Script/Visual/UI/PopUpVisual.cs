using System;
using System.Collections;
using System.Numerics;
using Script.EventBus;
using Script.SharedData;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

namespace Script.Visual.UI
{
    public class PopUpVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject popUpPanel;
        [SerializeField] private GameObject gameOverPopUp;
        [SerializeField] private GameObject newHighScorePopUp;
        
        private EventBindings<OnGameOver> _onGameOver;
        private EventBindings<ShowGameOverPopUp> _showGameOverPopUp;

        private void Awake()
        {
            _onGameOver = new EventBindings<OnGameOver>(ShowGameOverPopUp);
            _showGameOverPopUp = new EventBindings<ShowGameOverPopUp>(OnShowPopUp);
        }

        private void OnEnable()
        {
            Bus<OnGameOver>.Register(_onGameOver);
            Bus<ShowGameOverPopUp>.Register(_showGameOverPopUp);
        }
        
        private void OnDisable()
        {
            Bus<OnGameOver>.Unregister(_onGameOver);
            Bus<ShowGameOverPopUp>.Unregister(_showGameOverPopUp);
        }

        #region Event Method

        private void OnShowPopUp(ShowGameOverPopUp evt)
        {
            if (evt.HasNewHighScore)
            {
                newHighScorePopUp.transform.localScale = Vector3.zero;
                ShowPopUp(newHighScorePopUp, true);
                LeanTween.scale(newHighScorePopUp, Vector3.one, 0.5f)
                    .setEase(LeanTweenType.easeSpring)
                    .setIgnoreTimeScale(true);
            }
            else
            {
                gameOverPopUp.transform.localScale = Vector3.zero;
                ShowPopUp(gameOverPopUp, true);
                LeanTween.scale(gameOverPopUp, Vector3.one, 0.5f)
                    .setEase(LeanTweenType.easeSpring)
                    .setIgnoreTimeScale(true);
            }
        }
        
        private void ShowGameOverPopUp(OnGameOver evt)
        {
            ShowPopUp(popUpPanel,true);
        }

        #endregion
       
        
        private void ShowPopUp(GameObject objectToShow,bool value)
        {
            objectToShow.SetActive(value);
        }

        #region GameOverPopUp

        public void HomeBtnClicked()
        {
            StartCoroutine(HomeBtnClickedCoroutine());
        }
        
        private IEnumerator HomeBtnClickedCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            Bus<ChangingTimeToNormalSpeed>.Raise(new ChangingTimeToNormalSpeed());
            RunTimeScriptableObject.ResetAll();
        }
        
        public void RestartBtnClicked()
        {
            StartCoroutine(RestartBtnClickedCoroutine());
        }
        
        private IEnumerator RestartBtnClickedCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            SceneManager.LoadScene("GamePlay");
            Bus<ChangingTimeToNormalSpeed>.Raise(new ChangingTimeToNormalSpeed());
            RunTimeScriptableObject.ResetAll();
        }

        #endregion



        #region NewHighScorePopUp
        private IEnumerator HideNewHighScorePopUpCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            ShowPopUp(newHighScorePopUp,false);
            ShowPopUp(gameOverPopUp,true);
        }
        
        public void OkayBtnClicked()
        {
            StartCoroutine(HideNewHighScorePopUpCoroutine());
        }

        #endregion
       
    }
    
}

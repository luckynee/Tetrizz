using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Visual.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        public void PlayBtn()
        {
            StartCoroutine(PlayBtnClickCoroutine());
        }
        
        private IEnumerator PlayBtnClickCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            SceneManager.LoadScene("GamePlay");
        }

        public void ExitBtn()
        {
            StartCoroutine(ExitBtnClickCoroutine());
        }

        private IEnumerator ExitBtnClickCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            Application.Quit();
        }
    }
}

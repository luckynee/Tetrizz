using System;
using Script.EventBus;
using Script.SharedData;
using TMPro;
using UnityEngine;

namespace Script.manager
{
    public class NewHighScoreManager : MonoBehaviour
    {
        [SerializeField] private IntVariables score;
        [SerializeField] private TMP_InputField inputName;
        [SerializeField] private TextMeshProUGUI scoreText;
        private int _score;

        private void OnEnable()
        {
            _score = score.Value;
            scoreText.text = _score.ToString();
        }

        public void SubmitScore()
        {
            Bus<OnSubmitNewHighScore>.Raise(new OnSubmitNewHighScore(_score, inputName.text));
        }
    }
}

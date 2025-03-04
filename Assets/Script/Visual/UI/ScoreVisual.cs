using Script.SharedData;
using TMPro;
using UnityEngine;

namespace Script.Visual.UI
{
    public class ScoreVisual : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private IntVariables scoreValue;
        
        [Header("Level")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private IntVariables levelValue;
        
        [Header("Lines")]
        [SerializeField] private TextMeshProUGUI linesText;
        [SerializeField] private IntVariables linesValue;

        private void OnEnable()
        {
            scoreValue.OnValueChanged += UpdateScore;
            levelValue.OnValueChanged += UpdateLevel;
            linesValue.OnValueChanged += UpdateLines;
            
            levelText.text = levelValue.Value.ToString();
        }
        
        private void OnDisable()
        {
            scoreValue.OnValueChanged -= UpdateScore;
            levelValue.OnValueChanged -= UpdateLevel;
            linesValue.OnValueChanged -= UpdateLines;
        }

        private void UpdateScore(int arg0)
        {
            scoreText.text = arg0.ToString();
        }

        private void UpdateLevel(int arg0)
        {
            levelText.text = arg0.ToString();
        }
        
        private void UpdateLines(int arg0)
        {
            linesText.text = arg0.ToString();
        }

    }
}

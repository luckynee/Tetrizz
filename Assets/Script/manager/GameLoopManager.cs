using Script.SharedData;
using UnityEngine;

namespace Script.manager
{
    public class GameLoopManager : MonoBehaviour
    {
        [Header("Level Setting")]
        [SerializeField] private float maxSpeed = 5f; // Maximum speed multiplier
        [SerializeField] private int linesPerLevel = 10; // Lines required per level
        
        [Header("References")]
        [SerializeField] private IntVariables level; // Current game level
        private const float BaseSpeed = 1f; // Default game speed

        private void OnEnable()
        {
            level.OnValueChanged += OnLevelChanged;
        }

        private void OnDisable()
        {
            level.OnValueChanged -= OnLevelChanged;
        }

        private void Start()
        {
            UpdateGameSpeed(level.Value);
        }

        private void OnLevelChanged(int newLevel)
        {
            UpdateGameSpeed(newLevel);
        }

        private void UpdateGameSpeed(int currentLevel)
        {
            // Interpolate speed between base speed and max speed based on level
            var speedMultiplier = Mathf.Lerp(1f, maxSpeed, (float)currentLevel / 30);

            // Apply the new speed
            Time.timeScale = BaseSpeed * speedMultiplier;

            Debug.Log($"Level: {currentLevel}, Speed Multiplier: {speedMultiplier}, Time Scale: {Time.timeScale}");
        }
    }
}
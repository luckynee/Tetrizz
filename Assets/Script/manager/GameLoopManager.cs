using Script.EventBus;
using Script.SharedData;
using UnityEngine;

namespace Script.manager
{
    public class GameLoopManager : MonoBehaviour
    {
        [Header("Level Setting")]
        [SerializeField] private float maxSpeed = 5f; // Maximum speed multiplier
        
        [Header("References")]
        [SerializeField] private IntVariables level; // Current game level
        private const float BaseSpeed = 1f; // Default game speed
        
        private EventBindings<OnGameOver> _onGameOver;
        private EventBindings<ChangingTimeToNormalSpeed> _changingTimeToNormalSpeed;

        private void Awake()
        {
            _onGameOver = new EventBindings<OnGameOver>(OnGameOver);
            _changingTimeToNormalSpeed = new EventBindings<ChangingTimeToNormalSpeed>(SetTimeBackToNormal);
        }

        private void OnEnable()
        {
            level.OnValueChanged += OnLevelChanged;
            Bus<OnGameOver>.Register(_onGameOver);
            Bus<ChangingTimeToNormalSpeed>.Register(_changingTimeToNormalSpeed);
        }

        private void OnDisable()
        {
            level.OnValueChanged -= OnLevelChanged;
            Bus<OnGameOver>.Unregister(_onGameOver);
            Bus<ChangingTimeToNormalSpeed>.Unregister(_changingTimeToNormalSpeed);
        }

        private void Start()
        {
            UpdateGameSpeed(level.Value);
        }

        private void OnGameOver()
        {
            Time.timeScale = 0f;
        }
        
        private void SetTimeBackToNormal()
        {
            Time.timeScale = BaseSpeed;
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
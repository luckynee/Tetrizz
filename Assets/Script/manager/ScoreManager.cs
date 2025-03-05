using System.Linq;
using Dan.Main;
using Script.EventBus;
using Script.SharedData;
using UnityEngine;

namespace Script.manager
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private IntVariables score;
        [SerializeField] private IntVariables levels;
        [SerializeField] private IntVariables linesClear;

        private EventBindings<OnDestroyRow> _onDestroyRowEvent;
        private EventBindings<OnGameOver> _onGameOverEvent;

        private void Awake()
        {
            _onDestroyRowEvent = new EventBindings<OnDestroyRow>(OnRowDestroyed);
            _onGameOverEvent = new EventBindings<OnGameOver>(OnGameOver);
        }

        private void OnEnable()
        {
            Bus<OnDestroyRow>.Register(_onDestroyRowEvent);
            Bus<OnGameOver>.Register(_onGameOverEvent);
        }
        
        private void OnDisable()
        {
            Bus<OnDestroyRow>.Unregister(_onDestroyRowEvent);
            Bus<OnGameOver>.Unregister(_onGameOverEvent);
        }

        #region Event Method  

        private void OnRowDestroyed(OnDestroyRow evt)
        {
            switch (evt.DeletedRow.Count)
            {
                case 4:
                    AddVariablesValue(score,800 * levels.Value);
                    break;
                case 3:
                    AddVariablesValue(score,500 * levels.Value);
                    break;
                case 2:
                    AddVariablesValue(score,300 * levels.Value);
                    break;
                case 1:
                    AddVariablesValue(score,100 * levels.Value);
                    break;
            }
            
            AddVariablesValue(linesClear,evt.DeletedRow.Count);
            
            if (linesClear.Value >= levels.Value * 10)
            {
                AddVariablesValue(levels,1);
            }
        }
        
        private void OnGameOver()
        {
            var scoreList = LeaderBoardManager.Instance.GetScores;

            Leaderboards.Tetrizz.GetEntries(entries =>
            {
                // Find the lowest score in the leaderboard
                var lowestLeaderboardScore = entries
                    .Select(entry => entry.Score)
                    .DefaultIfEmpty(int.MaxValue) // Prevents error if there are no entries
                    .Min(); // Get the lowest score

                // Check if leaderboard has empty slots
                var hasEmptySlots = entries.Length < scoreList.Count;

                // If there are no leaderboard entries, or the current score is higher than the lowest, or there's an empty slot
                if (score.Value == 0)
                {
                    Bus<ShowGameOverPopUp>.Raise(new ShowGameOverPopUp(false));
                    return;
                }
                
                if (entries.Length == 0 || score.Value > lowestLeaderboardScore || hasEmptySlots)
                {
                    Bus<ShowGameOverPopUp>.Raise(new ShowGameOverPopUp(true));
                }
                else
                {
                    Bus<ShowGameOverPopUp>.Raise(new ShowGameOverPopUp(false));
                }
            });
        }




        #endregion

        private void AddVariablesValue(IntVariables variablesToAdd,int value)
        {
            variablesToAdd.Value += value;
        }
    }
}

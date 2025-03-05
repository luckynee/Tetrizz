using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Dan.Main;
using Script.EventBus;

namespace Script.manager
{
    public class LeaderBoardManager : MonoBehaviour
    {
        [SerializeField] private List<TextMeshProUGUI> names;
        [SerializeField] private List<TextMeshProUGUI> scores;
        
        private EventBindings<OnSubmitNewHighScore> _onSubmitNewHighScore;
        
        public List<TextMeshProUGUI> GetScores => scores;
        
        public static LeaderBoardManager Instance;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            _onSubmitNewHighScore = new EventBindings<OnSubmitNewHighScore>(SubmitScore);
        }

        private void Start()
        {
            GetLeaderBoard();
        }

        private void OnEnable()
        {
            Bus<OnSubmitNewHighScore>.Register(_onSubmitNewHighScore);
        }
        
        private void OnDisable()
        {
            Bus<OnSubmitNewHighScore>.Unregister(_onSubmitNewHighScore);
        }
        
        private void SubmitScore(OnSubmitNewHighScore evt)
        {
            SetLeaderBoardEntry(evt.Username, evt.NewHighScore);
        }

        private void GetLeaderBoard()
        {
            
            Leaderboards.Tetrizz.GetEntries(entries =>
            {
                foreach (var t in names)
                {
                    t.text = "";
                }
                
                foreach (var score in scores)
                {
                    score.text = "";
                }
                
                var length = Mathf.Min(entries.Length, names.Count);
                Debug.Log("Length: " + length);
                for (var i = 0; i < length; i++)
                {
                    names[i].text = entries[i].Username;
                    scores[i].text = entries[i].Score.ToString();
                }
            });
        }

        private void SetLeaderBoardEntry(string username, int score)
        {
            Leaderboards.Tetrizz.UploadNewEntry(username, score, isSuccess =>
            {
                if (isSuccess)
                {
                    Leaderboards.Tetrizz.ResetPlayer(GetLeaderBoard);
                }
              
            });
        }

    }
}

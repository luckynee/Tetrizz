using System;
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

        private void Awake()
        {
            _onDestroyRowEvent = new EventBindings<OnDestroyRow>(OnRowDestroyed);
        }

        private void OnEnable()
        {
            Bus<OnDestroyRow>.Register(_onDestroyRowEvent);
        }
        
        private void OnDisable()
        {
            Bus<OnDestroyRow>.Unregister(_onDestroyRowEvent);
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

        #endregion

        private void AddVariablesValue(IntVariables variablesToAdd,int value)
        {
            variablesToAdd.Value += value;
        }
    }
}

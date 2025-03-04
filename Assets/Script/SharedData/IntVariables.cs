using UnityEngine;
using UnityEngine.Events;

namespace Script.SharedData
{
    [CreateAssetMenu(menuName = "Variables/Int Variables")]
    public class IntVariables : RunTimeScriptableObject
    {
        [SerializeField] private int initialValue;
        [SerializeField] private int value;
        
        public event UnityAction<int> OnValueChanged = delegate {  };

        public int Value
        {
            get => value;
            set
            {
                if(this.value == value) return;
                this.value = value;
                OnValueChanged.Invoke(value);
            }
        }
        protected override void OnReset()
        {
            OnValueChanged.Invoke(value = initialValue);
        }
    }
}

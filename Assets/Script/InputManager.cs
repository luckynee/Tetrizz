using UnityEngine;

namespace Script
{
    public class InputManager : MonoBehaviour
    {
        public InputReader inputReader;
        
        private void Awake()
        {
            inputReader.EnabledInputReader();
        }
        
        private void OnDisable()
        {
            inputReader.DisableInputReader();
        }
    }
}

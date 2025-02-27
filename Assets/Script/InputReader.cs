using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Script
{
    public class InputReader : ScriptableObject , InputActions.IControllerActions
    {
        public event UnityAction<bool> DownPressed = delegate { };
        public event UnityAction LeftPressed = delegate { };
        public event UnityAction RightPressed = delegate { };
        public event UnityAction HardDrop = delegate { };
        
        
        private InputActions _inputActions;
        
        public void EnabledInputReader()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputActions();
                _inputActions.Controller.SetCallbacks(this);
            }
            
            _inputActions.Controller.Enable();
        }
        
        public void DisableInputReader()
        {
            _inputActions.Controller.Disable();
        }


        public void OnLeft(InputAction.CallbackContext context)
        {
            if(context.started)
                LeftPressed.Invoke();
        }

        public void OnRight(InputAction.CallbackContext context)
        {
            if(context.started)
                RightPressed.Invoke();
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            
        }
        
        public void OnDown(InputAction.CallbackContext context)
        {
            //TODO -> Fix this 
        }



        public void OnStore(InputAction.CallbackContext context)
        {
            
        }

        public void OnHardDrop(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                HardDrop.Invoke(); 
            }
        }
    }
}

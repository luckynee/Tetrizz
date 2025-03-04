using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.SharedData
{
    public abstract class RunTimeScriptableObject : ScriptableObject
    {
        static readonly List<RunTimeScriptableObject> Instances = new List<RunTimeScriptableObject>();

        private void OnEnable() => Instances.Add(this);
        private void OnDisable() => Instances.Remove(this);

        protected abstract void OnReset();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]//This method will be called before the first scene is loaded
        public static void ResetAll()
        {
            foreach (var instance in Instances)
            {
                instance.OnReset();
            }
        }
    }
}

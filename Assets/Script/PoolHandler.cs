using System;
using System.Linq;
using Script.EventBus;
using UnityEngine;

namespace Script
{
    public class PoolHandler : MonoBehaviour
    {
        [Header("Pool Settings")]
        public BlockPoolSetting blockPoolSetting;
        
        void Update()
        {
            if (!AllChildrenInactive()) return;
            BlockFactory.ReturnToPool(this);
        }

        private bool AllChildrenInactive()
        {
            return transform.Cast<Transform>().All(child => !child.gameObject.activeSelf);
        }
    }
}
using System.Collections.Generic;
using Assets.ManusVR.Scripts;
using UnityEngine;

namespace ManusVR
{
    [CreateAssetMenu]
    public class TrackingValues : ScriptableObject
    {
        public bool AreArmsSwitched = false;
        public Dictionary<device_type_t, float> HandYawOffset = new Dictionary<device_type_t, float>();

        void OnEnable()
        {
            if (!HandYawOffset.ContainsKey(device_type_t.GLOVE_LEFT))
                HandYawOffset.Add(device_type_t.GLOVE_LEFT, 0);
            if (!HandYawOffset.ContainsKey(device_type_t.GLOVE_RIGHT))
                HandYawOffset.Add(device_type_t.GLOVE_RIGHT, 0);
        }
    }
}

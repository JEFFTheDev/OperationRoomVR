// Copyright (c) 2018 ManusVR
using Assets.ManusVR.Scripts.PhysicalInteraction;
using UnityEngine.Events;

namespace Assets.ManusVR.Scripts.Extra
{
    //-------------------------------------------------------------------------
    public static class CustomEvents
    {
        //-------------------------------------------------
        [System.Serializable]
        public class UnityEventBool : UnityEvent<bool>
        {
        }

        //-------------------------------------------------
        [System.Serializable]
        public class UnityEventFloat : UnityEvent<float>
        {
        }

        //-------------------------------------------------
        [System.Serializable]
        public class UnityEventHand : UnityEvent<device_type_t>
        {
        }

        //-------------------------------------------------
        [System.Serializable]
        public class UnityEventPhalange : UnityEvent<Phalange>
        {
        }
    }
}

// Copyright (c) 2018 ManusVR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.ManusVR.Scripts.Factory;
using UnityEngine;

namespace Assets.ManusVR.Scripts
{
    public class HandManager : MonoBehaviour
    {
        public List<Hand> Hands = new List<Hand>();
        public Transform RootTransform;

        public Transform Left;
        public Transform Right;

        private Dictionary<device_type_t, Hand> _hands = new Dictionary<device_type_t, Hand>();
        [SerializeField]
        private Transform _cameraRig;
        [SerializeField]
        private KeyCode _autoAllignHandsKey = KeyCode.Space;
        public HandData HandData;


        // Use this for initialization
        public virtual void Awake ()
        {
            if (HandData == null)
                HandData = Component.FindObjectOfType<HandData>();
            if (HandData == null)
                Debug.Log("HandManager must have HandData");

            StartCoroutine(InitializeHand(device_type_t.GLOVE_LEFT));
            StartCoroutine(InitializeHand(device_type_t.GLOVE_RIGHT));
      
            // Show the created hands as public variable
            Hands = _hands.Values.ToList();

            // Parent the ManusVR prefab to the SteamVR prefab
            if (!_cameraRig)
            {
                if (Camera.main != null)
                    _cameraRig = Camera.main.transform.root;
                Debug.LogWarning("CameraRig reference not set, automatically retrieved root transform of main camera. To avoid usage of wrong transform, consider setting this reference.");
            }
            transform.root.parent = _cameraRig;
        }

        /// <summary>
        /// Initialize a hand
        /// </summary>
        /// <param name="deviceType">The devicetype that is dedicated to this hand</param>
        public virtual IEnumerator InitializeHand(device_type_t deviceType)
        {
            GameObject parent = deviceType == device_type_t.GLOVE_LEFT ? Left.gameObject : Right.gameObject;
            Hand hand = HandFactory.GetHand(parent, HandType.Normal, HandData, this, deviceType);
            hand.CalibrateKey = _autoAllignHandsKey;
            _hands.Add(deviceType, hand);
            yield break;
        }

        public Hand GetHandController(device_type_t deviceType)
        {
            Hand hand = _hands[deviceType];
            if (hand == null)
                Debug.LogWarning("There is no hand with devicetype " + deviceType);
            return hand;
        }
	
    }
}

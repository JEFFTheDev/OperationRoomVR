// Copyright (c) 2018 ManusVR
using Assets.ManusVR.Scripts.Factory;
using UnityEngine;

namespace Assets.ManusVR.Scripts
{
    public abstract class Finger : MonoBehaviour, ICollidingCounter
    {
        public FingerIndex Index { get; set; }
        public HandType HandType { get; set; }
        public device_type_t DeviceType { get; set; }
        public GameObject[] Phalanges = new GameObject[4];
        public Hand Hand { get; set; }

        public virtual void Start()
        {

        }

        /// <summary>
        /// Rotate the phalange on the given position
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="targetRotation"></param>
        public virtual void RotatePhalange(int pos, Quaternion targetRotation)
        {
            if (Index == FingerIndex.thumb && pos == 1)
                return;
            Phalanges[pos].transform.localRotation = targetRotation;
        }

        /// <summary>
        /// Return the amount of phalanges that are colliding with objects
        /// </summary>
        /// <returns></returns>
        public virtual int AmountOfCollidingObjects()
        {
            return 0;
        }
    }
}

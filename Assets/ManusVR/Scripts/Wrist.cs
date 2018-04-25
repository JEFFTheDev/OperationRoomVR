// Copyright (c) 2018 ManusVR
using UnityEngine;

namespace Assets.ManusVR.Scripts
{
    public abstract class Wrist : MonoBehaviour, ICollidingCounter {
        private Quaternion _lastRotation = Quaternion.identity;

        public device_type_t DeviceType { get; set; }
        public Hand Hand { get; set; }
        public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            if (Rigidbody == null)
                Rigidbody = gameObject.AddComponent<Rigidbody>();
            Rigidbody.centerOfMass = Vector3.zero;    
        }

        public virtual void Start()
        {
        
        }

        public virtual void FixedUpdate()
        {
            RotateWrist();   
        }

        public virtual void RotateWrist()
        {
            transform.rotation = WristRotation();
        }

        /// <summary>
        /// Calculate the rotation of the wrist
        /// </summary>
        /// <returns></returns>
        public virtual Quaternion WristRotation()
        {
            var wristRotation = Hand.HandData.ValidOutput(DeviceType)
                ? Hand.HandData.GetWristRotation(DeviceType)
                : _lastRotation;
            _lastRotation = wristRotation;
            return Quaternion.Euler(0.0f, Hand.HandData.TrackingValues.HandYawOffset[DeviceType], 0.0f) * wristRotation;
        }

        public virtual int AmountOfCollidingObjects()
        {
            return 0;
        }
    }
}

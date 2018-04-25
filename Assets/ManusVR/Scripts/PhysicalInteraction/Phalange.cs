// Copyright (c) 2018 ManusVR
using System;
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public class PhalangeData
    {
        public FingerIndex FingerIndex;
        public int Pos;
        public device_type_t DeviceTypeT;

        public PhalangeData(FingerIndex fingerIndex, int pos, device_type_t DeviceTypeT)
        {
            this.FingerIndex = fingerIndex;
            this.Pos = pos;
            this.DeviceTypeT = DeviceTypeT;
        }

        public override string ToString()
        {
            return (int) FingerIndex + Pos + "";
        }
    }

    public class Phalange : MonoBehaviour
    {
        private Collider[] _colliders;
        public CollisionDetector Detector { get; private set; }

        public PhalangeData PhalangeData { get; set; }
        public Rigidbody Rigidbody { get; private set; }

        public Action<PhalangeData, Collision, CollisionType> CollisionEntered;

        // Use this for initialization
        void Awake ()
        {
            Rigidbody = gameObject.GetComponent<Rigidbody>();
            if (Rigidbody == null)
                Rigidbody = gameObject.AddComponent<Rigidbody>();

            Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            Rigidbody.useGravity = false;
            Detector =  gameObject.AddComponent<CollisionDetector>();

            var layer = PhysicsLayer.GetLayer(Layer.Phalange);
            Detector.PhysicsLayers = layer.AllowedCollisions;
            PhysicsManager.Instance.Register(GetComponents<Collider>(), GetComponent<Rigidbody>(), layer);
        }

        void Start()
        {
            _colliders = GetComponents<Collider>();
            
            Detector.CollisionEnter += CollisionEnter;
            Detector.CollisionStay += CollisionStay;
            Detector.CollisionExit += CollisionExit;

            if (PhalangeData.Pos == 0 && PhalangeData.FingerIndex == 0)
            {
                Rigidbody.mass = 5;
            }

            else
            {
                Rigidbody.mass = 0.02f;
            }
        }

        private void CollisionEnter(Collision collision)
        {
            CheckCollision(collision, CollisionType.Enter);
        }

        private void CollisionStay(Collision collision)
        {
            CheckCollision(collision, CollisionType.Stay);
        }

        private void CollisionExit(Collision collision)
        {
            CheckCollision(collision, CollisionType.Exit);
        }

        /// <summary>
        /// Happens when this phalange is colliding with an object
        /// </summary>
        /// <param name="collision"></param>
        private void CheckCollision(Collision collision, CollisionType type)
        {
            foreach (var collider in _colliders)
            {
                if (PhysicsManager.Instance.ProcessCollision(collider, collision) && CollisionEntered != null)
                    CollisionEntered(PhalangeData, collision, type);
            } 
        }
    }
}
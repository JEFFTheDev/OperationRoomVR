// Copyright (c) 2018 ManusVR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public abstract class Interactable : MonoBehaviour, ICollidingCounter
    {
        public HashSet<CollisionDetector> Detectors
        {
            get { return _detectors; }
        }

        protected readonly HashSet<CollisionDetector> _detectors = new HashSet<CollisionDetector>();
        protected Collider[] _colliders;
        protected Joint Connection;

        [SerializeField, HideInInspector, Tooltip("Should the user be able to grab the object?")]
        public bool IsGrabbable;
        public Rigidbody Rigidbody;
        //[HideInInspector]
        public ObjectGrabber Hand;
        [Tooltip("Grab: The user is able to touch and grab this object. \n" +
                 "Push: The user is able to touch this object but not grab it. \n" + 
                 "Phalange: Not supported \n" +
                 "UI: Not supported \n")]
        public PhysicsLayer PhysicsLayer;

        [Tooltip("Highlight object when touched by a finger")]
        [SerializeField, HideInInspector]
        public bool HighlightOnImpact = true;
        [Tooltip("Highlight when object is grabbed")]
        [SerializeField, HideInInspector]
        public bool HighlightWhenGrabbed = true;
        [SerializeField, HideInInspector]
        public float DropDistance = 0.12f;
        [SerializeField, HideInInspector]
        public bool GravityWhenGrabbed = true;
        [SerializeField, HideInInspector]
        public bool GravityWhenReleased = true;
        [SerializeField, HideInInspector]
        public bool KinematicWhenReleased = false;
        [SerializeField, HideInInspector]
        public bool AttachHandToItem = false;
        [SerializeField, HideInInspector]
        public bool RotateWithHandRotation = true;

        public Collider[] Colliders { get { return _colliders; } }

        public bool HasCollision
        {
            get { return _hasCollision; }
            private set
            {
                if (_hasCollision == value)
                    return;

                _hasCollision = value;
                foreach (var coll in Colliders)
                {
                    coll.enabled = _hasCollision;
                }
            }
        }

        private bool _hasCollision = true;

        public Action OnGrabbed;
        public Action OnReleased;

        [SerializeField, FormerlySerializedAs("OnGrabbed")]
        private UnityEvent _onGrabbed;

        [SerializeField, FormerlySerializedAs("OnReleased")]
        private UnityEvent _onReleased;

        /// <summary>
        /// The amount of objects that are inside of the trigger of this interactable
        /// </summary>
        /// <returns></returns>
        public int TotalObjectsInTriggers { get { return _detectors.Sum(detector => detector.NumOfObjectsInTrigger); } }

        /// <summary>
        /// The total amount of objects that currently have collision with this interactable
        /// </summary>
        /// <returns></returns>
        public int TotalCollidingObjects { get { return _detectors.Sum(detector => detector.Collisions); } }

        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider>();
            Initialize(_colliders);
            if (Rigidbody == null)
                Rigidbody = GetComponent<Rigidbody>();

            OnGrabbed += () =>
            {
                if (_onGrabbed != null) _onGrabbed.Invoke();
            };

            OnReleased += () =>
            {
                if (OnReleased != null) _onReleased.Invoke();
            };
        }

        public virtual void Start()
        {           
            if (Rigidbody == null)
                Debug.LogWarning("Interactable needs to have a reference to the beloning rigidbody");
            PhysicsManager.Instance.Register(_colliders, Rigidbody, PhysicsLayer);
            Rigidbody.ResetCenterOfMass();
        }

        void OnValidate()
        {
            if(PhysicsLayer == null)
                PhysicsLayer = PhysicsLayer.GetLayer(Layer.Object);
        }

        protected virtual void CollisionEnter(Collision collision)
        {
            CheckCollision(collision, CollisionType.Enter);
        }

        protected virtual void CollisionExit(Collision collision)
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
                PhysicsManager.Instance.ProcessCollision(collider, collision);
            }
        }

        /// <summary>
        /// Attach this interactable to the given rigidbody
        /// </summary>
        /// <param name="connectedBody"></param>
        /// <param name="attachedJoint"></param>
        public virtual void Attach(Rigidbody connectedBody, ObjectGrabber hand)
        {
            if (Hand != null)
                Hand.ReleaseItem();
            Hand = hand;

            if (OnGrabbed != null)
                OnGrabbed.Invoke();
        }

        public virtual void Attach(Rigidbody connectedBody)
        {
            //if (Hand != null)
            //    Hand.ReleaseItem(this);
            //Hand = hand;

            //if (OnGrabbed != null)
            //    OnGrabbed.Invoke();
        }

        /// <summary>
        /// Detach the given joint from this interactable
        /// </summary>
        /// <param name="connection"></param>
        public virtual void Detach(ObjectGrabber hand)
        {
            var tempHand = Hand;

            if(Hand == null)
                return;

            Hand = null;
            tempHand.ReleaseItem();
            tempHand = null;
            if (OnReleased != null)
                OnReleased.Invoke();
        }

        public virtual void Detach()
        {
            Detach(Hand);
        }

        public void EnableCollision()
        {
            HasCollision = true;
        }

        public void DisableCollision()
        {
            HasCollision = false;
        }

        protected virtual void Initialize(Collider[] colliders)
        {
            foreach (var child in colliders)
            {
                var detector = child.GetComponent<CollisionDetector>();
                if (detector == null)
                    detector = child.gameObject.AddComponent<CollisionDetector>();

                detector.PhysicsLayers = new List<PhysicsLayer>(){PhysicsLayer.GetLayer(Layer.Phalange)};
                _detectors.Add(detector);

                detector.CollisionEnter += CollisionEnter;
                detector.CollisionExit += CollisionExit;

                // Copies of triggers are only neccesary when the object is grabbable
                if (child.isTrigger || !IsGrabbable)
                    continue;
                CopyTriggerCollider(child);
            }
        }

        /// <summary>
        /// Make a copy of the given collider
        /// </summary>
        /// <param name="child"></param>
        private Collider CopyTriggerCollider(Collider child)
        {
            const float colliderSizeMultiplier = 1.05f;
            // Add a extra trigger collider
            var colliderCopy = CopyComponent(child, child.gameObject) as Collider;

            if (colliderCopy is MeshCollider)
            {
                var meshCopy = colliderCopy as MeshCollider;
                // Make it convex otherwise it will not work with triggers
                meshCopy.convex = true;
                meshCopy.inflateMesh = true;
                meshCopy.skinWidth = 0.002f;
            }

            else if (colliderCopy is BoxCollider)
            {
                var boxCopy = colliderCopy as BoxCollider;
                var boxChild = child as BoxCollider;
                boxCopy.size = boxChild.size * colliderSizeMultiplier;
                boxCopy.center = boxChild.center;
            }

            else if (colliderCopy is SphereCollider)
            {
                var sphereCopy = colliderCopy as SphereCollider;
                var sphereChild = child as SphereCollider;
                sphereCopy.radius = sphereChild.radius * colliderSizeMultiplier;
                sphereCopy.center = sphereChild.center;
            }

            else if (colliderCopy is CapsuleCollider)
            {
                var capsuleCopy = colliderCopy as CapsuleCollider;
                var capsuleChild = child as CapsuleCollider;
                capsuleCopy.radius = capsuleChild.radius * colliderSizeMultiplier;
                capsuleCopy.height = capsuleChild.height;
                capsuleCopy.center = capsuleChild.center;
                capsuleCopy.direction = capsuleChild.direction;
            }
         
            colliderCopy.isTrigger = true;
            return colliderCopy;
        }

        private void OnDestroy()
        {
            PhysicsManager.Instance.Remove(GetComponentsInChildren<Collider>(), Rigidbody);
        }

        /// <summary>
        /// Make a copy of the given component
        /// </summary>
        /// <param name="original"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private Component CopyComponent(Component original, GameObject destination)
        {
            var type = original.GetType();
            var copy = destination.AddComponent(type);
            // Copied fields can be restricted with BindingFlags
            var fields = type.GetFields();
            foreach (var field in fields)
                field.SetValue(copy, field.GetValue(original));
            return copy;
        }

        public int AmountOfCollidingObjects()
        {
            var count = 0;
            foreach (var detector in _detectors)
            {
                if (detector.IsColliding)
                    count++;
            }
            return count;
        }
    }
}
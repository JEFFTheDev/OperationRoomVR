// Copyright (c) 2018 ManusVR
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public class TriggerBinder : MonoBehaviour {

        public Collider Collider { get; set; }
        //public List<Rigidbody> CollidingObjects = new List<Rigidbody>();
        public HashSet<Interactable> CollidingInteractables
        {
            get
            {
                _collidingInteractables.RemoveWhere(item => !item.HasCollision);
                return _collidingInteractables;
            }
        }

        private readonly HashSet<Interactable> _collidingInteractables = new HashSet<Interactable>();

        void Start()
        {
            Collider = GetComponent<Collider>();
        }

        /// <summary>
        /// Happens when the manus hand is entering a trigger
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerEnter(Collider collider)
        {
            PhysicsObject physicsObject = null;
            if (!PhysicsManager.Instance.GetPhysicsObject(collider.gameObject, out physicsObject))
                return;
            var interactable = collider.GetComponent<Interactable>();
            if (interactable == null)
                interactable = physicsObject.GameObject.GetComponent<Interactable>();
            if (interactable != null && interactable.IsGrabbable)
            {
                _collidingInteractables.Add(interactable);
            }
        }

        /// <summary>
        /// Happens when the manus hand exits a trigger
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerExit(Collider collider)
        {
            PhysicsObject physicsObject = null;
            if (!PhysicsManager.Instance.GetPhysicsObject(collider.gameObject, out physicsObject) || physicsObject.PhysicsLayer == PhysicsLayer.GetLayer(Layer.Phalange))
                return;

            var interactable = collider.GetComponent<Interactable>();
            if (interactable == null)
                interactable = physicsObject.GameObject.GetComponent<Interactable>();
            _collidingInteractables.Remove(interactable);
        }
    }
}

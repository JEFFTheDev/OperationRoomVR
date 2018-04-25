// Copyright (c) 2018 ManusVR
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.ManusVR.Scripts.PhysicalInteraction;
using UnityEngine;

namespace Assets.ManusVR.Scripts.ManusInterface
{
    public abstract class InterfaceItem : MonoBehaviour
    {
        public Action<Phalange> OnContactStart;
        public Action<Phalange> OnContactStay;
        public Action<Phalange> OnContactEnd;

        protected Vector3 CollisionPoint;
        protected Interactable Interactable;
        protected CollisionDetector Detector;
        protected Collider[] Colliders;

        protected device_type_t LastTouchedBy;
        protected bool CanInteract = true;

        private float _timeLastCollision;

        public HashSet<Phalange> CollidingPhalanges { get; private set; }

        protected virtual void Awake()
        {
            CacheColliders();
            EnableInteraction();
        }

        protected virtual void Start()
        {
            CollidingPhalanges = new HashSet<Phalange>();
            StartCoroutine(InitializeDetectorCoroutine());
        }

        protected virtual void CacheColliders()
        {
            Colliders = GetComponents<Collider>();
        }

        protected virtual IEnumerator InitializeDetectorCoroutine()
        {
            yield return null;
            Interactable = GetComponent<Interactable>();
            
            Detector = GetComponent<CollisionDetector>();
            if (Detector != null)
            {
                Detector.CollisionEnter += CollisionStart;
                Detector.CollisionStay += CollisionStay;
            }
        }

        public virtual void EnableInteraction()
        {
            if (CanInteract)
                return;

            CanInteract = true;

            if (Colliders == null || Colliders.Length == 0)
                return;
            
            foreach (Collider coll in Colliders)
            {
                coll.enabled = true;
            }
        }

        public virtual void DisableInteraction()
        {
            if (!CanInteract)
                return;

            CanInteract = false;

            if (Colliders == null || Colliders.Length == 0)
                return;

            foreach (Collider coll in Colliders)
            {
                coll.enabled = false;
            }
        }

        protected virtual void CollisionStart(Collision collision)
        {
            var phalange = collision.gameObject.GetComponent<Phalange>();
            if (phalange == null)
                return;

            CollidingPhalanges.Add(phalange);
            if(OnContactStart != null)
                OnContactStart(phalange);

            StartCoroutine(CollisionTimerCoroutine(collision));
        }

        protected virtual void CollisionStay(Collision collision)
        {
            var phalange = collision.gameObject.GetComponent<Phalange>();
            if (phalange == null || !CollidingPhalanges.Contains(phalange))
                return;

            CollisionPoint = Vector3.zero;
            foreach (var collisionContact in collision.contacts)
            {
                CollisionPoint += collisionContact.point;
            }
            CollisionPoint /= collision.contacts.Length;

            if (OnContactStay != null)
                OnContactStay(phalange);

            StartCoroutine(CollisionTimerCoroutine(collision));
        }

        protected virtual void CollisionExit(Collision collision)
        {
            var phalange = collision.gameObject.GetComponent<Phalange>();
            if (phalange == null || !CollidingPhalanges.Contains(phalange))
                return;

            if (OnContactEnd != null)
                OnContactEnd(phalange);

            CollidingPhalanges.Remove(phalange);
        }

        private IEnumerator CollisionTimerCoroutine(Collision collision)
        {
            float collisionTime = Time.time;
            _timeLastCollision = collisionTime;
            yield return new WaitForSeconds(0.05f);
            if(Mathf.Abs(_timeLastCollision - collisionTime) < 0.0001f)
                CollisionExit(collision);

        }

        private IEnumerator IgnoreCollisionUntilHandCanInteract(device_type_t hand, Collider otherCollider)
        {
            yield break;
            //Physics.IgnoreCollision(Colliders, otherCollider, true);
            //yield return new WaitUntil(() => (hand == device_type_t.GLOVE_LEFT ? InterfaceInteractionManager.Instance.LeftInteractionHand.InteractStatus : InterfaceInteractionManager.Instance.RightInteractionHand.InteractStatus) == InteractionType.Interact);
            //Physics.IgnoreCollision(Colliders, otherCollider, false);
        }

        void OnDrawGizmos()
        {
            if (CollidingPhalanges == null || CollidingPhalanges.Count == 0)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(CollisionPoint, 0.005f);
        }
    }
}

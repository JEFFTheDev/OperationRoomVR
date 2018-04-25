// Copyright (c) 2018 ManusVR
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    /// <summary>
    ///     Use this script to grab objects with the manus gloves
    /// </summary>
    public class ObjectGrabber : MonoBehaviour
    {
        public device_type_t DeviceType;                                // The deviceType that belongs to the grabber
        public TriggerBinder TriggerBinder;                             // The triggerbinder on the hand
        public Action<GameObject, device_type_t> OnItemGrabbed;
        public Interactable GrabbedItem { get { return _grabbedItem; } }
        public Rigidbody HandRigidbody { get { return _physicsHand.HandRigidbody; } }
        public HandData HandData;

        public bool HoldsItem
        {
            get { return _grabbedItem != null; }
        }

        private ThrowHandler _throwHandler;
        private double _averageOnGrab = 0;

        // Variables for releasing objects
        private const double AverageGrabOffset = 0.04f;

        private Interactable _grabbedItem;
        private PhysicsHand _physicsHand;

        public float DropDistance { get { return _physicsHand.DisconnectDistance; }}
        public float DropAngle
        {
            get { return _physicsHand.DisconnectAngle(); }
        }

        private void Start()
        {
            var controllers = GetComponents<PhysicsHand>();
            foreach (var controller in controllers)
                if (controller.DeviceType == DeviceType)
                    _physicsHand = controller;

            _throwHandler = gameObject.GetComponentsInChildren<ThrowHandler>()
                .FirstOrDefault(handler => handler.DeviceType == DeviceType);

            if (_throwHandler == null)
            {
                _throwHandler = gameObject.AddComponent<ThrowHandler>();
                _throwHandler.DeviceType = DeviceType;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateGrabObjects();
            UpdateReleaseObjects();
        }

        /// <summary>
        /// Try to grab items
        /// </summary>
        private void UpdateGrabObjects()
        {
            if (_grabbedItem != null || HandData.GetCloseValue(DeviceType) == CloseValue.Fist) return;
            // Always try to grab a item when the user is making a fist
            if (HandData.FirstJointAverage(DeviceType) > 0.32f && _physicsHand.AmountOfCollidingObjects() > 0)
                foreach (var rb in TriggerBinder.CollidingInteractables)
                    GrabItem(rb);


            if (_physicsHand.IsThumbColliding && _physicsHand.AmountOfCollidingObjects() > 1 && HandData.FirstJointAverage(DeviceType) > 0.07f)
                foreach (var rb in TriggerBinder.CollidingInteractables)
                    GrabItem(rb);
        }

        /// <summary>
        /// Try to release items
        /// </summary>
        private void UpdateReleaseObjects()
        {
            if (_grabbedItem == null || !HandData.ValidOutput(DeviceType))
                return;
            if (_grabbedItem != null && _grabbedItem.Hand != this)
                return;

            // Release item when the hand is completely open
            if (HandData.HandOpened(DeviceType))
            {
                Debug.Log("Released because the hand was fully open");
                //ReleaseItem();
            }

            if (HandData.FirstJointAverage(DeviceType) < _averageOnGrab)
            {
                ReleaseItem();
                //Debug.Log("Release this item " + HandData.Average(DeviceType) + " - " + _averageOnGrab);
            }
        }

        /// <summary>
        /// Release the grabbed item and turn collision with the object back on
        /// </summary>
        /// <returns></returns>
        private IEnumerator Release()
        {
            if (_grabbedItem == null) yield break;
            var item = _grabbedItem;
            _grabbedItem = null;
            item.Detach(this);
            _throwHandler.OnObjectRelease(item.Rigidbody);
            CollisionDetector detector = null;

            //todo check for every detector
            foreach (var collisionDetector in item.Detectors)
            {
                detector = collisionDetector;
                break;
            }

            HandRigidbody.centerOfMass = Vector3.zero;

            _physicsHand.EnableRotation(true);
            if (detector != null)
                yield return new WaitUntil(() => !detector.GetTriggerObjectsInLayer(Layer.Phalange).Any() && !TriggerBinder.CollidingInteractables.Contains(item));
            IgnoreCollision(item.Rigidbody, false);
        }

        public void ReleaseItem()
        {
            StartCoroutine(Release());
        }

        /// <summary>
        /// Ignore collision between the grabbed item and the hand
        /// </summary>
        /// <param name="rb"></param>
        /// <param name="ignore"></param>
        public void IgnoreCollision(Rigidbody rb, bool ignore)
        {
            foreach (var phalange in _physicsHand.GetComponentsInChildren<Phalange>())
            {
                    if (phalange == null) continue;
                    var fingerCollider = phalange.GetComponent<Collider>();                  
                    if (fingerCollider != null)
                    {
                        PhysicsObject physicsObject = null;
                        if (PhysicsManager.Instance.GetPhysicsObject(rb.gameObject, out physicsObject))
                            foreach (var c in physicsObject.Colliders)
                                Physics.IgnoreCollision(fingerCollider, c, ignore);
                    }

            }
        }

        /// <summary>
        /// Try to grab the given rigidbody
        /// </summary>
        /// <param name="interactable"></param>
        private void GrabItem(Interactable interactable)
        {
            if (_grabbedItem != null) return;
            PhysicsObject physicsObject = null;
            if (!PhysicsManager.Instance.GetPhysicsObject(interactable.gameObject, out physicsObject) || !interactable.IsGrabbable)
                return;

            _grabbedItem = interactable;
            _grabbedItem.Attach(_physicsHand.HandRigidbody, this);

            _averageOnGrab = HandData.FirstJointAverage(DeviceType) - AverageGrabOffset;

            // Ignore collision between the grabbed object and the grabbing hand
            IgnoreCollision(physicsObject.Rigidbody, true);

            if (OnItemGrabbed != null)
                OnItemGrabbed(_grabbedItem.gameObject, DeviceType);
            VibrateHand();

            if (_grabbedItem.AttachHandToItem)
                _physicsHand.EnableRotation(false);
            HandRigidbody.centerOfMass = HandRigidbody.transform.InverseTransformPoint(_grabbedItem.transform.position);
        }

        /// <summary>
        /// Vibrate the glove
        /// </summary>
        private void VibrateHand()
        {
            Manus.ManusSetVibration(HandData.Session, DeviceType, 0.7f, 150);
        }
    }
}


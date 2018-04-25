// Copyright (c) 2018 ManusVR
using System.Collections.Generic;
using System.Linq;
using cakeslice;
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    /// <summary>
    /// This class is used to easily make items interactable.
    /// In handles attaching and detaching the object to the hand of the player
    /// </summary>
    public class InteractableItem : Interactable
    {
        private readonly HashSet<Outline> _outlines = new HashSet<Outline>();

        private Rigidbody _target;
        private Vector3 _offsetPosition;
        private Quaternion _offsetRotation;

        public bool IsGrabbed { get { return Connection != null || _target != null; } }


        protected override void Initialize(Collider[] colliders)
        {
            base.Initialize(colliders);
            
            // Add outlines to all of the colliders
            foreach (Collider child in colliders)
            {
                var outline = child.GetComponent<Outline>();
                if (outline == null && child.GetComponent<MeshRenderer>())
                    outline = child.gameObject.AddComponent<Outline>();
                if (outline != null)
                    _outlines.Add(outline);
            }
        }

        public override void Attach(Rigidbody connectedBody, ObjectGrabber hand)
        {
            base.Attach(connectedBody, hand);
            Rigidbody.isKinematic = false;

            if (!AttachHandToItem)
            {
                _target = connectedBody;
                _offsetPosition = connectedBody.transform.InverseTransformPoint(Rigidbody.transform.position);
                _offsetRotation = Quaternion.Inverse(_target.transform.rotation) * transform.rotation;
            }

            else
            {
                // Destroy the old joint
                Destroy(Connection);

                // Add a fixed joint to the object
                ConfigurableJoint rotationJoint = connectedBody.gameObject.AddComponent<ConfigurableJoint>();

                Connection = rotationJoint;

                rotationJoint.xMotion = ConfigurableJointMotion.Locked;
                rotationJoint.yMotion = ConfigurableJointMotion.Locked;
                rotationJoint.zMotion = ConfigurableJointMotion.Locked;

                rotationJoint.angularXMotion = ConfigurableJointMotion.Locked;
                rotationJoint.angularYMotion = ConfigurableJointMotion.Locked;
                rotationJoint.angularZMotion = ConfigurableJointMotion.Locked;

                Connection.connectedBody = Rigidbody;
            }
            
            Rigidbody.useGravity = GravityWhenGrabbed;

            if (HighlightWhenGrabbed)
                ActivateOutline(true);
        }

        void FixedUpdate()
        {
            if (Hand == null) return;
            if (_target != null)
            {
                MoveBody(_target.transform.TransformPoint(_offsetPosition), Rigidbody, 3f, 10);
                    if (RotateWithHandRotation)
                RotateBody(_target.transform.rotation * _offsetRotation, Rigidbody, 3, 12, 1);
            }
            // release this object when it is to far away from the hand       
            if (_target != null)
            {
                if (DisconnectDistance() > DropDistance && AmountOfCollidingObjects() > 0)
                    Hand.ReleaseItem();
            }

            if (Connection != null)
            {
                if (Hand.DropDistance > DropDistance)
                    Hand.ReleaseItem();
            }
         
        }

        /// <summary>
        /// Dettach this object from the given hand
        /// </summary>
        /// <param name="hand"></param>
        public override void Detach(ObjectGrabber hand)
        {
            base.Detach(hand);
            Rigidbody.useGravity = GravityWhenReleased;
            Rigidbody.isKinematic = KinematicWhenReleased;
            Destroy(Connection);
            _target = null;
            if (HighlightWhenGrabbed)
                ActivateOutline(false);
        }

        internal void ActivateOutline(bool active)
        {
            foreach (var outline in _outlines)
                outline.enabled = active;
        }

        private float DisconnectDistance()
        {
            return Vector3.Distance(_target.transform.TransformPoint(_offsetPosition), transform.position);
        }

        private void MoveBody(Vector3 target, Rigidbody body, float maxPositionDelta, float maxVelocity)
        {
            var posDelta = target - body.position;
            var velocityTarget = Vector3.ClampMagnitude(posDelta / Time.fixedDeltaTime, 5);
            body.velocity = Vector3.MoveTowards(body.velocity, velocityTarget, maxPositionDelta);

            body.velocity = Vector3.ClampMagnitude(body.velocity, maxVelocity);
        }

        private void RotateBody(Quaternion targetRotation, Rigidbody body, float maxRotationDelta, float maxAngularVelocity, float multiplier)
        {
            body.maxAngularVelocity = maxAngularVelocity;
            var rotDelta = targetRotation * Quaternion.Inverse(body.transform.rotation);
            float angle;
            Vector3 axis;

            rotDelta.ToAngleAxis(out angle, out axis);
            if (angle > 180) angle -= 360;
            var angularTarget = angle * axis * multiplier;

            if (angularTarget.magnitude > 0.001f)
                body.angularVelocity = angularTarget;
            body.angularVelocity = Vector3.ClampMagnitude(body.angularVelocity, maxAngularVelocity);
        }

        /// <summary>
        /// Active the outline when there is collision between the object and fingers
        /// </summary>
        /// <param name="collision"></param>
        protected override void CollisionEnter(Collision collision)
        {
            base.CollisionEnter(collision);
            if (!HighlightOnImpact) return;
            foreach (var detector in _detectors)
            {
                if (detector.GetCollidingObjectsInLayer(Layer.Phalange).ToArray().Length <= 0) continue;
                ActivateOutline(true);
                return;
            }
        }

        /// <summary>
        /// Deactive the outline
        /// </summary>
        /// <param name="collision"></param>
        protected override void CollisionExit(Collision collision)
        {
            base.CollisionExit(collision);

            if (Hand != null || !HighlightOnImpact) return;
            foreach (var detector in _detectors)
                if (detector.GetCollidingObjectsInLayer(Layer.Phalange).ToArray().Length > 0)
                    return;

            ActivateOutline(false);
        }
    }
}




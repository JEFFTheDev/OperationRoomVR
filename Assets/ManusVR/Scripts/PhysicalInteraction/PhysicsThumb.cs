// Copyright (c) 2018 ManusVR
using Assets.ManusVR.Scripts.Factory;
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public class PhysicsThumb : Finger
    {
        private Rigidbody _thumbRigidbody;
        private PhysicsHand _physicsHand;
        private Phalange _phalange;

        public override void Start()
        {
            base.Start();
            //_thumbRigidbody = Phalanges[1].GetComponent<Rigidbody>();
            _physicsHand = Hand as PhysicsHand;
            if (_physicsHand == null)
                Debug.LogError("Physics thumb only works with a physics hand");
            // Get the thumb of the target hand
            ConfigureThumb(Phalanges[1]);

            AddHingeJoint(Phalanges[2], -Hand.WristTransform.right, _thumbRigidbody);
            _phalange = HandFactory.GetPhalange(Phalanges[2], Index, 2, DeviceType);
        }

        public override void RotatePhalange(int pos, Quaternion targetRotation)
        {
            if (pos == 1)
                RotateThumb();
        }

        private void RotateThumb()
        {
            if (_thumbRigidbody == null)
                return;
            var maxAngularVelocity = AmountOfCollidingObjects() > 0 ? 3 : 15;
            var maxRotationDelta = AmountOfCollidingObjects() > 0 ? 0.1f : 2f;
            RotateThumb(Hand.ThumbRotation(), _thumbRigidbody, maxRotationDelta, maxAngularVelocity);
        }

        private void ConfigureThumb(GameObject thumbGameObject)
        {
            _thumbRigidbody = thumbGameObject.AddComponent<Rigidbody>();
            _thumbRigidbody.mass = 1f;
            _thumbRigidbody.useGravity = false;
            _thumbRigidbody.centerOfMass = Vector3.zero;

            var thumbJoint = thumbGameObject.AddComponent<ConfigurableJoint>();
            thumbJoint.connectedBody = _physicsHand.Wrist.Rigidbody;

            thumbJoint.xMotion = ConfigurableJointMotion.Locked;
            thumbJoint.yMotion = ConfigurableJointMotion.Locked;
            thumbJoint.zMotion = ConfigurableJointMotion.Locked;
            thumbJoint.angularXMotion = ConfigurableJointMotion.Limited;
            var limit = new SoftJointLimit();
            limit.limit = -30;
            thumbJoint.lowAngularXLimit = limit;
            limit.limit = 30;
            thumbJoint.highAngularXLimit = limit;
        }


        /// <summary>
        ///     rotate the given rigidbody to the target rotation
        /// </summary>
        /// <param name="target"></param>
        /// <param name="body"></param>
        /// <param name="velocityMultiplier"></param>
        /// <param name="maxRotationDelta"></param>
        private void RotateThumb(Quaternion target, Rigidbody body, float maxRotationDelta, float maxAngularVelocity)
        {
            body.maxAngularVelocity = maxAngularVelocity;
            var rotDelta = target * Quaternion.Inverse(body.transform.rotation);
            float angle;
            Vector3 axis;

            rotDelta.ToAngleAxis(out angle, out axis);
            if (angle > 180) angle -= 360;

            var angularTarget = angle * axis;
            if (angularTarget.magnitude > 0.001f)
                body.angularVelocity = Vector3.MoveTowards(body.angularVelocity, angularTarget, maxRotationDelta);
        }

        private void AddHingeJoint(GameObject gameObject, Vector3 axis, Rigidbody connectedBody)
        {
            var joint = gameObject.GetComponent<HingeJoint>();
            if (joint != null)
            {
                Debug.LogWarning(gameObject.name + " already has a hingejoint attached to it");
            }
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();

            rb.mass = 0.1f;
            joint = gameObject.AddComponent<HingeJoint>();

            joint.useLimits = true;
            joint.axis = axis;

            joint.connectedBody = connectedBody;
            joint.useMotor = true;
        }

        public override int AmountOfCollidingObjects()
        {
            if (_phalange == null || _phalange.Detector == null)
                return 0;
            return _phalange.Detector.IsColliding ? 1 : 0;
        }
    }
}

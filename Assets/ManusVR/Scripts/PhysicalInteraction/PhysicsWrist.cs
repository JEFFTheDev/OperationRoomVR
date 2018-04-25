// Copyright (c) 2018 ManusVR
using Assets.ManusVR.Scripts.Factory;
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public class PhysicsWrist : Wrist
    {
        public PhysicsWristSettings PhysicsWristSettings { get; set; }

        private PhysicsHand _physicsHand;
        private Phalange _phalange;
        private ConfigurableJoint _wristJoint;
        private Rigidbody _targetRigidbody;

        public override void RotateWrist()
        {
            if (PhysicsWristSettings == null)
                return;

            if (_targetRigidbody != null)
            {
                var maxRotationDelta = _physicsHand.AmountOfCollidingObjects() > 0 ? PhysicsWristSettings.MaxRotDeltaCollding : PhysicsWristSettings.MaxRotDeltaNotColliding;
                _targetRigidbody.MoveRotation(Quaternion.RotateTowards(Rigidbody.rotation, WristRotation(), maxRotationDelta));
            }

            if (_wristJoint != null)
                return;
            var maxAngularVelocity = _physicsHand.AmountOfCollidingObjects() > 0 ? PhysicsWristSettings.MaxAngularVelocityColliding : PhysicsWristSettings.MaxAngularVelocityNotColliding;
            RotateBody(WristRotation(), Rigidbody, 100, maxAngularVelocity, 1);
        }

        private void OnDisable()
        {
            Destroy(_wristJoint);
        }

        public override void Start()
        {
            base.Start();
            _physicsHand = Hand as PhysicsHand;
            if (_physicsHand == null)
                Debug.LogWarning("PhysicsWrist needs a PhysicsHand");

            _phalange = HandFactory.GetPhalange(gameObject, 0, 0, DeviceType);

        }

        private void ConnectWristJoint()
        {
            if (_wristJoint != null)
                return;
            _wristJoint = Rigidbody.gameObject.AddComponent<ConfigurableJoint>();
 
            GameObject target = new GameObject("TargetBody");
            target.transform.position = _physicsHand.Target.WristTransform.position;
            target.transform.rotation = _physicsHand.Target.WristTransform.rotation;
            _targetRigidbody = target.AddComponent<Rigidbody>();
            _targetRigidbody.isKinematic = true;
            _wristJoint.connectedBody = _targetRigidbody;

            _wristJoint.angularXMotion = ConfigurableJointMotion.Locked;
            _wristJoint.angularYMotion = ConfigurableJointMotion.Locked;
            _wristJoint.angularZMotion = ConfigurableJointMotion.Locked;
        }


        /// <summary>
        ///     rotate the given rigidbody to the targetRotation rotation
        /// </summary>
        /// <param name="targetRotation"></param>
        /// <param name="body"></param>
        /// <param name="maxRotationDelta"></param>
        /// <param name="maxAngularVelocity"></param>
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

            if (_physicsHand.DisconnectAngle() <0.5f)
                ConnectWristJoint();
        }
    }
}
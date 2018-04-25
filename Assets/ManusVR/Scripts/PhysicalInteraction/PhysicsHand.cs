// Copyright (c) 2018 ManusVR
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public class PhysicsHand : Hand
    {
        public PhysicsHandSettings PhysicsHandSettings { get; set; }
        public PhysicsWristSettings PhysicsWristSettings { get; set; }
        public PhysicsFingerSettings PhysicsFingerSettings { get; set; }

        private Coroutine _ghostingCoroutine;

        private Wrist _wristTarget;

        public Hand Target;
        public ObjectGrabber Grabber { get; set; }
        
        public float DisconnectDistance
        {
            get {
                if (Wrist == null || _wristTarget == null)
                    return 0;
                return Vector3.Distance(Wrist.transform.position, _wristTarget.transform.position);
            }
        }

        public float DisconnectAngle()
        {
            if (Wrist == null || _wristTarget == null)
                return 0;
            return Quaternion.Angle(Wrist.transform.rotation, _wristTarget.transform.rotation);

        }

        /// <summary>
        ///     Is the thumb of this hand currently colliding with a object
        /// </summary>
        public bool IsThumbColliding
        {
            get
            {
                if (!FingerControllers.ContainsKey(FingerIndex.thumb)) return false;
                return FingerControllers[FingerIndex.thumb].AmountOfCollidingObjects() > 0;
            }
        }




        // Use this for initialization
        public override void Start()
        {
            _wristTarget = Target.Wrist;
            base.Start();
        }

        public override Finger CreateFinger(FingerIndex finger)
        {
            Finger f = base.CreateFinger(finger);
            PhysicsFinger physicsFinger = f as PhysicsFinger;
            physicsFinger.PhysicsFingerSettings = PhysicsFingerSettings;
            return f;
        }

        public override Wrist AddWristController(device_type_t deviceType)
        {
            Wrist wrist = base.AddWristController(deviceType);
            PhysicsWrist physicsWrist = wrist as PhysicsWrist;
            physicsWrist.PhysicsWristSettings = PhysicsWristSettings;
            return wrist;
        }


        /// <summary>
        /// Change if this hand is detecting collision
        /// </summary>
        /// <param name="detect"></param>
        private void DetectCollision(bool detect)
        {
            foreach (var rb in GetComponentsInChildren<Rigidbody>())
                rb.detectCollisions = detect;
        }

        public override void FixedUpdate()
        {
            if (PhysicsHandSettings == null)
                return;
            base.FixedUpdate();
            if (_wristTarget == null) return;
            // Check if the wrist is to far away from the wrist of the target
            if (Vector3.Distance(_wristTarget.transform.position, Wrist.Rigidbody.position) > 0.3f)
            {
                if (_ghostingCoroutine == null)
                    _ghostingCoroutine = StartCoroutine(StartGhosting());
            }
        }

        public override void MoveWrist()
        {
            if (_wristTarget == null) return;
            //var maxPositionDelta = AmountOfCollidingObjects() == 0 ? 3f : 0.08f;
            var maxVelocity = AmountOfCollidingObjects() > 0 ? PhysicsHandSettings.MaxVelocityColliding : PhysicsHandSettings.MaxVelocityNotColliding;
            MoveBody(_wristTarget.transform, Wrist.Rigidbody, 10, maxVelocity);
        }

        private IEnumerator WaitForitemOutOfRange()
        {
            Vector3 targetOffset = -_wristTarget.transform.up * 0.1f;
            while (true)
            {
                var hits = Physics.OverlapSphere(_wristTarget.transform.position + targetOffset, 0.13f);
                if (DisconnectDistance < 0.1f && !hits.Any(raycastHit => raycastHit.transform.GetComponent<InteractableItem>()))
                    break;
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator StartGhosting()
        {
            DetectCollision(false);
            yield return StartCoroutine(WaitForitemOutOfRange());
            DetectCollision(true);
            _ghostingCoroutine = null;
        }
    
        /// <summary>
        ///     Move the given rigidbody to the target location
        /// </summary>
        /// <param name="target"></param>
        /// <param name="body"></param>
        /// <param name="maxPositionDelta"></param>
        private void MoveBody(Transform target, Rigidbody body, float maxPositionDelta, float maxVelocity)
        {
            var posDelta = target.position - body.position;
            float multiplier = 1;
            if (AmountOfCollidingObjects() > 0)
                multiplier = Mathf.Clamp((1f - DisconnectDistance * 2.5f), 0.03f, 1f);

            var velocityTarget = Vector3.ClampMagnitude(posDelta / Time.fixedDeltaTime * multiplier, 5);
            body.velocity = Vector3.MoveTowards(body.velocity, velocityTarget, maxPositionDelta);

            body.velocity = Vector3.ClampMagnitude(body.velocity, maxVelocity);
        }

        public override int AmountOfCollidingObjects()
        {
            var count = 0;
            foreach (var finger in FingerControllers.Values)
                count += finger.AmountOfCollidingObjects();
            if (Wrist != null)
                count += Wrist.AmountOfCollidingObjects();
            if (Grabber.GrabbedItem && Grabber.GrabbedItem.AmountOfCollidingObjects() > 0)
                count += Grabber.GrabbedItem.AmountOfCollidingObjects();
            return count;
        }
    }
}
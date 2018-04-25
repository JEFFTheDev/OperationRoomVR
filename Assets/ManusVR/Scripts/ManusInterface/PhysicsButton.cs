// Copyright (c) 2018 ManusVR
using System;
using System.Collections;
using Assets.ManusVR.Scripts.PhysicalInteraction;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Assets.ManusVR.Scripts.ManusInterface
{
    public enum ButtonState
    {
        Idle,
        Touched,
        Triggered
    }

    [RequireComponent(typeof(Collider), typeof(InteractableItem))]
    public class PhysicsButton : Button
    {
        [Range(0, 1)]
        [Tooltip("Height at which the button press is triggered. \n This is a lerp between 0 and 1.")]
        public float TriggerHeightLerp = 1;

        [HideInInspector]
        public Vector2 HeightLimits = new Vector2(0f, 0.015f);
        [Range(0,1)]
        [Tooltip("Resting height of the button.\n This is a lerp between 0 and 1.")]
        public float RestingHeightLerp = 1f;

        private ButtonState _currentState;
        private float _jointPositionSpring = 500;
        private Vector3 _initialLocalPosition;
        private bool _isInterfaceButton;

        //References
        protected ConfigurableJoint ConfigurableJoint;
        protected GameObject JointAnchor;
        public Rigidbody Rb { get; private set; }
        private Coroutine _ignoreCollisionCoroutine;

        public ButtonState CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                if (_currentState == value)
                    return;

                _currentState = value;
                
                //Handle state change
                switch (_currentState)
                {
                    case ButtonState.Idle:
                        break;
                    case ButtonState.Touched:
                        break;
                    case ButtonState.Triggered:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private bool IsButtonPressed
        {
            get
            {
                return transform.localPosition.y < GetHeightByLerp(TriggerHeightLerp);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _isInterfaceButton = GetComponent<RectTransform>() != null;
            _initialLocalPosition = transform.localPosition;
            var localPos = transform.localPosition;
            if (_isInterfaceButton)
                localPos.z = GetHeightByLerp(RestingHeightLerp);
            else
                localPos.y = GetHeightByLerp(RestingHeightLerp);
            transform.localPosition = localPos;
            
            AddConfigurableJoint();
            GetRigidbody();
            JointAnchor = AddAnchor();
            //_initialInverseTransformPoint = transform.InverseTransformPoint(_trigger.transform.position);
            CurrentState = ButtonState.Idle;
        }

        protected virtual void FixedUpdate()
        {
            if(CurrentState != ButtonState.Triggered && IsButtonPressed)
                ButtonPressed();

            if (CurrentState == ButtonState.Triggered && Mathf.InverseLerp(_initialLocalPosition.y + HeightLimits.x,
                    _initialLocalPosition.y + HeightLimits.y, transform.localPosition.y) >=
                RestingHeightLerp - 0.00000001f)
                CurrentState = ButtonState.Idle;

            if (transform.localPosition.y < _initialLocalPosition.y + HeightLimits.x ||
                transform.localPosition.y > _initialLocalPosition.y + HeightLimits.y)
            {
                //Rb.velocity = Vector3.zero;
                if (Interactable != null && Interactable.TotalCollidingObjects > 0)
                {
                    if (_ignoreCollisionCoroutine != null)
                        StopCoroutine(_ignoreCollisionCoroutine);

                    //_ignoreCollisionCoroutine = StartCoroutine(IgnoreCollisionWhileColliding());
                }
            }
            

            //TryReturnToPosition();
        }

        IEnumerator IgnoreCollisionWhileColliding()
        {
            Rb.isKinematic = true;
            while(Interactable.TotalCollidingObjects > 0)
                yield return new WaitForFixedUpdate();

            var locPos = transform.localPosition;
            locPos.y = Mathf.Lerp(_initialLocalPosition.y + HeightLimits.x, _initialLocalPosition.y + HeightLimits.y,
                RestingHeightLerp);
            transform.localPosition = locPos;
            Rb.isKinematic = false;
            _ignoreCollisionCoroutine = null;
        }

        public override void ButtonPressed(device_type_t handType)
        {
            if (CurrentState == ButtonState.Triggered)
                return;

            base.ButtonPressed(handType);
            CurrentState = ButtonState.Triggered;
        }

        protected override void CollisionStart(Collision collision)
        {
            base.CollisionStart(collision);
            if (CurrentState == ButtonState.Idle)
            {
                CurrentState = ButtonState.Touched;
            }
        }

        protected virtual void TryReturnToPosition()
        {
            if (transform.localPosition.z <= 0)
            {
                var locPos = transform.localPosition;
                locPos.z = 0;
                transform.localPosition = locPos;
                Rb.velocity = Vector3.zero;
                CurrentState = ButtonState.Idle;
            }
            if (Rb.IsSleeping() && !FloatComparer.AreEqual(transform.localPosition.z, _initialLocalPosition.z, 0.000001f))
                transform.localPosition =
                    Vector3.MoveTowards(transform.localPosition, _initialLocalPosition, 0.01f);
        }

        private float GetHeightLerp()
        {
            return Mathf.InverseLerp(_initialLocalPosition.y + HeightLimits.x, _initialLocalPosition.y + HeightLimits.y,
                transform.localPosition.y);
        }

        private float GetHeightByLerp(float lerp)
        {
            return Mathf.Lerp(_initialLocalPosition.y + HeightLimits.x, _initialLocalPosition.y + HeightLimits.y,
                lerp);
        }

        protected virtual ConfigurableJoint AddConfigurableJoint()
        {
            ConfigurableJoint = GetComponent<ConfigurableJoint>();
            if(ConfigurableJoint == null)
                ConfigurableJoint = gameObject.AddComponent<ConfigurableJoint>();

            ConfigurableJoint.xMotion = ConfigurableJointMotion.Locked;
            ConfigurableJoint.yMotion = _isInterfaceButton ?  ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
            ConfigurableJoint.zMotion = _isInterfaceButton ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked;

            ConfigurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
            ConfigurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
            ConfigurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
            
            var drive = _isInterfaceButton ? ConfigurableJoint.zDrive : ConfigurableJoint.yDrive;
            drive.positionSpring = _jointPositionSpring;
            if(_isInterfaceButton)
                ConfigurableJoint.zDrive = drive;
            else
                ConfigurableJoint.yDrive = drive;

            var limit = ConfigurableJoint.linearLimit;
            limit.limit = HeightLimits.y;
            ConfigurableJoint.linearLimit = limit;
            return ConfigurableJoint;
        }

        Rigidbody GetRigidbody()
        {
            Rb = GetComponent<Rigidbody>();
            if(Rb == null)
                Rb = gameObject.AddComponent<Rigidbody>();

            Rb.drag = 20;
            Rb.mass = 1f;
            Rb.useGravity = false;
            return Rb;
        }

        GameObject AddAnchor()
        {
            GameObject anchor = new GameObject("Physics Button Anchor");
            anchor.transform.parent = transform.parent;
            anchor.transform.position = transform.position;

            var rb = anchor.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            ConfigurableJoint.connectedBody = rb;

            return anchor;
        }
        
        /// <summary>
        /// Vibrate the glove
        /// </summary>
        private void VibrateHand(device_type_t device)
        {
            //Manus.ManusSetVibration(HandData.Instance.Session, device, 0.7f, 150);
        }

        void OnDrawGizmos()
        {
            //var width = 0.005f;
            //var topCenter = transform.TransformPoint(Vector3.zero + Vector3.up * HeightLimits.y);
            //var bottomCenter = transform.TransformPoint(Vector3.zero + Vector3.up * TriggerDistance);
            //float centerY = Mathf.Lerp(topCenter.y, bottomCenter.y, 0.5f);
            //Gizmos.color = Color.red;
            //Gizmos.DrawCube(new Vector3(topCenter.x, centerY, topCenter.z), new Vector3(width, (topCenter.y - centerY) * 2, width));
            //topCenter = transform.TransformPoint(Vector3.zero + Vector3.up * HeightLimits.x);
            //bottomCenter = transform.TransformPoint(Vector3.zero + Vector3.up * TriggerDistance);
            //centerY = Mathf.Lerp(topCenter.y, bottomCenter.y, 0.5f);
            //Gizmos.color = Color.green;
            //Gizmos.DrawCube(new Vector3(topCenter.x, centerY, topCenter.z), new Vector3(width, (topCenter.y - centerY) * 2, width));
            var width = 0.005f;
            if (!Application.isPlaying)
                _initialLocalPosition = transform.localPosition;

            var topCenter = transform.parent.TransformPoint(new Vector3(0, GetHeightByLerp(1), 0));
            var bottomCenter = transform.parent.TransformPoint(new Vector3(0, GetHeightByLerp(TriggerHeightLerp), 0));
            float centerY = Mathf.Lerp(topCenter.y, bottomCenter.y, 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(topCenter.x, centerY, topCenter.z), new Vector3(width, (topCenter.y - centerY) * 2, width));
            topCenter = transform.parent.TransformPoint(new Vector3(0, GetHeightByLerp(0), 0));
            bottomCenter = transform.parent.TransformPoint(new Vector3(0, GetHeightByLerp(TriggerHeightLerp), 0));
            centerY = Mathf.Lerp(topCenter.y, bottomCenter.y, 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector3(topCenter.x, centerY, topCenter.z), new Vector3(width, (topCenter.y - centerY) * 2, width));
        }
    }
}

// Copyright (c) 2018 ManusVR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public enum ScaleBehaviour
    {
        RetainScale = 0,
        InheritFromTarget = 1,
        CustomScale = 2
    }

    public enum SnapState
    {
        Idle = 0,
        Snapping = 1,
        Snapped = 2,
        Releasing = 3
    }

    [RequireComponent(typeof(InteractableItem))]
    public class SnappableObject : MonoBehaviour
    {
        //Actions
        [Header("Snap Events")]
        public Action OnSnap;
        public Action OnRelease;

        //UnityEvents
        [SerializeField] private UnityEvent _onSnapEvent;
        [SerializeField] private UnityEvent _onReleaseEvent;

        //Snap Settings
        [Header("Snap Settings")]
        [Tooltip("A list of targets this object is allowed to snap to.")]
        public List<SnapTarget> PossibleTargets = new List<SnapTarget>();
        [HideInInspector]
        public SnapTarget CurrentlySnappedTo;
        private readonly List<SnapTarget> _targetsInRange = new List<SnapTarget>();

        //State variables
        [Tooltip("The time it takes in seconds to complete the snapping procedure.")]
        public float SnapTime = 0.2f;
        public SnapState SnapState = SnapState.Idle;
        [SerializeField]
        public ScaleBehaviour ScaleBehaviour;
        [SerializeField, HideInInspector]
        private readonly Vector3 _customScale = Vector3.one;
        private Transform _unsnappedParent;

        //Coroutine variables
        private Coroutine _snapCoroutine;

        //References
        private InteractableItem _interactableItem;
        private Collider[] _colliders;
        private Rigidbody _rb;
        private Collider _triggerCollider;
        private CollisionDetector[] _detectors;

        private bool CanSnap
        {
            get
            {
                return enabled && _targetsInRange != null && _targetsInRange.Count > 0 &&
                       _targetsInRange.Any(target => target.CurrentlySnapped == null);
            }
        }

        void Awake()
        {
            //Get Components
            _colliders = GetComponents<Collider>();
            _triggerCollider = _colliders.FirstOrDefault(coll => coll.isTrigger);
            if (_triggerCollider != null)
                _triggerCollider.enabled = enabled;
            _interactableItem = GetComponent<InteractableItem>();
            _rb = GetComponent<Rigidbody>();

            //Cache initial parent
            _unsnappedParent = transform.parent;

            //Event Handling
            OnSnap += () =>
            {
                if (_onSnapEvent != null) _onSnapEvent.Invoke();
            };
            OnRelease += () =>
            {
                if (_onReleaseEvent != null) _onReleaseEvent.Invoke();
            };

            _interactableItem.OnGrabbed += () =>
            {
                if (SnapState == SnapState.Snapped)
                    Release();
            };
        }

        void Start()
        {
            _detectors = GetComponentsInChildren<CollisionDetector>();
        }

        void OnEnable()
        {
            if (_snapCoroutine == null && CanSnap)
                _snapCoroutine = StartCoroutine(SnapCoroutine());
        }

        void OnDisable()
        {
            if (_snapCoroutine != null)
            {
                _rb.isKinematic = false;
                SnapState = SnapState.Idle;
                StopCoroutine(_snapCoroutine);
                _snapCoroutine = null;
            }
        }

        public IEnumerator SnapCoroutine()
        {
            SnapState = SnapState.Snapping;
            _rb.isKinematic = true;
            var wasGrabbable = _interactableItem.IsGrabbable;
            _interactableItem.IsGrabbable = false;
            yield return null;

            if (!CanSnap)
            {
                _rb.isKinematic = false;
                _interactableItem.IsGrabbable = wasGrabbable;
                _snapCoroutine = null;
                SnapState = SnapState.Idle;
                yield break;
            }

            SnapTarget snapTarget = _targetsInRange.Where(target => target.CurrentlySnapped == null).OrderBy(target => Quaternion.Angle(transform.rotation, target.transform.rotation)).First();

            //Making sure the item is handled correctly with the hands.
            _interactableItem.DisableCollision();
            _interactableItem.Detach();
            _rb.isKinematic = true;

            snapTarget.OnSnap(this);

            Vector3 targetScale = Vector3.one;
            switch (ScaleBehaviour)
            {
                case ScaleBehaviour.RetainScale:
                    targetScale = transform.localScale;
                    break;
                case ScaleBehaviour.InheritFromTarget:
                    targetScale = snapTarget.transform.localScale;
                    break;
                case ScaleBehaviour.CustomScale:
                    targetScale = _customScale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Bind funcs to prevent code repetition.
            Func<float> getDist = () => Vector3.Distance(transform.position, snapTarget.transform.position);
            Func<float> getAngle = () => Quaternion.Angle(transform.rotation, snapTarget.transform.rotation);

            //Calculate what amount of translation and rotation should be applied per frame.
            float angleDelta = getAngle() / (SnapTime / Time.fixedDeltaTime);
            float distDelta = getDist() / (SnapTime / Time.fixedDeltaTime);

            //Setting the transform to the target transform over a certain time.
            while (getDist() > 0.0001f || getAngle() > 0.001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, snapTarget.transform.position, distDelta);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, snapTarget.transform.rotation, angleDelta);

                if (ScaleBehaviour != ScaleBehaviour.RetainScale)
                    transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, distDelta);
                yield return new WaitForFixedUpdate();
            }

            _interactableItem.EnableCollision();
            CurrentlySnappedTo = snapTarget;
            transform.SetParent(snapTarget.transform, true);
            SnapState = SnapState.Snapped;

            if (OnSnap != null)
                OnSnap();

            yield return new WaitUntil(()=> _detectors.Sum(detector => detector.GetCollidingObjectsInLayer(Layer.Phalange).Count()) == 0);
            _interactableItem.IsGrabbable = wasGrabbable;
            _snapCoroutine = null;
        }

        void Release()
        {
            SnapState = SnapState.Releasing;
            CurrentlySnappedTo.OnRelease();
            CurrentlySnappedTo = null;
            transform.SetParent(_unsnappedParent, true);
            _rb.isKinematic = false;
            SnapState = SnapState.Idle;

            if (OnRelease != null)
                OnRelease();
        }

        void OnTriggerEnter(Collider other)
        {
            if (SnapState != SnapState.Idle)
                return;

            var snapTarget = other.gameObject.GetComponent<SnapTarget>();
            if (snapTarget == null || !PossibleTargets.Contains(snapTarget) || snapTarget.CurrentlySnapped != null)
                return;

            _targetsInRange.Add(snapTarget);

            if (_snapCoroutine == null && CanSnap)
                _snapCoroutine = StartCoroutine(SnapCoroutine());
        }

        void OnTriggerExit(Collider other)
        {
            var snapTarget = other.gameObject.GetComponent<SnapTarget>();
            _targetsInRange.Remove(snapTarget);
        }
    }
}

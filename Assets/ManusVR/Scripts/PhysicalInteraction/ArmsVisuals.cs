// Copyright (c) 2018 ManusVR
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public class ArmsVisuals : MonoBehaviour {

        [Header("Visual settings")]
        public SkinnedMeshRenderer LeftVisualHand;
        public SkinnedMeshRenderer RightVisualHand;
        public SkinnedMeshRenderer LeftVisualUnderarm;
        public SkinnedMeshRenderer RightVisualUnderarm;

        public Transform LeftUnderarm;
        public Transform RightUnderarm;
        public Transform LeftUnderarmRot;
        public Transform RightUnderarmRot;
        private PhysicsHandManager _handManager;

        [Range(0, 1)]
        public float MaxOpacity = 0.7f;

        private Renderer _leftHandRenderer;
        private Renderer _rightHandRenderer;
        private Renderer _leftUnderarm;
        private Renderer _rightUnderarm;
        private void Start()
        {
            _leftHandRenderer = LeftVisualHand.GetComponent<Renderer>();
            _rightHandRenderer = RightVisualHand.GetComponent<Renderer>();
            _leftUnderarm = LeftVisualUnderarm.GetComponent<Renderer>();
            _rightUnderarm = RightVisualUnderarm.GetComponent<Renderer>();
            _handManager = GetComponent<PhysicsHandManager>();
        }


        // Update is called once per frame
        void Update ()
        {
            if (_handManager == null || _handManager.HandControllers.Count < 2) return;
            ChangeArmsOpacity(_handManager.HandControllers[0].DisconnectDistance, _leftHandRenderer, _leftUnderarm);
            ChangeArmsOpacity(_handManager.HandControllers[1].DisconnectDistance, _rightHandRenderer, _rightUnderarm);

            LeftUnderarm.position = _handManager.HandControllers[0].WristTransform.position;
            RightUnderarm.position = _handManager.HandControllers[1].WristTransform.position;


            LeftUnderarm.rotation = LeftUnderarmRot.rotation;
            RightUnderarm.rotation = RightUnderarmRot.rotation;
        }

        void ChangeArmsOpacity(float distance, Renderer handRenderer, Renderer armRenderer)
        {
            distance *= Mathf.Round(distance * 5000f) / 100f;
            distance -= 0.2f;
            handRenderer.enabled = !(distance <= 0.04f);

            Color handColor = handRenderer.material.color;
            Color armColor = armRenderer.material.color;

            handColor.a = Mathf.Clamp(distance, 0, MaxOpacity);
            armColor.a = Mathf.Clamp(distance, 0, MaxOpacity);

            handRenderer.material.color = handColor;
            armRenderer.material.color = armColor;
        }
    }
}

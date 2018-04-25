// Copyright (c) 2018 ManusVR
using System.Collections;
using System.Collections.Generic;
using ManusVR;
using UnityEngine;
using Valve.VR;

namespace Assets.ManusVR.Scripts
{
    public class TrackingManager : MonoBehaviour
    {
        public enum EIndex
        {
            None = -1,
            Hmd = (int) OpenVR.k_unTrackedDeviceIndex_Hmd,
            Limit = (int) OpenVR.k_unMaxTrackedDeviceCount
        }

        public enum EUsableTracking
        {
            Controller,
            GenericTracker
        }

        private enum ERole
        {
            HMD,
            LeftHand,
            RightHand
        }

        private class TrackedDevice
        {
            public int index;
            public bool isValid;
        }

        public EUsableTracking trackingToUse = EUsableTracking.GenericTracker;
        private ETrackedDeviceClass pTrackingToUse = ETrackedDeviceClass.GenericTracker;

        public Transform HMD;
        public Transform leftTracker;
        public Transform rightTracker;
        public static TrackingManager Instance;
        private Transform[] trackerTransforms;

        private TrackedDevice[] devices;

        SteamVR_Events.Action newPosesAction;

        [SerializeField] private TrackingValues _trackingValues;
        public KeyCode switchArmsButton = KeyCode.None;

        // Use this for initialization
        void Start()
        {
            pTrackingToUse = trackingToUse == EUsableTracking.Controller ? ETrackedDeviceClass.Controller : ETrackedDeviceClass.GenericTracker;

            trackerTransforms = new Transform[3];
            trackerTransforms[(int) ERole.HMD] = HMD;
            trackerTransforms[(int) ERole.LeftHand] = leftTracker;
            trackerTransforms[(int) ERole.RightHand] = rightTracker;

            int num = System.Enum.GetNames(typeof(ERole)).Length;
            devices = new TrackedDevice[num];

            for (int i = 0; i < num; i++)
            {
                devices[i] = new TrackedDevice();
                devices[i].index = new int();
                devices[i].index = (int) EIndex.None;
                devices[i].isValid = new bool();
                devices[i].isValid = false;

                GetIndex(i);
            }

            bool _useTrackers = trackingToUse == TrackingManager.EUsableTracking.GenericTracker;

            // Rotate the offsets of the TrackerOffset when the user is using controllers
            Transform[] trackers = new Transform[2];
            trackers[0] = leftTracker;
            trackers[1] = rightTracker;
            if (!_useTrackers)
                foreach (var trackerOffset in trackers) 
                {
                    trackerOffset.localRotation = Quaternion.Euler(90, -180, 0);
                    var currentLocalPos = trackerOffset.localPosition;
                    trackerOffset.localPosition = new Vector3(-currentLocalPos.x, -currentLocalPos.y, -currentLocalPos.z);
                }

            if (_trackingValues.AreArmsSwitched)
                SwitchArms(false);
        }

        void Awake()
        {
            if (Instance == null)
                Instance = this;

            newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
        }

        void OnEnable()
        {
            var render = SteamVR_Render.instance;
            if (render == null)
            {
                enabled = false;
                return;
            }

            newPosesAction.enabled = true;
        }

        void OnDisable()
        {
            newPosesAction.enabled = false;

            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i] != null)
                    devices[i].isValid = false;
            }
                
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(switchArmsButton))
            {
                SwitchArms();
                return;
                // Switch the indices around.
                int leftNum = (int) ERole.LeftHand;
                int rightNum = (int) ERole.RightHand;
                int OldLeftIndex = devices[leftNum].index;

                devices[leftNum].index = devices[rightNum].index;
                devices[rightNum].index = OldLeftIndex;
            }
        }

        public void SwitchArms(bool updateSettings = true)
        {
            if (updateSettings)
                _trackingValues.AreArmsSwitched = !_trackingValues.AreArmsSwitched;
            Transform left = trackerTransforms[(int)ERole.LeftHand];
            trackerTransforms[(int)ERole.LeftHand] = trackerTransforms[(int)ERole.RightHand];
            trackerTransforms[(int)ERole.RightHand] = left;
        }

        private void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            for (int deviceNum = 0; deviceNum < devices.Length; deviceNum++)
            {
                // if no role is set or the tracked object is the head
                //if (myRole == ETrackedControllerRole.Invalid && !isHead)
                if (devices[deviceNum].index == (int) EIndex.None)
                    continue;

                int intIndex = (int) devices[deviceNum].index;
                devices[deviceNum].isValid = false;

                if (poses.Length <= intIndex)
                    continue;
                try
                {
                    if (!poses[intIndex].bDeviceIsConnected)
                        continue;
                }
                catch (System.IndexOutOfRangeException)
                {
                    // retry to get the glove index
                    GetIndex(deviceNum);
                    continue;
                }

                if (!poses[intIndex].bPoseIsValid)
                    continue;
                devices[deviceNum].isValid = true;

                var pose = new SteamVR_Utils.RigidTransform(poses[intIndex].mDeviceToAbsoluteTracking);

                // make sure the offset is localized
                trackerTransforms[deviceNum].localPosition = pose.pos;
                trackerTransforms[deviceNum].localRotation = pose.rot;
            }
        }
    
        void GetIndex(int deviceNum)
        {
            ERole role = (ERole) deviceNum;

            if (role == ERole.HMD)
            {
                devices[deviceNum].index = (int) EIndex.Hmd;
                return;
            }

            int DeviceCount = 0;

            for (uint i = 0; i < (uint) EIndex.Limit; i++)
            {
                ETrackedPropertyError error = new ETrackedPropertyError();
                ETrackedDeviceClass type;
                if (OpenVR.System != null)
                    type = (ETrackedDeviceClass) OpenVR.System.GetInt32TrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_DeviceClass_Int32, ref error);
                else
                {
                    continue;
                }

                if (pTrackingToUse == ETrackedDeviceClass.Controller && type == ETrackedDeviceClass.Controller
                    || pTrackingToUse == ETrackedDeviceClass.GenericTracker && type == ETrackedDeviceClass.GenericTracker)
                {
                    if (role == ERole.LeftHand && DeviceCount == 0 || role == ERole.RightHand && DeviceCount == 1)
                    {
                        devices[deviceNum].index = (int) i;
                        return;
                    }

                    DeviceCount++;
                }
            }
        }

        public void SetInputData(List<Vector3> leftPositions, List<Quaternion> leftRotations, 
            List<Vector3> rightPositions, List<Quaternion> rightRotations, List<Vector3> headPositions, List<Quaternion> headRotations)
        {
            newPosesAction.enabled = false;
            StartCoroutine(PlayRecording(leftPositions, rightPositions, leftRotations, rightRotations, headPositions, headRotations));
        }

        public void SetInputData(List<Vector3> leftPositions, List<Vector3> rightPositions)
        {
            newPosesAction.enabled = false;
            StartCoroutine(PlayRecording(leftPositions, rightPositions));
        }

        IEnumerator PlayRecording(List<Vector3> leftPositions, List<Vector3> rightPositions)
        {
            int count = leftPositions.Count - 1;
            while (true)
            {
                for (int i = 0; i < count; i++)
                {
                    leftTracker.localPosition = leftPositions[i];
                    rightTracker.localPosition = rightPositions[i];
                    yield return new WaitForFixedUpdate();
                    
                }
            }
        }IEnumerator PlayRecording(List<Vector3> leftPositions, List<Vector3> rightPositions, List<Quaternion> leftRotations, List<Quaternion> rightRotations
            , List<Vector3> headPositions, List<Quaternion> headRotations)
        {
            int count = leftPositions.Count - 1;
            while (true)
            {
                for (int i = 0; i < count; i++)
                {
                    trackerTransforms[(int)ERole.LeftHand].localPosition = leftPositions[i];
                    trackerTransforms[(int) ERole.LeftHand].localRotation = leftRotations[i];
                    trackerTransforms[(int)ERole.RightHand].localPosition = rightPositions[i];
                    trackerTransforms[(int) ERole.RightHand].localRotation = rightRotations[i];
                    trackerTransforms[(int) ERole.HMD].localPosition = headPositions[i];
                    trackerTransforms[(int) ERole.HMD].localRotation = headRotations[i];
                    yield return new WaitForFixedUpdate();
                    
                }
            }
        }
    }
}

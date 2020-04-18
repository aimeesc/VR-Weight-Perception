using System;
using System.Reactive.Subjects;
using UnityEngine;
using Valve.VR;

namespace exiii.Unity.SteamVR
{
    public class SteamPoseTracker : InteractorNode, ITracker
    {
        #region Inspector

        [Header(nameof(SteamPoseTracker))]
        [SerializeField]
        private SteamVR_Action_Pose m_PoseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

        [Tooltip("The device this action should apply to. Any if the action is not device specific.")]
        [SerializeField, UnchangeableInPlaying]
        private SteamVR_Input_Sources m_InputSource;

        [Tooltip("If not set, relative to parent")]
        [SerializeField]
        private Transform m_Origin;

        [Tooltip("Can be disabled to stop broadcasting bound device status changes")]
        [SerializeField]
        private bool m_BroadcastDeviceChanges = true;

        #endregion Inspector

        /// <summary>Returns whether or not the current pose is in a valid state</summary>
        public bool isValid { get { return m_PoseAction[m_InputSource].poseIsValid; } }

        /// <summary>Returns whether or not the pose action is bound and able to be updated</summary>
        public bool isActive { get { return m_PoseAction[m_InputSource].active; } }

        private int m_DeviceIndex = -1;

        private SteamVR_HistoryBuffer m_HistoryBuffer = new SteamVR_HistoryBuffer(30);

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!this.IsPrefab())
            {
                SearchTrackingOrigin();
            }
        }

        private void SearchTrackingOrigin()
        {
            if (m_Origin != null) { return; }

            EHLDebug.Log($"{name} : SearchTrackingOrigin", this);

            var rig = FindObjectOfType<CameraRig>();

            if (rig != null) { m_Origin = rig.transform; }
        }

#endif

        protected override void Start()
        {
            base.Start();

            if (m_PoseAction == null)
            {
                Debug.LogError("<b>[SteamVR]</b> No pose action set for this component");
                return;
            }

            CheckDeviceIndex();

            // seek origin.
            if (m_Origin == null && CameraRig.IsExist)
            {
                m_Origin = CameraRig.Instance.transform;
            }

            if (InteractorRoot != null)
            {
                ChangeTrackingType(InteractorRoot.TrackingSettings);
                InteractorRoot.SetTracker(this);
            }
        }

        private SteamVR_Input_Sources m_InitializeInputSource = SteamVR_Input_Sources.Any;

        public override void Initialize()
        {
            base.Initialize();

            Valve.VR.SteamVR.Initialize();

            if (m_PoseAction != null)
            {
                m_InitializeInputSource = m_InputSource;

                m_PoseAction[m_InputSource].onUpdate += SteamVR_Behaviour_Pose_OnUpdate;
            }
        }

        public override void Terminate()
        {
            base.Terminate();

            if (m_PoseAction != null)
            {
                m_PoseAction[m_InitializeInputSource].onUpdate -= SteamVR_Behaviour_Pose_OnUpdate;
            }

            m_HistoryBuffer.Clear();
        }

        private void SteamVR_Behaviour_Pose_OnUpdate(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
        {
            UpdateHistoryBuffer();

            UpdateTransform();
        }

        protected virtual void UpdateTransform()
        {
            CheckDeviceIndex();

            if (m_PoseAction[m_InputSource].active)
            {
                if (m_Origin != null)
                {
                    transform.position = m_Origin.transform.TransformPoint(m_PoseAction[m_InputSource].localPosition);
                    transform.rotation = m_Origin.rotation * m_PoseAction[m_InputSource].localRotation;
                }
                else
                {
                    transform.localPosition = m_PoseAction[m_InputSource].localPosition;
                    transform.localRotation = m_PoseAction[m_InputSource].localRotation;
                }
            }

            if (m_PositionUpdate != null)
            {
                m_PositionUpdate.OnNext(transform);
            }
        }

        protected virtual void CheckDeviceIndex()
        {
            if (m_PoseAction[m_InputSource].active && m_PoseAction[m_InputSource].deviceIsConnected)
            {
                int currentDeviceIndex = (int)m_PoseAction[m_InputSource].trackedDeviceIndex;

                if (m_DeviceIndex != currentDeviceIndex)
                {
                    m_DeviceIndex = currentDeviceIndex;

                    if (m_BroadcastDeviceChanges)
                    {
                        this.gameObject.BroadcastMessage("SetInputSource", m_InputSource, SendMessageOptions.DontRequireReceiver);
                        this.gameObject.BroadcastMessage("SetDeviceIndex", m_DeviceIndex, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the device index for the device bound to the pose.
        /// </summary>
        public int GetDeviceIndex()
        {
            if (m_DeviceIndex == -1)
                CheckDeviceIndex();

            return m_DeviceIndex;
        }

        /// <summary>Returns the current velocity of the pose (as of the last update)</summary>
        public Vector3 GetVelocity()
        {
            return m_PoseAction[m_InputSource].velocity;
        }

        /// <summary>Returns the current angular velocity of the pose (as of the last update)</summary>
        public Vector3 GetAngularVelocity()
        {
            return m_PoseAction[m_InputSource].angularVelocity;
        }

        /// <summary>Returns the velocities of the pose at the time specified. Can predict in the future or return past values.</summary>
        public bool GetVelocitiesAtTimeOffset(float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity)
        {
            return m_PoseAction[m_InputSource].GetVelocitiesAtTimeOffset(secondsFromNow, out velocity, out angularVelocity);
        }

        /// <summary>Uses previously recorded values to find the peak speed of the pose and returns the corresponding velocity and angular velocity</summary>
        public void GetEstimatedPeakVelocities(out Vector3 velocity, out Vector3 angularVelocity)
        {
            int top = m_HistoryBuffer.GetTopVelocity(10, 1);

            m_HistoryBuffer.GetAverageVelocities(out velocity, out angularVelocity, 2, top);
        }

        protected int m_LastFrameUpdated;

        protected void UpdateHistoryBuffer()
        {
            int currentFrame = Time.frameCount;
            if (m_LastFrameUpdated != currentFrame)
            {
                m_HistoryBuffer.Update(m_PoseAction[m_InputSource].localPosition, m_PoseAction[m_InputSource].localRotation, m_PoseAction[m_InputSource].velocity, m_PoseAction[m_InputSource].angularVelocity);
                m_LastFrameUpdated = currentFrame;
            }
        }

        /// <summary>
        /// Gets the localized name of the device that the action corresponds to.
        /// </summary>
        /// <param name="localizedParts">
        /// <list type="bullet">
        /// <item><description>VRInputString_Hand - Which hand the origin is in. E.g. "Left Hand"</description></item>
        /// <item><description>VRInputString_ControllerType - What kind of controller the user has in that hand.E.g. "Vive Controller"</description></item>
        /// <item><description>VRInputString_InputSource - What part of that controller is the origin. E.g. "Trackpad"</description></item>
        /// <item><description>VRInputString_All - All of the above. E.g. "Left Hand Vive Controller Trackpad"</description></item>
        /// </list>
        /// </param>
        public string GetLocalizedName(params EVRInputStringBits[] localizedParts)
        {
            if (m_PoseAction != null)
                return m_PoseAction.GetLocalizedOriginPart(m_InputSource, localizedParts);
            return null;
        }

        #region ITracker

        private Subject<Transform> m_PositionUpdate = new Subject<Transform>();

        public IObservable<Transform> OnPositionUpdate() => m_PositionUpdate;

        private void ChangeTrackingType(TrackingSettings trackingParameter)
        {
            switch (trackingParameter.TrackingType)
            {
                case ETrackingType.Controller:
                    if (trackingParameter.LRType == ELRType.Right)
                    {
                        m_InputSource = SteamVR_Input_Sources.RightHand;
                    }
                    else if (trackingParameter.LRType == ELRType.Left)
                    {
                        m_InputSource = SteamVR_Input_Sources.LeftHand;
                    }
                    break;

                case ETrackingType.Tracker:
                    if (trackingParameter.LRType == ELRType.Right)
                    {
                        m_InputSource = SteamVR_Input_Sources.RightShoulder;
                    }
                    else if (trackingParameter.LRType == ELRType.Left)
                    {
                        m_InputSource = SteamVR_Input_Sources.LeftShoulder;
                    }
                    break;

                case ETrackingType.Camera:
                    break;
            }
        }

        #endregion ITracker
    }
}


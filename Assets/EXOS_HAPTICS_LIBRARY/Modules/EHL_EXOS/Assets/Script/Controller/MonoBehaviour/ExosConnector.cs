using exiii.Extensions;
using exiii.Unity.Device;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.EXOS
{
    /// <summary>
    /// Apply angle data acquired from the device and summarize the generated forces
    /// </summary>
    public sealed class ExosConnector : InteractorNode,
        IReceiverObservable<IForceReceiver>,
        IReceiverObservable<IExosForceReceiver>,
        IReceiverObservable<IPositionReceiver>,
        IStateObservable<IPositionState>
    {
        #region Inspector

        [Header(nameof(ExosConnector))]
        [SerializeField]
        private ExosDevice m_Device;

        public ExosDevice Device => m_Device;

        [Header("Force")]
        [SerializeField]
        private bool m_ForceEnabled = true;

        public bool ForceEnabled
        {
            get { return m_ForceEnabled; }
            set { m_ForceEnabled = value; }
        }

        [Header("Joint")]
        [SerializeField]
        [FormerlySerializedAs("Joints")]
        private ExosJointReference[] m_Joints;

        #endregion Inspector

        private TransformState m_TransformState;

        private ExosForceController m_ExosForceReciever;
        private PositionController m_PositionReceiver;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (Device != null && m_Joints != null)
            {
                m_Joints.CheckNull().Foreach(x => x.OnValidate());

                if (m_Joints.CheckNull().Any(joint => !Device.HasJoint(joint.AxisType)))
                {
                    EHLDebug.LogWarning("Device Joint Settings is false : " + gameObject.name, this, "Controller", ELogLevel.Overview);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_TransformState = new TransformState(transform);

            m_ExosForceReciever = new ExosForceController(m_TransformState, m_Device);
            m_PositionReceiver = new PositionController(m_TransformState);
        }

        protected override void Start()
        {
            base.Start();

            InteractorRoot
                .OnPositionUpdate()
                .Subscribe(UpdatePosition);

            this.FixedUpdateAsObservable()
                .Subscribe(_ => UpdateForce());
        }

        private void UpdatePosition(Transform trans)
        {
            UpdateAngle();

            m_PositionReceiver.ResetVector();

            m_SubjectPositionReceiver.OnNext(m_PositionReceiver);

            m_ResultPosition.OnNext(m_PositionReceiver);
        }

        private void UpdateAngle()
        {
            if (m_Joints == null) { return; }

            m_Joints.CheckNull().Foreach(joint => joint.UpdateAngle());
        }

        private void UpdateForce()
        {
            m_ExosForceReciever.ResetVector();

            if (m_ForceEnabled)
            {
                m_SubjectForceReceiver.OnNext(m_ExosForceReciever);
                m_SubjectExosForceReceiver.OnNext(m_ExosForceReciever);

                m_Joints.CheckNull().Foreach(joint => joint.UpdateForce(m_ExosForceReciever));
            }
            else
            {
                Device.ResetForce();
            }
        }

#pragma warning disable 4014

        public override void Initialize()
        {
            base.Initialize();

            ConnectDevice(false);
        }

        public override void Terminate()
        {
            base.Terminate();

            Device.ResetForce();
        }

        public void Reconnect()
        {
            ConnectDevice(true);
        }

#pragma warning restore 4014

        private async Task<bool> ConnectDevice(bool disconnect)
        {
            if (Device == null)
            {
                EHLDebug.LogWarning("Device was not set : " + ExName, this);
                return false;
            }

            if (disconnect && Device.IsConnected)
            {
                await Device.TerminationAsync();
            }

            if (!Device.IsConnected || !Device.Enabled)
            {
                await Device.InitializeAsync();
            }

            if (!Device.IsConnected || !Device.Enabled)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Toggle force output to the device
        /// </summary>
        public void ChangeForceEnabled()
        {
            m_ForceEnabled = !m_ForceEnabled;
        }

        public bool TryGetJoint(EAxisType axis, out ExosJointReference joint)
        {
            joint = null;

            if (m_Joints == null) { return false; }

            joint = m_Joints.FirstOrDefault(x => x.AxisType == axis);

            if (joint != null) { return true; }

            return false;
        }

        #region ExMonoBehaviour

        public override void StartInjection(IRootScript root)
        {
            base.StartInjection(root);

            m_Joints.Foreach(joint => joint.SetReference(Device.GetJoint(joint.AxisType), root));
        }

        #endregion ExMonoBehaviour

        #region IPathController

        private Subject<IForceReceiver> m_SubjectForceReceiver = new Subject<IForceReceiver>();

        IDisposable IObservable<IForceReceiver>.Subscribe(IObserver<IForceReceiver> observer)
        {
            return m_SubjectForceReceiver.Subscribe(observer);
        }

        private Subject<IExosForceReceiver> m_SubjectExosForceReceiver = new Subject<IExosForceReceiver>();

        IDisposable IObservable<IExosForceReceiver>.Subscribe(IObserver<IExosForceReceiver> observer)
        {
            return m_SubjectExosForceReceiver.Subscribe(observer);
        }

        private Subject<IPositionReceiver> m_SubjectPositionReceiver = new Subject<IPositionReceiver>();

        IDisposable IObservable<IPositionReceiver>.Subscribe(IObserver<IPositionReceiver> observer)
        {
            return m_SubjectPositionReceiver.Subscribe(observer);
        }

        private Subject<IPositionState> m_ResultPosition = new Subject<IPositionState>();

        IDisposable IObservable<IPositionState>.Subscribe(IObserver<IPositionState> observer)
        {
            return m_ResultPosition.Subscribe(observer);
        }

        #endregion IPathController
    }
}


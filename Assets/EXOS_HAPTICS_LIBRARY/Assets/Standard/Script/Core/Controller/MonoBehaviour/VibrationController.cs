using exiii.Unity.Rx;
using System;
using System.Collections.Generic;

namespace exiii.Unity
{
    public class VibrationController : InteractorNode,
        IVibrationReceiver, IReceiverObservable<IVibrationReceiver>,
        IVibrationState, IStateObservable<IVibrationState>
    {
        private Queue<IVibrationParameter> m_VivrationQueue = new Queue<IVibrationParameter>();

        private IVibrationParameter m_VibrationParameter;

        protected override void Awake()
        {
            base.Awake();

            TransformState = new TransformState(transform);
        }

        private void Update()
        {
            ResetVibration();

            m_Receiver.OnNext(this);

            m_State.OnNext(this);
        }

        public void AddVibration(IVibrationParameter parameter)
        {
            m_VivrationQueue.Enqueue(parameter);
        }

        private void ResetVibration()
        {
            m_VibrationParameter = null;

            m_VivrationQueue.Clear();
        }

        #region IPath

        public TransformState TransformState { get; private set; }

        public bool HasVibration
        {
            get { return m_VivrationQueue.Count > 0; }
        }

        public IVibrationParameter VibrationParameter
        {
            get
            {
                if (m_VibrationParameter != null) { return m_VibrationParameter; }

                return m_VibrationParameter = m_VivrationQueue.Dequeue();
            }
        }

        private Subject<IVibrationReceiver> m_Receiver = new Subject<IVibrationReceiver>();

        IDisposable IObservable<IVibrationReceiver>.Subscribe(IObserver<IVibrationReceiver> observer)
        {
            return m_Receiver.Subscribe(observer);
        }

        private Subject<IVibrationState> m_State = new Subject<IVibrationState>();

        IDisposable IObservable<IVibrationState>.Subscribe(IObserver<IVibrationState> observer)
        {
            return m_State.Subscribe(observer);
        }

        #endregion IPath
    }
}
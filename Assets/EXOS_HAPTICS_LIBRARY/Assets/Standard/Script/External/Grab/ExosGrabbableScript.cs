using exiii.Unity.Rx;
using exiii.Unity.VIU.Utility;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace exiii.Unity.VIU.Grab
{
    public class ExosGrabbableScript : GrabbableBaseEx<ExosGrabbableScript.Grabber>, IGrabbableScript, IGrippableScript
    {
        [Serializable]
        public class UnityEventGrabbable : UnityEvent<ExosGrabbableScript> { }

        public class Grabber : IGrabber
        {
            private static ObjectPool<Grabber> m_pool;

            public static Grabber Get(IManipulator obj)
            {
                if (m_pool == null)
                {
                    m_pool = new ObjectPool<Grabber>(() => new Grabber());
                }

                var grabber = m_pool.Get();
                grabber.manipulator = obj;
                return grabber;
            }

            public static void Release(Grabber grabber)
            {
                grabber.manipulator = null;
                m_pool.Release(grabber);
            }

            public IManipulator manipulator { get; private set; }

            public RigidPose grabberOrigin
            {
                get { return new RigidPose(manipulator.transform); }
            }

            public RigidPose grabOffset { get; set; }
        }

        private IndexedTable<IManipulator, Grabber> m_eventGrabberSet;

        public bool alignPosition;
        public bool alignRotation;
        public Vector3 alignPositionOffset;
        public Vector3 alignRotationOffset;

        [Range(MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION)]
        [FormerlySerializedAs("followingDuration")]
        [SerializeField]
        private float m_followingDuration = DEFAULT_FOLLOWING_DURATION;

        [FormerlySerializedAs("overrideMaxAngularVelocity")]
        [SerializeField]
        private bool m_overrideMaxAngularVelocity = true;

        [FormerlySerializedAs("unblockableGrab")]
        [SerializeField]
        private bool m_unblockableGrab = true;

        [SerializeField]
        private bool m_allowMultipleGrabbers = true;

        [FormerlySerializedAs("afterGrabbed")]
        [SerializeField]
        private UnityEventGrabbable m_afterGrabbed = new UnityEventGrabbable();

        [FormerlySerializedAs("beforeRelease")]
        [SerializeField]
        private UnityEventGrabbable m_beforeRelease = new UnityEventGrabbable();

        [FormerlySerializedAs("onDrop")]
        [SerializeField]
        private UnityEventGrabbable m_onDrop = new UnityEventGrabbable(); // change rigidbody drop velocity here        

        public override float followingDuration { get { return m_followingDuration; } set { m_followingDuration = Mathf.Clamp(value, MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION); } }

        public override bool overrideMaxAngularVelocity { get { return m_overrideMaxAngularVelocity; } set { m_overrideMaxAngularVelocity = value; } }

        public bool unblockableGrab { get { return m_unblockableGrab; } set { m_unblockableGrab = value; } }

        public UnityEventGrabbable afterGrabbed { get { return m_afterGrabbed; } }

        public UnityEventGrabbable beforeRelease { get { return m_beforeRelease; } }

        public UnityEventGrabbable onDrop { get { return m_onDrop; } }

        public IManipulator grabbedObject { get { return isGrabbed ? currentGrabber.manipulator : null; } }

        private bool moveByVelocity { get { return !unblockableGrab && grabRigidbody != null && !grabRigidbody.isKinematic; } }

        private Transform m_OriginalParent;

        [Obsolete("Use grabRigidbody instead")]
        public Rigidbody rigid { get { return grabRigidbody; } set { grabRigidbody = value; } }

        protected override void Awake()
        {
            base.Awake();

            InitGrabPaent();

            afterGrabberGrabbed += () => m_afterGrabbed.Invoke(this);
            beforeGrabberReleased += () => m_beforeRelease.Invoke(this);
            onGrabberDrop += () => m_onDrop.Invoke(this);
        }

        protected virtual void OnDisable()
        {
            ClearGrabbers(true);
            ClearEventGrabberSet();
        }

        private void ClearEventGrabberSet()
        {
            if (m_eventGrabberSet == null) { return; }

            for (int i = m_eventGrabberSet.Count - 1; i >= 0; --i)
            {
                Grabber.Release(m_eventGrabberSet.GetValueByIndex(i));
            }

            m_eventGrabberSet.Clear();
        }

        public bool IsGrabbed { get { return isGrabbed; } }

        public virtual void OnStart(IManipulator obj)
        {
            if (m_eventGrabberSet == null) { m_eventGrabberSet = new IndexedTable<IManipulator, Grabber>(); }

            if (m_eventGrabberSet.ContainsKey(obj)) { return; }

            if (!m_allowMultipleGrabbers)
            {
                ClearGrabbers(false);
                ClearEventGrabberSet();
            }

            // TODO: chack it is safe
            // set grab parant.
            Scheduler.MainThread.Schedule(_ => SetGrabParent(obj));
            
            var grabber = Grabber.Get(obj);
            var offset = RigidPose.FromToPose(grabber.grabberOrigin, new RigidPose(transform));

            if (alignPosition) { offset.pos = alignPositionOffset; }
            if (alignRotation) { offset.rot = Quaternion.Euler(alignRotationOffset); }

            grabber.grabOffset = offset;

            m_eventGrabberSet.Add(obj, grabber);

            AddGrabber(grabber);
        }

        // init grab parent.
        private void InitGrabPaent()
        {
            m_OriginalParent = this.transform.parent;
        }

        // set grab parent.
        private void SetGrabParent(IManipulator obj)
        {            
            this.transform.parent = obj.transform;
        }

        // reset grab parent.
        private void ResetGrabParent()
        {
            transform.parent = m_OriginalParent;
        }

        public virtual void OnFixedUpdate(IManipulator obj)
        {
            if (isGrabbed && moveByVelocity && currentGrabber.manipulator == obj)
            {
                OnGrabRigidbody();
            }
        }

        public virtual void OnUpdate(IManipulator obj)
        {
            if (isGrabbed && !moveByVelocity && currentGrabber.manipulator == obj)
            {
                RecordLatestPosesForDrop(Time.time, 0.05f);
                OnGrabTransform();
            }
        }

        public virtual void OnEnd(IManipulator manipulator)
        {
            if (m_eventGrabberSet == null) { return; }

            // TODO: chack it is safe
            // reset parent.
            Scheduler.MainThread.Schedule(ResetGrabParent);

            Grabber grabber;

            if (!m_eventGrabberSet.TryGetValue(manipulator, out grabber)) { return; }

            RemoveGrabber(grabber);

            m_eventGrabberSet.Remove(manipulator);
            Grabber.Release(grabber);
        }

        // change original parent.
        public void ChangeOriginalParent(Transform parent)
        {
            m_OriginalParent = parent;
        }        

        #region IGrabbableScript

        public void OnStart(IGrabManipulation manipulation)
        {
            OnStart(manipulation.Manipulator);
        }

        public void OnUpdate(IGrabManipulation manipulation)
        {
            OnUpdate(manipulation.Manipulator);
        }

        public void OnFixedUpdate(IGrabManipulation manipulation)
        {
            OnFixedUpdate(manipulation.Manipulator);
        }

        public void OnEnd(IGrabManipulation manipulation)
        {
            OnEnd(manipulation.Manipulator);
        }

        #endregion

        #region IGrippableScript

        public void OnStart(IGripManipulation manipulation)
        {
            OnStart(manipulation.Manipulator);
        }

        public void OnUpdate(IGripManipulation manipulation)
        {
            OnUpdate(manipulation.Manipulator);
        }

        public void OnFixedUpdate(IGripManipulation manipulation)
        {
            OnFixedUpdate(manipulation.Manipulator);
        }

        public void OnEnd(IGripManipulation manipulation)
        {
            OnEnd(manipulation.Manipulator);
        }

        #endregion
    }
}
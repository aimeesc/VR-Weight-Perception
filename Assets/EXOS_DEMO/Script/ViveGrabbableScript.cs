using exiii.Unity.EXOS;
using exiii.Unity.VIU.Grab;
using exiii.Unity.VIU.Utility;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace exiii.Unity.Vive
{
    // TODO: Will delete this
    public class ViveGrabbableScript : GrabbableBaseEx<ViveGrabbableScript.Grabber>, IGrabbableScript, IGrippableScript
    {
        [Serializable]
        public class UnityEventGrabbable : UnityEvent<ViveGrabbableScript> { }

        public class Grabber : IGrabber
        {
            private static ObjectPool<Grabber> m_pool;

            public static Grabber Get(GameObject obj)
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

            public GameObject manipulator { get; private set; }

            public RigidPose grabberOrigin
            {
                get { return new RigidPose(manipulator.transform); }
            }

            public RigidPose grabOffset { get; set; }
        }

        private IndexedTable<GameObject, Grabber> m_eventGrabberSet;

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

        public GameObject grabbedObject { get { return isGrabbed ? currentGrabber.manipulator : null; } }

        private bool moveByVelocity { get { return !unblockableGrab && grabRigidbody != null && !grabRigidbody.isKinematic; } }

        [Obsolete("Use grabRigidbody instead")]
        public Rigidbody rigid { get { return grabRigidbody; } set { grabRigidbody = value; } }

        protected override void Awake()
        {
            base.Awake();

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

        public virtual void OnStart(GameObject obj)
        {
            if (m_eventGrabberSet == null) { m_eventGrabberSet = new IndexedTable<GameObject, Grabber>(); }

            if (m_eventGrabberSet.ContainsKey(obj)) { return; }

            if (!m_allowMultipleGrabbers)
            {
                ClearGrabbers(false);
                ClearEventGrabberSet();
            }

            var grabber = Grabber.Get(obj);
            var offset = RigidPose.FromToPose(grabber.grabberOrigin, new RigidPose(transform));

            if (alignPosition) { offset.pos = alignPositionOffset; }
            if (alignRotation) { offset.rot = Quaternion.Euler(alignRotationOffset); }

            grabber.grabOffset = offset;

            m_eventGrabberSet.Add(obj, grabber);

            AddGrabber(grabber);
        }

        public virtual void OnFixedUpdate(GameObject obj)
        {
            if (isGrabbed && moveByVelocity && currentGrabber.manipulator == obj)
            {
                OnGrabRigidbody();
            }
        }

        public virtual void OnUpdate(GameObject obj)
        {
            if (isGrabbed && !moveByVelocity && currentGrabber.manipulator == obj)
            {
                RecordLatestPosesForDrop(Time.time, 0.05f);
                OnGrabTransform();
            }
        }

        public virtual void OnEnd(GameObject manipulator)
        {
            if (m_eventGrabberSet == null) { return; }

            Grabber grabber;

            if (!m_eventGrabberSet.TryGetValue(manipulator, out grabber)) { return; }

            RemoveGrabber(grabber);

            m_eventGrabberSet.Remove(manipulator);
            Grabber.Release(grabber);
        }

        #region IGrabbableScript

        public void OnStart(IGrabManipulation manipulation)
        {
            if (manipulation.Manipulator == null) { return; }

            OnStart(manipulation.Manipulator.gameObject);
        }

        public void OnUpdate(IGrabManipulation manipulation)
        {
            if (manipulation.Manipulator == null) { return; }

            OnUpdate(manipulation.Manipulator.gameObject);
        }

        public void OnFixedUpdate(IGrabManipulation manipulation)
        {
            if (manipulation.Manipulator == null) { return; }

            OnFixedUpdate(manipulation.Manipulator.gameObject);
        }

        public void OnEnd(IGrabManipulation manipulation)
        {
            if (manipulation.Manipulator == null) { return; }

            OnEnd(manipulation.Manipulator.gameObject);
        }

        #endregion

        #region IGrippableScript

        public void OnStart(IGripManipulation manipulation)
        {
            if (manipulation.Manipulator == null) { return; }

            OnStart(manipulation.Manipulator.gameObject);
        }

        public void OnUpdate(IGripManipulation manipulation)
        {
            if (manipulation.Manipulator == null) { return; }

            OnUpdate(manipulation.Manipulator.gameObject);
        }

        public void OnFixedUpdate(IGripManipulation manipulation)
        {
            if (manipulation.Manipulator == null) { return; }

            OnFixedUpdate(manipulation.Manipulator.gameObject);
        }

        public void OnEnd(IGripManipulation manipulation)
        {
            if (manipulation.Manipulator == null) { return; }

            OnEnd(manipulation.Manipulator.gameObject);
        }

        #endregion
    }
}
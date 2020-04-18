using exiii.Collections;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    // TODO: make class and refactoring for tools.
    public class ToolsHolder : SegmentHolder
    {
        #region Inspector

        [Header(nameof(ToolsHolder))]
              
        [SerializeField]
        [FormerlySerializedAs("CollisionDetectionMode")]
        private CollisionDetectionMode m_CollisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        public CollisionDetectionMode CollisionDetectionMode { get { return m_CollisionDetectionMode; } }

        [SerializeField]
        [FormerlySerializedAs("WaitOnStart")]
        private float m_WaitOnStart = 1;

        #endregion Inspector        

        public Tools Tools { get; protected set; }

        private SphereCollider m_TriggerCollider;

        public Collider TriggerCollider { get { return m_TriggerCollider; } }

        public ColliderState ColliderState { get; private set; }

        public virtual bool IsCollided
        {
            get { return Tools.IsCollided; }
        }

        public override IPenetrator Penetrator { get { return Tools; } }

        public VisualizeGL VisualizeGL { get; private set; }

        // on awake.
        protected override void Awake()
        {
            base.Awake();

            m_TriggerCollider = gameObject.GetOrAddComponent<SphereCollider>();
            m_TriggerCollider.isTrigger = true;

            // prepare tools.
            Tools = new Tools(this);

            // ref point.            
            InitialPoint = transform;
            TerminalPoint = Tools.ToolsObject.transform;

            // add debug.
            if (EHLDebug.DebugDrawGL)
            {
                VisualizeGL = gameObject.AddComponent<VisualizeGL>();

                this.UpdateAsObservable().Subscribe(_ => UpdateVisualize()).AddTo(this);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            ColliderState = gameObject.GetOrAddComponent<ColliderState>();

            Tools.Initialize(m_WaitOnStart);
        }

        // on destroy.
        public void OnDestroy()
        {
            // destroy tools.
            Tools?.DestroyObject();
        }

        public override void SetValues(float size, float mass)
        {
            m_TriggerCollider.center = Vector3.zero;
            m_TriggerCollider.radius = size / 2;

            Tools.SetValues(size, mass);
        }

        public override void SetVisible(bool visible)
        {
            var replicator = TerminalPoint.GetComponent<ToolsStateReplicator>();
            if (replicator != null)
            {
                replicator.SetDefaultVisible(visible);
            }

            Tools.SetVisible(visible);
        }

        // update visualize for debug.
        private void UpdateVisualize()
        {
            if (VisualizeGL == null || EHLDebug.DebugDrawGL == false) { return; }

            VisualizeGL.ToolsPositions.Add(ClosestSegment.TerminalPoint);
            VisualizeGL.Forces.Add(new OrientedSegment(ClosestSegment.TerminalPoint, ClosestSegment.TerminalPoint + ClosestSegment.Vector));
        }
    }
}
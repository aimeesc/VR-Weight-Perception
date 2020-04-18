using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using exiii.Extensions;

namespace exiii.Unity
{
    public enum ESpringType
    {
        UnitySpring,
        ExSpring,
        UnityJoint,

        None = 255,
    }

    public class Tools : PenetratorBase
    {
        const string s_ToolsName = "Tools_";

        public GameObject ToolsObject { get; }
        public Collider ToolsCollider { get; }

        public bool InRig { get; private set; }
        public bool IsCollided { get; private set; }

        public PhysicsRoot PhysicsRoot { get; }

        private Rigidbody Rigidbody { get; }
        private ToolsHolder ToolsHolder { get; }
        private Material ToolsMaterial { get; }
        private StateReplicator StateReplicator { get; }

        private ToolsHolder m_ParentToolsHolder;

        private FollowTarget m_FollowTarget;

        private float m_Radius;

        // fixed parameter.
        private float m_ContactOffset = 0.001f;
        private float m_MaxReturnLength = 0.1f;
        private float m_MinReturnLength = 0.01f;
        private float m_AlertLength = 0.03f;
        private float m_MaxFrictionLength = 0.005f;
        private float m_FrictionOffsetLength = 0.001f;
        private float m_MaxFriction = 2.0f;

        private float m_Alpha = 0.5f;

        private List<IExTag> m_IgnoreTags = new List<IExTag>
        {
            new ExTag<ETag>(ETag.Tools),
            new ExTag<ETag>(ETag.SystemCollider),
        };

        // constructor.
        public Tools(ToolsHolder holder)
        {
            ToolsHolder = holder;
            ToolsObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            ToolsObject.name = s_ToolsName + ToolsHolder.transform.name;

            ToolsCollider = ToolsObject.GetComponent<Collider>();

            var tag = ToolsObject.AddComponent<Classifier>();
            tag.SetExTag(new ExTag<ETag>(ETag.Tools));

            Rigidbody = ToolsObject.GetOrAddComponent<Rigidbody>();

            Rigidbody.useGravity = false;
            // Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            Rigidbody.collisionDetectionMode = ToolsHolder.CollisionDetectionMode;

            PhysicsRoot = new PhysicsRoot(Rigidbody, EHLParameterHolder.PhysicsParameter);

            // material ref.
            ToolsMaterial = EHLMaterialHolder.InstantiateTransparent();

            var renderer = ToolsObject.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material = ToolsMaterial;
            }

            MoveToRig();
            ResetToolsPosition();

            //EHLDebug.StartDebugSphere(ToolsObject);

            //EHLDebug.AddDebugSphere(ToolsObject.transform.position, 0.01f, Color.red);
            //EHLDebug.AddDebugSphere(ToolsHolder.transform.position, 0.01f, Color.blue);

            // create follow target.
            m_FollowTarget = ToolsObject.GetOrAddComponent<FollowTarget>();
            m_FollowTarget.SetTargetTransform(ToolsHolder.transform);

            m_FollowTarget.Parameter = EHLParameterHolder.FollowTargetParameter;

            m_FollowTarget.SetPhysicsRoot(PhysicsRoot);

            // create state replicator.
            StateReplicator = ToolsObject.GetOrAddComponent<ToolsStateReplicator>();
            StateReplicator.InitTarget(ToolsHolder.transform);

            Collider = ToolsObject.GetOrAddComponent<SphereCollider>();

            Collider.contactOffset = m_ContactOffset;
            Collider.enabled = false;
            Collider.material = EHLMaterialHolder.InstantiateTools();

            // initialize rx.
            ToolsHolder.FixedUpdateAsObservable().Subscribe(_ => UpdateToolsState()).AddTo(ToolsHolder);
            ToolsHolder.FixedUpdateAsObservable().Subscribe(_ => UpdateConnectState()).AddTo(ToolsHolder);

            ToolsObject.OnCollisionStayAsObservable().Subscribe(OnCollisionStay);
            CycleManager.LateFixedUpdate().Subscribe(_ => CalcDependency());
        }

        private bool m_JointBuilded = false;
        private IDisposable m_OppositesCountChanged = null;

        public void Initialize(float waitOnStart)
        {
            ToolsObject.layer = ToolsHolder.gameObject.layer;

            // initialize rx for collider state.
            if (ToolsHolder.ColliderState != null)
            {
                if (m_OppositesCountChanged == null)
                {
                    m_OppositesCountChanged = ToolsHolder.ColliderState.OnOppositesCountChanged().Subscribe(OnOppositesCountChanged).AddTo(ToolsHolder);
                }
            }
            else
            {
                Debug.LogWarning("ToolsHolder.ColliderState is null", ToolsObject);
            }

            m_ParentToolsHolder = ToolsHolder.transform.parent.GetComponent<ToolsHolder>();

            if (EHLParameterHolder.UseBrake)
            {
                if (m_Brake == null)
                {
                    BuildBrake();
                }

                if (m_Brake != null)
                {
                    m_Brake.layer = ToolsHolder.gameObject.layer;
                }
            }

            if (EHLParameterHolder.UseSpring)
            {
                PhysicsJointFactory.BuildSpring(ToolsHolder, m_ParentToolsHolder);
            }

            if (EHLParameterHolder.UseJoint && !m_JointBuilded)
            {
                PhysicsJointFactory.BuildJoint(ToolsHolder, m_ParentToolsHolder);

                m_JointBuilded = true;
            }

            Collider.enabled = false;

            Observable
                .Timer(TimeSpan.FromSeconds(waitOnStart))
                .First()
                .Where(_ => Collider != null)
                .Subscribe(_ => Collider.enabled = true);
        }

        public void SetValues(float size, float mass)
        {
            m_Radius = size / 2;

            ToolsObject.transform.localScale = Vector3.one * size;
            Rigidbody.mass = mass;
        }

        // change visible.
        public void SetVisible(bool visible)
        {
            MeshRenderer renderer = ToolsObject.GetComponent<MeshRenderer>();
            if (renderer == null) { return; }

            renderer.enabled = visible;
        }

        // reset tools position.
        public void ResetToolsPosition()
        {
            if (InRig)
            {
                ToolsObject.transform.MoveTo(ToolsHolder.transform);
            }
            else
            {
                ToolsObject.transform.ResetLocal(true, true, false);
            }
        }

        // destroy tools object.
        public void DestroyObject()
        {
            UnityEngine.Object.Destroy(ToolsObject);
        }

        // get opposits count.
        private int GetOppositsCount()
        {
            if (ToolsHolder.ColliderState == null) { return 0; }

            return ToolsHolder.ColliderState.Opposites
                .Where(x => !x.RootsHasTag(m_IgnoreTags))
                .Count();
        }

        // move tools to rig.
        private void MoveToRig()
        {
            if (ToolsObject == null) { return; }

            // set tools to rig.
            CameraRig.SetTools(ToolsObject);

            InRig = true;
        }

        // move tools to local.
        private void MoveToHolder()
        {
            if (ToolsObject == null) { return; }

            // set tools to holder.
            ToolsObject.transform.parent = ToolsHolder.transform;

            InRig = false;
        }

        // on change opposites of collider state.
        private void OnOppositesCountChanged(int count)
        {
            if (GetOppositsCount() > 0 && !ToolsHolder.IsCollided)
            {
                IsCollided = true;

                MoveToRig();
            }
        }

        // return god object to default position.
        private void ReturnTools()
        {
            IsCollided = false;

            // reset follower velocity.
            PhysicsRoot.ResetState();

            // reset tools position.
            ResetToolsPosition();

            ResetBrake();
        }

        // update tools state.
        private void UpdateToolsState()
        {
            /*
            if (false)
            {
                ToolsObject.transform.LookAt(ToolsObject.transform.position + m_BrakeDirection, ToolsHolder.transform.forward);
            }
            else if (m_ParentToolsHolder != null)
            {
                ToolsObject.transform.LookAt(m_ParentToolsHolder.Tools.ToolsObject.transform);
            }
            */

            if (IsCollided)
            {
                var length = ToolsHolder.ClosestSegment.Length;
                var penetrationRatio = Mathf.Clamp01((length - m_FrictionOffsetLength) / m_MaxFrictionLength);
                var friction = penetrationRatio * m_MaxFriction;

                Collider.material.staticFriction = friction;
                Collider.material.dynamicFriction = friction;

                if (length > m_AlertLength)
                {
                    ToolsMaterial.color = Color.red.Alpha(m_Alpha);
                }
                else if (length > m_MinReturnLength)
                {
                    ToolsMaterial.color = Color.yellow.Alpha(m_Alpha);
                }
                else
                {
                    ToolsMaterial.color = Color.green.Alpha(m_Alpha);
                }

                if (GetOppositsCount() == 0 && length < m_MinReturnLength)
                {
                    ReturnTools();
                }

                if (m_Brake != null)
                {
                    UpdateBrake(penetrationRatio, friction * 2);
                }

                Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                PhysicsRoot.StableMode = true;
            }
            else
            {
                if (m_Brake != null)
                {
                    ResetBrake();
                }

                Collider.material.staticFriction = 0;
                Collider.material.dynamicFriction = 0;

                ToolsMaterial.color = Color.white.Alpha(m_Alpha);

                Rigidbody.constraints = RigidbodyConstraints.None;
                PhysicsRoot.StableMode = false;
            }
        }

        // update connect state.
        private void UpdateConnectState()
        {
            if (GetOppositsCount() == 0 && ToolsHolder.ClosestSegment.Length > m_MaxReturnLength)
            {
                ReturnTools();
            }
        }

        private GameObject m_Brake;
        private Collider m_BrakeCollider;
        private Vector3 m_BrakeDirection;

        private void BuildBrake()
        {
            m_Brake = GameObject.CreatePrimitive(PrimitiveType.Cube);
            m_BrakeCollider = m_Brake.GetComponent<Collider>();

            m_Brake.transform.parent = ToolsObject.transform;

            ResetBrake();

            m_BrakeCollider.material = EHLMaterialHolder.InstantiateTools();

            var renderer = m_Brake.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }
        }

        private void UpdateBrake(float penetrationRatio, float friction)
        {
            m_Brake.transform.localScale = Vector3.one * penetrationRatio * 1.1f;

            m_BrakeCollider.material.staticFriction = friction;
            m_BrakeCollider.material.dynamicFriction = friction;

            LineDrawerGL.DrawLine(ToolsObject.transform.position, ToolsObject.transform.position + m_BrakeDirection * penetrationRatio * m_Radius, Color.red);
        }

        private void ResetBrake()
        {
            if (m_Brake == null) { return; }

            m_Brake.transform.ResetLocal(true, true, false);
            m_Brake.transform.localScale = Vector3.zero;
        }

        /*
        // for tactile.
        public float CalcTactileRatio()
        {
            return Mathf.Clamp01(ToolsHolder.ClosestSegment.Length / m_MinReturnLength);
        }
        */

        private Dictionary<Collider, Collision> m_DicCollision = new Dictionary<Collider, Collision>();

        private Dictionary<Collider, float> m_DicDependency = new Dictionary<Collider, float>();
        private Dictionary<Collider, Vector3> m_DicDependencyVector = new Dictionary<Collider, Vector3>();

        private void OnCollisionStay(Collision collision)
        {
            if (collision == null || collision.rigidbody == null) { return; }

            if (collision.collider == ToolsCollider || collision.collider == m_BrakeCollider) { return; }

            if (!m_DicCollision.ContainsKey(collision.collider))
            {
                m_DicCollision.Add(collision.collider, collision);
            }
        }

        private void CalcDependency()
        {
            m_DicDependency.Clear();
            m_DicDependencyVector.Clear();

            if (m_DicCollision.Count == 0)
            {
                return;
            }
            else if (m_DicCollision.Count == 1)
            {
                var collider = m_DicCollision.Keys.ElementAt(0);
                m_DicDependency.Add(collider, 1);
                return;
            }

            var total = m_DicCollision.Values.Sum(x => x.impulse.magnitude);

            if (total == 0)
            {
                m_DicCollision.Clear();
                return;
            }

            m_DicCollision
                .Where(x => x.Value.impulse.sqrMagnitude != 0)
                .Foreach(x => m_DicDependency.Add(x.Key, Mathf.Clamp01(x.Value.impulse.magnitude / total)));

            m_DicCollision.Foreach(x => m_DicDependencyVector.Add(x.Key, x.Value.impulse / total));

            // m_DicDependencyVector.Foreach(x => LineDrawerGL.DrawLine(Center, Center + x.Value * m_Radius, Color.red));

            if (m_Brake != null)
            {
                var max = m_DicCollision.Values.Select(x => x.impulse).OrderBy(x => x.magnitude).First();
                m_BrakeDirection = Vector3.RotateTowards(m_BrakeDirection, - max.normalized, EHLParameterHolder.MoveLimit, EHLParameterHolder.RotateLimit);
            }

            m_DicCollision.Clear();
        }

        #region IPenetrator

        public override EPenetratorType PenetratorType => EPenetratorType.CalclateOnPenetrator;

        public override ECorrectionType CorrectionType => ECorrectionType.Explicit;

        public override Vector3 Center => Collider.bounds.center;

        public override bool TryCalcPenetration(IPenetrable target, out OrientedSegment penetration)
        {
            if (target.Root == null || target.Colliders == null)
            {
                penetration = new OrientedSegment(Center, Center);
                return false;
            }

            foreach (var pair in m_DicDependency)
            {
                if (!target.Colliders.Contains(pair.Key)) { continue; }

                penetration = ToolsHolder.ClosestSegment.Multiply(pair.Value);
                return ToolsHolder.ClosestSegment.HasLength;
            }

            penetration = new OrientedSegment(Center, Center);
            return false;
        }

        #endregion
    }
} 
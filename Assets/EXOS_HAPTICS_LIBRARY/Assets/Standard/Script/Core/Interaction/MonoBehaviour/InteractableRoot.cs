using exiii.Extensions;
using exiii.Unity.Rx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    [SelectionBase, DisallowMultipleComponent]
    [RequireComponent(typeof(MultiCollider))]
    public class InteractableRoot : RootScript, IInteractableRoot,
        IManipulable<ITouchManipulation>,
        IManipulable<IGripManipulation>,
        IManipulable<IGrabManipulation>,
        IManipulable<IUseManipulation>,
        IStateObservable<ISizeState>
    {
        #region Inspector

        [Header(nameof(InteractableRoot))]
        [SerializeField]
        private bool m_CalcPenetration = true;

        public bool CalcPenetration
        {
            get { return m_CalcPenetration; }
            set { m_CalcPenetration = value; }
        }

        [SerializeField]
        [FormerlySerializedAs("isPhysicalObject")]
        private bool m_IsPhysicalObject = false;

        public bool IsPhysicalObject
        {
            get { return m_IsPhysicalObject; }
            set { m_IsPhysicalObject = value; }
        }

        [SerializeField]
        private bool m_IsStickyGrab = false;

        public bool IsStickyGrab => m_IsStickyGrab;

        [SerializeField]
        private PhysicalProperties m_SheardPhysicalProperties;

        public PhysicalProperties SheardPhysicalProperties
        {
            get { return m_SheardPhysicalProperties; }
            set { m_SheardPhysicalProperties = value; }
        }

        [SerializeField, Unchangeable]
        private PhysicalProperties m_PhysicalProperties;

        public PhysicalProperties PhysicalProperties => m_PhysicalProperties;

        #endregion Inspector

        [FormerlySerializedAs("rigidbody")]
        private Rigidbody m_Rigidbody;

        public Rigidbody Rigidbody
        {
            get
            {
                if (m_Rigidbody != null) { return m_Rigidbody; }

                return m_Rigidbody = GetComponent<Rigidbody>();
            }
        }

        private Transform m_OriginalParent;

        private TransformState m_TransformState;

        private ManipulationState m_ManipulationState;

        private IPositionEffector[] m_PositionEffectors;

        private ReactiveCollection<IManipulation> m_ManipulationsForKinematic = new ReactiveCollection<IManipulation>();

        protected override void Awake()
        {
            base.Awake();

            m_TransformState = new TransformState(transform);

            m_ManipulationState = new ManipulationState(this);

            m_PhysicalProperties = Instantiate(m_SheardPhysicalProperties);

            m_OriginalParent = transform.parent;
        }

        public override void RootInjection()
        {
            base.RootInjection();

            if (m_SurfaceContainer == null)
            {
                m_SurfaceContainer = GetComponentInChildren<SurfaceContainerBase>();
            }

            if (m_SurfaceContainer != null)
            {
                m_ShapeContainer = m_SurfaceContainer;
            }
            else
            {
                m_ShapeContainer = GetComponentInChildren<ShapeContainerBase>();
            }

            if (m_ShapeContainer == null)
            {
                m_ShapeContainer = this.GetOrAddComponent<ShapeContainerMock>();

                m_ShapeContainer.StartInjection(this);
                m_ShapeContainer.FinishInjection();
            }
        }

        protected override void Start()
        {
            base.Start();

            m_TouchableScripts = GetComponentsInChildren<ITouchableScript>().ToList();

            m_GrabbableScripts = GetComponentsInChildren<IGrabbableScript>().ToList();

            m_GrippableScripts = GetComponentsInChildren<IGrippableScript>().ToList();

            m_UsableScripts = GetComponentsInChildren<IUsableScript>().ToList();

            // Effector
            m_SurfaceVisualEffector = GetComponentInChildren<ISurfaceVisualEffector>();

            m_SizeEffectors = GetComponentsInChildren<ISizeEffector>();

            // Generator
            m_TouchForceGenerators = GetComponentsInChildren<ITouchForceGenerator>().ToList();

            m_SurfaceForceGenerators = GetComponentsInChildren<ISurfaceForceGenerator>().ToList();

            m_GripForceGenerators = GetComponentsInChildren<IGripForceGenerator>().ToList();

            m_GrabForceGenerators = GetComponentsInChildren<IGrabForceGenerator>().ToList();

            m_TouchPositionGenerators = GetComponentsInChildren<ITouchPositionGenerator>().ToList();

            m_GrabPositionEffectGenerators = GetComponentsInChildren<IGrabPositionGenerator>().ToList();

            m_TouchVibrationGenerators = GetComponentsInChildren<ITouchVibrationGenerator>().ToList();

            if (CalcPenetration)
            {
                var touchGenerator = new TouchEffectGenerator(this);

                m_TouchForceGenerators.Add(touchGenerator);
                m_TouchPositionGenerators.Add(touchGenerator);
            }

            m_PositionEffectors = GetComponentsInChildren<IPositionEffector>().ToArray();

            m_ManipulationsForKinematic
                .ObserveRemove()
                .Where(_ => m_ManipulationsForKinematic.Count == 0)
                .Subscribe(_ => RestoreRigidbodyIsKinematic());

            m_ManipulationsForKinematic
                .ObserveAdd()
                .Where(_ => m_ManipulationsForKinematic.Count == 1)
                .Subscribe(_ => ChengeRigidbodyIsKinematic(true));
        }

        private bool m_StoredKinematic;

        private void ChengeRigidbodyIsKinematic(bool kinematic)
        {
            m_StoredKinematic = Rigidbody.isKinematic;

            Rigidbody.isKinematic = kinematic;
        }

        private void RestoreRigidbodyIsKinematic()
        {
            Rigidbody.isKinematic = m_StoredKinematic;

            if (!Rigidbody.isKinematic)
            {
                Rigidbody.velocity = m_TransformState.Velocity;
                Rigidbody.angularVelocity = m_TransformState.AngulerVelocity;
            }
        }

        private void AddManipulationsForKinematic(IManipulation manipulation)
        {
            if (!m_ManipulationsForKinematic.Contains(manipulation))
            {
                m_ManipulationsForKinematic.Add(manipulation);

                manipulation.OnDisposing().Subscribe(_ => RemoveManipulationsForKinematic(manipulation));
            }
        }

        private void RemoveManipulationsForKinematic(IManipulation manipulation)
        {
            if (m_ManipulationsForKinematic.Contains(manipulation))
            {
                m_ManipulationsForKinematic.Remove(manipulation);
            }
        }

        /*
        private void Update()
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.right * m_PhysicsState.AngulerVelocity.x / 1080, Color.red);
            Debug.DrawLine(transform.position, transform.position + Vector3.up * m_PhysicsState.AngulerVelocity.y / 1080, Color.green);
            Debug.DrawLine(transform.position, transform.position + Vector3.forward * m_PhysicsState.AngulerVelocity.z / 1080, Color.blue);
        }
        */

        protected void FixedUpdate()
        {
            if (m_ManipulationState == null)
            {
                return;
            }

            if (m_ManipulationState.IsGripped || m_ManipulationState.IsGrabbed)
            {
                Rigidbody.velocity = Vector3.zero;
            }
        }

        #region IManipulable

        public IManipulationState ManipulationState
        {
            get { return m_ManipulationState; }
        }

        #endregion IManipulable

        #region IReference

        PhysicalProperties ILinkTo<PhysicalProperties>.Value => m_PhysicalProperties;

        /*
        public void GetReference(out PhysicalProperties reference)
        {
            reference = m_PhysicalProperties;
        }
        */

        #endregion IReference

        #region Touch

        [Header("Touchable Settings")]
        [SerializeField]
        private ShapeContainerBase m_ShapeContainer;

        [SerializeField]
        private SurfaceContainerBase m_SurfaceContainer;

        [SerializeField]
        private bool m_TouchManipulationEnabled = true;

        public bool TouchManipulationEnabled
        {
            get { return m_TouchManipulationEnabled; }
            set { m_TouchManipulationEnabled = value; }
        }

        public IShapeContainer ShapeContainer
        {
            get
            {
                if (m_SurfaceContainer != null)
                {
                    return m_SurfaceContainer;
                }
                else
                {
                    return m_ShapeContainer;
                }
            }
        }

        public ISurfaceContainer SurfaceContainer
        {
            get { return m_SurfaceContainer; }
        }

        private List<ITouchForceGenerator> m_TouchForceGenerators;

        private List<ISurfaceForceGenerator> m_SurfaceForceGenerators;

        private List<ITouchPositionGenerator> m_TouchPositionGenerators;

        private ISurfaceVisualEffector m_SurfaceVisualEffector;

        private List<ITouchVibrationGenerator> m_TouchVibrationGenerators;

        private List<ITouchableScript> m_TouchableScripts;

        public virtual bool TryStartManipulation(ITouchManipulation manipulation)
        {
            if (!m_TouchManipulationEnabled) { return false; }

            if (m_TouchableScripts?.Count == 0 && !CalcPenetration) { return false; }

            if (m_ManipulationState.Touch.Contains(manipulation)) { return false; }

            manipulation.AddCycleScripts(m_TouchableScripts);

            manipulation.RegisterCycle(m_ManipulationState.Touch);

            manipulation.OnDisposing()
                .Where(_ => m_SurfaceVisualEffector != null)
                .Subscribe(_ => OnSurfaceEffectReset(m_SurfaceVisualEffector, manipulation.SurfaceStateSet));

            var ForceObservable = manipulation.GetObservable<IForceReceiver>();

            ForceObservable
                .Where(x => manipulation.TryUpdateTouchState(ShapeContainer))
                .Subscribe(x => OnTouchForceGenerate(manipulation, x, manipulation.ShapeStateSet));

            ForceObservable
                .Where(x => manipulation.TryUpdateTouchState(ShapeContainer))
                .Where(x => manipulation.TryUpdateSurfaceState(SurfaceContainer))
                .Subscribe(x => OnSurfaceForceGenerate(manipulation, x, manipulation.SurfaceStateSet));

            var PositionObservable = manipulation.GetObservable<IPositionReceiver>();

            PositionObservable
                .Where(x => manipulation.TryUpdateTouchState(ShapeContainer))
                .Subscribe(x => OnTouchPositionGenerate(manipulation, x, manipulation.ShapeStateSet));

            PositionObservable
                .Where(x => manipulation.TryUpdateTouchState(ShapeContainer))
                .Where(x => manipulation.TryUpdateSurfaceState(SurfaceContainer))
                .Subscribe(x => OnSurfaceEffectGenerate(m_SurfaceVisualEffector, manipulation.SurfaceStateSet));

            PositionObservable
                .Where(x => !manipulation.TryUpdateTouchState(ShapeContainer))
                .Subscribe(x => OnSurfaceEffectReset(m_SurfaceVisualEffector, manipulation.SurfaceStateSet));

            var VibrationObservable = manipulation.GetObservable<IVibrationReceiver>();

            VibrationObservable
                .Where(x => manipulation.TryUpdateTouchState(ShapeContainer))
                .Subscribe(x => OnVibrationGenerate(x));

            return true;
        }

        public void OnTouchForceGenerate(ITouchManipulation manipulation, IForceReceiver receiver, IShapeStateSet shapeStateSet)
        {
            if (m_TouchForceGenerators == null) { return; }

            m_TouchForceGenerators.Foreach(x => x.OnGenerate(receiver, shapeStateSet));
        }

        public void OnSurfaceForceGenerate(ITouchManipulation manipulation, IForceReceiver receiver, ISurfaceStateSet surfaceStateSet)
        {
            if (m_SurfaceForceGenerators == null) { return; }

            foreach(var state in surfaceStateSet.Collection.CheckNull())
            {
                m_SurfaceForceGenerators.Foreach(x => x.OnGenerate(receiver, state));
            }
        }

        public void OnTouchPositionGenerate(ITouchManipulation manipulation, IPositionReceiver receiver, IShapeStateSet shapeStateSet)
        {
            if (m_TouchPositionGenerators == null) { return; }

            m_TouchPositionGenerators.Foreach(x => x.OnGenerate(receiver, shapeStateSet));
        }

        public void OnSurfaceEffectGenerate(ISurfaceVisualEffector effector, ISurfaceStateSet dataSet)
        {
            if (effector == null) { return; }

            foreach(var state in dataSet.Collection.CheckNull())
            {
                effector.AddEffect(state);
            }
        }

        public void OnSurfaceEffectReset(ISurfaceVisualEffector effector, ISurfaceStateSet dataSet = null)
        {
            if (effector == null) { return; }

            foreach (var state in dataSet.Collection)
            {
                effector.ResetEffect(state);
            }
        }

        public void OnVibrationGenerate(IVibrationReceiver receiver)
        {
            m_TouchVibrationGenerators.Foreach(x => x.OnGenerate(receiver, null));
        }

        #endregion Touch

        #region Grip

        [Header("Grippable Settings")]
        [SerializeField]
        private bool m_GripManipulationEnabled = true;

        public bool GripManipulationEnabled
        {
            get { return m_GripManipulationEnabled; }
            set { m_GripManipulationEnabled = value; }
        }

        private List<IGripForceGenerator> m_GripForceGenerators;

        private ISizeEffector[] m_SizeEffectors;

        private List<IGrippableScript> m_GrippableScripts;

        public virtual bool TryStartManipulation(IGripManipulation manipulation)
        {
            if (!m_GripManipulationEnabled) { return false; }

            if (m_GrippableScripts.Count == 0) { return false; }

            if (m_ManipulationState.Grip.Contains(manipulation)) { return false; }

            AddManipulationsForKinematic(manipulation);

            manipulation.AddCycleScripts(m_GrippableScripts);

            manipulation.RegisterCycle(m_ManipulationState.Grip);

            // Transform
            manipulation
                .OnManipulateStart()
                .Where(x => gameObject.activeInHierarchy && !gameObject.isStatic)
                .Subscribe(x => transform.parent = manipulation.Manipulator.transform)
                .AddTo(this);

            // HACK: need correspond to the case when changing an enabled state directly
            manipulation
                .OnManipulateEnd()
                .Where(x => gameObject.activeInHierarchy && !gameObject.isStatic)
                .Subscribe(x => transform.parent = m_OriginalParent)
                .AddTo(this);

            // Force
            //IObservable<IForceReceiver> ForceObservable;
            var ForceObservable = manipulation.GetObservable<IForceReceiver>();

            ForceObservable
                .Subscribe(x => OnGripForceGenerate(x, null));

            // Size
            manipulation
                .OnManipulateUpdate()
                .Where(_ => manipulation.TryUpdateGripState())
                .Subscribe(x => OnUpdateSize(manipulation.GripState));

            manipulation
                .OnManipulateEnd()
                .Subscribe(_ => OnSizeReset());

            return true;
        }

        public void OnGripForceGenerate(IForceReceiver receiver, IGripState data)
        {
            if (m_GripForceGenerators == null) { return; }

            m_GripForceGenerators.Foreach(x => x.OnGenerate(receiver, data));
        }

        private void OnUpdateSize(IGripState state)
        {
            m_SizeEffectors.Foreach(x => x.OnChangeSizeRatio(state.SizeState));
        }

        private void OnSizeReset()
        {
            m_SizeEffectors.Foreach(x => x.OnResetSizeRatio());
        }

        #endregion Grip

        #region Grab

        [Header("Grabbable Settings")]
        [SerializeField]
        private bool m_GrabManipulationEnabled = true;

        public bool GrabManipulationEnabled
        {
            get { return m_GrabManipulationEnabled; }
            set { m_GrabManipulationEnabled = value; }
        }

        private List<IGrabForceGenerator> m_GrabForceGenerators;

        private List<IGrabPositionGenerator> m_GrabPositionEffectGenerators;

        private List<IGrabbableScript> m_GrabbableScripts;

        private Dictionary<GameObject, IGrabManipulation> m_ToggleManipulations = new Dictionary<GameObject, IGrabManipulation>();

        public virtual bool TryStartManipulation(IGrabManipulation manipulation)
        {
            if (!m_GrabManipulationEnabled) { return false; }

            if (m_GrabbableScripts.Count == 0) { return false; }

            if (m_ManipulationState.Grab.Contains(manipulation)) { return false; }

            AddManipulationsForKinematic(manipulation);

            // manipulation.AddCycleScripts(m_GrabbableScripts);

            manipulation.RegisterCycle(m_ManipulationState.Grab);

            manipulation
                .OnManipulateStart()
                .Where(x => gameObject.activeInHierarchy)
                .Where(_ => m_GrabManipulationEnabled)
                .Subscribe(x => BeginEffectors(manipulation))
                .AddTo(this);

            // HACK: need correspond to the case when changing an enabled state directly
            manipulation
                .OnManipulateEnd()
                .Where(x => gameObject.activeInHierarchy)
                .Where(_ => m_GrabManipulationEnabled)
                .Subscribe(x => EndEffectors())
                .AddTo(this);

            manipulation
                .OnManipulateStart()
                .Where(_ => m_GrabManipulationEnabled)
                .Subscribe(x => OnGrabStart(manipulation));

            manipulation
                .OnManipulateUpdate()
                .Subscribe(x => OnGrabUpdate(manipulation));

            manipulation
                .OnManipulateFixedUpdate()
                .Subscribe(x => OnGrabFixedUpdate(manipulation));

            manipulation
                .OnManipulateEnd()
                .Where(_ => m_GrabManipulationEnabled)
                .Subscribe(x => OnGrabEnd(manipulation));

            //IObservable<IForceReceiver> ForceObservable;
            var ForceObservable = manipulation.GetObservable<IForceReceiver>();

            ForceObservable
                .Subscribe(x => OnGrabForceGenerate(x, null));

            //IObservable<IPositionReceiver> PositionObservable;
            var PositionObservable = manipulation.GetObservable<IPositionReceiver>();

            PositionObservable
                .Subscribe(x => OnGrabPositionEffectGenerate(x, null));

            return true;
        }

        private void BeginEffectors(IGrabManipulation manipulation)
        {
            IObservable<IPositionState> observable;
            if (manipulation.InteractorRoot.TryGetStateObservable(out observable))
            {
                m_PositionEffectors
                    .Foreach(effector => observable.Subscribe(effector.OnUpdatePosition).AddTo(manipulation.Disposer));
            }
        }

        private void EndEffectors()
        {
            m_PositionEffectors
                .Foreach(x => x.OnResetPosition());
        }

        public void OnGrabStart(IGrabManipulation manipulation)
        {
            var grabber = manipulation.Manipulator.gameObject;

            if (IsStickyGrab)
            {
                if (m_ToggleManipulations.ContainsKey(grabber))
                {
                    var ToggleManipulation = m_ToggleManipulations[grabber];

                    m_ToggleManipulations.Remove(grabber);

                    m_GrabbableScripts.Foreach(x => { x.OnEnd(manipulation); });

                    ToggleManipulation.CancelManipulation(this);
                }
                else
                {
                    m_ToggleManipulations.Add(grabber, manipulation);

                    manipulation.IsManualManipulation = true;

                    m_GrabbableScripts.Foreach(x => { x.OnStart(manipulation); });
                }
            }
            else
            {
                m_GrabbableScripts.Foreach(x => { x.OnStart(manipulation); });
            }
        }

        public void OnGrabUpdate(IGrabManipulation manipulation)
        {
            m_GrabbableScripts.Foreach(x => x.OnUpdate(manipulation));
        }

        public void OnGrabFixedUpdate(IGrabManipulation manipulation)
        {
            m_GrabbableScripts.Foreach(x => x.OnFixedUpdate(manipulation));
        }

        public void OnGrabEnd(IGrabManipulation manipulation)
        {
            if (!IsStickyGrab)
            {
                m_GrabbableScripts.Foreach(x => { x.OnEnd(manipulation); });
            }
        }

        public void OnGrabForceGenerate(IForceReceiver receiver, IGrabState data)
        {
            if (m_GrabForceGenerators == null) { return; }

            m_GrabForceGenerators.Foreach(x => x.OnGenerate(receiver, data));
        }

        public void OnGrabPositionEffectGenerate(IPositionReceiver receiver, IGrabState data)
        {
            if (m_GrabPositionEffectGenerators == null) { return; }

            m_GrabPositionEffectGenerators.Foreach(x => x.OnGenerate(receiver, data));
        }

        #endregion Grab

        #region Use

        [Header("Usable Settings")]
        [SerializeField]
        private bool m_UseManipulationEnabled = true;

        public bool UseManipulationEnabled
        {
            get { return m_UseManipulationEnabled; }
            set { m_UseManipulationEnabled = value; }
        }

        private List<IUsableScript> m_UsableScripts;

        public virtual bool TryStartManipulation(IUseManipulation manipulation)
        {
            if (!m_UseManipulationEnabled) { return false; }

            if (m_UsableScripts.Count == 0) { return false; }

            if (m_ManipulationState.Use.Contains(manipulation)) { return false; }

            manipulation.AddCycleScripts(m_UsableScripts);

            manipulation.RegisterCycle(m_ManipulationState.Use);

            return true;
        }

        #endregion Use

        #region IPathState

        private Subject<ISizeState> m_State = new Subject<ISizeState>();

        public IDisposable Subscribe(IObserver<ISizeState> observer)
        {
            return m_State.Subscribe(observer);
        }

        #endregion IPathState
    }
}
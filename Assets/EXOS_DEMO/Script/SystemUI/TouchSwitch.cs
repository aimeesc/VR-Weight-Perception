using exiii.Unity.EXOS;
using exiii.Unity.Rx;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace exiii.Unity.UI
{
    internal enum SwitchType
    {
        Momentary,
        Alternate,
    }

    [Serializable]
    public class UnityEventGameObject : UnityEvent<GameObject> { }

    [Serializable]
    public class UnityEventBool : UnityEvent<bool> { }

    public class TouchSwitch : ShapeContainerBase, ITouchableScript, ITouchForceGenerator, IValueHolder<bool>
    {
        #region Inspector

        [SerializeField]
        private SwitchType m_SwitchType;

        [SerializeField]
        private Color m_OffColor = Color.white;

        [SerializeField]
        private Color m_OnColor = Color.cyan;

        [SerializeField]
        private Vector3 m_LocalSurfaceNormal;

        private Vector3 WorldSurfaceNormal => transform.TransformDirection(m_LocalSurfaceNormal);

        [SerializeField]
        private Vector3 m_LocalSurfacePosition;

        [SerializeField]
        private float m_Stroke = 0.02f;

        [SerializeField]
        private float m_OnThreashold = 0.01f;

        [SerializeField]
        private float m_ReleaseThreashold = 0.003f;

        [SerializeField]
        private float m_Thickness = 0.05f;

        [Header("Debug")]
        [SerializeField, Unchangeable]
        private float m_Penetration;

        [SerializeField, Unchangeable]
        private bool m_IsOn;

        [SerializeField, Unchangeable]
        private bool m_SwitchState;

        public bool SwitchState => m_SwitchState;

        [SerializeField]
        private UnityEventGameObject m_TurnOn = new UnityEventGameObject();

        [SerializeField]
        private UnityEventGameObject m_TurnOff = new UnityEventGameObject();

        [SerializeField]
        private UnityEventBool m_ChangeState = new UnityEventBool();

        #endregion Inspector

        private Plane m_Surface;

        private Collider m_Collider;

        private Vector3 m_OriginalPosition;
        private Quaternion m_OriginalRotation;
        private Vector3 m_OriginalScale;

        private GameObject m_DisplayModel;

        private Material m_Material;

        private GameObject m_Toucher;

        private ILinkTo<PhysicalProperties> m_PhysicalProperties;

        public bool Value
        {
            get { return m_IsOn; }

            set
            {
                m_IsOn = value;
                ApplyColor();

                m_OnValueChanged.OnNext(value);
            }
        }

        private Subject<bool> m_OnValueChanged = new Subject<bool>();

        public IObservable<bool> OnValueChanged => m_OnValueChanged;

        protected override void Awake()
        {
            base.Awake();

            m_Surface = new Plane(WorldSurfaceNormal, transform.TransformPoint(m_LocalSurfacePosition));
        }

        private void Update()
        {
            m_Surface.SetNormalAndPosition(WorldSurfaceNormal, transform.TransformPoint(m_LocalSurfacePosition));
        }

        public override void StartInjection(IRootScript root)
        {
            base.StartInjection(root);

            m_Collider = root.gameObject.GetComponentInChildren<Collider>();

            if (m_Collider == null) { root.gameObject.SetActive(false); }

            m_PhysicalProperties = root.gameObject.GetComponentInChildren<ILinkTo<PhysicalProperties>>();

            if (m_PhysicalProperties == null)
            {
                gameObject.SetActive(false);
                return;
            }

            m_DisplayModel = root.gameObject.GetComponentsInChildren<IExosPrefab>()
                .Where(x => x.PrefabType == EPrefabType.DisplayModel)
                .Select(x => x.gameObject)
                .FirstOrDefault();

            if (m_DisplayModel == null) { root.gameObject.SetActive(false); }

            m_Material = m_DisplayModel.GetComponentInChildren<Renderer>()?.material;

            m_OriginalPosition = m_DisplayModel.transform.localPosition;
            m_OriginalRotation = m_DisplayModel.transform.localRotation;
            m_OriginalScale = m_DisplayModel.transform.localScale;

            ApplyColor();
        }

        public void OnStart(ITouchManipulation manipulation)
        {
            m_Penetration = 0;
            m_Toucher = manipulation.Manipulator.gameObject;

            CheckSwitchState();
        }

        public void OnUpdate(ITouchManipulation manipulation)
        {
            CheckSwitchState();

            DeformDisplayModel();
        }

        public void OnFixedUpdate(ITouchManipulation manipulation)
        {

        }

        public void OnEnd(ITouchManipulation manipulation)
        {
            ResetDisplayModel();

            m_Penetration = 0;

            CheckSwitchState();

            m_Toucher = null;
        }

        private void CheckSwitchState()
        {
            var stateBuff = m_SwitchState;

            if (!m_SwitchState && m_Penetration >= m_OnThreashold)
            {
                m_SwitchState = true;
            }

            if (m_SwitchState && m_Penetration <= m_ReleaseThreashold)
            {
                m_SwitchState = false;
            }

            switch (m_SwitchType)
            {
                case SwitchType.Momentary:
                    Momentary();
                    break;

                case SwitchType.Alternate:
                    Alternate(stateBuff);
                    break;
            }

            ApplyColor();
        }

        private void Momentary()
        {
            if (Value != m_SwitchState)
            {
                Value = m_SwitchState;

                if (Value)
                {
                    m_TurnOn.Invoke(m_Toucher);
                    m_ChangeState.Invoke(Value);
                }
                else
                {
                    m_TurnOff.Invoke(m_Toucher);
                    m_ChangeState.Invoke(Value);
                }
            }
        }

        private void Alternate(bool stateBuff)
        {
            if (stateBuff != m_SwitchState && m_SwitchState)
            {
                if (Value)
                {
                    Value = false;
                    m_TurnOff.Invoke(m_Toucher);
                    m_ChangeState.Invoke(Value);
                }
                else
                {
                    Value = true;
                    m_TurnOn.Invoke(m_Toucher);
                    m_ChangeState.Invoke(Value);
                }
            }
        }

        private void ApplyColor()
        {
            if (m_Material == null) { return; }

            if (Value)
            {
                m_Material.color = m_OnColor;
            }
            else
            {
                m_Material.color = m_OffColor;
            }
        }

        private void DeformDisplayModel()
        {
            m_DisplayModel.transform.localPosition = m_OriginalPosition - m_LocalSurfaceNormal * Mathf.Clamp(m_Penetration, 0, m_Stroke);
        }

        private void ResetDisplayModel()
        {
            m_DisplayModel.transform.localPosition = m_OriginalPosition;
            m_DisplayModel.transform.localRotation = m_OriginalRotation;
            m_DisplayModel.transform.localScale = m_OriginalScale;
        }

        public void OnGenerate(IForceReceiver receiver, IShapeStateSet state)
        {
            receiver.AddForceRatio(state.SummarizedOutput.InitialPoint, state.SummarizedOutput.Vector * m_PhysicalProperties.Value.Elasticity);

            m_Penetration = state.SummarizedOutput.Length;

            if (m_Penetration > m_Thickness) { m_Penetration = 0; }
        }

        protected override bool TryCalcPenetration(IPenetrator penetrator, out OrientedSegment penetration)
        {
            var closestPoint = m_Surface.ClosestPointOnPlane(penetrator.Center);

            var check = m_Collider.ClosestPoint(closestPoint);

            if (check != closestPoint) { closestPoint = penetrator.Center; }

            var result = new PenetrationStatus(penetrator.Center, closestPoint, !m_Surface.GetSide(penetrator.Center));

            return penetrator.TryCalcCorrection(result, out penetration);
        }
    }
}
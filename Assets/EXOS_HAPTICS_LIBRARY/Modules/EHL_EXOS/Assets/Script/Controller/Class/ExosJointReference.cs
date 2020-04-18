using exiii.Extensions;
using exiii.Unity.Device;
using exiii.Unity.Rx;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable 414

namespace exiii.Unity.EXOS
{
    enum ECollectionType
    {
        Gamma,
        ArcTan,
    }

    [Serializable]
    public class ExosJointReference
    {
        #region Inspector

        [Header(nameof(ExosJointReference))]
        [SerializeField, Unchangeable]
        private string m_Name;

        [SerializeField]
        private EAxisType m_AxisType;

        public EAxisType AxisType { get { return m_AxisType; } }

        [SerializeField]
        [FormerlySerializedAs("ForceGain")]
        private float m_ForceGainBoth = 1.0f;

        [SerializeField]
        private float m_ForceGainOneWay = 1.0f;

        //[SerializeField]
        private float m_Gamma = 2.0f;

        //[SerializeField]
        private float m_GammaLength = 0.1f;

        [SerializeField, Unchangeable]
        private EForceMode m_ForceMode = EForceMode.Both;

        //[SerializeField]
        private ECollectionType m_CorrectionType = ECollectionType.ArcTan;

        #endregion Inspector

        private ExosJoint m_ExosJoint;

        private ExosAxis m_PhysicsAxis;

        private ExosHinge m_Hinge;

        public Subject<Vector3> m_TouchLength = new Subject<Vector3>();

        public IObservable<Vector3> TouchLength { get { return m_TouchLength; } }

        public void OnValidate()
        {
            m_Name = m_AxisType.EnumToString();
            m_ForceMode = EForceMode.Both;
        }

        public void SetReference(ExosJoint joint, IRootScript root)
        {
            m_ExosJoint = joint;

            var axises = root
                .gameObject
                .GetComponentsInChildren<ExosAxis>()
                .Where(x => x.AxisType == m_AxisType)
                .ToArray();

            var models = root
                .gameObject
                .GetComponentsInChildren<ExosPrefab>();

            if (models == null) { return; }

            var model = models.Where(x => x.PrefabType == EPrefabType.PhysicsModel).FirstOrDefault();

            if (model == null) { return; }

            m_PhysicsAxis = model
                .GetComponentsInChildren<ExosAxis>()
                .Where(x => x.AxisType == m_AxisType)
                .FirstOrDefault();

            if (m_PhysicsAxis == null)
            {
                Debug.LogError($"[{m_Name}] PhysicsAxis is not found");
                return;
            }

            m_Hinge = new ExosHinge(axises, m_PhysicsAxis);
        }

        public void UpdateAngle()
        {
            if (m_Hinge != null)
            {
                m_Hinge.AngleRatio = m_ExosJoint.AngleRatio;
            }
        }

        private const float a =  1 / (Mathf.PI * 0.25f);

        public void UpdateForce(IExosForceState data)
        {
            if (data.ForceList == null) { return; }

            if (data.ForceModeDic.ContainsKey(m_AxisType)) { m_ForceMode = data.ForceModeDic[m_AxisType]; }

            m_ExosJoint.ForceRatio = 0;

            foreach (var segment in data.ForceList)
            {
                var closestVector = m_PhysicsAxis.GetClosestSegmentToAxis(segment.InitialPoint);

                var x = closestVector.magnitude;

                switch (m_CorrectionType)
                {
                    case ECollectionType.Gamma:
                        closestVector = closestVector.normalized * m_GammaLength * Mathf.Pow(closestVector.magnitude / m_GammaLength, 1.0f / m_Gamma);
                        break;

                    case ECollectionType.ArcTan:
                        closestVector = closestVector.normalized * m_GammaLength * Mathf.Atan(closestVector.magnitude / m_GammaLength * 10) * a;
                        break;
                }

                m_TouchLength.OnNext(new Vector3(x, closestVector.magnitude, 0));

                var rotation = Vector3.Cross(segment.Vector, closestVector);

                var torque = Vector3.Dot(rotation, m_PhysicsAxis.WorldAxis);

                if (m_ForceMode == EForceMode.Both)
                {
                    m_ExosJoint.ForceRatio += torque * m_ForceGainBoth;
                }
                else
                {
                    var direction = Mathf.Sign(torque);

                    // Hack:calucrate for gripper
                    m_ExosJoint.ForceRatio += direction * segment.Length * m_ForceGainOneWay;
                }
            }

            foreach (var force in data.DirectForceList)
            {
                if (m_ExosJoint.AxisType == force.AxisType) { m_ExosJoint.ForceRatio += force.ForceRatio; }
            }

            if (m_ForceMode == EForceMode.Positive)
            {
                if (m_ExosJoint.ForceRatio < 0) { m_ExosJoint.ForceRatio = 0; }
            }

            if (m_ForceMode == EForceMode.Negative)
            {
                if (m_ExosJoint.ForceRatio > 0) { m_ExosJoint.ForceRatio = 0; }
            }
        }
    }
}


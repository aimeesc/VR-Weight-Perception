using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public class ToolsBuilderMesh : ExMonoBehaviour
    {
        // axis kind.
        public enum EOrder
        {
            Main,
            Second,
            Third,
        }

        // local axis for Ray.
        public enum EAxis
        {
            RayX,   // x, y, z.
            RayY,   // y, x, z.
            RayZ,   // z, x, y.
        }

        [SerializeField, FormerlySerializedAs("MeshCollider")]
        private MeshCollider m_MeshCollider = null;

        [SerializeField, FormerlySerializedAs("ToolsUnit")]
        private float m_ToolsUnit = 0.2f;

        [SerializeField, FormerlySerializedAs("LocalAxisForRay")]
        private EAxis m_Axis = EAxis.RayX;

        [SerializeField, FormerlySerializedAs("ToolsLayer")]
        private int m_ToolsLayer = 0;

        [SerializeField, FormerlySerializedAs("ToolsVisible")]
        private bool m_ToolsVisible = true;

        private Vector3Int m_BoundsMin = Vector3Int.zero;
        private Vector3Int m_BoundsMax = Vector3Int.zero;

        // on awake.
        protected override void Awake()
        {
            base.Awake();

            // init tools.
            InitTools();
        }

        // init tools for mesh by mesh collider.
        private void InitTools()
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 defaultPos = m_MeshCollider.transform.position;
            Quaternion defaultRot = m_MeshCollider.transform.rotation;

            // move origin for calcration.
            m_MeshCollider.transform.position = Vector3.zero;
            m_MeshCollider.transform.rotation = Quaternion.identity;

            // calc bounds.
            m_BoundsMin = ToUnitIndex(m_MeshCollider.bounds.min);
            m_BoundsMax = ToUnitIndex(m_MeshCollider.bounds.max);
            float lenRay = CalcRayLength();

            // for direction.
            for (int indexSecond = GetMin(EOrder.Second); indexSecond <= GetMax(EOrder.Second); ++indexSecond)
            {
                for (int indexThird = GetMin(EOrder.Third); indexThird <= GetMax(EOrder.Third); ++indexThird)
                {
                    float second = indexSecond * m_ToolsUnit;
                    float third = indexThird * m_ToolsUnit;

                    // x plus.
                    Vector3 fromXP = GetFrom(second, third);
                    Vector3 toXP = GetTo(second, third);
                    Vector3 dirXP = toXP - fromXP;
                    Ray rayXP = new Ray(fromXP, dirXP);
                    RaycastHit hitXP;
                    if (m_MeshCollider.Raycast(rayXP, out hitXP, lenRay))
                    {
                        points.Add(hitXP.point);
                    }

                    // x minus.
                    Ray rayXM = new Ray(toXP, -dirXP);
                    RaycastHit hitXM;
                    if (m_MeshCollider.Raycast(rayXM, out hitXM, lenRay))
                    {
                        points.Add(hitXM.point);
                    }
                }
            }

            m_MeshCollider.transform.position = defaultPos;
            m_MeshCollider.transform.rotation = defaultRot;

            // create tools.
            points.ForEach(p => CreateTools(p));
        }

        // create tools.
        private void CreateTools(Vector3 point)
        {
            string name = string.Format("Tools:{0}", this.name);
            GameObject tools = new GameObject(name);
            tools.layer = (m_ToolsLayer == 0) ? this.gameObject.layer : m_ToolsLayer;
            tools.transform.SetParent(this.transform);
            tools.transform.localPosition = point;
            tools.transform.localRotation = Quaternion.identity;

            ToolsHolder holder = tools.AddComponent<ToolsHolder>();
            holder.InitialPoint = tools.transform;
            holder.SetValues(0.02f, 0.1f);
            holder.SetVisible(m_ToolsVisible);
        }

        // calc unit index.
        private int ToUnitIndex(float val)
        {
            return Mathf.RoundToInt(val / m_ToolsUnit);
        }
        private Vector3Int ToUnitIndex(Vector3 val)
        {
            Vector3Int ret = Vector3Int.zero;
            ret.x = ToUnitIndex(val.x);
            ret.y = ToUnitIndex(val.y);
            ret.z = ToUnitIndex(val.z);
            return ret;
        }

        // calc quantization value.
        private Vector3 ToQuant(Vector3 val)
        {
            val.x = m_ToolsUnit * Mathf.Round(val.x / m_ToolsUnit);
            val.y = m_ToolsUnit * Mathf.Round(val.y / m_ToolsUnit);
            val.z = m_ToolsUnit * Mathf.Round(val.z / m_ToolsUnit);

            return val;
        }

        // calc ray length by axis.
        private float CalcRayLength()
        {
            switch (m_Axis)
            {
                case EAxis.RayX:
                    return m_BoundsMax.x - m_BoundsMin.x;
                case EAxis.RayY:
                    return m_BoundsMax.y - m_BoundsMin.y;
                case EAxis.RayZ:
                    return m_BoundsMax.z - m_BoundsMin.z;
            }
            return 0.0f;
        }

        // get from by axis.
        private Vector3 GetFrom(float second, float third)
        {
            switch (m_Axis)
            {
                case EAxis.RayX:
                    return new Vector3(GetMin(0), second, third);
                case EAxis.RayY:
                    return new Vector3(second, GetMin(0), third);
                case EAxis.RayZ:
                    return new Vector3(second, third, GetMin(0));
            }
            return Vector3.zero;
        }

        // get to by axis.
        private Vector3 GetTo(float second, float third)
        {
            switch (m_Axis)
            {
                case EAxis.RayX:
                    return new Vector3(GetMax(0), second, third);
                case EAxis.RayY:
                    return new Vector3(second, GetMax(0), third);
                case EAxis.RayZ:
                    return new Vector3(second, third, GetMax(0));
            }
            return Vector3.zero;
        }

        // get min by axis.
        private int GetMin(EOrder order)
        {
            return GetBounds(order, ref m_BoundsMin);
        }

        // get max by axis.
        private int GetMax(EOrder order)
        {
            return GetBounds(order, ref m_BoundsMax);
        }

        // get bounds by axis.
        private int GetBounds(EOrder order, ref Vector3Int val)
        {
            switch (m_Axis)
            {
                case EAxis.RayX:
                    switch (order)
                    {
                        case EOrder.Main:
                            return val.x;
                        case EOrder.Second:
                            return val.y;
                        case EOrder.Third:
                            return val.z;
                    }
                    break;
                case EAxis.RayY:
                    switch (order)
                    {
                        case EOrder.Main:
                            return val.y;
                        case EOrder.Second:
                            return val.x;
                        case EOrder.Third:
                            return val.z;
                    }
                    break;
                case EAxis.RayZ:
                    switch (order)
                    {
                        case EOrder.Main:
                            return val.z;
                        case EOrder.Second:
                            return val.x;
                        case EOrder.Third:
                            return val.y;
                    }
                    break;                
            }
            return 0;
        }
    }
}

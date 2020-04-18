using exiii.Extensions;
using exiii.Unity.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public class BoneRenderer : MonoBehaviour
    {
        [SerializeField]
        private GameObject RootGameObject;

        [SerializeField]
        private bool m_DebugOnly = false;

        [SerializeField]
        private bool m_AddBoneLine = true;

        [SerializeField]
        private float m_Width = 0.005f;

        [SerializeField]
        private Material m_Material;

        [SerializeField]
        private bool m_AddJointModel = true;

        [SerializeField]
        private float m_Size = 0.02f;

        [SerializeField, PrefabField]
        private GameObject m_JointModel;

        private List<LineRenderer> m_Bones = new List<LineRenderer>();

        private void Start()
        {
            if (!Debug.isDebugBuild && m_DebugOnly)
            {
                enabled = false;
                return;
            }

            if (RootGameObject == null) { RootGameObject = gameObject; }

            BuildBones(RootGameObject, true);
        }

        private void Update()
        {
            UpdatePositionAll();
        }

        private void BuildBones(GameObject self, bool start = true)
        {
            SetRenderer(self, start);

            // HACK: make the rule for object search
            self.Children()/*.Where(x => x.GetComponent<IExosOrigin>() == null)*/.Foreach(x => BuildBones(x, false));

            SetJointModel(self);
        }

        private void SetRenderer(GameObject self, bool start)
        {
            LineRenderer renderer = self.GetComponent<LineRenderer>();

            if (start == true || !m_AddBoneLine)
            {
                if (renderer != null) { Destroy(renderer); }
                return;
            }

            if (renderer == null)
            {
                renderer = self.gameObject.AddComponent<LineRenderer>();

                renderer.startWidth = m_Width;
                renderer.endWidth = m_Width;

                if (m_Material) { renderer.material = m_Material; }
            }

            renderer.useWorldSpace = false;

            if (renderer != null)
            {
                m_Bones.Add(renderer);

                UpdatePosition(renderer);
            }
        }

        private void SetJointModel(GameObject self)
        {
            if (m_JointModel == null || !m_AddJointModel) { return; }

            var obj = Instantiate(m_JointModel, self.transform);
            obj.transform.localScale = Vector3.one * m_Size;
        }

        private void UpdatePosition(LineRenderer renderer)
        {
            var target = renderer.transform.InverseTransformPoint(renderer.transform.parent.position);

            renderer.SetPosition(0, Vector3.zero);
            renderer.SetPosition(1, target);
        }

        private void UpdatePositionAll()
        {
            foreach (var renderer in m_Bones)
            {
                UpdatePosition(renderer);
            }
        }
    }
}
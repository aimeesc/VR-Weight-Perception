using exiii.Unity.Sample;
using exiii.Unity.EXOS;
using System;
using System.Linq;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using exiii.Unity.Linq;
using exiii.Extensions;

namespace exiii.Unity.UI
{
    public abstract class HapticsEditorBase : DetectorBase<InteractableRoot>
    {
        #region Inspector

        [Header("Reference")]
        [SerializeField]
        private GameObject m_RootGameObject;

        public GameObject RootGameObject => m_RootGameObject;

        [SerializeField]
        private Material m_OutlineMaterial;

        public Material OutlineMaterial => m_OutlineMaterial;

        [SerializeField]
        private Transform m_Origin;

        public Transform Origin => m_Origin;

        [SerializeField]
        [PrefabField]
        private GameObject m_TargetPrefab;

        public GameObject TargetPrefab => m_TargetPrefab;

        [SerializeField, Unchangeable]
        private HapticsEditorHandlerBase[] m_Handlers;

        #endregion

        protected GameObjectHashSet<SelectedObject> m_SelectedObject = new GameObjectHashSet<SelectedObject>();

        public bool HasSelected
        {
            get { return (m_SelectedObject.Count > 0); }
        }

        protected override void Start()
        {
            base.Start();

            m_Handlers = m_RootGameObject.GetComponentsInChildren<HapticsEditorHandlerBase>();

            foreach (var handler in m_Handlers)
            {
                handler.SetObjectsReference(m_SelectedObject.GetGameObjects());
            }

            // ExosManager.Cleanup.AddListener(m_SelectedObject.RemoveReserved);
        }

        public override void Terminate()
        {
            base.Terminate();

            DeselectAll();
        }

        public void Select(InteractableRoot exosObject, bool setValueToObject = false)
        {
            if (exosObject == null || m_SelectedObject.Contains(exosObject)) { return; }

            m_SelectedObject.Add(new SelectedObject(exosObject, m_OutlineMaterial, m_Origin));

            if (setValueToObject)
            {
                SetValueToObjectAll();
            }
            else
            {
                GetValueFromObject(exosObject.gameObject);
            }
        }

        private bool DeselectOne(InteractableRoot exosObject)
        {
            if (exosObject == null || !m_SelectedObject.Contains(exosObject)) { return false; }

            m_SelectedObject[exosObject].Destroy();

            m_SelectedObject.Remove(exosObject);

            return true;
        }

        private bool DeselectOne(SelectedObject selected)
        {
            return DeselectOne(selected.Interactable);
        }

        public bool Deselect(InteractableRoot exosObject)
        {
            return DeselectOne(exosObject);
        }

        public void DeselectAll()
        {
            //m_SelectedObject.Reverse().ToArray().Foreach(x => DeselectOne(x));

            while (m_SelectedObject.Count() > 0)
            {
                if (DeselectOne(m_SelectedObject.First()) == false) { break; }
            }
        }

        public void SetValueToObjectAll()
        {
            foreach (var handler in m_Handlers)
            {
                handler.SetValueToObject();
            }
        }

        public void GetValueFromObject(GameObject obj)
        {
            foreach (var handler in m_Handlers)
            {
                handler.GetValueFromObject(obj);
            }
        }

        public void GetValueFromObjectAll()
        {
            foreach (var handler in m_Handlers)
            {
                handler.GetValueFromObject();
            }
        }

        public void GenerateObject()
        {
            var obj = Instantiate(m_TargetPrefab, transform.position, transform.rotation);
            obj.name = m_TargetPrefab.name;

            var exosObject = obj.GetComponentInChildren<ExosInteractableRoot>();

            if (exosObject != null)
            {
                Select(exosObject, true);
 
                // Debug.Log("Add because GenerateObject");
            }            
        }
    }

    public class SelectedObject : IHasGameObject
    {
        public GameObject gameObject { get; }
        public InteractableRoot Interactable { get; }

        public Rigidbody Rigidbody => Interactable.Rigidbody;

        public PhysicalProperties PhysicalProperties => Interactable.PhysicalProperties;

        //public GameObject OutlineObject { get; }

        private Shader m_OutlineShader = null;

        public SelectedObject(InteractableRoot exosObject, Material outlineMaterial, Transform lineOrigin)
        {
            gameObject = exosObject.gameObject;
            Interactable = exosObject;

            ShowOutline(exosObject.gameObject, outlineMaterial);
        }

        // NOTE:ExosObjectかOutlineに内包したい
        // 対象にアウトライン用Materialを付与.
        protected void ShowOutline( GameObject gameObject, Material outlineMaterial )
        {
            if (gameObject == null )
            {
                return;
            }

            MeshRenderer[] arMeshRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach ( MeshRenderer rMeshRenderer in arMeshRenderer )
            {
                // アウトラインマテリアルがあるなら処理しない.
                List<Material> arMat = new List<Material>(rMeshRenderer.materials);
                if ( arMat.Find( k => k.shader == outlineMaterial.shader ) != null )
                {
                    continue;
                }

                // 新規マテリアル配列の作成.
                arMat.Add(outlineMaterial);

                // 新マテリアルを設定.
                rMeshRenderer.materials = arMat.ToArray();
            }

            // アウトライン用のシェーダーを記録.
            m_OutlineShader = outlineMaterial.shader;

            // エッジの法線を共通化する.
            MeshFilter[] arMeshFilter = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach ( MeshFilter rMeshFilter in arMeshFilter )
            {
                MeshNormalAverage(rMeshFilter.mesh);
            }
        }

        public void Destroy()
        {
            // if (OutlineObject != null) { GameObject.Destroy(OutlineObject); }

            MeshRenderer[] arMeshRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer rMeshRenderer in arMeshRenderer)
            {
                // アウトラインシェーダーを持つマテリアルを削除.
                List<Material> arMat = new List<Material>(rMeshRenderer.materials);
                arMat.RemoveAll(k => k.shader == m_OutlineShader);

                // 新マテリアルを設定.
                rMeshRenderer.materials = arMat.ToArray();
            }
        }

        // NOTE// NOTE:ExosObjectかOutlineに内包したい
        // 対象のエッジの法線を共通化する.
        protected void MeshNormalAverage(Mesh mesh)
        {
            Dictionary<Vector3, List<int>> map = new Dictionary<Vector3, List<int>>();

            #region build the map of vertex and triangles' relation
            for (int v = 0; v < mesh.vertexCount; ++v)
            {
                if (!map.ContainsKey(mesh.vertices[v]))
                {
                    map.Add(mesh.vertices[v], new List<int>());
                }

                map[mesh.vertices[v]].Add(v);
            }
            #endregion

            Vector3[] normals = mesh.normals;
            Vector3 normal;

            #region the same vertex use the same normal(average)
            foreach (var p in map)
            {
                normal = Vector3.zero;

                foreach (var n in p.Value)
                {
                    normal += mesh.normals[n];
                }

                normal /= p.Value.Count;

                foreach (var n in p.Value)
                {
                    normals[n] = normal;
                }
            }
            #endregion

            mesh.normals = normals;
        }
    }
}
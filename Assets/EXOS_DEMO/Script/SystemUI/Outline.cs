using exiii.Unity.Linq;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class Outline : MonoBehaviour
    {
        public enum OutlineColor
        {
            blue,
            red
        }

        [SerializeField]
        private Material outlineMaterial;

        [SerializeField]
        private Material cloneMaterial;

        private GameObject target;

        // Use this for initialization
        private void Awake()
        {
            cloneMaterial = Instantiate(outlineMaterial);
        }

        // Update is called once per frame
        private void Update()
        {
            CloneTransform();
        }

        public bool Initialize(GameObject target, OutlineColor color)
        {
            this.target = target;

            CloneTransform();
            return CloneMesh(color);
        }

        private void CloneTransform()
        {
            // Transformを同期させる処理
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;

            transform.localScale = new Vector3(
                transform.localScale.x / transform.lossyScale.x * target.transform.lossyScale.x,
                transform.localScale.y / transform.lossyScale.y * target.transform.lossyScale.y,
                transform.localScale.z / transform.lossyScale.z * target.transform.lossyScale.z
            );
        }

        private bool CloneMesh(OutlineColor color)
        {
            // メッシュを同期させる処理
            SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponentInChildren<SkinnedMeshRenderer>();
            MeshRenderer meshRenderer = target.GetComponentInChildren<MeshRenderer>();
            MeshFilter meshFilter = target.GetComponentInChildren<MeshFilter>();

            if (skinnedMeshRenderer != null)
            {
                var skinnedMeshSlave = gameObject.AddComponent<SkinnedMeshRenderer>();

                skinnedMeshSlave.sharedMesh = skinnedMeshRenderer.sharedMesh;
                skinnedMeshSlave.materials = new Material[skinnedMeshRenderer.materials.Length];
                skinnedMeshSlave.enabled = skinnedMeshRenderer.enabled;

                for (int i = 0; i < skinnedMeshSlave.materials.Length; i++)
                {
                    skinnedMeshSlave.materials[i] = outlineMaterial;
                }

                // アウトラインの色の変更
                switch (color)
                {
                    case OutlineColor.blue:
                        cloneMaterial.SetColor("_Color", new Color(0, 0.9f, 0.9f, 0.3f));
                        break;

                    case OutlineColor.red:
                        cloneMaterial.SetColor("_Color", new Color(0.9f, 0, 0, 0.3f));
                        break;
                }
            }
            else if (meshRenderer != null && meshFilter != null)
            {
                var meshFilterSlave = gameObject.AddComponent<MeshFilter>();
                meshFilterSlave.mesh = meshFilter.mesh;

                var outlineMaterials = new Material[meshRenderer.materials.Length];

                switch (color)
                {
                    case OutlineColor.blue:
                        cloneMaterial.SetColor("_Color", Color.cyan);
                        break;

                    case OutlineColor.red:
                        cloneMaterial.SetColor("_Color", Color.red);
                        break;
                }

                for (int i = 0; i < outlineMaterials.Length; i++)
                {
                    outlineMaterials[i] = cloneMaterial;
                }

                var meshRendererSlave = gameObject.AddComponent<MeshRenderer>();

                meshRendererSlave.materials = outlineMaterials;
                meshRendererSlave.enabled = meshRenderer.enabled;
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
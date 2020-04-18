using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class AddCollider : MonoBehaviour
    {
        [SerializeField]
        bool m_IsTrigger = true;

        // Use this for initialization
        void Awake()
        {
            var meshes = GetComponentsInChildren<MeshFilter>();

            foreach (var mesh in meshes)
            {
                var collider = mesh.GetComponentInChildren<Collider>();

                if (collider != null) { continue; }

                var box = mesh.gameObject.AddComponent<BoxCollider>();

                box.isTrigger = m_IsTrigger;

                box.center = mesh.sharedMesh.bounds.center;
                box.size = mesh.sharedMesh.bounds.size;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class FastenableChecker : MonoBehaviour
    {
        private bool m_IsCollideScrew = false;
        public bool IsCollideScrew
        {
            set { this.m_IsCollideScrew = value; }
            get { return this.m_IsCollideScrew; }
        }


        private bool m_IsFastenable = false;
        public bool IsFastenable
        {
            set { this.m_IsFastenable = value; }
            get { return this.m_IsFastenable; }
        }

        private string m_ScrewName;
        public string ScrewName
        {
            set { this.m_ScrewName = value; }
            get { return this.m_ScrewName; }
        }


        private void OnTriggerStay(Collider other)
        {
            m_IsCollideScrew = false;
            if (other.gameObject.tag == "Screw")
            {
                m_IsCollideScrew = true;
                m_IsFastenable = false;
                Transform bitTransform = this.transform;
                Vector3 bitPosition = bitTransform.position;
                Vector3 collideObjectPosition = other.transform.position;

                float xDistance = Mathf.Abs(bitPosition.x - collideObjectPosition.x);
                float yDistance = Mathf.Abs(bitPosition.y - collideObjectPosition.y);
                if (xDistance <= 0.02f && yDistance <= 0.02f)
                {
                    m_IsFastenable = true;
                }
                m_ScrewName = other.gameObject.name;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            m_IsCollideScrew = false;
            m_IsFastenable = false;
        }
    }
}


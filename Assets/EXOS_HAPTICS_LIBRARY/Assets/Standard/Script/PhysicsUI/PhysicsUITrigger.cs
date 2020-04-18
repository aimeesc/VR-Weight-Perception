using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.PhysicsUI
{
    public class PhysicsUITrigger : PhysicsUINode
    {
        [Header("PhysicsUITrigger")]        

        [SerializeField]
        [FormerlySerializedAs("RootUI")]
        protected PhysicsUINode m_RootUI = null;

        protected override void Start()
        {
            base.Start();

            // init rig and head.
            if (m_CameraRig == null && CameraRig.IsExist)
            {
                m_CameraRig = CameraRig.Instance.transform;
                Camera camera = m_CameraRig.GetComponentInChildren<Camera>();
                m_Head = camera?.transform;
            }

            // init manager.
            InitManager(this);
        }

        // update.
        public override void Update()
        {
            // update position.
            UpdatePosition();

            base.Update();
        }

        // on click trigger.
        public void OnClickTrigger()
        {
            if (m_Manager.IsExistUI())
            {
                m_Manager.CloseAllUI();
            }
            else
            {
                PushNewUI(m_CameraRig, m_Head, m_RootUI);
            }
        }

        // init position.
        protected override void InitPosition()
        {
        }

        // update position from origin.
        private void UpdatePosition()
        {
            if (!m_FollowHead) { return; }

            Vector3 posOrigin = m_Head.position;
            Vector3 dirOrigin = m_Head.forward;

            // rotate offset.
            Vector3 rotateOffset = m_PositionOffset;
            Vector3 dirOriginH = new Vector3(dirOrigin.x, 0.0f, dirOrigin.z);
            rotateOffset = Quaternion.FromToRotation(Vector3.forward, dirOriginH) * rotateOffset;

            // set position.
            Vector3 posTrigger = posOrigin + rotateOffset;
            this.transform.position = posTrigger;
        }

        
    }
}

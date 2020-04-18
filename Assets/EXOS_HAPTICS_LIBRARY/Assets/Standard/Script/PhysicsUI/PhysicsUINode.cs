using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using exiii.Unity.Linq;

namespace exiii.Unity.PhysicsUI
{
    public class PhysicsUINode : InteractableRoot
    {
        [Header("PhysicsUINode")]

        [SerializeField]
        [FormerlySerializedAs("FollowHead")]
        protected bool m_FollowHead = true;

        [SerializeField]
        [FormerlySerializedAs("CameraRig")]
        protected Transform m_CameraRig = null;

        [SerializeField]
        [FormerlySerializedAs("Head")]
        protected Transform m_Head = null;

        [SerializeField]
        [FormerlySerializedAs("PositionOffset")]
        protected Vector3 m_PositionOffset = Vector3.zero;

        [SerializeField]
        [FormerlySerializedAs("LayerRecursive")]
        private bool m_LayerRecursive = true;

        // UI manager.
        protected PhysicsUINode m_Manager = null;

        // managed ui tree.
        protected Stack<PhysicsUINode> m_TreeUI = null;

        // lifetime.
        protected float m_LifeTime = 0.0f;

        // start.
        protected override void Start()
        {
            // init UI position.
            InitPosition();

            base.Start();
        }

        // update.
        public virtual void Update()
        {
            m_LifeTime += Time.deltaTime;
        }

        // Is Enable control.
        public bool IsControlActive()
        {
            if (!IsActive)
            {
                return false;
            }

            return (m_LifeTime > 1.0f);
        }

        // open new UI.
        public void PushNewUI(Transform rig, Transform head, PhysicsUINode node)
        {
            if (m_TreeUI == null) { m_TreeUI = new Stack<PhysicsUINode>(); }

            // exist check.
            if (IsAlreadyExist(node) && !IsDuplicatable())
            {
                return;
            }

            // test code.
            if (m_TreeUI.Count != 0) {

                // Hide peek UI.
                PhysicsUINode peek = m_TreeUI.Peek();
                peek.Hide();
            }

            // create new object.
            GameObject objNew = Instantiate(node.gameObject, rig);
            objNew.name.Replace("(Clone)", "");
            if (m_LayerRecursive)
            {
                objNew.DescendantsAndSelf().ForEach(_ => _.layer = gameObject.layer);
            }

            // get panel reference.
            node = objNew.GetComponent<PhysicsUINode>();
            if (node == null) { return; }

            // init transform.           
            node.InitTransform(rig, head);

            // init manager.
            node.InitManager(m_Manager);

            // test code.
            m_TreeUI.Push(node);
        }

        // pop UI.
        public void PopUI()
        {
            if (m_TreeUI == null || m_TreeUI.Count == 0) { return; }

            PhysicsUINode node = m_TreeUI.Peek();
            if (node == null) { return; }

            // destro ui.
            node = m_TreeUI.Pop();
            DestroyImmediate(node.gameObject);

            // show old UI.
            if (m_TreeUI.Count != 0)
            {
                PhysicsUINode old = m_TreeUI.Peek();
                old.Show();
            }
        }

        // close UI.
        public void CloseAllUI()
        {
            if (m_TreeUI == null || m_TreeUI.Count == 0) { return; }

            foreach (PhysicsUINode node in m_TreeUI)
            {
                DestroyImmediate(node.gameObject);
            }
            m_TreeUI.Clear();
        }

        // is exist ui.
        public bool IsExistUI()
        {
            return (m_TreeUI != null && m_TreeUI.Count > 0);
        }

        // is already exist.
        public bool IsAlreadyExist(PhysicsUINode node)
        {
            if (m_TreeUI == null || m_TreeUI.Count == 0)
            {
                return false;
            }

            foreach (PhysicsUINode ui in m_TreeUI)
            {
                if (ui.GetType() == node.GetType())
                {
                    return true;
                }
            }

            return false;
        }

        // is duplicatable.
        public virtual bool IsDuplicatable()
        {
            return false;
        }

        // Hide UI.
        public void Hide()
        {
            m_LifeTime = 0.0f;

            // reset all ui state.
            var uirigids = GetComponentsInChildren<PhysicsUIRigidRestrictorBase>();
            foreach (PhysicsUIRigidRestrictorBase uirigid in uirigids)
            {
                uirigid.ResetUIDefault();
            }

            this.gameObject.SetActive(false);
        }

        // Show UI.
        public void Show()
        {
            m_LifeTime = 0.0f;
            this.gameObject.SetActive(true);
        }

        // on close UI.
        public void OnClickClose()
        {
            if (!IsControlActive()) { return; }
            if (m_Manager == null) { return; }

            // close.
            m_Manager.PopUI();
        }

        // init transform.
        public void InitTransform(Transform rig, Transform head)
        {
            m_CameraRig = rig;
            m_Head = head;
        }

        // init manager.
        public void InitManager(PhysicsUINode manager)
        {
            m_Manager = manager;
        }

        // UI Camera event.
        [EnumAction(typeof(ECameraActions))]
        public void OnClickUICameraEvent(int action)
        {
            if (!IsControlActive()) { return; }

            UICameraEventContainer container = GetComponent<UICameraEventContainer>();
            container?.Emit((ECameraActions)action);
        }

        // init UI position.
        protected virtual void InitPosition()
        {
            Vector3 posOrigin = m_Head.position;
            Vector3 dirOrigin = m_Head.forward;

            // rotate offset.
            Vector3 rotateOffset = m_PositionOffset;
            Vector3 dirOriginH = new Vector3(dirOrigin.x, 0.0f, dirOrigin.z);
            Quaternion offsetRotation = Quaternion.FromToRotation(Vector3.forward, dirOriginH);
            Quaternion panelRotation = Quaternion.FromToRotation(m_CameraRig.forward, dirOriginH);
            rotateOffset = offsetRotation * rotateOffset;

            // set position.
            Vector3 posTrigger = posOrigin + rotateOffset;
            this.transform.position = posTrigger;
            this.transform.rotation = panelRotation * this.transform.rotation;
        }
    }
}

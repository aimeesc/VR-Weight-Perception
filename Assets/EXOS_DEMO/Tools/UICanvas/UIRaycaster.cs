using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using exiii.Unity.EXOS;
using UnityEngine.Serialization;

namespace exiii.Unity.Develop
{
    public class UIRaycaster : ExMonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("NaviMaterial")]
        private Material m_NaviMaterial;

        protected GameObject m_ControllerR = null;
        protected UIRaycasterNavi m_RaycastNavi = null;
        protected UIRaycasterInfo m_UIRaycastInfo = null;

        protected UIBehaviour m_LastTarget = null;
        protected UIBehaviour m_LastCursor = null;

        // on start.
        protected override void Start()
        {
            m_UIRaycastInfo = new UIRaycasterInfo();
            m_RaycastNavi = new UIRaycasterNavi(m_NaviMaterial);
        }

        // on update.
        public void Update()
        {
            // controller initializer.
            InitControllerR();

            // update controller ray.
            UpdateControllerRayR();

            // update navigation ray.
            UpdateNavigationRayR();
        }

        // init controller.
        protected bool InitControllerR()
        {
            if (m_ControllerR != null) { return true; }

            if (RightHand.IsExist)
            {
                m_ControllerR = RightHand.Instance.gameObject;

                EHLEventGeneratorBase generator = m_ControllerR.GetComponentInChildren<EHLEventGeneratorBase>();
                if (generator != null)
                {
                    generator.UseEvent.OnStart().Subscribe(_ => OnUseStart());
                    generator.UseEvent.OnStay().Subscribe(_ => OnUseStay());
                    generator.UseEvent.OnEnd().Subscribe(_ => OnUseEnd());
                }

                return true;
            }

            /*
            SteamPoseTracker[] targets = UnityEngine.Object.FindObjectsOfType<SteamPoseTracker>();
            foreach (SteamPoseTracker target in targets)
            {
                //if (!target.viveRole.IsRole(typeof(HandRole), (int)HandRole.RightHand)) { continue; }

                m_ControllerR = target.gameObject;

                ExosEventGeneratorBase generator = target.GetComponentInChildren<ExosEventGeneratorBase>();
                if (generator != null)
                {
                    generator.UseEvent.OnStart().Subscribe(_ => OnUseStart());
                    generator.UseEvent.OnStay().Subscribe(_ => OnUseStay());
                    generator.UseEvent.OnEnd().Subscribe(_ => OnUseEnd());
                }

                return true;
            }
            */

            return false;
        }

        // update controller ray.
        protected void UpdateControllerRayR()
        {
            m_UIRaycastInfo.Target = null;

            if (m_ControllerR == null) { return; }

            var pointer_data = new PointerEventData(EventSystem.current);

            Vector3 pos = m_ControllerR.transform.position;
            Quaternion rot = m_ControllerR.transform.rotation;

            // TODO: set plane distance.
            // create ray to UI.
            var ray = new Ray(pos, rot * Vector3.forward);
            var plane = new Plane(Vector3.back, 3.0f);
            float enter = 0.0f;
            Vector3 hit = Vector3.zero;
            if (plane.Raycast(ray, out enter))
            {
                hit = ray.GetPoint(enter);
                Vector3 mouse = Camera.main.WorldToScreenPoint(hit);
                pointer_data.position = mouse;
            }
            else
            {
                return;
            }

            // check raycast result.
            var raycast_result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer_data, raycast_result);
            foreach (RaycastResult result in raycast_result)
            {
                // TODO: create ExUIBehavior.
                UIBehaviour ui = null;

                ui = result.gameObject.GetComponent<Button>();
                if (ui == null)
                {
                    ui = result.gameObject.GetComponent<RawImage>();
                    if (ui == null) { continue; }
                }
                else
                {
                    m_LastTarget = ui;
                }

                m_UIRaycastInfo.IsChanged = (m_LastCursor != ui);
                m_UIRaycastInfo.Target = ui;
                m_UIRaycastInfo.EventData = pointer_data;
                m_UIRaycastInfo.HitPosition = hit;
                m_LastCursor = ui;

                return;
            }

            // TODO:calc correct hit position.
        }

        // update navigation ray R.
        protected void UpdateNavigationRayR()
        {
            if (m_UIRaycastInfo.Target == null || m_UIRaycastInfo.IsChanged)
            {
                DestroyNavigation();
                return;
            }

            if (m_UIRaycastInfo.IsExistNavi)
            {
                m_RaycastNavi.Update(m_ControllerR.transform);
                return;
            }

            // create navi.
            CreateNavigation();
        }

        // create navi.
        protected void CreateNavigation()
        {
            if (m_RaycastNavi.IsActive) { return; }

            // create navi.
            m_RaycastNavi.Create(m_ControllerR.transform, m_UIRaycastInfo);

            // set flag for navi.
            m_UIRaycastInfo.IsExistNavi = true;

            // set focus to UI.
            EventSystem.current.SetSelectedGameObject(m_UIRaycastInfo.Target.gameObject);
        }

        // destroy navi.
        protected void DestroyNavigation()
        {
            // destroy navi.
            m_RaycastNavi.Destroy();

            // set flag for navi.
            m_UIRaycastInfo.IsExistNavi = false;

            // change pressed state.
            UIExButton exBtn = m_LastTarget as UIExButton;
            if (exBtn != null)
            {
                exBtn.ResetPressedState();
            }
            m_LastTarget = null;

            EventSystem.current.SetSelectedGameObject(null);
        }

        // on event UseStart.
        public void OnUseStart()
        {
            if (m_UIRaycastInfo.Target == null) { return; }

            Button btn = m_UIRaycastInfo.Target as Button;
            if (btn == null) { return; }

            btn.OnPointerDown(m_UIRaycastInfo.EventData);
        }

        // on event UseStay()
        public void OnUseStay()
        {
            if (m_UIRaycastInfo.Target == null) { return; }

            Button btn = m_UIRaycastInfo.Target as Button;
            if (btn == null) { return; }

            btn.image.overrideSprite = btn.spriteState.pressedSprite;

            // change pressed state.
            UIExButton exBtn = m_UIRaycastInfo.Target as UIExButton;
            if (exBtn != null)
            {
                exBtn.ChangePressedState();
            }
        }

        // on event UseEnd.
        public void OnUseEnd()
        {
            if (m_UIRaycastInfo.Target == null) { return; }

            Button btn = m_UIRaycastInfo.Target as Button;
            if (btn == null) { return; }

            btn.image.overrideSprite = null;

            btn.OnPointerUp(m_UIRaycastInfo.EventData);
            btn.OnPointerClick(m_UIRaycastInfo.EventData);

            // change pressed state.
            UIExButton exBtn = m_UIRaycastInfo.Target as UIExButton;
            if (exBtn != null)
            {
                exBtn.ResetPressedState();
            }
        }
    }

    // capture info.
    public class UIRaycasterInfo
    {
        public UIBehaviour Target = null;
        public PointerEventData EventData = null;
        public Vector3 HitPosition = Vector3.zero;
        public bool IsExistNavi = false;
        public bool IsChanged = false;
    }
}


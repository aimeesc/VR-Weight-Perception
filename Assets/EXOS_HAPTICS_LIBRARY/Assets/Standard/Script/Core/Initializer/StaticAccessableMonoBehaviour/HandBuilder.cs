using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using exiii.Extensions;
using System;

namespace exiii.Unity
{
    public class HandBuilder : StaticAccessableMonoBehaviour<HandBuilder>
    {
        #region Inspector

        [Header(nameof(HandBuilder))]
        [SerializeField, FormerlySerializedAs("SceneHand")]
        private EHLHand m_SceneHand = null;

        [SerializeField]
        private ActivateSetting[] m_ActivateSettings;

        [SerializeField]
        [FormerlySerializedAs("LayerRecursive")]
        private bool m_LayerRecursive = true;

        #endregion Inspector

        protected override void OnValidate()
        {
            base.OnValidate();

            m_ActivateSettings.Foreach(x => x.OnValidate());
        }

        // on awake.
        protected override void Awake()
        {
            base.Awake();

            // change hand target by setting.
            if (m_SceneHand != null)
            {
                EHLHandTarget.TryChangeTarget(m_SceneHand);
            }

            int layer = (m_LayerRecursive) ? this.gameObject.layer : 0;

            // build rig with setting.
            EHLHand.BuildRig(layer);

            // build hand with setting.
            EHLHand.BuildHand(layer);

            // activate target.
            m_ActivateSettings.Foreach(x => x.ActiveChange());
        }

        [Serializable]
        private class ActivateSetting
        {
#pragma warning disable 414
            [SerializeField, HideInInspector]
            private string m_Name;
#pragma warning restore 414

            [SerializeField, FormerlySerializedAs("m_HandType")]
            private EHandMode m_HandMode = EHandMode.Controller;

            [SerializeField, FormerlySerializedAs("ActivateTarget")]
            private GameObject[] m_ActivateTarget = null;

            [SerializeField]
            private bool m_ActiveChangeTo = false;

            public void ActiveChange()
            {
                if (!EHLHand.IsExist) { return; }

                if (m_HandMode == EHLHand.Instance.HandMode)
                {
                    m_ActivateTarget.Foreach(x => x.SetActive(m_ActiveChangeTo));
                }
            }

            public void OnValidate()
            {
                m_Name = m_HandMode.EnumToString();
            }
        }
    }

}

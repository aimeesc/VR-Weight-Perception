using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Develop
{
    public class UI3DCanvas : StaticAccessableMonoBehaviour<UI3DCanvas>
    {
        #region Static

        public static bool TryGetCanvas(out Canvas canvas)
        {
            if (!IsExist)
            {
                canvas = null;
                return false;
            }

            canvas = Instance.m_Canvas;
            return (canvas != null);
        }

        public static bool TryGetSetting(out UICanvasSetting setting)
        {
            if (!IsExist)
            {
                setting = null;
                return false;
            }

            setting = Instance.m_UICanvasSetting;
            return (setting != null);
        }

        #endregion Static

        #region Inspector

        [Header(nameof(UI3DCanvas))]
        [SerializeField]
        private Canvas m_Canvas;

        [SerializeField]
        private UICanvasSetting m_UICanvasSetting;

        #endregion Inspector

        // on start
        protected override void Start()
        {
            if (m_UICanvasSetting != null)
            {
                m_UICanvasSetting.Initialize(m_Canvas);
            }
        }
    }
}

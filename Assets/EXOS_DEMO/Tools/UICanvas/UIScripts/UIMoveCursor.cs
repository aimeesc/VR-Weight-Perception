using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System;

namespace exiii.Unity.Develop
{
    public class UIMoveCursor : UIBase
    {       
        // On click close.
        public void OnClickClose()
        {
            UICanvasSetting setting = null;
            if (!UI3DCanvas.TryGetSetting(out setting)) { return; }

            setting.DestroyAllWidget();
            setting.CreateWidget<UIDemoMenu>();
        }
    }
}
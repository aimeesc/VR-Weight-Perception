using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace exiii.Unity.Develop
{
    public class UIDemoMenu : UIBase
    {
        // On click move cursor button.
        public void OnClickMoveCursor()
        {
            UICanvasSetting setting = null;
            if (!UI3DCanvas.TryGetSetting(out setting)) { return; }

            setting.DestroyAllWidget();
            setting.CreateWidget<UIMoveCursor>();
        }

        
    }
}
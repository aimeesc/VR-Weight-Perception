using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Develop
{
    public class CheckFocus : PanelUserBase
    {
        void OnApplicationFocus(bool focus)
        {
            Panel.Enabled = !focus;
        }
    }
}


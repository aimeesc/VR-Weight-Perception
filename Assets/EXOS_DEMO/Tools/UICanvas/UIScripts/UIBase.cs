using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System;

namespace exiii.Unity.Develop
{
    public class UIBase : ExMonoBehaviour
    {
        // UI Camera event.
        [EnumAction(typeof(ECameraActions))]
        public void OnClickUICameraEvent(int action)
        {
            UICameraEventContainer container = GetComponentInParent<UICameraEventContainer>();
            container?.Emit((ECameraActions)action);
        }

        // UI Debug event.
        [EnumAction(typeof(EDebugActions))]
        public void OnClickUIDebugEvent(int action)
        {
            UIDebugEventContainer container = GetComponentInParent<UIDebugEventContainer>();
            container?.Emit((EDebugActions)action);
        }
    }
}

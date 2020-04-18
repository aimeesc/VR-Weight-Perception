using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.UI
{
    public class ObjectSwitchUse : ObjectSwitchBase, IUsableScript
    {
        public void OnStart(IUseManipulation manipulation)
        {
            if (m_Target == null) { return; }

            SwitchEnable();
        }

        public void OnUpdate(IUseManipulation manipulation)
        {
            
        }

        public void OnFixedUpdate(IUseManipulation manipulation)
        {
            
        }

        public void OnEnd(IUseManipulation manipulation)
        {
            
        }
    }
}
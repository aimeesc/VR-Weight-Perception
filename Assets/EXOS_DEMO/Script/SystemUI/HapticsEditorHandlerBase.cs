using System.Collections;
using System.Collections.Generic;
using exiii.Unity.Rx;
using UnityEngine;

namespace exiii.Unity.UI
{
    public abstract class HapticsEditorHandlerBase : MonoBehaviour
    {
        protected IEnumerable<GameObject> TargetObjects;

        public void SetObjectsReference(IEnumerable<GameObject> objects)
        {
            TargetObjects = objects;
        }

        public abstract void SetValueToObject();

        public abstract void GetValueFromObject();

        public abstract void GetValueFromObject(GameObject obj);
    }
}

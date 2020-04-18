using exiii.Unity.EXOS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class KinematicUntilGrab : InteractableNode, IGrabbableScript
    {
        protected override void Start()
        {
            InteractableRoot.Rigidbody.isKinematic = true;
        }

        public void OnEnd(IGrabManipulation manipulation)
        {
            
        }

        public void OnFixedUpdate(IGrabManipulation manipulation)
        {
            
        }

        public void OnStart(IGrabManipulation manipulation)
        {
            InteractableRoot.Rigidbody.isKinematic = false;

            Destroy(this);
        }

        public void OnUpdate(IGrabManipulation manipulation)
        {
            
        }
    }
}
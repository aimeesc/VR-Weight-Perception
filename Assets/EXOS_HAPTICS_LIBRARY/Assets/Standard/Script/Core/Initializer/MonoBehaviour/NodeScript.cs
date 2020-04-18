using UnityEngine;
using System.Collections;

namespace exiii.Unity
{
	public abstract class NodeScript : ExMonoBehaviour, INodeScript
	{
        #region INodeScript

        public IRootScript RootScript { get; private set; }

        public GameObject RootGameObject
        {
            get
            {
                if (RootScript == null)
                {
                    return null;
                }
                else
                {
                    return RootScript.gameObject;
                }
            }
        }

        public bool InjectFinished { get; private set; }

        public virtual void StartInjection(IRootScript root)
        {
            RootScript = root;
        }

        public virtual void FinishInjection()
        {
            InjectFinished = true;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (RootScript == null)
            {
                Debug.LogWarning($"{ExName} / {GetType().ToString()} : RootScript not injected", this);
            }
        }

        #endregion INodeScript
    }
}

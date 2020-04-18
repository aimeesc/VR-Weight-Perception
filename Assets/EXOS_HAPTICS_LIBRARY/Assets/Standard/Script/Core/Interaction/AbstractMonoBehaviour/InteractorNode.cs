using UnityEngine;

namespace exiii.Unity
{
    public abstract class InteractorNode : NodeScript, IInteractorNode
    {
        protected IInteractorRoot InteractorRoot { get; private set; }

        public override void StartInjection(IRootScript root)
        {
            base.StartInjection(root);

            InteractorRoot = root.gameObject.GetComponent<IInteractorRoot>();
        }
    }
}
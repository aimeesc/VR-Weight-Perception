namespace exiii.Unity
{
    /// <summary>
    /// Base of exos function script
    /// </summary>
    public abstract class InteractableNode : NodeScript
    {
        protected IInteractableRoot InteractableRoot { get; private set; }

        public override void StartInjection(IRootScript root)
        {
            base.StartInjection(root);

            InteractableRoot = root.gameObject.GetComponent<IInteractableRoot>();
        }
    }
}
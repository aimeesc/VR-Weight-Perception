using exiii.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class ExActionContainer<TAction> : MonoBehaviour, IEnumerable<ExActionBase<TAction>>
    {
        protected abstract IEnumerable<ExActionBase<TAction>> Actions { get; }

        private void OnValidate()
        {
            if (Actions != null)
            {
                Actions.Foreach(x => x.OnValidate());
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Actions.GetEnumerator();
        }

        public IEnumerator<ExActionBase<TAction>> GetEnumerator()
        {
            return Actions.GetEnumerator();
        }
    }
}
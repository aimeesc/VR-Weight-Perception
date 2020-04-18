using exiii.Unity.Rx;
using System;
using System.Collections.Generic;

namespace exiii.Unity
{
    public abstract class DetectedHolder<TType> : DetectorBase
    {
        public HashSet<TType> Targets { get; } = new HashSet<TType>();

        private Subject<IEnumerable<TType>> m_TargetsChanged = new Subject<IEnumerable<TType>>();

        public IObservable<IEnumerable<TType>> OnTargetsChanged => m_TargetsChanged;

        public override void OnEnter(IDetectable contact)
        {
            if (contact.RootGameObject == null) { return; }

            var target = contact.RootGameObject.GetComponent<TType>();

            if (target != null)
            {
                Targets.Add(target);
                m_TargetsChanged.OnNext(Targets);
            }
        }

        public override void OnExit(IDetectable contact)
        {
            if (contact.RootGameObject == null) { return; }

            var target = contact.RootGameObject.GetComponent<TType>();

            if (Targets.Contains(target))
            {
                Targets.Remove(target);
                m_TargetsChanged.OnNext(Targets);
            }
        }
    }
}
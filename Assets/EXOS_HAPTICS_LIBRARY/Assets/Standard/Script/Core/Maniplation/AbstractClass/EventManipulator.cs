using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public abstract class EventManipulator<TInterface, TClass> : ColliderManipulator<TInterface, TClass>
        where TInterface : class, ICycleManipulation<TInterface>
        where TClass : EventManipulation<TInterface>, TInterface
    {
        public enum EManipulationTarget
        {
            All,
            Closest,
        }

        #region Inspector

        [Header("EventManipulator")]

        [SerializeField, FormerlySerializedAs("ManipulationTarget")]
        private EManipulationTarget m_ManipulationTarget = EManipulationTarget.All;

        [SerializeField, Unchangeable]
        private HashSet<GameObject> m_EventTargets = new HashSet<GameObject>();

        public IEnumerable<GameObject> EventTargets => m_EventTargets;

        #endregion Inspector

        private void AddTarget(GameObject target)
        {
            if (target == null) { return; }

            if (m_EventTargets.Contains(target)) { return; }

            EHLDebug.Log($"{ExName}.TargetAdded : {target}", this, "Manipulation");

            m_EventTargets.Add(target);

            target
                .OnDisableAsObservable()
                .Subscribe(_ => RemoveTarget(target))
                .AddTo(target);
        }

        private void RemoveTarget(GameObject target)
        {
            if (target == null) { return; }

            if (!m_EventTargets.Contains(target)) { return; }

            EHLDebug.Log($"{ExName}.TargetRemoved : {target}", this, "Manipulation");

            m_EventTargets.Remove(target);

            OnRemove(target);
        }

        protected void OnEventManipulationStart()
        {
            switch (m_ManipulationTarget)
            {
                case EManipulationTarget.All:
                    {
                        var maniplables = EventTargets
                            .SelectMany(x => x.GetComponentsInChildren<IManipulable<TInterface>>(x))
                            .Distinct();
                        if (maniplables == null) { return; }
                        OnManipulationStart(maniplables);
                        break;
                    }
                case EManipulationTarget.Closest:
                    {
                        var maniplables = EventTargets
                            .OrderBy(_ => Vector3.Distance(_.transform.position, this.transform.position))
                            .SelectMany(_ => _.GetComponentsInChildren<IManipulable<TInterface>>(_))
                            .FirstOrDefault();
                        if (maniplables == null) { return; }
                        OnManipulationStart(maniplables);
                        break;
                    }
            }
        }

        protected void OnEventManipulationEnd()
        {
            OnManipulationEnd(ManipulationTargets.Keys);
        }

        protected void OnRemove(GameObject target)
        {
            if (target == null) { return; }

            var maniplables = target
                .GetComponentsInChildren<IManipulable<TInterface>>()
                .Distinct();

            maniplables.Foreach(x => CancelManipulation(x));
        }

        #region ColliderManipulator

        public override void OnEnter(IManipulable<TInterface> component)
        {
            AddTarget(component.gameObject);
        }

        public override void OnExit(IManipulable<TInterface> component)
        {
            RemoveTarget(component.gameObject);
        }

        #endregion ColliderManipulator
    }
}
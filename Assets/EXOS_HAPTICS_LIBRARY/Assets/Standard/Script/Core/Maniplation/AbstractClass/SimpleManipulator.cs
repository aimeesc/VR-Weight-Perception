using exiii.Extensions;
using exiii.Unity.Rx;
using UnityEngine;
using System;

namespace exiii.Unity
{
    public abstract class SimpleManipulator<TInterface, TClass> : ImmidiateManipulator<TInterface, TClass>
        where TInterface : class, ICycleManipulation<TInterface>
        where TClass : SimpleManipulation<TInterface>, TInterface
    {
        #region Inspector

        [Header("SimpleManipulator")]
        [SerializeField]
        private GameObject m_EventSetHolder;

        #endregion Inspector

        private IExEventSet<IManipulable<TInterface>> m_EventSet;

        protected override void Awake()
        {
            base.Awake();

            m_EventSet = m_EventSetHolder.GetComponent<IExEventSet<IManipulable<TInterface>>>();

            if (m_EventSet == null)
            {
                Debug.LogWarning("Target has no IExEventSet", this);
            }
        }

        protected override void Start()
        {
            base.Start();

            m_EventSet.OnStart().Subscribe(x => OnManipulationStart(x));

            m_EventSet.OnEnd().Subscribe(x => OnManipulationEnd(x));
        }
    }
}
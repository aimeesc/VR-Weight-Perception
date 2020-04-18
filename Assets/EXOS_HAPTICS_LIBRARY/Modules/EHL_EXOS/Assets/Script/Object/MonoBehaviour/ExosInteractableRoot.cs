using exiii.Extensions;
using exiii.Unity.Device;
using exiii.Unity.Rx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity.EXOS
{    
    public class ExosInteractableRoot : InteractableRoot
    {
        static ExosInteractableRoot()
        {
            ManipulationFilterSetting.AddPathType(typeof(IExosForceReceiver), new ExTag<EPathType>(EPathType.Force));
        }

        protected override void Start()
        {
            base.Start();

            // Generator
            m_TouchForceGenerators = GetComponentsInChildren<IExosTouchForceGenerator>().ToList();

            m_SurfaceForceGenerators = GetComponentsInChildren<IExosSurfaceForceGenerator>().ToList();

            m_GripForceGenerators = GetComponentsInChildren<IExosGripForceGenerator>().ToList();

            m_GrabForceGenerators = GetComponentsInChildren<IExosGrabForceGenerator>().ToList();

            if (CalcPenetration)
            {
                var touchGenerator = new ExosTouchEffectGenerator(this);

                m_TouchForceGenerators.Add(touchGenerator);
            }
        }

        #region Touch

        private List<IExosTouchForceGenerator> m_TouchForceGenerators;

        private List<IExosSurfaceForceGenerator> m_SurfaceForceGenerators;

        public override bool TryStartManipulation(ITouchManipulation manipulation)
        {
            if (base.TryStartManipulation(manipulation) == false) { return false; }

            var ForceObservable = manipulation.GetObservable<IExosForceReceiver>();

            ForceObservable
                .Where(x => manipulation.TryUpdateTouchState(ShapeContainer))
                .Subscribe(x => OnTouchForceGenerate(manipulation, x, manipulation.ShapeStateSet));

            ForceObservable
                .Where(x => manipulation.TryUpdateTouchState(ShapeContainer))
                .Where(x => manipulation.TryUpdateSurfaceState(SurfaceContainer))
                .Subscribe(x => OnSurfaceForceGenerate(manipulation, x, manipulation.SurfaceStateSet));

            ForceObservable
                .Where(x => !ManipulationState.IsManipulated(EManipulationType.Grip))
                .Subscribe(x => x.ChangeForceMode(EAxisType.Pinch, EForceMode.Both));

            return true;
        }

        public void OnTouchForceGenerate(ITouchManipulation manipulation, IExosForceReceiver receiver, IShapeStateSet data)
        {
            if (m_TouchForceGenerators == null) { return; }

            m_TouchForceGenerators.Foreach(x => x.OnGenerate(receiver, data));
        }

        public void OnSurfaceForceGenerate(ITouchManipulation manipulation, IExosForceReceiver receiver, ISurfaceStateSet data)
        {
            if (m_SurfaceForceGenerators == null) { return; }

            m_SurfaceForceGenerators.Foreach(x => x.OnGenerate(receiver, data));
        }

        #endregion Touch

        #region Grip

        private List<IExosGripForceGenerator> m_GripForceGenerators;

        public override bool TryStartManipulation(IGripManipulation manipulation)
        {
            if (base.TryStartManipulation(manipulation) == false) { return false; }

            // Force
            // IObservable<IExosForceReceiver> ForceObservable;
            var ForceObservable = manipulation.GetObservable<IExosForceReceiver>();

            ForceObservable
                .Subscribe(x => OnGripForceGenerate(x, null));

            ForceObservable
                .Subscribe(x => x.ChangeForceMode(EAxisType.Pinch, EForceMode.Negative));

            return true;
        }

        public void OnGripForceGenerate(IExosForceReceiver receiver, IGripState data)
        {
            if (m_GripForceGenerators == null) { return; }

            m_GripForceGenerators.Foreach(x => x.OnGenerate(receiver, data));
        }

        #endregion Grip

        #region Grab

        private List<IExosGrabForceGenerator> m_GrabForceGenerators;

        private Dictionary<GameObject, IGrabManipulation> m_ToggleManipulations = new Dictionary<GameObject, IGrabManipulation>();

        public override bool TryStartManipulation(IGrabManipulation manipulation)
        {
            if (base.TryStartManipulation(manipulation) == false) { return false; }

            // IObservable<IExosForceReceiver> ForceObservable;
            var ForceObservable = manipulation.GetObservable<IExosForceReceiver>();

            ForceObservable
                .Subscribe(x => OnGrabForceGenerate(x, null));

            ForceObservable
                .Where(x => !ManipulationState.IsManipulated(EManipulationType.Grip))
                .Subscribe(x => x.ChangeForceMode(EAxisType.Pinch, EForceMode.Both));

            return true;
        }

        public void OnGrabForceGenerate(IExosForceReceiver receiver, IGrabState data)
        {
            if (m_GrabForceGenerators == null) { return; }

            m_GrabForceGenerators.Foreach(x => x.OnGenerate(receiver, data));
        }

        #endregion Grab

        #region Use

        public override bool TryStartManipulation(IUseManipulation manipulation)
        {
            if (base.TryStartManipulation(manipulation) == false) { return false; }

            // IObservable<IExosForceReceiver> ForceObservable;
            var ForceObservable = manipulation.GetObservable<IExosForceReceiver>();

            ForceObservable
                .Where(x => !ManipulationState.IsManipulated(EManipulationType.Grip))
                .Subscribe(x => x.ChangeForceMode(EAxisType.Pinch, EForceMode.Both));

            return true;
        }

        #endregion Use
    }
}


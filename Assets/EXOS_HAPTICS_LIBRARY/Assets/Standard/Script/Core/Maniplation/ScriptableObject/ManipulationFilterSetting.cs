using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "ForceFilterSetting", menuName = "EXOS/Force/ForceFilterSetting")]
    public class ManipulationFilterSetting : ExScriptableObject
    {
        #region Static

        private static Dictionary<Type, IExTag> s_Dictionary = new Dictionary<Type, IExTag>();

        static ManipulationFilterSetting()
        {
            s_Dictionary.Add(typeof(IForceReceiver), new ExTag<EPathType>(EPathType.Force));
            s_Dictionary.Add(typeof(IPositionReceiver), new ExTag<EPathType>(EPathType.Position));
            s_Dictionary.Add(typeof(IVibrationReceiver), new ExTag<EPathType>(EPathType.Vibration));
        }

        public static void AddPathType(Type type, IExTag tag)
        {
            if (s_Dictionary.ContainsKey(type)) { return; }

            s_Dictionary.Add(type, tag);
        }

        #endregion

        #region Inspector

        [Header(nameof(ManipulationFilterSetting))]
        [SerializeField]
        private ExTagContainer m_ExTag;

        public IExTag ExTag { get { return m_ExTag; } }

        [SerializeField]
        private EListType m_ListType = EListType.BlackOR;

        [SerializeField]
        [FormerlySerializedAs("m_ForceFilterParameters")]
        private ManipulationFilter[] m_FilterParameters;

        #endregion Inspector  

        private ExTag m_TagAll = new ExTag<EPathType>(EPathType.All);

        public bool CheckPathType(Type type)
        {
            IExTag tag;

            if (m_ExTag.CheckTag(m_TagAll)) { return true; }

            var result = s_Dictionary.TryGetValue(type, out tag) && m_ExTag.CheckTag(tag);

            //Debug.Log($"[{ExName}] CheckPathType : {result}");

            return result;
        }

        public bool CheckFilter(IManipulatorState manipulatorState)
        {
            //Debug.Log($"[{ExName}] CheckFilter");

            switch (m_ListType)
            {
                case EListType.WhiteOR:
                    foreach (var parameter in m_FilterParameters)
                    {
                        IManipulator manipulator;

                        if (manipulatorState.TryGetManipulator(parameter.ManipulationType, out manipulator))
                        {
                            if (manipulator.IsManipulating == parameter.IsManipulated) { return true; }
                        }
                    }

                    return false;

                case EListType.BlackOR:
                    foreach (var parameter in m_FilterParameters)
                    {
                        IManipulator manipulator;

                        if (manipulatorState.TryGetManipulator(parameter.ManipulationType, out manipulator))
                        {
                            if (manipulator.IsManipulating == parameter.IsManipulated) { return false; }
                        }
                    }

                    return true;
            }

            return false;
        }

        public bool CheckFilter(IManipulatorState manipulatorState, IManipulationState manipulationState)
        {
            //Debug.Log($"[{ExName}] CheckFilter");

            switch (m_ListType)
            {
                case EListType.WhiteOR:
                    foreach (var parameter in m_FilterParameters)
                    {
                        IManipulator manipulator;

                        if (manipulatorState.TryGetManipulator(parameter.ManipulationType, out manipulator))
                        {
                            if (manipulationState.IsManipulatedBy(manipulator) == parameter.IsManipulated) { return true; }
                        }
                    }

                    return false;

                case EListType.BlackOR:
                    foreach (var parameter in m_FilterParameters)
                    {
                        IManipulator manipulator;

                        if (manipulatorState.TryGetManipulator(parameter.ManipulationType, out manipulator))
                        {
                            if (manipulationState.IsManipulatedBy(manipulator) == parameter.IsManipulated) { return false; }
                        }
                    }

                    return true;
            }

            return false;
        }
    }

    [Serializable]
    public class ManipulationFilter
    {
        #region Inspector

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private EManipulationType m_ManipulationType;

        public EManipulationType ManipulationType { get { return m_ManipulationType; } }

        [SerializeField]
        private bool m_IsManipulated;

        public bool IsManipulated { get { return m_IsManipulated; } }

        #endregion Inspector
    }
}
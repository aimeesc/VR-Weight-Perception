using exiii.Unity.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "EHLHand", menuName = "EXOS/Hand/EHLHand")]
    public class EHLHand : StaticAccessableScriptableObject<EHLHand>
    {
        #region Static

        static EHLHand()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        // resume default setting.
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!IsExist) { return; }

            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    //Instance.m_DefaultPenetratorBuildParameter = new PenetratorBuildParameter(Instance.m_PenetratorBuildParameter);
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    //Instance.m_PenetratorBuildParameter = Instance.m_DefaultPenetratorBuildParameter;
                    break;
            }
        }
#endif

        // get device setting.
        public static string GetDeviceType()
        {
            if (!IsExist) { return string.Empty; }

            return Instance.name;
        }

        public static bool TryGetPenetratorBuildParameter(out PenetratorBuildParameter parameter)
        {
            if (!IsExist)
            {
                parameter = null;
                return false;
            }

            parameter = Instance.m_PenetratorBuildParameter;

            return true;
        }

        public static bool TryGetPenetratorParameter(out PenetratorParameter parameter)
        {
            if (!IsExist)
            {
                parameter = null;
                return false;
            }

            parameter = Instance.m_PenetratorParameter;

            return true;
        }

        // get tactile type.
        public static ETactileType GetTactileType()
        {
            if (!IsExist) { return ETactileType.None; }

            return Instance.m_TactileType;
        }

        // change tactile type.
        public static void ChangeTactileType(ETactileType type)
        {
            if (!IsExist) { return; }

            Instance.m_TactileType = type;
        }

        // build rig.
        public static bool BuildRig(int layer = 0)
        {
            if (!IsExist) { return false; }

            EHLDebug.Log($"Build: Rig", null, "Controller");

            // build rigs.
            Instance.BuildRigPrefabs(layer);

            return true;
        }

        // build hand.
        public static bool BuildHand(int layer = 0)
        {
            if (!IsExist) { return false; }

            EHLDebug.Log($"Build: {GetDeviceType()}", null, "Controller");

            // build hands.
            Instance.BuildHandPrefabs(layer);

            return true;
        }

        // find instance.
        public static ToolsHolder FindTactileTarget()
        {
            if (!IsExist) { return null; }
            if (Instance.m_TactilePrefabIndex >= Instance.m_Hands.Count) { return null; }

            // find instance.
            GameObject gameObject = Instance.m_Hands[Instance.m_TactilePrefabIndex];
            if (gameObject == null) { return null; }

            // find toolsHolder.
            ToolsHolder[] toolsHolders = gameObject.GetComponentsInChildren<ToolsHolder>();
            foreach (ToolsHolder toolsHolder in toolsHolders)
            {
                if (toolsHolder.name == Instance.m_TactileTransformName)
                {
                    return toolsHolder;
                }
            }

            return null;
        }

        #endregion

        public enum ETactileType
        {
            None,
            OneShot,
            Loop,
        }

        #region Inspector

        [Header("EHLHand")]
        [SerializeField]
        private EHandMode m_HandMode = EHandMode.Controller;

        public EHandMode HandMode { get { return m_HandMode; } }

        [SerializeField]
        private EDeviceEquipType m_DeviceEquipType = EDeviceEquipType.Default;

        public EDeviceEquipType DeviceEquipType { get { return m_DeviceEquipType; } }

        [PrefabField, SerializeField, FormerlySerializedAs("RigPrefabs")]
        private GameObject[] m_RigPrefabs = null;

        [PrefabField, SerializeField, FormerlySerializedAs("HandPrefabs")]
        private GameObject[] m_HandPrefabs = null;

        [Header("Penetrator")]

        [SerializeField]
        private PenetratorBuildParameter m_PenetratorBuildParameter;

        [SerializeField]
        private PenetratorParameter m_PenetratorParameter;

        // Hack: behind tactile settings
        //[Header("Tactile")]

        //[SerializeField]
        [FormerlySerializedAs("TactileType")]
        private ETactileType m_TactileType = ETactileType.None;

        //[SerializeField]
        [FormerlySerializedAs("TactilePrefabIndex")]
        private int m_TactilePrefabIndex = 0;

        //[SerializeField]
        [FormerlySerializedAs("TactileTransformName")]
        private string m_TactileTransformName = string.Empty;

        #endregion

        private List<GameObject> m_Hands = new List<GameObject>();
        private List<GameObject> m_Rigs = new List<GameObject>();

        // for resume.
        private PenetratorBuildParameter m_DefaultPenetratorBuildParameter;

        // Initialize VR Rig.
        private void BuildRigPrefabs(int layer)
        {
            // clear instance.
            m_Rigs.Clear();

            // build require object.
            foreach (var prefab in m_RigPrefabs)
            {
                if (prefab == null) { continue; }

                var ExObj = Instantiate(prefab);
                ExObj.gameObject.name = prefab.gameObject.name;

                if (layer != 0)
                {
                    ExObj.gameObject.DescendantsAndSelf()
                        .ForEach(_ => _.layer = layer);
                }

                m_Rigs.Add(ExObj);
            }
        }

        // Initialize VR Controller.
        private void BuildHandPrefabs(int layer)
        {
            // clear instance.
            m_Hands.Clear();

            // build require object.
            foreach (var prefab in m_HandPrefabs)
            {
                if (prefab == null) { continue; }

                var ExObj = Instantiate(prefab);
                ExObj.gameObject.name = prefab.gameObject.name;

                if (layer != 0)
                {
                    ExObj.gameObject.DescendantsAndSelf()
                        .ForEach(_ => _.layer = layer);
                }

                m_Hands.Add(ExObj);
            }
        }
    }
}
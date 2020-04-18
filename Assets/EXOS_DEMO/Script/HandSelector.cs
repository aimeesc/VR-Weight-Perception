using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;

namespace exiii.Unity.Sample
{
    public class HandSelector : MonoBehaviour
    {
        // hand select.
        public enum EHandSelect
        {
            VRController,
            ManusVR,
            LeapMotion,
            OculusQuest,
        }

        [SerializeField, FormerlySerializedAs("ButtonVRController")]
        private Button m_ButtonVRController = null;

        [SerializeField, FormerlySerializedAs("ButtonManusVR")]
        private Button m_ButtonManusVR = null;

        [SerializeField, FormerlySerializedAs("ButtonLeapMotion")]
        private Button m_ButtonLeapMotion = null;

        [SerializeField, FormerlySerializedAs("Scenes")]
        private ScenesTable m_Scenes = null;

        [SerializeField, FormerlySerializedAs("Hands")]
        private HandTable m_Hands = null;

        // on awake.
        public void Awake()
        {
            // init triggers.
            m_ButtonVRController.OnClickAsObservable()
                .Subscribe(_ => OnHandSelect(EHandSelect.VRController));
            m_ButtonManusVR.OnClickAsObservable()
                .Subscribe(_ => OnHandSelect(EHandSelect.ManusVR));
            m_ButtonLeapMotion.OnClickAsObservable()
                .Subscribe(_ => OnHandSelect(EHandSelect.LeapMotion));

#if UNITY_ANDROID || (UNITY_ANDROID && UNITY_EDITOR)
            this.UpdateAsObservable().Delay(TimeSpan.FromSeconds(0.2))
                .Subscribe(_ => OnHandSelect(EHandSelect.OculusQuest));
#endif // UNITY_ANDROID || (UNITY_ANDROID && UNITY_EDITOR)
        }

        // on click selector.
        private void OnHandSelect(EHandSelect select)
        {
            var container = FindObjectOfType<KeyboardSceneEventContainer>();
            if (container == null) { return; }

            // change hand setting.
            if (m_Hands != null && m_Hands.GetTable().ContainsKey(select))
            {
                EHLHandTarget.TryChangeTarget(m_Hands.GetTable()[select]);
                EHLHandTarget.ChangeAllowOverwirte(false);
            }

            // emit scene changer.
            if (m_Scenes != null && m_Scenes.GetTable().ContainsKey(select))
            {
                container.Emit(m_Scenes.GetTable()[select]);
            }
        }

#region SceneTable
        
        [System.Serializable]
        public class ScenesTable : TableBase<EHandSelect, ESceneActions, ScenesPair>
        {
        }

        [System.Serializable]
        public class ScenesPair : KeyAndValue<EHandSelect, ESceneActions>
        {
            public ScenesPair(EHandSelect key, ESceneActions value) : base(key, value)
            {
            }
        }

#endregion // SceneTable.

#region HandTable

        [System.Serializable]
        public class HandTable : TableBase<EHandSelect, EHLHand, HandPair>
        {
        }

        [System.Serializable]
        public class HandPair : KeyAndValue<EHandSelect, EHLHand>
        {
            public HandPair(EHandSelect key, EHLHand value) : base(key, value)
            {
            }
        }

#endregion // HandTable

#region ForSerialization

        [System.Serializable]
        public class TableBase<TKey, TValue, Type> where Type : KeyAndValue<TKey, TValue>
        {
            [SerializeField]
            private List<Type> list;
            private Dictionary<TKey, TValue> table;


            public Dictionary<TKey, TValue> GetTable()
            {
                if (table == null)
                {
                    table = ConvertListToDictionary(list);
                }
                return table;
            }

            /// <summary>
            /// Editor Only
            /// </summary>
            public List<Type> GetList()
            {
                return list;
            }

            static Dictionary<TKey, TValue> ConvertListToDictionary(List<Type> list)
            {
                Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
                foreach (KeyAndValue<TKey, TValue> pair in list)
                {
                    dic.Add(pair.Key, pair.Value);
                }
                return dic;
            }
        }

        [System.Serializable]
        public class KeyAndValue<TKey, TValue>
        {
            public TKey Key;
            public TValue Value;

            public KeyAndValue(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
            public KeyAndValue(KeyValuePair<TKey, TValue> pair)
            {
                Key = pair.Key;
                Value = pair.Value;
            }
        }

#endregion // ForSerialization
    }
}

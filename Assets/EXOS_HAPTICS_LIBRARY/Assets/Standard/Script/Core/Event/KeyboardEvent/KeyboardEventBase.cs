using System;
using UnityEngine;

namespace exiii.Unity
{
    public enum EKeyTiming
    {
        KeyDown,
        KeyUp,
        Key,
    }

    [Serializable]
    public abstract class KeyboardEventBase<TAction> : ExEventBase<TAction>
    {
        #region Inspector

        [SerializeField, UnchangeableInPlaying]
        private KeyCode m_KeyCode;

        public KeyCode KeyCode => m_KeyCode;

        [SerializeField, UnchangeableInPlaying]
        private EKeyTiming m_KeyTiming;

        public EKeyTiming KeyTiming => m_KeyTiming;

        #endregion Inspector
    }
}
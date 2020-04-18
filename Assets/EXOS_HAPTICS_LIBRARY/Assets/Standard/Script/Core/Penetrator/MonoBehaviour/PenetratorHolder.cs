using UnityEngine;

namespace exiii.Unity
{
    public abstract class PenetratorHolder : ExMonoBehaviour
    {
        #region Inspector

        [Header(nameof(PenetratorHolder))]
        [SerializeField]
        private bool m_UseToPenetration = true;

        public bool UseToPenetration { get { return m_UseToPenetration; } }

        #endregion

        public abstract IPenetrator Penetrator { get; }

        public abstract void SetValues(float size, float mass);

        public abstract void SetVisible(bool visible);
    }
}
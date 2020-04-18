using UnityEngine;

namespace exiii.Unity
{
    public abstract class ResourcesBase<T> : ScriptableObject
        where T : ResourcesBase<T>
    {
        #region Static

        protected static string AssetName { get; set; }

        protected static bool DoNameCheck { get; set; } = true;

        #endregion Static

        #region Inspector

        [Header("ResourcesBase")]
#pragma warning disable 414
        [SerializeField, Unchangeable]
        private string m_AssetName;

#pragma warning restore 414

        #endregion Inspector

        protected virtual void CheckName()
        {
            m_AssetName = AssetName;

            if (!DoNameCheck) { return; }

            if (name != AssetName)
            {
                Debug.LogError($"Don't change singleton resource file name : {name} / {AssetName}", this);
            }
        }

        protected virtual void OnValidate()
        {
            CheckName();
        }
    }
}
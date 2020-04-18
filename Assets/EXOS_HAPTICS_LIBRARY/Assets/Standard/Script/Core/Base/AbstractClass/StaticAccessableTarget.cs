using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public abstract class StaticAccessableTarget<T> : SingletonResource<StaticAccessableTarget<T>>
        where T : StaticAccessableScriptableObject<T>
    {
        [Header("StaticAccessableTarget")]
        [SerializeField]
        public T targetObject;

        [SerializeField, FormerlySerializedAs("AllowOverWrite")]
        public bool AllowOverWrite = true;

        public static T TargetObject
        {
            get { return Instance?.targetObject; }
        }
    }
}
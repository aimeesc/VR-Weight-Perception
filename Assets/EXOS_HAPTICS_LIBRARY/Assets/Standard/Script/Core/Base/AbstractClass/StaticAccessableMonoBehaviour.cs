using exiii.Unity.Rx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace exiii.Unity
{
    /// <summary>
    /// Superclass of accessable with static reference
    /// </summary>
    /// <typeparam name="T">Specify the class to inherit from</typeparam>s
    public abstract class StaticAccessableMonoBehaviour<T> : ExMonoBehaviour where T : StaticAccessableMonoBehaviour<T>
    {
        #region Static
        private static Subject<T> s_ReferenceAdded = new Subject<T>();

        public static IObservable<T> OnReferenceAdded() => s_ReferenceAdded;

        private static Subject<T> s_ReferenceRemoved = new Subject<T>();

        public static IObservable<T> OnReferenceRemoved() => s_ReferenceAdded;

        private static List<T> s_InstanceList = new List<T>();

        public static IReadOnlyCollection<T> Collection { get { return s_InstanceList; } }

        public static bool IsExist { get { return s_InstanceList.Count > 0; } }

        public static T Instance
        {
            get
            {
                if (IsExist)
                {
                    return s_InstanceList.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        public static bool TryGetInstance(out T resultInstance)
        {
            resultInstance = Instance;
            return IsExist;
        }

        public static bool TrySetInstance(T target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("if you want to dispose static reference, please use TryRemoveReference method");
            }

            if (IsExist && Instance == target)
            {
                return true;
            }

            if (IsExist && Instance != target)
            {
                TryRemoveInstance(target);
            }

            if (!s_InstanceList.Contains(target))
            {
                s_InstanceList.Add(target);
                s_ReferenceAdded.OnNext(target);
                return true;
            }

            return false;
        }

        public static bool TryRemoveInstance(T target)
        {
            if (s_InstanceList.Contains(target))
            {
                s_InstanceList.Remove(target);
                s_ReferenceRemoved.OnNext(target);
                return true;
            }

            return false;
        }

        #endregion Static

        protected override void Awake()
        {
            base.Awake();

            TrySetInstance(this as T);
        }

        public override void Initialize()
        {
            base.Initialize();

            TrySetInstance(this as T);
        }

        public override void Terminate()
        {
            base.Terminate();

            TryRemoveInstance(this as T);
        }
    }
}
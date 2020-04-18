using exiii.Extensions;
using exiii.Unity.Linq;
using exiii.Unity.Rx;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace exiii.Unity
{
    /// <summary>
    /// Root object in prefab builder
    /// </summary>
    [DisallowMultipleComponent]
    [SelectionBase]
    public class RootScript : ExMonoBehaviour, IRootScript
    {
        #region Inspector

        [Header(nameof(RootScript))]
        [SerializeField]
        private ExosPrefab[] m_ExosPrefabs = new ExosPrefab[0];

        #endregion Inspector

        public bool RootInjected { get; private set; }

        protected override void OnValidate()
        {
            base.OnValidate();
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            if (!RootInjected)
            {
                RootInjection();
            }

            base.Start();
        }

        #region IRootScript

        public virtual void BuildHierarchy()
        {
            var prefabs = m_ExosPrefabs
                .Concat(GetComponentsInChildren<IExosPrefab>())
                .Distinct();

            prefabs.Foreach(x => x.BuildHierarchy(this));
        }

        /// <summary>
        /// Implementation of IRootScript
        /// </summary>
        public virtual void RootInjection()
        {
            RootInjected = true;

            // find child component without root tree.
            var children = gameObject
                .GetComponentsInChildren<INodeScript>()
                .Where(x => x.gameObject.activeInHierarchy)
                .Where(x => x.gameObject.AncestorsAndSelf().OfComponent<RootScript>().First() == this);

            children.Where(x => !x.InjectFinished).Foreach(target => target.StartInjection(this));
            children.Where(x => !x.InjectFinished).Foreach(target => target.FinishInjection());
        }

        #endregion IRootScript

        #region Static

        /// <summary>
        /// Retrieve the root object of the specified type
        /// </summary>
        /// <typeparam name="TRoot">Type of target root object</typeparam>
        /// <param name="obj">Target object</param>
        /// <returns>Retrieved root objectof the specified type</returns>
        public static TRoot GetRootObject<TRoot>(GameObject obj) where TRoot : IRootScript
        {
            if (obj == null) { throw new ArgumentNullException(); }

            var exosObject = obj.AncestorsAndSelf().Where(x => x.GetComponent<TRoot>() != null);

            if (!exosObject.Any()) { return default(TRoot); }

            return exosObject.First().GetComponent<TRoot>();
        }

        /// <summary>
        /// Retrieve the root object of the specified type
        /// </summary>
        /// <typeparam name="TRoot">Type of target root object</typeparam>
        /// <param name="component">Target component</param>
        /// <returns>Retrieved root objectof the specified type</returns>
        public static TRoot GetRootObject<TRoot>(Component component) where TRoot : IRootScript
        {
            if (component == null) { throw new ArgumentNullException(); }

            return GetRootObject<TRoot>(component.gameObject);
        }

        /// <summary>
        /// Retrieve the root object
        /// </summary>
        /// <param name="obj">Target object</param>
        /// <returns>>Retrieved root object</returns>
        public static RootScript GetRootObject(GameObject obj)
        {
            if (obj == null) { throw new ArgumentNullException(); }

            return GetRootObject<RootScript>(obj);
        }

        /// <summary>
        /// Retrieve the root object
        /// </summary>
        /// <param name="component">Target component</param>
        /// <returns>Retrieved root object of the specified type</returns>
        public static RootScript GetRootObject(Component component)
        {
            if (component == null) { throw new ArgumentNullException(); }

            return GetRootObject<RootScript>(component.gameObject);
        }

        // gameObject
        /// <summary>
        /// Retrieve the root object as GameObject
        /// </summary>
        /// <param name="obj">Target object</param>
        /// <returns>Retrieved root object as GameObject</returns>
        public static GameObject GetRootGameObject(GameObject obj)
        {
            if (obj == null) { throw new ArgumentNullException(); }

            var result = GetRootObject<IRootScript>(obj);

            return result?.gameObject;
        }

        /// <summary>
        /// Retrieve the root object as GameObject
        /// </summary>
        /// <param name="component">Target object</param>
        /// <returns>Retrieved root object as GameObject</returns>
        public static GameObject GetRootGameObject(Component component)
        {
            if (component == null) { throw new ArgumentNullException(); }

            return GetRootGameObject(component.gameObject);
        }

        // gameObject.GetComponent
        /// <summary>
        /// Retrieve the component attached to root object
        /// </summary>
        /// <typeparam name="T">Target compornent type</typeparam>
        /// <param name="obj">Target object</param>
        /// <returns>Retrieved compornent</returns>
        public static T GetRootComponent<T>(GameObject obj)
        {
            if (obj == null) { throw new ArgumentNullException(); }

            var result = GetRootObject<IRootScript>(obj);

            return (result != null) ? result.gameObject.GetComponent<T>() : default(T);
        }

        /// <summary>
        /// Retrieve the component attached to root object
        /// </summary>
        /// <typeparam name="T">Target compornent type</typeparam>
        /// <param name="component">Target compornent</param>
        /// <returns>Retrieved compornent</returns>
        public static T GetRootComponent<T>(Component component)
        {
            if (component == null) { throw new ArgumentNullException(); }

            return GetRootComponent<T>(component.gameObject);
        }

        // gameObject.GetComponents
        /// <summary>
        /// Retrieve the component attached to root object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T[] GetRootComponents<T>(GameObject obj)
        {
            if (obj == null) { throw new ArgumentNullException(); }

            var result = GetRootObject<IRootScript>(obj);

            return result?.gameObject.GetComponents<T>();
        }

        /// <summary>
        /// Retrieve the component attached to root object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T[] GetRootComponents<T>(Component component)
        {
            if (component == null) { throw new ArgumentNullException(); }

            return GetRootComponents<T>(component.gameObject);
        }

        // gameObject.GetComponentInChildren
        /// <summary>
        /// Retrieve the component attached to root object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetRootComponentInChildren<T>(GameObject obj)
        {
            if (obj == null) { throw new ArgumentNullException(); }

            var result = GetRootObject<IRootScript>(obj);

            return (result != null) ? result.gameObject.GetComponentInChildren<T>() : default(T);
        }

        /// <summary>
        /// Retrieve the component attached to root object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T GetRootComponentInChildren<T>(Component component)
        {
            if (component == null) { throw new ArgumentNullException(); }

            return GetRootComponentInChildren<T>(component.gameObject);
        }

        // gameObject.GetComponentsInChildren
        /// <summary>
        /// Retrieve the component attached to root object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T[] GetRootComponentsInChildren<T>(GameObject obj)
        {
            if (obj == null) { throw new ArgumentNullException(); }

            var result = GetRootObject<IRootScript>(obj);

            return result?.gameObject.GetComponentsInChildren<T>();
        }

        /// <summary>
        /// Retrieve the component attached to root object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T[] GetRootComponentsInChildren<T>(Component component)
        {
            if (component == null) { throw new ArgumentNullException(); }

            return GetRootComponentsInChildren<T>(component);
        }

        //
        /// <summary>
        /// Retrieve the component attached to root object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool RootComponentIsExist<T>(GameObject obj)
        {
            if (obj == null) { throw new ArgumentNullException(); }

            var result = GetRootObject<IRootScript>(obj);

            return (result != null) ? result.gameObject.GetComponentInChildren<T>() != null : false;
        }

        /// <summary>
        /// Retrieve the component attached to root object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool RootComponentIsExist<T>(Component component)
        {
            if (component == null) { throw new ArgumentNullException(); }

            return RootComponentIsExist<T>(component.gameObject);
        }

        #endregion Static
    }
}
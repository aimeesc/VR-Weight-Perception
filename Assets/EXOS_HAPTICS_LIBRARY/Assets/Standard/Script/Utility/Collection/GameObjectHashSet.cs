using exiii.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// GameObject Hash
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GameObjectHashSet<T> : ICollection<T> where T : IHasGameObject
    {
        private Dictionary<GameObject, T> data = new Dictionary<GameObject, T>();

        /// <summary>
        /// Gets or sets the value with the key.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T this[GameObject obj]
        {
            get { return data[obj]; }
            set { data[obj] = value; }
        }

        /// <summary>
        /// Gets or sets the value with the key.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T this[IHasGameObject obj]
        {
            get { return data[obj.gameObject]; }
            set { data[obj.gameObject] = value; }
        }

        /// <summary>
        /// Gets the number of pairs contained in the GameObjectHashSet.
        /// </summary>
        public int Count { get { return data.Count; } }

        /// <summary>
        /// It check readonly.
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// It's Add the pair.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            data.Add(item.gameObject, item);
        }

        /// <summary>
        /// It's all cleared the hash in the pairs.
        /// </summary>
        public void Clear()
        {
            data.Clear();
        }

        /// <summary>
        /// It's the check the key in the hash.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            // HACK:
            if (item == null || item.gameObject == null) { return false; }

            return data.ContainsKey(item.gameObject);
        }

        /// <summary>
        /// It's check the key in the hash.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Contains(IHasGameObject obj)
        {
            // HACK:
            if (obj == null || obj.gameObject == null) { return false; }

            return data.ContainsKey(obj.gameObject);
        }

        /// <summary>
        /// It's check the key in the hash.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Contains(GameObject obj)
        {
            // HACK:
            if (obj == null) { return false; }

            return data.ContainsKey(obj);
        }

        /// <summary>
        /// It's the remove pair by the key.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            // HACK:
            if (item == null || item.gameObject == null) { return false; }

            return data.Remove(item.gameObject);
        }

        /// <summary>
        /// It's the remove pair by IHasGameObject
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(IHasGameObject item)
        {
            // HACK:
            if (item == null || item.gameObject == null) { return false; }

            return data.Remove(item.gameObject);
        }

        /// <summary>
        /// It's the remove pair by GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Remove(GameObject obj)
        {
            // HACK:
            if (obj == null) { return false; }

            return data.Remove(obj);
        }

        /// <summary>
        /// It's copy the array to the hash.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(T[] array, int index)
        {
            var GameObjects = array.CheckNull().Select(x => x.gameObject);

            data.Keys.CopyTo(GameObjects.ToArray(), index);
            data.Values.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the key Enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GameObject> GetGameObjects()
        {
            return data.Keys;
        }

        /// <summary>
        /// Gets the value Enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
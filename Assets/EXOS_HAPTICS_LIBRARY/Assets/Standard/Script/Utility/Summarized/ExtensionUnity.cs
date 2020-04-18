using System;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public static class ExtensionUnity
    {
        // UnityEngine.Object
        public static TObject Copy<TObject>(this TObject obj) where TObject : UnityEngine.Object
        {
            if (obj == null) { return null; }

            TObject copy = UnityEngine.Object.Instantiate(obj);
            copy.name = obj.name;

            return copy;
        }

        // UnityEngine.Transform
        public static TComponent GetOrAddComponent<TComponent>(this Transform transform) where TComponent : UnityEngine.Component
        {
            if (transform == null) { return null; }

            var compornent = transform.GetComponent<TComponent>();

            if (compornent == null) { compornent = transform.gameObject.AddComponent<TComponent>(); }

            return compornent;
        }

        // UnityEngine.GameObject
        public static TComponent GetOrAddComponent<TComponent>(this GameObject obj) where TComponent : UnityEngine.Component
        {
            if (obj == null) { return null; }

            var compornent = obj.GetComponent<TComponent>();

            if (compornent == null) { compornent = obj.AddComponent<TComponent>(); }

            return compornent;
        }

        // UnityEngine.Component
        public static TComponent GetOrAddComponent<TComponent>(this Component component) where TComponent : UnityEngine.Component
        {
            if (component == null) { return null; }

            var compornent = component.GetComponent<TComponent>();

            if (compornent == null) { compornent = component.gameObject.AddComponent<TComponent>(); }

            return compornent;
        }

        //string
        public static string ColorTag(this string str, Color color)
        {
            if (str == null) { throw new ArgumentNullException(); }

            return $"<color={ColorToHex(color)}>{str}</color>";
        }

        private const int ColorDepth = 255;

        private static string ColorToHex(Color color)
        {
            return $"#{ValueToHex(color.r)}{ValueToHex(color.g)}{ValueToHex(color.b)}";
        }

        private static string ValueToHex(float colorValue)
        {
            return Mathf.RoundToInt(colorValue * ColorDepth).ToString("X2");
        }

        public static string SizeTag(this string str, int size)
        {
            if (str == null) { throw new ArgumentNullException(); }

            return $"<size={size}>{str}</size>";
        }

        public static string BoldTag(this string str)
        {
            if (str == null) { throw new ArgumentNullException(); }

            return $"<b>{str}</b>";
        }

        public static string ItalyTag(this string str)
        {
            if (str == null) { throw new ArgumentNullException(); }

            return $"<i>{str}</i>";
        }

        // Transform
        public static bool MoveTo(this Transform transform, Transform target)
        {
            if (transform == null || target == null) { return false; }

            transform.rotation = target.rotation;
            transform.position = target.position;

            return true;
        }

        public static bool CopyTo(this Transform transform, Transform target)
        {
            if (transform == null || target == null) { return false; }

            transform.localScale = target.localScale;
            transform.rotation = target.rotation;
            transform.position = target.position;

            return true;
        }

        public static void ResetLocalState(this Transform transform)
        {
            if (transform == null) return;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        // Vector3
        public static Vector3 Limit(this Vector3 vector, float limit)
        {
            if (vector.magnitude > limit)
            {
                vector = vector.normalized * limit;
            }

            return vector;
        }

        public static ICollection<T> FindObjectsOfInterface<T>() where T : class
        {
            var list = new List<T>();

            foreach (var n in GameObject.FindObjectsOfType<Component>())
            {
                var component = n as T;

                if (component != null)
                {
                    list.Add(component);
                }
            }

            return list;
        }

        //Color
        public static Color Alpha(this Color color, float a)
        {
            color.a = a;
            return color;
        }
    }
}
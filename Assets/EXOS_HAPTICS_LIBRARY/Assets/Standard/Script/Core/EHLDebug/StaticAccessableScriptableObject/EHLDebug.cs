using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public enum ELogLevel
    {
        All = 0,
        Detailed = 10,
        Overview = 20,
        None = 255,
    }

    public struct DebugSphere
    {
        public Vector3 position;
        public float size;
        public Color color;

        public DebugSphere(Vector3 position, float size, Color color)
        {
            this.position = position;
            this.size = size;
            this.color = color;
        }
    }

    /// <summary>
    /// Expantion of debug output tools
    /// </summary>
    public abstract class EHLDebug : StaticAccessableScriptableObject<EHLDebug>
    {
        #region Status

        public static ELogLevel LogLevel
        {
            get { return Instance.m_LogLevel; }
            set { Instance.m_LogLevel = value; }
        }

        public static bool DebugDrawGL
        {
            get { return Instance.m_DebugDrawGL && UnityEngine.Debug.isDebugBuild; }
        }

        public static bool DebugInspector
        {
            get { return Instance.m_DebugInspector && UnityEngine.Debug.isDebugBuild; }
        }

        #endregion Status

        #region Log

        /// <summary>
        /// Extended Log output. Output can be turned off.
        /// </summary>
        /// <param name="str">Log strings</param>
        /// <param name="context">Context object</param>
        /// <param name="type">kind of log</param>
        /// <param name="allowed">level of log</param>
        public static void Log(string str, UnityEngine.Object context, string type = "", ELogLevel allowed = ELogLevel.Detailed)
        {
            if (!IsExist || !Instance.m_LogEnable || allowed < LogLevel) { return; }

            var settings = GetLogSetting(type);

            if (settings.Enabled) { Instance.InstanceLog(GetTag(settings) + str, context); }
        }

        /// <summary>
        /// Extended Log output. Output can be turned off.
        /// </summary>
        /// <param name="str">Log strings</param>
        /// <param name="context">Context object</param>
        /// <param name="type">kind of log</param>
        /// <param name="allowed">level of log</param>
        public static void LogWarning(string str, UnityEngine.Object context, string type = "", ELogLevel allowed = ELogLevel.Detailed)
        {
            if (!IsExist || !Instance.m_LogEnable || allowed < LogLevel) { return; }

            var settings = GetLogSetting(type);

            if (settings.Enabled) { Instance.InstanceLogWarning(GetTag(settings) + str, context); }
        }

        /// <summary>
        /// Extended Log output. Output can be turned off.
        /// </summary>
        /// <param name="str">Log strings</param>
        /// <param name="context">Context object</param>
        /// <param name="type">kind of log</param>
        /// <param name="allowed">level of log</param>
        public static void LogError(string str, UnityEngine.Object context, string type = "", ELogLevel allowed = ELogLevel.Detailed)
        {
            if (!IsExist || !Instance.m_LogEnable || allowed < LogLevel) { return; }

            var settings = GetLogSetting(type);

            if (settings.Enabled) { Instance.InstanceLogError(GetTag(settings) + str, context); }
        }

        #endregion Log

        #region Draw

        private static List<DebugSphere> s_DebugSpheres = new List<DebugSphere>();

        private static IDisposable s_DebugSphere = null;

        [Conditional("DEBUG")]
        public static void AddDebugSphere(Vector3 position, float size, Color color)
        {
            var sphere = new DebugSphere(position, size, color);
            s_DebugSpheres.Add(sphere);
        }

        [Conditional("DEBUG")]
        public static void StartDebugSphere(GameObject obj)
        {
            if (s_DebugSphere != null) { return; }

            s_DebugSphere = obj.UpdateAsObservable().Where(_ => IsExist)
                .Subscribe(_ => s_DebugSpheres.ForEach(x => DrawSphere(x.position, x.size, x.color)))
                .AddTo(obj);
        }

        /// <summary>
        /// Draw a sphere to display
        /// </summary>
        /// <param name="sphere">The sphere you want to draw</param>
        /// <param name="color">The color</param>
        [Conditional("DEBUG")]
        public static void DrawSphere(ISphere sphere, Color color)
        {
            DrawSphere(sphere.Center, sphere.Radius * 2, color);
        }

        /// <summary>
        /// Draw a sphere to display
        /// </summary>
        /// <param name="position">Center position of the sphere</param>
        /// <param name="size">The sphere diameter</param>
        /// <param name="color">The sphere color</param>
        [Conditional("DEBUG")]
        public static void DrawSphere(Vector3 position, float size, Color color)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, size * Vector3.one);

            MaterialPropertyBlock property = new MaterialPropertyBlock();
            property.SetColor("_Color", color);

            Graphics.DrawMesh(Instance.m_Sphere, matrix, Instance.m_Material, 0, null, 0, property);
        }

        /// <summary>
        /// Draw a sphere to display
        /// </summary>
        /// <param name="position">Center position of the sphere</param>
        /// <param name="size">The sphere diameter</param>
        /// <param name="material">The sphere material</param>
        [Conditional("DEBUG")]
        public static void DrawSphere(Vector3 position, float size, Material material)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, size * Vector3.one);

            Graphics.DrawMesh(Instance.m_Sphere, matrix, material, 0);
        }

        /// <summary>
        /// Draw a cube to display
        /// </summary>
        /// <param name="position">Center position of the cube</param>
        /// <param name="rotation">Rotation of the cube</param>
        /// <param name="size">The cube size</param>
        /// <param name="color">the cube color</param>
        [Conditional("DEBUG")]
        public static void DrawCube(Vector3 position, Quaternion rotation, float size, Color color)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, size * Vector3.one);

            MaterialPropertyBlock property = new MaterialPropertyBlock();
            property.SetColor("_Color", color);

            Graphics.DrawMesh(Instance.m_Cube, matrix, Instance.m_Material, 0, null, 0, property);
        }

        [Conditional("DEBUG")]
        public static void DrawLine(Vector3 start, Vector3 end)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            UnityEngine.Debug.DrawLine(start, end);
        }

        [Conditional("DEBUG")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            UnityEngine.Debug.DrawLine(start, end, color);
        }

        [Conditional("DEBUG")]
        public static void DrawLine(OrientedSegment segment)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            UnityEngine.Debug.DrawLine(segment.InitialPoint, segment.TerminalPoint);
        }

        [Conditional("DEBUG")]
        public static void DrawLine(OrientedSegment segment, Color color)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            UnityEngine.Debug.DrawLine(segment.InitialPoint, segment.TerminalPoint, color);
        }

        [Conditional("DEBUG")]
        public static void DrawRay(OrientedSegment segment)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            UnityEngine.Debug.DrawRay(segment.InitialPoint, segment.VectorNormalized);
        }

        [Conditional("DEBUG")]
        public static void DrawRay(OrientedSegment segment, Color color)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            UnityEngine.Debug.DrawRay(segment.InitialPoint, segment.VectorNormalized, color);
        }

        [Conditional("DEBUG")]
        public static void DrawRay(Ray ray)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            UnityEngine.Debug.DrawRay(ray.origin, ray.direction);
        }

        [Conditional("DEBUG")]
        public static void DrawRay(Ray ray, Color color)
        {
            if (!IsExist || !Instance.m_DebugDrawEnable) { return; }

            UnityEngine.Debug.DrawRay(ray.origin, ray.direction, color);
        }

        #endregion Draw

        #region Private

        private static ExosLogSetting GetLogSetting(string type)
        {
            if (!IsExist)
            {
                UnityEngine.Debug.LogWarning("ExosDebug instance not found");
                return null;
            }

            if (type == "") { return Instance.m_DefaultSetting; }

            ExosLogSetting settings = Instance.m_LogSettings.Where(x => x.Type == type).FirstOrDefault();

            if (settings == null)
            {
                UnityEngine.Debug.LogWarning(type + " settings not found, use Default settings");
                settings = Instance.m_DefaultSetting;
            }

            return settings;
        }

        private static string GetTag(ExosLogSetting settings)
        {
            if (!IsExist)
            {
                UnityEngine.Debug.LogWarning("ExosDebug instance not found");
                return "";
            }

            return Instance.InstanceGetTag(settings);
        }

        #endregion Private

        #region Instance

        #region Inspector

        [SerializeField]
        private ELogLevel m_LogLevel;

        [Header("LogSettings")]
        [SerializeField]
        [FormerlySerializedAs("LogEnable")]
        private bool m_LogEnable;

        [SerializeField]
        [FormerlySerializedAs("DefaultSetting")]
        private ExosLogSetting m_DefaultSetting;

        [SerializeField]
        [FormerlySerializedAs("LogSettings")]
        private ExosLogSetting[] m_LogSettings;

        [Header("DebugDraw")]
        [SerializeField]
        [FormerlySerializedAs("DebugDrawEnable")]
        private bool m_DebugDrawEnable;

        [SerializeField]
        [FormerlySerializedAs("material")]
        private Material m_Material;

        [SerializeField]
        [FormerlySerializedAs("sphere")]
        private Mesh m_Sphere;

        [SerializeField]
        [FormerlySerializedAs("cube")]
        private Mesh m_Cube;

        [Header("DebugDrawGL")]
        [SerializeField]
        private bool m_DebugDrawGL;

        [Header("DebugInspector")]
        [SerializeField]
        private bool m_DebugInspector;

        #endregion Inspector

        //abstract method
        protected abstract void InstanceLog(string str, UnityEngine.Object context);

        protected abstract void InstanceLogWarning(string str, UnityEngine.Object context);

        protected abstract void InstanceLogError(string str, UnityEngine.Object context);

        protected abstract string InstanceGetTag(ExosLogSetting settings);

        #endregion Instance

        [Serializable]
        public class ExosLogSetting
        {
            public const string DefaultLogType = "Default";

            [SerializeField]
            private string type = DefaultLogType;

            public string Type { get { return type; } }

            [SerializeField]
            private bool enabled = true;

            public bool Enabled { get { return enabled; } }

            [SerializeField]
            private Color logColor = Color.white;

            public Color LogColor { get { return logColor; } }
        }
    }
}
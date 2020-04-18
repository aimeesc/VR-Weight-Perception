using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using exiii.Unity.Rx;
using UnityEngine.Events;
using exiii.Extensions;

namespace exiii.Unity
{
	public class LineDrawerGL : StaticAccessableMonoBehaviour<LineDrawerGL>
	{
        #region Static

        private static List<GLLineData> s_Lines = new List<GLLineData>();
        private static List<GLStrokeData> s_Strokes = new List<GLStrokeData>();

        static LineDrawerGL()
        {
            Setup();
        }

        private static void Setup()
        {
            if (!IsExist) { return; }

            if (Instance.LineMaterial == null)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");

                Instance.LineMaterial = new Material(shader);

                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Instance.LineMaterial.hideFlags = HideFlags.HideAndDontSave;

                // Turn on alpha blending
                Instance.LineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                Instance.LineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

                // Turn backface culling off
                Instance.LineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

                // Turn off depth writes
                Instance.LineMaterial.SetInt("_ZWrite", 0);
            }
        }

        private static void RenderLine(Transform transform)
        {
            if (!IsExist) { return; }

            if (Instance.LineMaterial == null) { return; }

            Instance.LineMaterial.SetPass(0);

            GL.PushMatrix();

            // Set transformation matrix for drawing to
            // match our transform
            //GL.MultMatrix(transform.localToWorldMatrix);

            // Draw lines
            GL.Begin(GL.LINES);

            foreach (var line in s_Lines)
            {
                GL.Color(line.Color);
                GL.Vertex3(line.Start.x, line.Start.y, line.Start.z);
                GL.Vertex3(line.End.x, line.End.y, line.End.z);
            }

            GL.End();
            GL.PopMatrix();
        }

        private static void RenderOneStroke(Transform transform)
        {
            if (!IsExist) { return; }

            if (Instance.LineMaterial == null) { return; }

            Instance.LineMaterial.SetPass(0);

            GL.PushMatrix();

            // Set transformation matrix for drawing to
            // match our transform
            //GL.MultMatrix(transform.localToWorldMatrix);

            // Draw lines
            GL.Begin(GL.LINES);

            foreach (var strokeData in s_Strokes)
            {
                bool first = true;
                Vector3 preVector = default(Vector3);

                for (int i = 0; i < strokeData.Stroke.Length; i++ )
                {
                    var stroke = strokeData.Stroke[i];

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        GL.Color(strokeData.CalcColor(i));

                        GL.Vertex3(preVector.x, preVector.y, preVector.z);
                        GL.Vertex3(stroke.x, stroke.y, stroke.z);
                    }

                    preVector = stroke;
                }

                if (strokeData.Close && strokeData.Stroke.Count() > 1)
                {
                    var lastVector = strokeData.Stroke.Last();
                    var firstVector = strokeData.Stroke.First();

                    GL.Vertex3(lastVector.x, lastVector.y, lastVector.z);
                    GL.Vertex3(firstVector.x, firstVector.y, firstVector.z);
                }
            }

            GL.End();
            GL.PopMatrix();
        }

        private static void Clear()
        {
            s_Lines.Clear();
            s_Strokes.Clear();
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            if (!IsExist) { return; }

            s_Lines.Add(new GLLineData(start, end, color));
        }

        public static void DrawLine(Vector3 start, Vector3 end)
        {
            if (!IsExist) { return; }

            s_Lines.Add(new GLLineData(start, end, Instance.DefaultColor));
        }

        public static void DrawLine(OrientedSegment segment, Color color)
        {
            if (!IsExist) { return; }

            s_Lines.Add(new GLLineData(segment.InitialPoint, segment.TerminalPoint, color));
        }

        public static void DrawLine(OrientedSegment segment)
        {
            if (!IsExist) { return; }

            s_Lines.Add(new GLLineData(segment.InitialPoint, segment.TerminalPoint, Instance.DefaultColor));
        }

        public static void DrawLines(IEnumerable<OrientedSegment> segments, Color color)
        {
            if (!IsExist) { return; }

            s_Lines.AddRange(segments.Select(x => new GLLineData(x.InitialPoint, x.TerminalPoint, color)));
        }

        public static void DrawLines(IEnumerable<OrientedSegment> segments, Color start, Color end)
        {
            if (!IsExist) { return; }

            var count = segments.Count();

            for (int i = 0; i < count; i++ )
            {
                s_Lines.Add(new GLLineData(segments.ElementAt(i), Color.Lerp(start, end, (float)i / (float)count)));
            }
        }

        public static void DrawLines(IEnumerable<OrientedSegment> segments)
        {
            if (!IsExist) { return; }

            s_Lines.AddRange(segments.Select(x => new GLLineData(x.InitialPoint, x.TerminalPoint, Instance.DefaultColor)));
        }

        public static void DrawOneStroke(IEnumerable<Vector3> strokes, Color color, bool close = false)
        {
            if (!IsExist) { return; }

            s_Strokes.Add(new GLStrokeData(strokes, color, close));
        }

        public static void DrawOneStroke(IEnumerable<Vector3> strokes, Color start, Color end,  bool close = false)
        {
            if (!IsExist) { return; }

            s_Strokes.Add(new GLStrokeData(strokes, start, end, close));
        }

        public static void DrawOneStroke(IEnumerable<Vector3> strokes, bool close = false)
        {
            if (!IsExist) { return; }

            s_Strokes.Add(new GLStrokeData(strokes, Instance.DefaultColor, close));
        }

        #endregion

        [Serializable]
        private struct GLLineData
        {
            [SerializeField]
            public Vector3 Start;

            [SerializeField]
            public Vector3 End;

            [SerializeField]
            public Color Color;

            public GLLineData(Vector3 start, Vector3 end, Color color)
            {
                Start = start;
                End = end;
                Color = color;
            }

            public GLLineData(OrientedSegment segment, Color color)
            {
                Start = segment.InitialPoint;
                End = segment.TerminalPoint;
                Color = color;
            }
        }

        [Serializable]
        private struct GLStrokeData
        {
            [SerializeField]
            public Vector3[] Stroke;

            [SerializeField]
            public Color StartColor;

            [SerializeField]
            public Color EndColor;

            [SerializeField]
            public bool Close;

            public GLStrokeData(IEnumerable<Vector3> stroke, Color color, bool close = false)
            {
                Stroke = stroke.ToArray();
                StartColor = color;
                EndColor = color;
                Close = close;
            }

            public GLStrokeData(IEnumerable<Vector3> stroke, Color start, Color end,  bool close = false)
            {
                Stroke = stroke.ToArray();
                StartColor = start;
                EndColor = end;
                Close = close;
            }

            public Color CalcColor(int index)
            {
                return Color.Lerp(StartColor, EndColor, (float)index / (float)Stroke.Length);
            }
        }

        #region MonoBehaviour
        
        [SerializeField]
        private Material m_LineMaterial;

        public Material LineMaterial
        {
            get { return m_LineMaterial; }
            protected set { m_LineMaterial = value; }
        }

        [SerializeField]
        private Color m_DefaultColor = Color.white;

        public Color DefaultColor { get { return m_DefaultColor; } }

        public UnityEvent CleanupEvent = new UnityEvent();

        private IDisposable m_CleanupTimer;

        protected override void Awake()
        {
            base.Awake();

            if (m_CleanupTimer == null)
            {
                m_CleanupTimer = Observable
                    .FromMicroCoroutine<Unit>(_ => UpdateCleanup(), FrameCountType.EndOfFrame)
                    .Subscribe(onNext => { }, onError => Debug.LogError(onError), () => Debug.LogWarning("[EXOS_SDK] CleanupTimer completed"));
            }

            Setup();
        }

        private void OnPostRender()
        {
            RenderLine(transform);
            RenderOneStroke(transform);
        }

        private IEnumerator UpdateCleanup()
        {
            while (IsActive)
            {
                yield return null;

                try
                {
                    Clear();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        #endregion
    }
}

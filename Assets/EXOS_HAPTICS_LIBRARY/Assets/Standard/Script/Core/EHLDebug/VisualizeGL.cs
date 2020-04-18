using exiii.Collections;
using exiii.Unity;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public class VisualizeGL : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        private int m_CountFrame = 30;

        #endregion

        public LimitedList<Vector3> ToolsPositions { get; } = new LimitedList<Vector3>();

        public LimitedList<OrientedSegment> Forces { get; } = new LimitedList<OrientedSegment>();

        public LimitedList<OrientedSegment> BreakePositions { get; } = new LimitedList<OrientedSegment>();

        private Material m_FlushMaterial;

        private void Start()
        {
            ToolsPositions.MaxItemCount = m_CountFrame;
            Forces.MaxItemCount = m_CountFrame;

            BreakePositions.MaxItemCount = 1;

            m_FlushMaterial = EHLMaterialHolder.InstantiateFlushMaterial();

            EHLMaterialHolder.StartFlush();
        }

        private void Update()
        {
            LineDrawerGL.DrawOneStroke(ToolsPositions, Color.blue, Color.red);
            LineDrawerGL.DrawLines(Forces, Color.blue, Color.red);

            BreakePositions.ForEach(x => EHLDebug.DrawSphere(x.InitialPoint, 0.01f, m_FlushMaterial));
            BreakePositions.ForEach(x => EHLDebug.DrawSphere(x.TerminalPoint, 0.01f, m_FlushMaterial));
        }
    }
}
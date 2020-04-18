using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public enum CollectionType
    {
        SumAndMax,
        Average,
        AverageInActive,
        SumAndMaxClamp,
    }

    public class TouchManipulation : ImmidiateManipulation<ITouchManipulation>, ITouchManipulation
    {
        private ITouchManipulator m_Manipulator;

        private ShapeStateSet m_ShapeStateSet;
        private SurfaceStateSet m_SurfaceStateSet;
        
        private bool m_TouchStateUpdated = false;
        private bool m_SurfaceStateUpdated = false;

        public CollectionType CollectionType { get; set; } = CollectionType.SumAndMax;

        public TouchManipulation(IInteractorRoot controller, ITouchManipulator manipulator, IManipulable<ITouchManipulation> manipulable)
            : base(controller, manipulator, manipulable)
        {
            m_Manipulator = manipulator;

            m_ShapeStateSet = new ShapeStateSet(m_Manipulator);
            m_SurfaceStateSet = new SurfaceStateSet(m_Manipulator);

            if (EHLDebug.DebugDrawGL)
            {
                controller.gameObject
                    .UpdateAsObservable()
                    .Subscribe(_ => SetupDebugDraw())
                    .AddTo(this.Disposer);
            }
        }

        private void SetupDebugDraw()
        {
            m_ShapeStateSet.Collection.Foreach(x => LineDrawerGL.DrawLine(x.ClosestSegment, Color.yellow));            

            LineDrawerGL.DrawLine(m_ShapeStateSet.SummarizedOutput, Color.magenta);
        }

        #region Override

        public override ITouchManipulation Manipulation => this;

        public override void ResetManipulation()
        {
            m_TouchStateUpdated = false;
            m_SurfaceStateUpdated = false;
        }

        #endregion

        #region ITouchManipulation

        public IShapeStateSet ShapeStateSet => m_ShapeStateSet;
        public ISurfaceStateSet SurfaceStateSet => m_SurfaceStateSet;

        public bool PenetratorIsDefault => m_Manipulator.PenetratorIsDefault;

        public bool TryUpdateTouchState(IShapeContainer shape)
        {
            if (m_TouchStateUpdated) { return m_ShapeStateSet.Enabled; }

            m_TouchStateUpdated = true;

            if (shape == null) { return false; }

            m_ShapeStateSet.Clear();

            CalcPenetrationOnPenetrator(shape);

            CalcPenetrationOnPenetrable(shape);

            m_ShapeStateSet.UpdateSummarizedOutput(SummarizeShapeVector());

            return true;
        }

        private void CalcPenetrationOnPenetrator(IPenetrable penetrable)
        {
            var penetrators = m_Manipulator.PenetratorContainer.Penetrators
                .Where(x => x != null)
                .Where(x => x.PenetratorType == EPenetratorType.CalclateOnPenetrator);

            foreach (var penetrator in penetrators)
            {
                OrientedSegment penetration;

                if (penetrator.TryCalcPenetration(penetrable, out penetration))
                {
                    m_ShapeStateSet.Add(new ShapeState(m_Manipulator, penetration));
                }
            }
        }

        private void CalcPenetrationOnPenetrable(IShapeContainer shape)
        {
            if (!shape.HasShapeData) { return; }

            var penetrators = m_Manipulator.PenetratorContainer.Penetrators
                .Where(x => x != null)
                .Where(x => x.PenetratorType == EPenetratorType.CalclateOnPenetrable);

            if (penetrators.Count() == 0) { return; }

            shape.CalcShapeStateSet(penetrators, m_ShapeStateSet);
        }

        private OrientedSegment SummarizeShapeVector()
        {
            var segments = m_ShapeStateSet.Collection.Select(x => x.ClosestSegment);

            OrientedSegment output = OrientedSegment.zero;

            if (segments.Count() == 0 || segments.Where(holder => holder.HasLength).Count() == 0)
            {
                output.InitialPoint = ExtensionVector.Average(segments.Select(holder => holder.InitialPoint));
                output.TerminalPoint = output.InitialPoint;
            }
            else
            {
                output.InitialPoint = OrientedSegment.WeightedAverageOfInitial(segments);

                switch (CollectionType)
                {
                    case CollectionType.SumAndMax:
                        output.TerminalPoint = output.InitialPoint + ExtensionVector.Sum(segments.Select(x => x.Vector));
                        output.Length = segments.Max(x => x.Length);
                        break;

                    case CollectionType.Average:
                        output.TerminalPoint = output.InitialPoint + ExtensionVector.Average(segments.Select(x => x.Vector));
                        break;

                    case CollectionType.AverageInActive:
                        output.TerminalPoint = output.InitialPoint + ExtensionVector.Average(segments.Select(x => x.Vector));
                        break;

                    case CollectionType.SumAndMaxClamp:
                        output.TerminalPoint = output.InitialPoint + ExtensionVector.Sum(segments.Select(x => x.Vector));
                        var max = segments.Max(x => x.Length);
                        if (output.Length > max) { output.Length = max; }
                        break;
                }
            }

            return output;
        }

        public bool TryUpdateSurfaceState(ISurfaceContainer surface)
        {
            if (m_SurfaceStateUpdated) { return m_SurfaceStateSet.Enabled; }

            m_SurfaceStateUpdated = true;

            if (surface == null) { return false; }

            if (!m_ShapeStateSet.Enabled) { return false; }

            m_SurfaceStateSet.Clear();

            surface.CalcSurfaceStateSet(m_ShapeStateSet, m_SurfaceStateSet, true);

            return true;
        }

        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using exiii.Extensions;
using exiii.Unity.Linq;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;

namespace exiii.Unity
{
    public class PenetratorBuilder : PenetratorContainerBase
    {
        #region Inspector

        [Header(nameof(PenetratorBuilder))]
        [SerializeField]
        private PenetratorBuildParameter m_DefaultBuildParameter;

        [SerializeField]
        private PenetratorParameter m_DefaultPenetratorParameter;

        #endregion Inspector

        private PenetratorBuildParameter m_PenetratorBuildParameter;

        private PenetratorParameter m_PenetratorParameter;

        private Dictionary<Transform, int> m_DefaultTarget = null;

        private PenetratorCollection m_PenetratorCollection;

        public override IReadOnlyCollection<IPenetrator> Penetrators
        {
            get { return m_PenetratorCollection.Penetrators; }
        }

        private bool BuildFinished
        {
            get { return m_PenetratorCollection != null; }
        }

        // on awake.
        protected override void Awake()
        {
            base.Awake();

            // init default target on awake.
            InitializeDefaultTarget();
        }

        public override void StartInjection(IRootScript root)
        {
            base.StartInjection(root);

            Build();
        }

        // build tools.
        public void Build()
        {
            if (BuildFinished) { return; }

            PenetratorBuildParameter buildParameter;

            if (EHLHand.TryGetPenetratorBuildParameter(out buildParameter))
            {
                m_PenetratorBuildParameter = buildParameter;
            }
            else
            {
                m_PenetratorBuildParameter = m_DefaultBuildParameter;
            }

            PenetratorParameter penetratorParameter;

            if (EHLHand.TryGetPenetratorParameter(out penetratorParameter))
            {
                m_PenetratorParameter = penetratorParameter;
            }
            else
            {
                m_PenetratorParameter = m_DefaultPenetratorParameter;
            }

            switch (m_PenetratorBuildParameter.PenetrationType)
            {
                case ETouchType.Tools:
                    m_PenetratorCollection = new PenetratorCollection<ToolsHolder>();
                    break;

                case ETouchType.Penalty:
                    m_PenetratorCollection = new PenetratorCollection<BonePenetratorHolder>();
                    break;
            }

            var targets = new Dictionary<Transform, PenetratorParameter>();

            foreach(var pair in m_DefaultTarget)
            {
                switch (m_PenetratorBuildParameter.BuildType)
                {
                    case EBuildType.All:
                        targets.Add(pair.Key, m_PenetratorParameter);
                        break;

                    case EBuildType.FromEnd:
                        if (pair.Value < m_PenetratorBuildParameter.BuildDepth)
                        {
                            targets.Add(pair.Key, m_PenetratorParameter);
                        }
                        break;

                    case EBuildType.FingerMeta:
                        var meta = pair.Key.GetComponent<FingerMeta>();

                        if (meta != null)
                        {
                            targets.Add(pair.Key, meta.PenetratorParameter);
                        }
                        break;
                }
            }

            var totalDensity = targets.Sum(x => x.Value.Density);

            var parameters = targets.Select(x => new AttachParameter(x.Key, CalcSize(x.Key, x.Value.Size), CalcMass(totalDensity, x.Value.Density), x.Value.Visible, x.Value.UseToPenetration));

            m_PenetratorCollection.AttachHolders(parameters);
        }

        // rebuild tools.
        public void Rebuild()
        {
            // destroy prev build.
            Destroy();

            // build next.
            Build();
        }

        // destroy tools.
        public void Destroy()
        {
            if (m_PenetratorCollection == null) { return; }

            m_PenetratorCollection.Clear();
            m_PenetratorCollection = null;
        }

        // initialize default target.
        private void InitializeDefaultTarget()
        {
            m_DefaultTarget = new Dictionary<Transform, int>();

            // save targets with depth.
            Transform[] targets = GetComponentsInChildren<Transform>();

            // prepare origin.
            List<Transform> origings = targets
                .Where(_ => _.childCount == 0)
                .ToList();

            // prepare default target.
            foreach (Transform origin in origings)
            {
                Transform current = origin;
                int depth = 0;
                while (current != this.transform)
                {
                    if (!m_DefaultTarget.ContainsKey(current))
                    {
                        // add new transform.
                        m_DefaultTarget.Add(current, depth);
                    }
                    else
                    {
                        // override with depth.
                        m_DefaultTarget[current] = Mathf.Max(depth, m_DefaultTarget[current]);
                    }

                    depth++;
                    current = current.parent;
                }
            }
        }

        private float CalcSize(Transform holder, float size)
        {
            switch (m_PenetratorBuildParameter.SizeType)
            {
                case ESizeType.Absolute:
                    return size;

                case ESizeType.LimitConflict:
                    return CalcLimitToolsSize(holder);

                case ESizeType.Max:
                    return CalcMaxToolsSize(holder);

                case ESizeType.FingerMeta:
                    return size;
            }

            return m_PenetratorParameter.Size;
        }

        // calc limit tools size.
        private float CalcLimitToolsSize(Transform holder)
        {
            // calc min length of eath transform.
            float minLength = m_DefaultTarget
                .Where(_ => _.Key != holder)
                .Min(_ => Vector3.Distance(_.Key.transform.position, holder.position));

            // ret min length.
            return Mathf.Min(m_PenetratorParameter.Size, minLength);
        }

        // calc max tools size.
        private float CalcMaxToolsSize(Transform holder)
        {
            // calc min length of eath transform.
            float minLength = m_DefaultTarget
                .Where(_ => _.Key != holder)
                .Min(_ => Vector3.Distance(_.Key.transform.position, holder.position));

            // ret max length.
            return minLength;
        }

        private float CalcMass(float totalDensity, float density)
        {
            if (totalDensity == 0 || density == 0) { return 0.001f; }

            return m_PenetratorBuildParameter.TotalMass * (density / totalDensity);
        }
    }
}

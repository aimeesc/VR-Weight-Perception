using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class PenetratorCollection
    {
        protected List<IPenetrator> ListPenetrator { get; set; }

        public IReadOnlyCollection<IPenetrator> Penetrators
        {
            get { return ListPenetrator; }
        }

        public abstract void AttachHolders(IEnumerable<AttachParameter> targets);

        public abstract void Clear();
    }

    public class PenetratorCollection<THolder> : PenetratorCollection
        where THolder : PenetratorHolder
    {
        #region Inspector

        [Header("PenetratorContainer")]
        [SerializeField, Unchangeable]
        private List<THolder> m_Holders = new List<THolder>();

        public IReadOnlyCollection<THolder> Holders
        {
            get { return m_Holders; }
        }

        #endregion Inspector

        // attach tools holder.
        public override void AttachHolders(IEnumerable<AttachParameter> targets)
        {
            foreach(var target in targets)
            {
                // attach tools.
                THolder holder = target.transform.GetOrAddComponent<THolder>();

                if (holder == null) { return; }

                holder.SetValues(target.Size, target.Mass);
                holder.SetVisible(target.Visible);

                // save ref.
                if (target.UseToPenetration) { m_Holders.Add(holder); }
            }

            ListPenetrator = m_Holders.Select(x => x.Penetrator).ToList();
        }

        public override void Clear()
        {
            // destroy tools holder components.
            m_Holders.ForEach(holder => GameObject.DestroyImmediate(holder));
            m_Holders.Clear();
        }
    }

    public struct AttachParameter
    {
        public Transform transform;

        public float Size;
        public float Mass;

        public bool Visible;
        public bool UseToPenetration;

        public AttachParameter(Transform target, float size, float mass, bool visible, bool use)
        {
            this.transform = target;

            this.Size = size;
            this.Mass = mass;

            this.Visible = visible;
            this.UseToPenetration = use;
        }
    }
}
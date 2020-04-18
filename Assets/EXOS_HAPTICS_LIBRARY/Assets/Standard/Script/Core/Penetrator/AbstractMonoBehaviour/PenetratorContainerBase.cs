using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class PenetratorContainerBase : InteractorNode, IPenetratorContainer
    {
        public abstract IReadOnlyCollection<IPenetrator> Penetrators { get; }
    }

    public abstract class PenetratorContainerBase<THolder> : PenetratorContainerBase
        where THolder : PenetratorHolder
    {
        #region Inspector

        [Header("PenetratorContainer")]
        [SerializeField, Unchangeable]
        private List<THolder> m_Holders;

        public IReadOnlyCollection<THolder> Holders
        {
            get { return m_Holders; }
        }

        #endregion Inspector

        protected List<IPenetrator> ListPenetrator { get; set; }

        public override IReadOnlyCollection<IPenetrator> Penetrators
        {
            get { return ListPenetrator; }
        }

        public override void Initialize()
        {
            base.Initialize();

            Build();
        }

        public void Build()
        {
            m_Holders = RootGameObject.GetComponentsInChildren<THolder>().ToList();

            ListPenetrator = m_Holders.Select(x => x.Penetrator).ToList();
        }
    }
}
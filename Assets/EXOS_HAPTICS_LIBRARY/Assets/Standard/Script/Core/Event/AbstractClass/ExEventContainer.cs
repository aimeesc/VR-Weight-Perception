using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using exiii.Extensions;
using System.Linq;

namespace exiii.Unity
{
	public abstract class ExEventContainer<TAction, TEvent> : ExMonoBehaviour, IEnumerable<TEvent>
        where TEvent : ExEventBase<TAction>
    {
        protected Dictionary<TAction, List<ExActionBase<TAction>>> Dictionaly { get; } = new Dictionary<TAction, List<ExActionBase<TAction>>>();

        protected override void OnValidate()
        {
            base.OnValidate();

            if (Events != null)
            {
                Events.Foreach(x => x.UpdateName());
            }
        }

        protected override void Start()
        {
            base.Start();

            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneLoaded += SceneLoaded;

            SetTarget();
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetTarget();
        }

        public void Emit(TAction action)
        {
            Events.Where(x => x.Action.Equals(action))
                .Where(x => Dictionaly.ContainsKey(x.Action))
                .Foreach(x => EventInvoke(Dictionaly[x.Action]));
        }

        protected void EventInvoke(IEnumerable<ExActionBase<TAction>> enumerable)
        {
            if (enumerable == null) { return; }

            enumerable.Foreach(x => x.Events.Invoke());
        }

        protected void SetDictionaly(IEnumerable<ExActionBase<TAction>> enumerable)
        {
            Dictionaly.Clear();

            foreach (var action in enumerable)
            {
                if (!Dictionaly.ContainsKey(action.Action))
                {
                    Dictionaly.Add(action.Action, new List<ExActionBase<TAction>>());
                }

                List<ExActionBase<TAction>> list;
                if (Dictionaly.TryGetValue(action.Action, out list))
                {
                    list.Add(action);
                }
            }
        }

        public abstract void SetTarget();

        #region IEnumerable

        protected abstract IEnumerable<TEvent> Events { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Events.GetEnumerator();
        }

        public IEnumerator<TEvent> GetEnumerator()
        {
            return Events.GetEnumerator();
        }

        #endregion IEnumerable
    }
}

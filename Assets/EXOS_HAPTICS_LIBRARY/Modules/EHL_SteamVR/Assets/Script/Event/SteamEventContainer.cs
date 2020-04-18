using exiii.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace exiii.Unity.SteamVR
{ 
    public abstract class SteamEventContainer<TAction> : ExMonoBehaviour, IEnumerable<SteamEventBase<TAction>>
    {
        protected abstract IEnumerable<SteamEventBase<TAction>> SteamEvents { get; }

        protected Dictionary<TAction, List<ExActionBase<TAction>>> Dictionaly { get; } = new Dictionary<TAction, List<ExActionBase<TAction>>>();

        protected override void OnValidate()
        {
            base.OnValidate();

            if (SteamEvents != null)
            {
                SteamEvents.Foreach(x => x.UpdateName());
            }
        }

        protected override void Start()
        {
            base.Start();

            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneLoaded += SceneLoaded;

            UpdateTarget();
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateTarget();
        }

        private void Update()
        {
            // update steam event action.
            UpdateSteamEventAction();
        }

        // update steam event action.
        protected abstract void UpdateSteamEventAction();      

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

        public abstract void UpdateTarget();

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SteamEvents.GetEnumerator();
        }

        public IEnumerator<SteamEventBase<TAction>> GetEnumerator()
        {
            return SteamEvents.GetEnumerator();
        }

        #endregion IEnumerable
    }
}

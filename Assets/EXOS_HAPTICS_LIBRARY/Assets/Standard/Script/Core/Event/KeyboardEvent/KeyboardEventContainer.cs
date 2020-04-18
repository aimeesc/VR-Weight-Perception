using exiii.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace exiii.Unity
{
    public abstract class KeyboardEventContainer<TAction> : ExEventContainer<TAction, KeyboardEventBase<TAction>>
    {
        private void Update()
        {
            Events
                .Where(x => x.KeyTiming == EKeyTiming.KeyDown && Input.GetKeyDown(x.KeyCode))
                .Where(x => Dictionaly.ContainsKey(x.Action))
                .Foreach(x => EventInvoke(Dictionaly[x.Action]));

            Events
                .Where(x => x.KeyTiming == EKeyTiming.Key && Input.GetKey(x.KeyCode))
                .Where(x => Dictionaly.ContainsKey(x.Action))
                .Foreach(x => EventInvoke(Dictionaly[x.Action]));

            Events
                .Where(x => x.KeyTiming == EKeyTiming.KeyUp && Input.GetKeyUp(x.KeyCode))
                .Where(x => Dictionaly.ContainsKey(x.Action))
                .Foreach(x => EventInvoke(Dictionaly[x.Action]));
        }
    }
}
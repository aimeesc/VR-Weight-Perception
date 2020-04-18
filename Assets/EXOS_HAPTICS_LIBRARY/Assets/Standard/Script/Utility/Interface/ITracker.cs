using System;
using UnityEngine;

namespace exiii.Unity
{
    public interface ITracker : IHasGameObject
    {
        IObservable<Transform> OnPositionUpdate();
    }
}
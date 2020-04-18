using exiii.Unity.Rx;
using System.Collections.Concurrent;
using UnityEngine;

namespace exiii.Unity
{
    internal static class ReceiverSubjectCache<TReceiver> where TReceiver : IReceiver
    {
        private static ConcurrentDictionary<object, Subject<TReceiver>> m_Dictionary = new ConcurrentDictionary<object, Subject<TReceiver>>();

        public static Subject<TReceiver> GetSubject(object obj)
        {
            return m_Dictionary.GetOrAdd(obj, (x) => new Subject<TReceiver>());
        }

        public static bool RemoveSubject(object obj)
        {
            Subject<TReceiver> subject;

            return m_Dictionary.TryRemove(obj, out subject);
        }
    }
}
using exiii.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// Interface for Exos Tag
    /// </summary>
    public interface IExTag
    {
        string ExTag { get; }
    }

    [Serializable]
    public class ExTag : IExTag
    {
        #region Static

        public static string MakeTag<TEnum>(TEnum exTag) where TEnum : struct
        {
            return string.Concat(exTag.GetType().Name, ".", exTag.EnumToString());
        }

        #endregion

        public ExTag(string exTag)
        {
            m_ExTag = exTag;
        }

        [SerializeField, Unchangeable]
        private string m_ExTag;

        string IExTag.ExTag { get { return m_ExTag; } }

        public override string ToString()
        {
            return m_ExTag;
        }
    }

    [Serializable]
    public class ExTag<TEnum> : ExTag where TEnum : struct
    {
        public ExTag(TEnum exTag) : base (MakeTag(exTag)) {  }
    }

    public static class ExTagExtensions
    {
        internal static bool CheckTag(this IExTag exTag, string tag)
        {
            if (exTag == null) { return false; }

            return exTag.ExTag.Equals(tag);
        }

        public static bool CheckTag(this IExTag exTag, IExTag tag)
        {
            if (tag == null) { return false; }

            return exTag.CheckTag(tag.ExTag);
        }

        public static bool CheckTag(this IExTag exTag, IEnumerable<IExTag> tags)
        {
            if (tags == null) { return false; }

            return tags.Any(tag => exTag.CheckTag(tag.ExTag));
        }
    }

    public class ExTagComparer : EqualityComparer<IExTag>
    {
        public static ExTagComparer Instance { get; } = new ExTagComparer();

        public override bool Equals(IExTag x, IExTag y)
        {
            return x.ExTag == y.ExTag;
        }

        public override int GetHashCode(IExTag obj)
        {
            return obj.GetHashCode();
        }
    }
}
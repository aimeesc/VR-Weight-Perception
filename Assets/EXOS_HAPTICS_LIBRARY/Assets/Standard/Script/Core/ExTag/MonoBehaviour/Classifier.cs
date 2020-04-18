using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// Interface for Exos Tag with MonoBehaviour
    /// </summary>
    public interface IClassifier : IExTag, IMonoBehaviour { }

    /// <summary>
    /// Exos Tag with MonoBehaviour made by script
    /// </summary>
    public abstract class ClassifierBase : MonoBehaviour, IClassifier
    {
        public abstract string ExTag { get; }
    }

    public class Classifier : ClassifierBase
    {
        #region Static

        private static IEnumerable<Classifier> FindExTagsToEnum(IExTag target)
        {
            return FindObjectsOfType<Classifier>().Where(tag => tag.CheckTag(target));
        }

        public static Classifier[] FindExTags(IExTag target)
        {
            return FindExTagsToEnum(target).ToArray();
        }

        public static GameObject[] FindExObject(IExTag target)
        {
            return FindExTagsToEnum(target).Select(x => x.gameObject).ToArray();
        }

        #endregion Static

        #region Inspector

        [Header("Classifier")]
        [SerializeField, Unchangeable]
        private string m_ExTag;

        #endregion Inspector

        public override string ExTag
        {
            get { return m_ExTag; }
        }

        public virtual void SetExTag(IExTag exTag)
        {
            m_ExTag = exTag.ExTag;
        }


    }

    /// <summary>
    /// Base class of Exos Tag with MonoBehaviour chosen by inspector
    /// </summary>
    public class ClassifierContainer : ClassifierBase
    {
        #region Inspector

        [SerializeField]
        private ExTagContainer m_ExTag;

        public ExTagContainer ExTagContainer
        {
            get { return m_ExTag; }
            set { m_ExTag = value; }
        }

        #endregion Inspector

        public override string ExTag
        {
            get { return m_ExTag.ExTag; }
        }
    }

    public static class ClassifierExtensions
    {
        #region GameObject

        // GetExTag
        internal static IEnumerable<IClassifier> GetExTag(this GameObject self, string target)
        {
            return self.GetComponentsInChildren<IClassifier>().Where(tag => tag.CheckTag(target));
        }

        // GetExTagInChildren
        public static IClassifier GetExTagInChildren(this GameObject self, IExTag target)
        {
            return self.GetExTag(target.ExTag).FirstOrDefault();
        }

        // GetExTagsInChildren
        public static IClassifier[] GetExTagsInChildren(this GameObject self, IExTag target)
        {
            return self.GetExTag(target.ExTag).ToArray();
        }

        // GetTagedGameObject
        private static IEnumerable<GameObject> GetTagedGameObject(this GameObject self, string target)
        {
            return self.GetExTag(target).Select(x => x.gameObject);
        }

        // GetTagedGameObjectInChildren
        public static GameObject GetTagedGameObjectInChildren(this GameObject self, IExTag target)
        {
            return self.GetTagedGameObject(target.ExTag).FirstOrDefault();
        }

        // GetTagedGameObjectsInChildren
        public static GameObject[] GetTagedGameObjectsInChildren(this GameObject self, IExTag target)
        {
            return self.GetTagedGameObject(target.ExTag).ToArray();
        }

        public static bool HasExTag(this GameObject obj, IExTag tag)
        {
            if (tag == null) { return false; }

            var tags = obj.GetComponents<IClassifier>();

            return tag.CheckTag(tags);
        }

        public static bool HasExTag(this GameObject obj, IEnumerable<IExTag> tags, EDetectionType detection = EDetectionType.Any)
        {
            if (tags == null) { return false; }

            switch (detection)
            {
                case EDetectionType.Any:
                    return tags.Any(x => obj.HasExTag(x));

                case EDetectionType.All:
                    return tags.All(x => obj.HasExTag(x));

                default:
                    return tags.Any(x => obj.HasExTag(x));
            }
        }

        #endregion GameObject

        #region Component

        // GetExTag
        internal static IEnumerable<IClassifier> GetExTag(this Component self, string target)
        {
            return self.GetComponentsInChildren<IClassifier>().Where(tag => tag.CheckTag(target));
        }

        // GetExTagInChildren
        public static IClassifier GetExTagInChildren(this Component self, IExTag target)
        {
            return self.GetExTag(target.ExTag).FirstOrDefault();
        }

        // GetExTagsInChildren
        public static IClassifier[] GetExTagsInChildren(this Component self, IExTag target)
        {
            return self.GetExTag(target.ExTag).ToArray();
        }

        // GetTagedGameObject
        private static IEnumerable<GameObject> GetTagedGameObject(this Component self, string target)
        {
            return self.GetExTag(target).Select(x => x.gameObject);
        }

        // GetTagedGameObjectInChildren
        public static GameObject GetTagedGameObjectInChildren(this Component self, IExTag target)
        {
            return self.GetTagedGameObject(target.ExTag).FirstOrDefault();
        }

        // GetTagedGameObjectsInChildren
        public static GameObject[] GetTagedGameObjectsInChildren(this Component self, IExTag target)
        {
            return self.GetTagedGameObject(target.ExTag).ToArray();
        }

        #endregion Component
    }
}
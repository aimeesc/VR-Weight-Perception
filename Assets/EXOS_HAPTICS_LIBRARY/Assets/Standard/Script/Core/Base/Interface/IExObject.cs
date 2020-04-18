using System.Collections.Generic;

namespace exiii.Unity
{
    public interface IExObject
    {
        string ExName { get; }

        IReadOnlyCollection<IExTag> ExTags { get; }

        bool IsActive { get; }

        void Initialize();

        void Terminate();
    }
}
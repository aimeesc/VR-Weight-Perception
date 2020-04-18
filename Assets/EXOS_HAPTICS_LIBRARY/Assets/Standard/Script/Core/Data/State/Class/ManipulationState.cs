using System;
using System.Collections.Generic;
using System.Linq;

namespace exiii.Unity
{
    public class ManipulationState : IManipulationState, IDisposable
    {
        private InteractableRoot m_ObjectBase;

        public HashSet<ITouchManipulation> Touch { get; } = new HashSet<ITouchManipulation>();

        public HashSet<ITouchManipulation> Touched { get; } = new HashSet<ITouchManipulation>();

        public HashSet<IGrabManipulation> Grab { get; } = new HashSet<IGrabManipulation>();

        public HashSet<IGripManipulation> Grip { get; } = new HashSet<IGripManipulation>();

        public HashSet<IUseManipulation> Use { get; } = new HashSet<IUseManipulation>();

        public ManipulationState(InteractableRoot objectBase)
        {
            m_ObjectBase = objectBase;
        }

        public bool IsTouched
        {
            get { return Touched.Count > 0; }
        }

        public bool IsTouchedBy(IManipulator<ITouchManipulation> toucher)
        {
            if (toucher == null) { return false; }

            return Touched.Select(x => x.Manipulator).Contains(toucher);
        }

        public bool IsGrabbed
        {
            get { return Grab.Count > 0; }
        }

        public bool IsGrabbedBy(IManipulator<IGrabManipulation> graber)
        {
            return Grab.Select(x => x.Manipulator).Contains(graber);
        }

        public bool IsGripped
        {
            get { return Grip.Count > 0; }
        }

        public bool IsGrippedBy(IManipulator<IGripManipulation> gripper)
        {
            return Grip.Select(x => x.Manipulator).Contains(gripper);
        }

        public bool IsUsed
        {
            get { return Use.Count > 0; }
        }

        public bool IsUsedBy(IManipulator<IUseManipulation> user)
        {
            return Use.Select(x => x.Manipulator).Contains(user);
        }

        #region IManipulationState

        public bool IsManipulated(EManipulationType type)
        {
            switch (type)
            {
                case EManipulationType.Touch:
                    return IsTouched;

                case EManipulationType.Grab:
                    return IsGrabbed;

                case EManipulationType.Grip:
                    return IsGripped;

                case EManipulationType.Use:
                    return IsUsed;
            }

            return false;
        }

        public bool IsManipulatedBy(IManipulator manipulator)
        {
            switch (manipulator.ManipulationType)
            {
                case EManipulationType.Touch:
                    return IsTouchedBy(manipulator as IManipulator<ITouchManipulation>);

                case EManipulationType.Grab:
                    return IsGrabbedBy(manipulator as IManipulator<IGrabManipulation>);

                case EManipulationType.Grip:
                    return IsGrippedBy(manipulator as IManipulator<IGripManipulation>);

                case EManipulationType.Use:
                    return IsUsedBy(manipulator as IManipulator<IUseManipulation>);
            }

            return false;
        }

        #endregion IManipulationState

        #region IDisposable Support

        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                    if (m_ObjectBase != null)
                    {
                        foreach (var item in Touch.ToList()) { item.CancelManipulation(m_ObjectBase); }
                        foreach (var item in Touched.ToList()) { item.CancelManipulation(m_ObjectBase); }
                        foreach (var item in Grab.ToList()) { item.CancelManipulation(m_ObjectBase); }
                        foreach (var item in Grip.ToList()) { item.CancelManipulation(m_ObjectBase); }
                        foreach (var item in Use.ToList()) { item.CancelManipulation(m_ObjectBase); }
                    }
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~ManipulationState() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
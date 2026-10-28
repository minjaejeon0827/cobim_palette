using System;

namespace CobimNet
{
    public class PropertyChangedDisableSection : IDisposable
    {
        public enum PropertyChangedSectionEndMode
        {
            AbsoluteEnable,
            RestoreEnable,
        }

        public WeakReference<BindableBase> Target { get; protected set; }

        public bool EnableStateWhenStart { get; protected set; }

        public PropertyChangedSectionEndMode EndMode { get; protected set; } = PropertyChangedSectionEndMode.RestoreEnable;

        public PropertyChangedDisableSection(BindableBase bindable)
        {
            this.Target = new WeakReference<BindableBase>(bindable);
            this.EnableStateWhenStart = bindable.EnablePropertyChanged;
            bindable.EnablePropertyChanged = false;
        }

        public void Dispose()
        {
            BindableBase target;
            if (!this.Target.TryGetTarget(out target))
                return;
            switch (this.EndMode)
            {
                case PropertyChangedSectionEndMode.AbsoluteEnable:
                    target.EnablePropertyChanged = true;
                    break;
                case PropertyChangedSectionEndMode.RestoreEnable:
                    target.EnablePropertyChanged = this.EnableStateWhenStart;
                    break;
            }
        }

        #region Sample

        #endregion Sample
    }
}

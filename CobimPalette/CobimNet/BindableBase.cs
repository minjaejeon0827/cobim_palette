using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using CobimNet.Extensions;
using CobimNet.Interface;

namespace CobimNet
{
    public abstract class BindableBase : INotifyPropertyChangedExtend
    {
        public static event PropertyChangedEventHandler StaticPropertyChanged;

        public static void StaticChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler staticPropertyChanged = BindableBase.StaticPropertyChanged;
            // TODO : if �������� == null ���� ���� is null ������ ��� (2024.08.14 minjae)
            // ���� URL - https://husk321.tistory.com/405
            if (staticPropertyChanged is null)
                return;
            staticPropertyChanged((object)null, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        public bool EnablePropertyChanged { get; set; } = true;

        //[JsonIgnore]
        //public bool IsPropertyChanged
        //{
        //    get => this._IsPropertyChanged && !(this._IsPropertyChanged = false);
        //    set => this._IsPropertyChanged = value;
        //}
        //private bool _IsPropertyChanged;

        public void Changed<TProperty>(Expression<Func<TProperty>> property) => this.OnPropertyChanged(property.GetMemberInfo().Name);

        public void Changed([CallerMemberName] string name = "") => this.OnPropertyChanged(name);

        public void Changed(params string[] names)
        {
            if ((names is not null ? names.Length : 0) <= 0)
                return;
            foreach (string name in names)
                this.OnPropertyChanged(name);
        }

        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property) => this.OnPropertyChanged(property.GetMemberInfo().Name);

        public void NotifyOfPropertyChange([CallerMemberName] string name = "") => this.OnPropertyChanged(name);

        public void NotifyOfPropertyChange(params string[] names)
        {
            if ((names is not null ? names.Length : 0) <= 0)
                return;
            foreach (string name in names)
                this.OnPropertyChanged(name);
        }

        public bool SetAndNotify<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            this.OnPropertyChanged(name);
            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            // if(!this.EnablePropertyChanged)
            if (false == this.EnablePropertyChanged)
                return;
            // PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            // TODO : if �������� != null ���� ���� is not null ������ ��� (2024.08.14 minjae)
            // ���� URL - https://husk321.tistory.com/405
            //if(propertyChanged is not null)
            //   propertyChanged((object)this, new PropertyChangedEventArgs(name));

            // PropertyChanged �� null�� �ƴϸ� Invoke�� ȣ���Ѵ�
            // ���� URL - https://m.cafe.daum.net/aspdotnet/6TQG/3149
            // ���� 2 URL - https://hwigyeom.ntils.com/3
            // ���� 3 URL - https://cafe.daum.net/aspdotnet/WJSu/65
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // TODO : ���� �ʿ�� ������Ƽ "IsPropertyChanged" ��� ���� (2024.08.14 minjae)
            // this.IsPropertyChanged = true;
        }

        public PropertyChangedDisableSection StartPropertyChangedDisableSection() => new PropertyChangedDisableSection(this);

        #region Sample

        #endregion Sample
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CobimNet.Interface
{
    public interface INotifyPropertyChangedExtend : INotifyPropertyChanged
    {
        [JsonIgnore]
        bool EnablePropertyChanged { get; set; }

        // TODO : 추후 필요시 프로퍼티 "IsPropertyChanged" 사용 예정 (2024.08.14 minjae)
        // [JsonIgnore]
        // bool IsPropertyChanged { get; set; }

        void Changed([CallerMemberName] string name = "");

        void NotifyOfPropertyChange([CallerMemberName] string name = "");

        bool SetAndNotify<T>(ref T field, T value, [CallerMemberName] string name = "");
    }

    #region Sample

    #endregion Sample
}

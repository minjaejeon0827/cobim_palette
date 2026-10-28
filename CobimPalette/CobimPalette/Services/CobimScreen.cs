using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;
using CobimNet;

namespace CobimPalette.Services
{
    // TODO : 추후 필요시 "CobimScreen" 클래스 구현 예정 (2024.09.24 jbh)
    [HiddenVm]
    public abstract class CobimScreen<TData> : CobimScreen
    {
        #region 생성자

        public CobimScreen() : base()
        {

        }

        #endregion 생성자
    }

    // TODO : 추후 필요시 "CobimScreen" 클래스 구현 예정 (2024.09.24 jbh)
    [HiddenVm]
    public abstract class CobimScreen : BindableBase, IDisposable
    {
        #region 프로퍼티 

        // To detect redundant calls
        private bool _DisposedValue { get; set; }

        // Instantiate a SafeHandle instance.
        private SafeHandle? _SafeHandle = new SafeFileHandle(IntPtr.Zero, true);

        #endregion 프로퍼티 

        #region 생성자


        public CobimScreen()
        {
        }

        #endregion 생성자

        #region Dispose

        // TODO : 화면 뷰(V.xaml) 리소스 해제 메서드 "Dispose" 필요시 구현 예정 (2024.09.25 jbh)
        // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/standard/garbage-collection/implementing-dispose
        // 참고 URL - https://just-my-blog.tistory.com/55
        // 참고 URL - https://huiyu.tistory.com/entry/C-%ED%91%9C%EC%A4%80-Dispose-%ED%8C%A8%ED%84%B4
        public void Dispose()
        {
            // Dispose(true);
            // GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // if(false == _DisposedValue)
            // {
            //     if(true == disposing)
            //     {
            //         _SafeHandle?.Dispose();
            //         _SafeHandle = null;
            //     }
            // 
            //     _DisposedValue = true;
            // }
        }

        #endregion Dispose

        #region 기본 메소드 

        public new void NotifyOfPropertyChange([CallerMemberName] string name = "") => base.NotifyOfPropertyChange(name);

        public new void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property) => base.NotifyOfPropertyChange<TProperty>(property);

        public new bool SetAndNotify<T>(ref T field, T value, [CallerMemberName] string propertyName = "") => base.SetAndNotify<T>(ref field, value, propertyName);

        #endregion 기본 메소드
    }
}

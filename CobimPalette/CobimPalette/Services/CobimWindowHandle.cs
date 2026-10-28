using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CobimPalette.Services
{
    // TODO : 필요시 윈도우 핸들 클래스 CobimWindowHandle 사용 예정 (2024.11.28 jbh)
    // 참고 URL - https://www.codeproject.com/Articles/24403/Parent-Process-Window
    /// <summary>
    /// Cobim 애드인 프로그램 윈도우 핸들 클래스 
    /// </summary>
    public class CobimWindowHandle : IWin32Window
    {
        #region 프로퍼티

        /// <summary>
        /// 
        /// </summary>
        private IntPtr _Hwnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IntPtr Handle { get { return _Hwnd; } }

        #endregion 프로퍼티

        #region 생성자

        public CobimWindowHandle(IntPtr hwnd)
        {
            _Hwnd = hwnd;
        }

        #endregion 생성자 

        #region Sample

        #endregion Sample
    }
}

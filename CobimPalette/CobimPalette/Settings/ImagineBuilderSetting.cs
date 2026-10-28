using Serilog;

using System.Diagnostics;
using System.Reflection;

using CobimPalette.Common.LogBase;

namespace CobimPalette.Settings
{
    /// <summary>
    /// (주)상상진화 기본 설정
    /// </summary>
    public class ImagineBuilderSetting // : BindableBase
    {
        #region 프로퍼티

        #endregion 프로퍼티

        #region 생성자

        // TODO : 필요시 생성자 "ImagineBuilderSetting" 구현 예정 (2024.08.20 minjae)
        public ImagineBuilderSetting()
        {
            // _HomePage = null;
        }

        #endregion 생성자 

        #region ConnectImagineBuilder

        /// <summary>
        /// (주)상상진화 기업 홈페이지 연결
        /// </summary>
        public void ConnectImagineBuilder(string pImagineBuilderURL)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                Log.Information(Logger.GetMethodPath(currentMethod) + "(주)상상진화 기업 홈페이지 연결 시작");

                // TODO : (주)상상진화 기업 홈페이지 팝업 화면 출력 구현 (2024.08.20 minjae)
                // 참고 URL   - https://yongtech.tistory.com/58
                // 참고 2 URL - https://findfun.tistory.com/485

                // TODO : (주)상상진화 기업 홈페이지 구글 크롬(chrome.exe)으로 출력 구현 (2024.08.20 minjae)
                // 참고 URL - https://www.codeproject.com/Questions/5286855/How-do-I-open-Google-chrome-in-Csharp
                // Process.Start("chrome.exe", pURL);
                // Process.Start("firefox.exe", pURL);

                // TODO : .net FrameWork 말고 .net Core 6.0 이상 버전에서  (주)상상진화 기업 홈페이지 출력 오류시 아래 처럼 구현 (2024.08.20 minjae)
                // 참고 URL - https://endev.tistory.com/m/237
                Process.Start(new ProcessStartInfo(pImagineBuilderURL) { UseShellExecute = true });

                Log.Information(Logger.GetMethodPath(currentMethod) + "(주)상상진화 기업 홈페이지 연결 완료");
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion ConnectImagineBuilder

        #region Sample

        #endregion Sample
    }
}

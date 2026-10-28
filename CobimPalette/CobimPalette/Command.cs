using Serilog;

using System.Reflection;
using Exception = System.Exception;
using MessageBox = System.Windows.MessageBox;

using CobimPalette.Common.CobimBase;
using CobimPalette.Common.LogBase;
using CobimPalette.Settings;

using Autodesk.AutoCAD.Runtime;

// TODO : AutoCAD API 개발 커뮤니티 사이트 필요시 참고 (2024.11.08 minjae)
// 참고 URL - https://forums.autodesk.com/t5/net/bd-p/152

// This line is not mandatory, but improves loading performances
// 필수 아니지만 로딩 성능 향상 시켜주는 기능
[assembly: CommandClass(typeof(CobimPalette.Command))]

namespace CobimPalette
{
    /// <summary>
    /// AutoCAD2025 애드인 프로그램 명령어 클래스 
    /// Revit 애드인 프로그램 Command.cs와 비슷한 기능 
    /// </summary>
    internal class Command
    {
        [CommandMethod("Button1Command")]
        public void Button1Command()
        {
            MessageBox.Show("Button1 테스트");
        }

        [CommandMethod("Button2Command")]
        public void Button2Command()
        {
            MessageBox.Show("Button2 테스트");
        }

        #region CmdWebExplorer

        /// <summary>
        /// WebExplorer Command 
        /// </summary>
        [CommandMethod("CmdWebExplorer")]
        public void CmdWebExplorer()
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                Log.Information(Logger.GetMethodPath(currentMethod) + "WebExplorer Command - Execute 시작");

                // Palette - WebView2 탐색기 화면 출력
                AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.WebView2ExplorerTitle, CobimHelper.WebView2ExplorerGuId);

                Log.Information(Logger.GetMethodPath(currentMethod) + "WebExplorer Command - Execute 종료");
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion CmdWebExplorer

        #region CmdWebView2CDE

        /// <summary>
        /// 성과품(CDE) Command 
        /// </summary>
        //[CommandMethod("CmdWebView2CDE")]
        //public void CmdWebView2CDE() 
        //{
        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        Log.Information(Logger.GetMethodPath(currentMethod) + "성과품(CDE) Command - Execute 시작");

        //        // Palette - WebView2 탐색기 화면 출력
        //        AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.WebView2ExplorerTitle, CobimHelper.WebView2ExplorerGuId);

        //        Log.Information(Logger.GetMethodPath(currentMethod) + "성과품(CDE) Command - Execute 종료");
        //    }
        //    catch(Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
        //    }
        //}

        #endregion CmdWebView2CDE


        #region CmdHubBox

        // TODO : 아래 주석친 코드 필요시 참고 (2024.12.04 minjae)
        /// <summary>
        /// HubBox Command 
        /// </summary>
        // [CommandMethod("CmdHubBox")]
        // public void CmdHubBox() 
        // {
        //     var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록
        // 
        //     try
        //     {
        //         Log.Information(Logger.GetMethodPath(currentMethod) + "HubBox Command - Execute 시작");
        // 
        //         // Palette - HubBox 화면 출력
        //         // AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.HubBoxTitle, CobimHelper.HubBoxGuId);
        // 
        //         Log.Information(Logger.GetMethodPath(currentMethod) + "HubBox Command - Execute 종료");
        //     }
        //     catch(Exception ex)
        //     {
        //         Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //         MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
        //     }
        // }

        #endregion CmdHubBox

        #region CmdImagineBuilder

        /// <summary>
        /// (주)상상진화 기업 홈페이지 Command 
        /// </summary>
        //[CommandMethod("CmdImagineBuilder")]
        //public void CmdImagineBuilder() 
        //{
        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        Log.Information(Logger.GetMethodPath(currentMethod) + "(주)상상진화 기업 홈페이지 Command - Execute 시작");

        //        // (주)상상진화 기업 홈페이지 URL 주소로 연동
        //        AppSetting.Default.ImagineBuilderBase.ConnectImagineBuilder(CobimHelper.ImagineBuilderURL);

        //        Log.Information(Logger.GetMethodPath(currentMethod) + "(주)상상진화 기업 홈페이지 Command - Execute 종료");
        //    }
        //    catch(Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
        //    }
        //}

        #endregion CmdImagineBuilder
    }

    #region Sample

    #endregion Sample
}

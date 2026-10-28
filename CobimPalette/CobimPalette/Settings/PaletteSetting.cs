using Serilog;

using System.Reflection;
using System.Windows.Controls;

using CobimPalette.Common.CobimBase;
using CobimPalette.Common.LogBase;

using CobimPalette.Views.Pages.Palettes.Project.BIM360.HubBox;
using CobimPalette.Views.Pages.Palettes.Project.WebView2CDE.WebView2Explorer;

using static CobimPalette.Common.Extensions.PaletteExtensions;

using Autodesk.AutoCAD.Windows;

namespace CobimPalette.Settings
{
    // TODO : Palette 기본 설정 클래스 PaletteSetting 추후 구현 예정 (2024.11.08 jbh)
    /// <summary>
    /// Palette 기본 설정 
    /// </summary>
    public class PaletteSetting
    {
        #region 프로퍼티

        /// <summary>
        /// 생성된 CustomPaletteSet 객체 관리자 
        /// </summary>
        private PaletteSetManager _PaletteSetManager { get; set; }

        /// <summary>
        /// 사용자 정의 Palette CustomPaletteSet 클래스 객체 
        /// </summary>
        private CustomPaletteSet _CustomPaletteSet { get; set; }

        /// <summary>
        /// Palette - WebView2 탐색기 화면 뷰 
        /// </summary>
        private WebView2ExplorerV _WebView2ExplorerV { get; set; }

        /// <summary>
        /// Palette - HubBox 화면 뷰
        /// </summary>
        private HubBoxV _HubBoxV { get; set; }

        #endregion 프로퍼티 

        #region 생성자

        // TODO : 필요시 생성자 "PaletteSetting" 구현 예정 (2024.11.11 jbh)
        public PaletteSetting() 
        { 
        
        }

        #endregion 생성자

        #region CreatePalette

        /// <summary>
        /// Palette 화면 싱글톤 객체 생성
        /// </summary>
        public void CreatePalette()
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // 싱글톤 패턴 사용해서 처음 한번만 프로퍼티 "_PaletteSetManager", "_WebView2ExplorerV", "_HubBoxV" 객체 생성
                if(_PaletteSetManager is null) _PaletteSetManager = new PaletteSetManager();   
                if(_WebView2ExplorerV is null) _WebView2ExplorerV = new WebView2ExplorerV();
                if(_HubBoxV is null) _HubBoxV = new HubBoxV();
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion CreatePalette

        #region GetPalette

        /// <summary>
        /// Palette 화면 객체 가져오기 
        /// 메서드 파라미터 paletteName - Palette 화면 뷰 이름
        /// </summary>
        public Page GetPalette(string paletteName)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                switch(paletteName)
                {
                    case CobimHelper.TypeOfWebView2Explorer:   // Palette - WebView2 탐색기 화면 뷰인 경우 
                        return _WebView2ExplorerV;
                    case CobimHelper.TypeOfHubBox:             // Palette - HubBox 화면 뷰인 경우 
                        return _HubBoxV;
                    default:
                        throw new Exception($"[Palette] {paletteName} 화면 가져오기 오류!\r\n관리자에게 문의하세요.");
                        // break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion GetPalette

        #region RegisterPalette

        /// <summary>
        /// AutoCAD 응용 프로그램 UI 화면에 Palette 객체 생성 및 Palette 화면 등록 및 출력 
        /// </summary>
        public void RegisterPalette(string paletteTitle, Guid paletteGuId)
        {     
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // Log.Information(Logger.GetMethodPath(currentMethod) + "[Palette] 화면 등록 및 출력 시작");

                // 특정 paletteGuId 값을 가진 Palette 객체가 등록된 적이 없는 경우 
                if(false == _PaletteSetManager.PaletteIsRegistered(paletteGuId))
                {
                    DockSides paletteDockSides = DockSides.Right;

                    switch(paletteTitle)
                    {
                        case CobimHelper.WebView2ExplorerTitle:   // Palette - WebView2 탐색기 화면 뷰인 경우 
                            _PaletteSetManager.RegisterPalette(paletteGuId, paletteTitle, _WebView2ExplorerV, paletteDockSides);
                            break;
                        case CobimHelper.HubBoxTitle:   // Palette - HubBox 화면 뷰인 경우 
                            _PaletteSetManager.RegisterPalette(paletteGuId, paletteTitle, _HubBoxV, paletteDockSides);
                            break;
                        // TODO : 아래 주석친 case 레이블(case CobimHelper.TypeOfExplorer:) 필요시 사용 예정 (2024.09.25 jbh)
                        // case CobimHelper.ExpTitle:
                        //     _PaletteSetManager.RegisterPalette(dockablePaneId, RibbonHelper.PanName_CDE, _ExplorerV, dockablePaneState);
                        //     _PaletteSetManager.RegisterPalette(dockablePaneId, paletteTitle, _ExplorerV, dockablePaneState);
                        //     break;
                        default:
                            throw new Exception($"[Palette] {paletteTitle} 화면 등록 및 출력 오류!\r\n관리자에게 문의하세요.");
                            // break;
                    }
                }

                Log.Information(Logger.GetMethodPath(currentMethod) + $"[Palette] {paletteTitle} 화면 등록 및 출력 완료");
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion RegisterPalette

        #region ShowPalette

        /// <summary>
        /// AutoCAD 응용 프로그램 UI 화면에 이미 종료한 Palette 화면 출력 
        /// </summary>
        public void ShowPalette(string paletteTitle, Guid paletteGuId)
        {
            bool isVisible = false;                              // Palette 화면 출력 여부 (출력 - true / 출력안함 - false)

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                _CustomPaletteSet = _PaletteSetManager.GetPalette(paletteGuId);

                isVisible = (true == _PaletteSetManager.PaletteIsRegistered(paletteGuId)) && (true == _CustomPaletteSet.GetVisible());

                if(false == isVisible)
                {
                    _CustomPaletteSet.SetData(true, DockSides.Right, false);
                    Log.Information(Logger.GetMethodPath(currentMethod) + $"[Palette] {paletteTitle} 화면 출력 완료");
                }
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion ShowPalette

        #region Sample

        #endregion Sample
    }
}

using Serilog;

using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;

using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

using CobimPalette.Common.CobimBase;
using CobimPalette.Common.LogBase;

using CobimPalette.Rest.Api.CobimBase.HubBox;
using CobimPalette.ViewModels.Pages.Palettes.Project.WebView2CDE.WebView2Explorer;


namespace CobimPalette.Views.Pages.Palettes.Project.WebView2CDE.WebView2Explorer
{
    // TODO : WebView2 런타임 dll 파일 "WebView2Loader.dll" 로드 오류 메세지 "Unable to load DLL 'WebView2Loader.dll' or one of its dependencies: 지정된 모듈을 찾을 수 없습니다. (0x8007007E)"
    //        출력시 AutoCAD2025 응용 프로그램, Microsoft Edge WebView2 런타임 프로그램 삭제 후 다시 설치 진행하기 (2024.11.18 jbh)
    // 참고 URL - https://www.autodesk.com/kr/support/technical/article/caas/sfdcarticles/sfdcarticles/KOR/Unable-to-run-AutoCAD-A-dependent-component-Micorosoft-Edge-WebView2-when-launching-AutoCAD.html
    // 참고 2 URL - https://www.autodesk.com/support/technical/article/caas/sfdcarticles/sfdcarticles/Unable-to-run-AutoCAD-A-dependent-component-Micorosoft-Edge-WebView2-when-launching-AutoCAD.html

    // TODO : Nuget 패키지 WebView2 설치 및 사용하여 웹사이트 URL 주소를 WPF 화면 뷰와 연동하여
    //        인터넷 홈페이지를 WPF 화면 뷰로 출력 하도록 구현 (2024.11.15 jbh)
    // 참고 URL - https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/wpf
    // 참고 2 URL - https://kimstack.tistory.com/m/4
    // 참고 3 URL - https://chashtag.tistory.com/entry/CWPF-WebView2-%EC%82%AC%EC%9A%A9%EB%B0%A9%EB%B2%95-WebBrowser-%EB%8C%80%EC%B2%B4-JS%ED%86%B5%EC%8B%A0

    /// <summary>
    /// WebView2ExplorerV.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WebView2ExplorerV : Page
    {
        #region 프로퍼티

        /// <summary>
        /// 뷰모델 WebView2ExplorerVM에 매핑되는 WebView2 탐색기 뷰 이름(WebView2ExplorerV)
        /// </summary>
        private string _PaletteName { get; set; }

        /// <summary>
        /// COBIM 웹 탐색기 브라우저에 존재하는 바인딩 할 메서드 객체 이름 
        /// </summary>
        private string _ObjectName { get; set; }

        #endregion 프로퍼티

        #region 생성자

        public WebView2ExplorerV()
        {
            InitializeComponent();
            InitSettingAsync();
        }

        #endregion 생성자

        #region InitSetting

        /// <summary>
        /// WebView2 기술 기반 
        /// 응용 프로그램 CobimPalette 
        /// 화면 초기 셋팅
        /// </summary>
        private async void InitSettingAsync()
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                Log.Information(Logger.GetMethodPath(currentMethod) + "WebView2 화면 초기 셋팅 시작");

                // TODO : 아래 코드가 하는 기능이 무엇인지 파악 예정 (2024.11.19 jbh)
                var env = await CoreWebView2Environment.CreateAsync(null, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TestWV"));

                // await cobimWebExplorer.EnsureCoreWebView2Async(null);   // async로 corewebview2를 초기화 처리
                await cobimWebExplorer.EnsureCoreWebView2Async(env);

                // cobimWebExplorer.CreationProperties = new CoreWebView2CreationProperties();
                cobimWebExplorer.Source = new Uri(HubBoxHelper.requestLoginURL); // HubBox 웹 탐색기 - 로그인 Rest API 주소 

                // 호출되는 자바스크립트 웹페이지에서 C# 클래스에 등록된 API 호출 가능 하도록 아래처럼 구현  
                // TODO : COBIM WebView2 웹 탐색기 화면 뷰(WebView2ExplorerV.xaml) 호출시 DataContext 객체 생성 구현 (2024.11.18 jbh)
                // 참고 URL - https://yeko90.tistory.com/entry/wpf-DataContext-%EB%8B%A4%EC%96%91%ED%95%9C-%EB%B0%A9%EB%B2%95
                this.DataContext = new WebView2ExplorerVM();
                this._PaletteName = CobimHelper.TypeOfWebView2Explorer;
                this._ObjectName = CobimHelper.boundAsync;

                WebView2ExplorerVM webView2ExplorerVM = (WebView2ExplorerVM)this.DataContext;
                webView2ExplorerVM.InitSetting(_PaletteName);

                // 뷰모델 클래스 WebView2ExplorerVM.cs가 바인딩이 안되는 원인 파악 예정 (2024.11.19 jbh)
                cobimWebExplorer.CoreWebView2.AddHostObjectToScript(this._ObjectName, this.DataContext);

                //TestCls test = new TestCls();
                //cobimWebExplorer.CoreWebView2.AddHostObjectToScript(this._ObjectName, test);

                Log.Information(Logger.GetMethodPath(currentMethod) + "WebView2 화면 초기 셋팅 완료");
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion InitSetting

        #region Sample

        // TODO : 아래 주석친 코드 필요시 참고 (2024.11.20 jbh)
        //[ClassInterface(ClassInterfaceType.AutoDual)]
        //[ComVisible(true)]
        //public class TestCls
        //{
        //    public async Task<string> DownLoadAsync(object jsonObj)
        //    {
        //        MessageBox.Show("다운로드 테스트");
        //        return "success";
        //    }

        //    public async Task<string> CheckOutAsync(object jsonObj)
        //    {
        //        MessageBox.Show("체크아웃 테스트");
        //        return "success";
        //    }

        //    public async Task<string> CheckInAsync(object jsonObj)
        //    {
        //        MessageBox.Show("체크인 테스트");
        //        return "success";
        //    }

        //    public async Task<string> CancelWorkAsync(object jsonObj)
        //    {
        //        MessageBox.Show("컨테이너 작업취소 테스트");
        //        return "success";
        //    }

        //    public async Task<string> OpenFileAsync(object jsonObj)
        //    {
        //        MessageBox.Show("파일열기 테스트");
        //        return "success";
        //    }
        //}

        #endregion Sample
    }
}

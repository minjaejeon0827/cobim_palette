using Serilog;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

using System.IO;
using System.Text;
using System.Net.Http;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.Json;
using System.Runtime.InteropServices;

using CobimPalette.Commands;
using CobimPalette.Common.CobimBase;
using CobimPalette.Common.LogBase;
using CobimPalette.Interface.Page;
using CobimPalette.Settings;
using CobimPalette.Services.Page;
using CobimPalette.Utils.String;

using CobimPalette.Rest.Api.CobimBase.HubBox;
using CobimPalette.Rest.Api.CobimBase.WebView2Explorer;

using CobimPalette.Data.Models.CobimBase.Login;
using CobimPalette.Data.Models.CobimBase.WebView2Explorer;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using CobimPalette.Views.Windows.LoadingWork;
using System.Windows.Threading;

// TODO : Nuget 패키지 WebView2 설치 및 사용하여 웹사이트 URL 주소를 WPF 화면 뷰와 연동하여
//        인터넷 홈페이지를 WPF 화면 뷰로 출력 하도록 구현 (2024.11.15 jbh)
// 참고 URL - https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/wpf
// 참고 2 URL - https://kimstack.tistory.com/m/4
// 참고 3 URL - https://chashtag.tistory.com/entry/CWPF-WebView2-%EC%82%AC%EC%9A%A9%EB%B0%A9%EB%B2%95-WebBrowser-%EB%8C%80%EC%B2%B4-JS%ED%86%B5%EC%8B%A0


// TODO : AutoCAD 애드인 프로그램의 경우 자립(Stand Alone) 방식으로 독자적으로 프로그램을 실행할 수 없고, 
//        AutoCAD 응용 프로그램에 애드인 프로그램 dll 파일을 연동 후 AutoCAD 응용 프로그램을 선실행 해야만
//        AutoCAD 애드인 프로그램 또한 실행할 수 있다 . (2024.09.30 jbh)
// 자립(Stand Alone) 방식
// 참고 URL - https://www.inflearn.com/community/questions/24062/%EC%8A%A4%ED%83%A0%EB%93%9C-%EC%96%BC%EB%A1%A0-stand-alone-%EB%B0%A9%EC%8B%9D?srsltid=AfmBOorEtdzMSrxQfZKvOSz4jCs-JRyw3MeBsyAZZ30PhiuS5MJboVSD

namespace CobimPalette.ViewModels.Pages.Palettes.Project.WebView2CDE.WebView2Explorer
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    // TODO : 아래 주석친 코드 사용시 WebView2 웹탐색기 화면 뷰 비하인드 코드(WebView2ExplorerV.xaml.cs)
    //        -> 메서드 "InitSettingAsync" 호출 -> 메서드 "cobimWebExplorer.CoreWebView2.AddHostObjectToScript(this._ObjectName, webView2ExplorerVM);" 호출시 
    //        WebView2 웹탐색기 화면 뷰모델(WebView2ExplorerVM.cs) 바인딩이 실패하여
    //        AutoCAD2025 애드인 프로그램(CobimPalette.dll) -> WebView2 웹탐색기 화면 로그인
    //        -> WebView2 웹탐색기 화면에서 마우스 우클릭 -> 컨텍스트 메뉴 "체크아웃", "체크인", "다운로드", "작업취소", "파일열기" 클릭시
    //        아래와 같은 비동기 메서드 5가지("DownLoadAsync", "CheckOutAsync", "CheckInAsync", "CancelWorkAsync", "OpenFileAsync")가
    //        정상적으로 실행 안 되는 오류 발생
    //        하여 코드를 아래처럼 변경
    //        (기존) public class WebView2ExplorerVM : PageBase<WebView2ExplorerView>, IPage
    //        (변경) public class WebView2ExplorerVM : PageBase, IPage

    //public class WebView2ExplorerVM : PageBase<WebView2ExplorerView>, IPage
    public class WebView2ExplorerVM : PageBase, IPage
    {
        #region 프로퍼티

        /// <summary>
        /// 입력 파라미터 데이터 모델 객체 (Rest API 포함)
        /// 체크아웃, 체크인, 다운로드, 작업취소, 파일열기 등등...
        ///// </summary>
        public WebView2ExplorerView InputParamData { get => _InputParamData; set { SetAndNotify(ref _InputParamData, value); } }
        private WebView2ExplorerView _InputParamData = new();

        /// <summary>
        /// 테스트 버튼 Command
        /// </summary>
        public ICommand TestCommand { get; set; }

        /// <summary>
        /// HttpClient client
        /// </summary>
        private HttpClient _Client { get; set; }

        /// <summary>
        /// 폴더 (디렉토리) 공통 경로 
        /// (응용 프로그램 - CobimExplorer.exe, Revit 애드인 프로그램 - CobimDockableDialogs.dll, AutoCAD 애드인 프로그램 - CobimPalette.dll)
        /// </summary>
        private string _CommonDirectoryPath { get; set; }

        /// <summary>
        /// 컨테이너 정보 
        /// </summary>
        private ContainerPack _ContainerPack { get; set; }

        // TODO : 필요시 프로퍼티 "_PaletteName" 사용 예정 (2024.10.17 jbh)
        /// <summary>
        /// 뷰모델 WebView2ExplorerVM에 매핑되는 WebView2 탐색기 뷰 이름(WebView2ExplorerV)
        /// </summary>
        // private string _PaletteName { get; set; }

        /// <summary>
        /// Palette 화면 객체 (WebView2 탐색기 뷰 - WebView2ExplorerV.xaml)
        /// </summary>
        private Page _Palette { get; set; }

        /// <summary>
        /// Window - 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면
        /// </summary>
        private CommonLoadingWorkV _CommonLoadingWorkV { get; set; }

        // TODO : 필요시 프로퍼티 "LoginURL" 사용 예정 (2024.10.17 jbh)
        /// <summary>
        /// AutoCAD 애드인 프로그램(CobimPalette.dll) 전용 
        /// 웹 탐색기 - 로그인 Rest API 주소 
        /// </summary>
        // public string LoginURL { get => _LoginURL; set { _LoginURL = value; NotifyOfPropertyChange(); } }
        // private string _LoginURL;

        /// <summary>
        /// 파일 다운로드 처리 진행 여부 
        /// </summary>
        public bool IsDownLoadWorking { get => _IsDownLoadWorking; set { _IsDownLoadWorking = value; NotifyOfPropertyChange(nameof(IsDownLoadWorking)); } }
        private bool _IsDownLoadWorking;

        /// <summary>
        /// 체크아웃 처리 진행 여부 
        /// </summary>
        public bool IsCheckOutWorking { get => _IsCheckOutWorking; set { _IsCheckOutWorking = value; NotifyOfPropertyChange(nameof(IsCheckOutWorking)); } }
        private bool _IsCheckOutWorking;

        /// <summary>
        /// 연관 컨테이너에 속한 파일 다운로드 처리 진행 여부 
        /// </summary>
        public bool IsLinkedContainerWorking { get => _IsLinkedContainerWorking; set { _IsLinkedContainerWorking = value; NotifyOfPropertyChange(nameof(IsLinkedContainerWorking)); } }
        private bool _IsLinkedContainerWorking;

        /// <summary>
        /// 체크인 처리 진행 여부 
        /// </summary>
        public bool IsCheckInWorking { get => _IsCheckInWorking; set { _IsCheckInWorking = value; NotifyOfPropertyChange(nameof(IsCheckInWorking)); } }
        private bool _IsCheckInWorking;

        /// <summary>
        /// 컨테이너 작업 취소 처리 진행 여부 
        /// </summary>
        public bool IsCancelWorking { get => _IsCancelWorking; set { _IsCancelWorking = value; NotifyOfPropertyChange(nameof(IsCancelWorking)); } }
        private bool _IsCancelWorking;

        /// <summary>
        /// 파일 변환 처리 진행 여부 
        /// </summary>
        public bool IsTranslateWorking { get => _IsTranslateWorking; set { _IsTranslateWorking = value; NotifyOfPropertyChange(nameof(IsTranslateWorking)); } }
        private bool _IsTranslateWorking;

        /// <summary>
        /// 컨테이너에 속한 파일(확장자 .dwg, .dxf) 열기 처리 진행 여부 
        /// </summary>
        public bool IsOpenFileWorking { get => _IsOpenFileWorking; set { _IsOpenFileWorking = value; NotifyOfPropertyChange(nameof(IsOpenFileWorking)); } }
        private bool _IsOpenFileWorking;

        #endregion 프로퍼티

        #region 생성자

        public WebView2ExplorerVM()
        {
            // this._LoginURL = HubBoxHelper.requestLoginURL;   // HubBox 웹 탐색기 - 로그인 Rest API 주소 
            //InitSetting(pPaletteName);

            TestCommand = new ButtonCommand(CobimHelper.VoidMethodType, Test, CanExecuteMethod);
        }

        #endregion 생성자

        #region 기본 메소드

        #region InitSetting

        // TODO : 초기 셋팅 메서드 "InitSetting" 필요시 구현 예정 (2024.10.08 jbh)
        // 참고 URL - https://chat.openai.com/c/b9d52fcf-aab0-49e3-8008-8852767a9982
        /// <summary>
        /// WebView2 기술 기반 
        /// 응용 프로그램 CobimPalette
        /// 뷰모델 초기 셋팅 
        /// </summary>
        public void InitSetting(string paletteName)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // 1. 뷰모델 WebView2ExplorerVM에 매핑되는 WebView2 탐색기 뷰 이름(WebView2ExplorerV) 값 셋팅
                // _PaletteName = paletteName;

                // 2. Palette 화면 객체 셋팅 (WebView2 탐색기 뷰 - WebView2ExplorerV.xaml) 
                _Palette = AppSetting.Default.PaletteBase.GetPalette(paletteName);

                // 3. Window - 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면 객체 값 할당  
                _CommonLoadingWorkV = AppSetting.Default.WindowBase.GetCommonLoadingWorkV();

                // 4. 데이터 모델 객체 InputParamData에 HttpClient client 객체 값 할당 
                _Client = AppSetting.Default.HttpClientBase.GetHttpClient();
                InputParamData.SetHttpClient(_Client);

                // 5. 데이터 모델 객체 InputParamData에 폴더 (디렉토리) 공통 경로 string 클래스 객체 값(문자열) 할당 
                _CommonDirectoryPath = AppSetting.Default.DirectoryBase.GetCommonDirectoryPath();
                InputParamData.SetCommonDirectoryPath(_CommonDirectoryPath);
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        #endregion InitSetting

        #region CanExecuteMethod

        private bool CanExecuteMethod(object arg)
        {
            return true;
        }

        #endregion CanExecuteMethod

        #region Test

        /// <summary>
        /// Test 메서드 - AutoCAD (확장자 .dwg, dxf) 파일 열기 
        /// </summary>
        private void Test(object obj)
        {
            string textFilePath = string.Empty;                    // 텍스트 파일 전체 경로(+ 파일명 포함) 

            var currentMethod = MethodBase.GetCurrentMethod();     // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // 사용자 주로 사용하는 복제 매개변수 정보들을 템플릿 처럼 쉽게 불러오기 위해 사용 
                // 텍스트 파일에서 불러오기 (설정읽기)
                // 참고 URL - https://sheepone.tistory.com/97

                Log.Information(Logger.GetMethodPath(currentMethod) + "OpenFileDialog 화면 출력");

                // TODO : 해당 using 문장을 벗어나면 OpenFileDialog 클래스 객체 openFileDialog Dipose 처리 (리소스 해제 및 자원 반납) 구현 (2024.02.26 jbh)
                // 참고 URL - https://hongjinhyeon.tistory.com/92

                OpenFileDialog openFileDialog = new OpenFileDialog();
                // OpenFileDialog 화면 출력시 초기 폴더(디렉토리) 경로 설정
                // openFileDialog.InitialDirectory = AABIMHelper.SettingsSeumteoDirPath;
                openFileDialog.Filter = "텍스트 파일|*.txt|모든 파일|*.*";
                // openFileDialog.FilterIndex   = 1;   // 1번 선택시 텍스트 파일(*.txt), 2번 선택시 모든 파일(*.*)
                // openFileDialog.FilterIndex = (int)EnumFileTypes.TEXT;   // 1번 선택시 텍스트 파일(*.txt), 2번 선택시 모든 파일(*.*)

                // TODO : 프로퍼티 "RestoreDirectory" 사용해서 OpenFileDialog 화면 닫기 전에 이전에 선택한 디렉터리로 복원 설정 구현 (2024.06.24 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.forms.filedialog.restoredirectory?view=windowsdesktop-8.0
                // 참고 2 URL - https://devit.koreacreatorfesta.com/entry/C-OpenFileDialog
                openFileDialog.RestoreDirectory = true;

                if (true == openFileDialog.ShowDialog())
                {
                    // 이미지 파일 선택 후 버튼 "열기(O)"을 클릭 했을 경우 
                    // if (openFileDialog.FileName.Length >= (int)EnumSelectFile.SELECT
                    //    && dialogResult.OK == dialogResult)
                    textFilePath = openFileDialog.FileName;   // 전체 경로(+ 파일명 포함)는 openFileDialog.FileName  // 참고 - 선택한 파일명은 openFileDialog.SafeFileName
                }

                Log.Information(Logger.GetMethodPath(currentMethod) + "OpenFileDialog 화면 종료");
            }
            catch (Exception ex)
            {

            }
        }

        #endregion Test

        #endregion 기본 메소드 

        #region IsOpenedFile

        // TODO : 해당 파일 경로(filePath)에 존재하는 AutoCAD 문서가 AutoCAD 응용 프로그램에서 열려있는 경우
        //        체크인 처리시 오류가 발생하므로 AutoCAD 문서가 AutoCAD 응용 프로그램에서 열려 있으면 오류 처리 메서드 "IsOpenedFile" 구현 (2024.10.22 jbh)
        // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.io.file.open?view=net-8.0
        // 참고 2 URL - https://www.csharpstudy.com/Tip/Tip-file-open.aspx
        // 참고 3 URL - https://bigexecution.tistory.com/288
        // 참고 4 URL - https://blog.naver.com/wonjinho81/221630185577

        /// <summary>
        /// AutoCAD 파일(확장자 .dwg, .dxf) 열려있는지 여부 확인
        /// </summary>
        private bool IsOpenedFile(string pFilePath)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                using (File.Open(pFilePath, FileMode.Open)) { }
                return false;   // AutoCAD 파일(확장자 .dwg, .dxf)이 열려있지 않은 경우 
            }
            catch (Exception ex)
            {
                // 이미 열려있는 AutoCAD 파일(확장자 .dwg, .dxf)을 다시 open 하는경우 예외 발생
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                return true;
            }
        }

        #endregion IsOpenedFile

        // TODO : "정용직 수석님"이 개발하신 웹탐색기에서 입력 파라미터를 object 또는 Json 문자열로 받을 때, 
        //        해당 Json 데이터를 파싱하도록 구현 예정 (2024.09.24 jbh)
        // 참고 URL - https://m.blog.naver.com/smhrd_official/222448275383
        // 참고 2 URL - https://shlee0882.tistory.com/45
        // 참고 3 URL - https://lovemewithoutall.github.io/it/json-dot-net/

        // TODO : "정용직 수석님"이 개발하신 웹탐색기에서 입력 파라미터를 object 형식으로 값 "5"를 받는 것과
        //        웹탐색기에서 마우스 우클릭해서 컨텍스트 메뉴 클릭시 Alert 메시지 박스가 출력되는지 테스트 하기 (2024.09.27 jbh)

        #region DownLoadAsync

        /// <summary>
        /// 파일 다운로드
        /// </summary>
        // TODO : WebView2 웹 기술 사용시 아래처럼 비동기 메서드 "DownLoadAsync"의 메서드 파라미터 "object jsonObj" 선언 불가 
        //        왜냐하면 파라미터 유형으로 "object" 선언시 WebView2 웹 기술 라이브러리가 "object" 유형을 인식 못하고 
        //        자바스크립트 소스코드에서 넘어오는 입력 파라미터 값을 제대로 못 넘겨주는 오류가 발생하기 때문
        //        하여 메서드 파라미터 선언 방식을 (기존) "object jsonObj" -> (변경) "string jsonObj" 필요 (2024.11.20 jbh)
        // public async Task<string> DownLoadAsync(object jsonObj)
        public async Task<string> DownLoadAsync(string jsonObj)
        {
            // TODO : Json 문자열 객체 jsonObj -> Dictionary<string, object?> 객체로 복원(JsonSerializer.Deserialize) 구현 (2024.11.20 jbh)
            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.text.json.jsonserializer.deserialize?view=net-8.0#system-text-json-jsonserializer-deserialize-1(system-string-system-text-json-jsonserializeroptions)
            // 참고 2 URL - https://www.csharpstudy.com/Data/Json-SystemTextJson.aspx
            Dictionary<string, object?> downLoadDic = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonObj);

            string resMsg = string.Empty;                        // 파일 다운로드 처리 결과(성공 또는 실패) 문자열 초기화  

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // 파일 다운로드 처리 중인 경우 
                if(true == IsDownLoadWorking)
                    throw new Exception(CobimHelper.DownLoad_LoadingWork);

                IsDownLoadWorking = true;   // 파일 다운로드 처리 중 시작

                // COBIM 웹 탐색기 데이터 모델 객체 InputParamData에 데이터셋 새로 할당 
                InputParamData.SetData(new RequestHeader(downLoadDic[EnumDownLoad.TENANT.ToDescription()].ToString(),
                                                         downLoadDic[EnumDownLoad.AUTHORIZATION.ToDescription()].ToString(),
                                                         downLoadDic[EnumDownLoad.ACCESS.ToDescription()].ToString(),
                                                         string.Empty),
                                       new ContainerPack(downLoadDic[EnumDownLoad.PROJECTID.ToDescription()].ToString(),
                                                         downLoadDic[EnumDownLoad.TEAMID.ToDescription()].ToString(),
                                                         downLoadDic[EnumDownLoad.CONTAINERID.ToDescription()].ToString(),
                                                         string.Empty,
                                                         string.Empty),
                                       new FilePack(downLoadDic[EnumDownLoad.FILEID.ToDescription()].ToString(),
                                                    downLoadDic[EnumDownLoad.FILENAME.ToDescription()].ToString()));

                // 파일 다운로드 처리 중 화면 출력
                AppSetting.Default.WindowBase.ShowCommonLoadingWork(CobimHelper.DownLoad_LoadingWork, _Palette);

                // 파일 다운로드 진행
                await WebView2ExplorerRestServer.GetFileDownLoadAsync(InputParamData);

                // TODO : 아래 오류 메시지 해결하기 위해 Dispatcher.Invoke 사용 (2024.11.28 jbh)
                // 오류 메시지 - "다른 스레드가 이 개체를 소유하고 있어 호출한 스레드가 해당 개체에 액세스할 수 없습니다"
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.threading.dispatcher.invoke?view=windowsdesktop-6.0
                // 참고 2 URL - https://insurang.tistory.com/entry/WPF-C-Error-%EB%8B%A4%EB%A5%B8-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%EC%9D%B4-%EA%B0%9C%EC%B2%B4%EB%A5%BC-%EC%86%8C%EC%9C%A0%ED%95%98%EA%B3%A0-%EC%9E%88%EC%96%B4-%ED%98%B8%EC%B6%9C%ED%95%9C-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%ED%95%B4%EB%8B%B9-%EA%B0%9C%EC%B2%B4%EC%97%90-%EC%95%A1%EC%84%B8%EC%8A%A4%ED%95%A0-%EC%88%98-%EC%97%86%EC%8A%B5%EB%8B%88%EB%8B%A4-%EC%97%90%EB%9F%AC-%EC%93%B0%EB%A0%88%EB%93%9C-Thread-DispatcherInvoke
                _CommonLoadingWorkV.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    resMsg = "success";  // 파일 다운로드 성공
                    MessageBox.Show("파일 다운로드 완료!");
                }));

                // 테스트용 다운로드 처리 중 화면이 정상 출력되는지 1분간 대기 
                // await Task.Delay(TimeSpan.FromMinutes(1));

                // 메세지 리턴 방식
                // 1 성공
                // code : "success",
                // msg: "success"

                // 2 실패
                // code : "fail",
                // msg: 인재inc Rest API 오류 메시지
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);

                // TODO : 오류 발생시 메시지 박스 출력 구현 (2024.10.18 jbh)
                // 메시지 박스 출력 사유
                // - COBIM 웹 탐색기 JSP 파일에서 오류 메시지를 Alert 으로 대신 출력할 경우
                // AutoCAD 응용 프로그램 -> AutoCAD 문서의 백그라운드에서 돌고있는 UI쓰레드와 충돌이 발생하여
                // AutoCAD 응용 프로그램 자체가 종료(ShutDown)되는 현상이 발생함.
                // 해당 오류 해결 방법
                // 1. AutoCAD 2025 응용 프로그램 최신 버전 업데이트
                // 2. 해당 오류를 개선하기 위해 JSP 파일에서 오류 메시지를 Alert 으로 출력 안 하고
                // 아래처럼 MessageBox.Show(ex.Message, CobimHelper.ErrorTitle); 형식으로 출력함.
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);

                resMsg = ex.Message;  // 인재 INC로 Rest API 통신 결과 오류 메시지 발생시 string 클래스 객체 resMsg에 할당
            }
            finally
            {
                // 파일 다운로드 처리 중이 아닌 경우 
                if (resMsg != CobimHelper.DownLoad_LoadingWork)
                {
                    IsDownLoadWorking = false;   // 파일 다운로드 처리 중 종료
                    AppSetting.Default.WindowBase.HideCommonLoadingWork(_Palette);   // 파일 다운로드 처리 중 화면 숨기기
                }
            }
            return resMsg;   // 결과 메시지 리턴
        }

        #endregion DownLoadAsync

        #region CheckOutAsync

        /// <summary>
        /// 컨테이너 체크아웃 + (참조/연관) 관계 컨테이너 파일 다운로드  
        /// </summary>
        // TODO : WebView2 웹 기술 사용시 아래처럼 비동기 메서드 "CheckOutAsync"의 메서드 파라미터 "object jsonObj" 선언 불가 
        //        왜냐하면 파라미터 유형으로 "object" 선언시 WebView2 웹 기술 라이브러리가 "object" 유형을 인식 못하고 
        //        자바스크립트 소스코드에서 넘어오는 입력 파라미터 값을 제대로 못 넘겨주는 오류가 발생하기 때문
        //        하여 메서드 파라미터 선언 방식을 (기존) "object jsonObj" -> (변경) "string jsonObj" 필요 (2024.11.20 jbh)
        //public async Task<string> CheckOutAsync(object jsonObj)
        public async Task<string> CheckOutAsync(string jsonObj)
        {
            // TODO : Json 문자열 객체 jsonObj -> Dictionary<string, object?> 객체로 복원(JsonSerializer.Deserialize) 구현 (2024.11.20 jbh)
            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.text.json.jsonserializer.deserialize?view=net-8.0#system-text-json-jsonserializer-deserialize-1(system-string-system-text-json-jsonserializeroptions)
            // 참고 2 URL - https://www.csharpstudy.com/Data/Json-SystemTextJson.aspx
            Dictionary<string, object?> checkOutDic = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonObj);

            string checkOutContainerId = string.Empty;           // 체크아웃 요청 컨테이너 아이디

            string resMsg = string.Empty;                        // 컨테이너 체크아웃 + (참조/연관) 관계 컨테이너 파일 다운로드 처리 결과(성공 또는 실패) 문자열 초기화

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // 연관 컨테이너 파일 다운로드 처리 중인 상태에서 다른 컨테이너의 체크아웃을 중복 요청하는 경우 
                if (true == IsLinkedContainerWorking)
                    throw new Exception(CobimHelper.LinkedContainer_LoadingWork);

                // 체크아웃 처리 중인 경우 
                if (true == IsCheckOutWorking)
                    throw new Exception(CobimHelper.CheckOut_LoadingWork);

                IsCheckOutWorking = true;   // 체크아웃 처리 중 시작

                // 체크아웃 요청 컨테이너 아이디 문자열 값 string 클래스 객체 checkOutContainerId에 할당 
                checkOutContainerId = checkOutDic[EnumCheckOut.CONTAINERID.ToDescription()].ToString();

                // COBIM 웹 탐색기 데이터 모델 객체 InputParamData에 데이터셋 새로 할당
                InputParamData.SetData(new RequestHeader(checkOutDic[EnumCheckOut.TENANT.ToDescription()].ToString(),
                                                         checkOutDic[EnumCheckOut.AUTHORIZATION.ToDescription()].ToString(),
                                                         checkOutDic[EnumCheckOut.ACCESS.ToDescription()].ToString(),
                                                         string.Empty),
                                       new ContainerPack(checkOutDic[EnumCheckOut.PROJECTID.ToDescription()].ToString(),
                                                         checkOutDic[EnumCheckOut.TEAMID.ToDescription()].ToString(),
                                                         checkOutDic[EnumCheckOut.CONTAINERID.ToDescription()].ToString(),
                                                         string.Empty,
                                                         string.Empty),
                                       new FilePack(checkOutDic[EnumCheckOut.FILEID.ToDescription()].ToString(),
                                                    checkOutDic[EnumCheckOut.FILENAME.ToDescription()].ToString()));

                // 컨테이너 체크아웃 + (참조/연관) 관계 컨테이너 파일 다운로드 처리 중 화면 출력
                AppSetting.Default.WindowBase.ShowCommonLoadingWork(CobimHelper.CheckOut_LoadingWork, _Palette);

                // Rest API 10.컨테이너 체크아웃 + 다운로드 호출
                // 체크아웃 호출을 위해 만족 해야할 사항(조건)
                // 1) 컨테이너 생성자 == 체크아웃 요청자(로그인 사용자 아이디)
                // 2) 컨테이너 readYn 필드 값이 'N' 일 때
                // 3) 검토 요청 중인 컨테이너가 아닐 때(statusCode != 'comstt20')
                await WebView2ExplorerRestServer.GetCheckOutAsync(InputParamData);

                // 컨테이너 체크아웃 + (참조/연관) 관계 컨테이너 파일 다운로드 처리 중 화면 숨기기
                AppSetting.Default.WindowBase.HideCommonLoadingWork(_Palette);

                // 체크아웃 요청한 컨테이너 아이디와 연관된 컨테이너 목록 조회
                // (Rest API 8.연관 컨테이너 목록 조회 호출) 
                var linkedContainerList = await WebView2ExplorerRestServer.GetLinkedContainerListAsync(InputParamData);

                // 연관 컨테이너 목록이 존재하지 않는 경우 
                // if(true == linkedContainerList.Count.Equals((int)EnumLinkedRelation.NONE))
                if (linkedContainerList.Count == (int)EnumLinkedRelation.NONE)
                    MessageBox.Show($"컨테이너 아이디 {checkOutContainerId}\r\n체크아웃 완료!\r\n연관 컨테이너 목록이 존재하지 않습니다.");

                // TODO : 아래 주석친 테스트 코드 (연관 컨테이너가 존재하는 경우) 필요시 참고 (2024.10.08 jbh)
                // TODO : 아래 연관 컨테이너가 존재하는 경우 리스트 객체 "linkedContainerList" 컨테이너 아이디(containerId) 오름차순 정렬 (Sort 메서드) (2024.10.08 jbh)
                // 참고 URL - https://www.techiedelight.com/ko/sort-list-in-descending-order-in-csharp/
                // 연관 컨테이너 목록이 존재하는 경우 
                // if (linkedContainerList.Count >= (int)EnumLinkedRelation.EXISTENCE)
                //     linkedContainerList.Sort((containerA, containerB) => containerA[ContainerHelper.containerId].ToString().CompareTo(containerB[ContainerHelper.containerId].ToString()));

                // 연관 컨테이너 목록이 존재하는 경우 
                // + 연관 컨테이너 파일 다운로드 처리 중 화면 출력 
                // if(true == (linkedContainerList.Count >= (int)EnumLinkedRelation.EXISTENCE) && false == IsLinkedContainerWorking)
                if (linkedContainerList.Count >= (int)EnumLinkedRelation.EXISTENCE)
                {
                    IsLinkedContainerWorking = true;   // 연관 컨테이너 파일 다운로드 처리 중 시작

                    AppSetting.Default.WindowBase.ShowCommonLoadingWork(CobimHelper.LinkedContainer_LoadingWork, _Palette);

                    foreach (var linkedContainer in linkedContainerList)
                    {
                        // TODO : 다운로드할 파일 정보 프로퍼티 "_FilePack"에 속한 프로퍼티 "_FileId", "_FileName"에 문자열 재할당 (2024.10.08 jbh)
                        // 파일 아이디
                        InputParamData.GetFilePack().SetFileId(linkedContainer[ContainerHelper.fileId].ToString());

                        // 파일명(이름)   
                        InputParamData.GetFilePack().SetFileName(linkedContainer[ContainerHelper.fileNm].ToString());

                        // TODO : 연관 컨테이너 다운로드 하는 경우 다운로드 받은 파일의 상위 폴더명(컨테이너 아이디)을
                        //        각각의 연관 컨테이너 아이디가 아니라 루트 컨테이너 아이디(체크아웃 요청한 컨테이너 아이디)
                        //        하나로만 상위 폴더명으로 설정 해야 하므로 아래 소스코드는 주석 처리 진행 (2024.10.08 jbh)
                        // 체크아웃 요청한 컨테이너 아이디 
                        // InputParamData.GetContainerPack().SetContainerId(linkedContainer[ContainerHelper.containerId].ToString());

                        // 연관 컨테이너 아이디
                        InputParamData.GetContainerPack().SetLinkedContainerId(linkedContainer[ContainerHelper.containerId].ToString());

                        string linkedContainerId = InputParamData.GetContainerPack().GetLinkedContainerId();

                        // TODO : 체크아웃 요청한 컨테이너 아이디(checkOutContainerId) 문자열 값과 연관 컨테이너 아이디(linkedContainerId)의 문자열이 동일한 경우 파일 다운로드 대상 제외
                        //        이미 체크아웃 + 파일 다운로드 처리 동시에 하였으므로 파일 중복 다운로드 안 하기 위해 continue 처리 구현 (2024.10.08 jbh)
                        //if(true == InputParamData.GetContainerPack().GetContainerId().Equals(InputParamData.GetContainerPack().GetLinkedContainerId())) continue;
                        if (checkOutContainerId == linkedContainerId) continue;

                        // 연관 컨테이너에 속한 파일 다운로드 진행
                        else await WebView2ExplorerRestServer.GetFileDownLoadAsync(InputParamData);
                    }

                    // TODO : 아래 오류 메시지 해결하기 위해 Dispatcher.Invoke 사용 (2024.11.28 jbh)
                    // 오류 메시지 - "다른 스레드가 이 개체를 소유하고 있어 호출한 스레드가 해당 개체에 액세스할 수 없습니다"
                    // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.threading.dispatcher.invoke?view=windowsdesktop-6.0
                    // 참고 2 URL - https://insurang.tistory.com/entry/WPF-C-Error-%EB%8B%A4%EB%A5%B8-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%EC%9D%B4-%EA%B0%9C%EC%B2%B4%EB%A5%BC-%EC%86%8C%EC%9C%A0%ED%95%98%EA%B3%A0-%EC%9E%88%EC%96%B4-%ED%98%B8%EC%B6%9C%ED%95%9C-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%ED%95%B4%EB%8B%B9-%EA%B0%9C%EC%B2%B4%EC%97%90-%EC%95%A1%EC%84%B8%EC%8A%A4%ED%95%A0-%EC%88%98-%EC%97%86%EC%8A%B5%EB%8B%88%EB%8B%A4-%EC%97%90%EB%9F%AC-%EC%93%B0%EB%A0%88%EB%93%9C-Thread-DispatcherInvoke
                    _CommonLoadingWorkV.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        MessageBox.Show($"컨테이너 아이디 {checkOutContainerId}\r\n체크아웃 완료!\r\n연관 컨테이너 파일 다운로드 완료!");
                    }));
                }

                resMsg = "success";   // 컨테이너 체크아웃 + (참조/연관) 관계 컨테이너 파일 다운로드 성공

                // 메세지 리턴 방식
                // 1 성공
                // code : "success",
                // msg: "success"

                // 2 실패
                // code : "fail",
                // msg: 인재inc Rest API 오류 메시지
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);

                // TODO : 오류 발생시 메시지 박스 출력 구현 (2024.10.18 jbh)
                // 메시지 박스 출력 사유
                // - COBIM 웹 탐색기 JSP 파일에서 오류 메시지를 Alert 으로 대신 출력할 경우
                // AutoCAD 응용 프로그램 -> AutoCAD 문서의 백그라운드에서 돌고있는 UI쓰레드와 충돌이 발생하여
                // AutoCAD 응용 프로그램 자체가 종료(ShutDown)되는 현상이 발생함.
                // 해당 오류 해결 방법
                // 1. AutoCAD 2025 응용 프로그램 최신 버전 업데이트
                // 2. 해당 오류를 개선하기 위해 JSP 파일에서 오류 메시지를 Alert 으로 출력 안 하고
                // 아래처럼 MessageBox.Show(ex.Message, CobimHelper.ErrorTitle); 형식으로 출력함.
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);

                resMsg = ex.Message;  // 인재 INC로 Rest API 통신 결과 오류 메시지 발생시 string 클래스 객체 resMsg에 할당
            }
            finally
            {
                // 컨테이너 체크아웃 또는 (참조/연관) 관계 컨테이너 파일 다운로드 처리 중이 아닌 경우
                // if(true == IsCheckOutWorking || true == IsLinkedContainerWorking)
                if (resMsg != CobimHelper.CheckOut_LoadingWork && resMsg != CobimHelper.LinkedContainer_LoadingWork)
                {
                    IsCheckOutWorking = false;
                    IsLinkedContainerWorking = false;
                    AppSetting.Default.WindowBase.HideCommonLoadingWork(_Palette);   // 컨테이너 체크아웃 + (참조/연관) 관계 컨테이너 파일 다운로드 처리 중 화면 숨기기

                }
            }
            return resMsg;   // 결과 메시지 리턴
        }

        #endregion CheckOutAsync

        #region CheckInAsync

        /// <summary>
        /// 컨테이너 체크인 + (컨테이너에 속한 파일) 웹서버로 업로드 + HubBox 파일 변환 요청   
        /// </summary>
        // TODO : WebView2 웹 기술 사용시 아래처럼 비동기 메서드 "CheckInAsync"의 메서드 파라미터 "object jsonObj" 선언 불가 
        //        왜냐하면 파라미터 유형으로 "object" 선언시 WebView2 웹 기술 라이브러리가 "object" 유형을 인식 못하고 
        //        자바스크립트 소스코드에서 넘어오는 입력 파라미터 값을 제대로 못 넘겨주는 오류가 발생하기 때문
        //        하여 메서드 파라미터 선언 방식을 (기존) "object jsonObj" -> (변경) "string jsonObj" 필요 (2024.11.20 jbh)
        // public async Task<string> CheckInAsync(object jsonObj)
        public async Task<string> CheckInAsync(string jsonObj)
        {
            // TODO : Json 문자열 객체 jsonObj -> Dictionary<string, object?> 객체로 복원(JsonSerializer.Deserialize) 구현 (2024.11.20 jbh)
            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.text.json.jsonserializer.deserialize?view=net-8.0#system-text-json-jsonserializer-deserialize-1(system-string-system-text-json-jsonserializeroptions)
            // 참고 2 URL - https://www.csharpstudy.com/Data/Json-SystemTextJson.aspx
            Dictionary<string, object?> checkInDic = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonObj);

            // string resultTranslateCode = string.Empty;  // 특정 컨테이너에 속한 파일 변환 처리 결과 상태 코드 문자열 초기화
            // string resultTranslateMsg = string.Empty;   // 특정 컨테이너에 속한 파일 변환 처리 관련 공통 메세지 초기화

            // string checkInContainerId = string.Empty;   // 체크인 요청 컨테이너 아이디            
            // string statusCode = string.Empty;           // 컨테이너 검토상태("comstt10": 미진행, "comstt20": 진행, "comstt30": 완료)
            // string readYn     = string.Empty;           // 컨테이너 잠금 상태 ("Y": 잠금 ON, "N": 잠금 OFF)
            // string workUserId = string.Empty;           // 컨테이너 작업중인 사용자 아이디

            string fileName = string.Empty;                // 체크인 처리할 컨테이너에 속한 파일명
            string fileId = string.Empty;                  // 체크인 처리하기 전 파일 아이디  
            string revsnVal = string.Empty;                // 체크인 처리하기 전 파일 버전 

            string checkInFileId = string.Empty;           // 체크인 처리 완료 후 파일 아이디  
            string checkInRevsnVal = string.Empty;         // 체크인 처리 완료 후 파일 버전 

            // TODO : C# 문자열 보간 기능 사용해서 웹서버로 업로드 할 상위 폴더 경로 문자열로 변환 (2024.09.02 jbh)
            // 참고 URL - https://gaeunhan.tistory.com/61
            string dirPath = string.Empty;                 // 웹서버로 업로드 할 파일의 상위 디렉토리(폴더) 경로
            string filePath = string.Empty;                // 웹서버로 업로드 할 파일 전체 경로 

            string deleteContainerId = string.Empty;       // 체크인 완료(파일 웹서버로 업로드 포함) 후 사용자 PC 로컬에 존재하는 삭제할 파일의 상위 컨테이너 아이디
            string deleteDirPath = string.Empty;           // 체크인 완료(파일 웹서버로 업로드 포함) 후 사용자 PC 로컬에 존재하는 삭제할 파일의 상위 디렉토리(폴더) 경로

            string resMsg = string.Empty;                  // 컨테이너 체크인 + (컨테이너에 속한 파일) 웹서버로 업로드 처리 결과(성공 또는 실패) 문자열 초기화

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                if(true == IsTranslateWorking)   // 파일 변환 처리 중인 상태에서 다른 컨테이너의 체크인을 중복 요청하는 경우
                    throw new Exception(CobimHelper.Translate_LoadingWork);

                if(true == IsCheckInWorking)   // 체크인 처리 중인 경우 
                    throw new Exception(CobimHelper.CheckIn_LoadingWork);

                IsCheckInWorking = true;   // 체크인 처리 중 시작 

                // 체크인 Comment 메시지 공백 또는 null인 경우 
                if(true == string.IsNullOrWhiteSpace(checkInDic[EnumCheckIn.COMMENT.ToDescription()].ToString()))
                    throw new Exception(CobimHelper.CheckIn_CommentWhiteSpace);

                // TODO : 다른 PC에서도 파일 업로드 기능이 실행될 수 있도록 웹서버로 업로드 할 파일 전체 경로 "dirPath"의 루트 디렉토리 값을 내문서 폴더 (Environment.SpecialFolder.MyDocuments)로 설정 (2024.10.11 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.environment.specialfolder?view=net-7.0
                // 웹서버로 업로드 할 파일의 상위 디렉토리 폴더 전체 경로 예시 - "C:\Users\bhjeon\Documents\COBIM\CobimPalette\bmcb\inc-001-bmcb-t005\bmcb-imbu-func_016-sptl_901-002"
                // SpecialFolder 시스템 디렉토리(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) 사용
                // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.10.14 jbh)
                // dirPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}" +
                //           $"\\{CobimHelper.CobimBaseDirectory}" + 
                //           $"\\{CobimHelper.AssemblyName}" +
                //           $"\\{checkInDic[EnumCheckIn.PROJECTID.ToDescription()].ToString()}" +
                //           $"\\{checkInDic[EnumCheckIn.TEAMID.ToDescription()].ToString()}" +
                //           $"\\{checkInDic[EnumCheckIn.CONTAINERID.ToDescription()].ToString()}";

                // TODO : StringBuilder 클래스 객체 sbDirPath 생성 -> string 클래스 객체 dirPath에 문자열 추가 (2024.10.15 jbh)
                // StringBuilder vs String 비교 
                // 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%ED%99%9C%EC%9A%A9-String-StringBuilder-%EC%84%B1%EB%8A%A5-%EC%B0%A8%EC%9D%B4
                // StringBuilder sbDirPath = new StringBuilder($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}");
                // sbDirPath.Append($"\\{CobimHelper.AssemblyName}");
                StringBuilder sbDirPath = new StringBuilder($"{_CommonDirectoryPath}");
                sbDirPath.Append($"\\{checkInDic[EnumCheckIn.PROJECTID.ToDescription()].ToString()}");
                sbDirPath.Append($"\\{checkInDic[EnumCheckIn.TEAMID.ToDescription()].ToString()}");
                sbDirPath.Append($"\\{checkInDic[EnumCheckIn.CONTAINERID.ToDescription()].ToString()}");

                dirPath = sbDirPath.ToString();

                // 체크인 처리 하기 전 파일 이름, 파일 아이디, 파일 버전 문자열 할당 
                fileName = checkInDic[EnumCheckIn.FILENAME.ToDescription()].ToString();
                fileId = checkInDic[EnumCheckIn.FILEID.ToDescription()].ToString();
                revsnVal = checkInDic[EnumCheckIn.REVSNVAL.ToDescription()].ToString();

                // TODO : 웹서버로 업로드 할 파일 전체 경로 구현 (2024.10.11 jbh)
                // 참고 URL - https://dlgnlfus.tistory.com/129
                filePath = dirPath + $"\\{fileName}";   // 웹서버로 업로드 할 파일 전체 경로 

                // TODO : 웹서버로 업로드 할 파일 전체 경로에 있는 파일 정보 불러 오도록 구현 (2024.10.11 jbh)
                // 참고 URL   - https://chashtag.tistory.com/37
                // 참고 2 URL - https://www.delftstack.com/ko/howto/csharp/get-filesize-in-csharp/
                // 참고 3 URL - https://dlgnlfus.tistory.com/129
                if(true == File.Exists(filePath))
                {
                    if(true == IsOpenedFile(filePath))   // AutoCAD 파일(확장자 .dwg, .dxf)이 이미 열려있는 경우 
                        throw new Exception($"파일명 : {fileName}\r\n\r\n해당 문서가 열려있는 상태입니다.\r\n문서를 닫은 후 다시 체크인 요청하세요.");

                    _ContainerPack = new ContainerPack(checkInDic[EnumCheckIn.PROJECTID.ToDescription()].ToString(),
                                                       checkInDic[EnumCheckIn.TEAMID.ToDescription()].ToString(),
                                                       checkInDic[EnumCheckIn.CONTAINERID.ToDescription()].ToString(),
                                                       string.Empty,
                                                       string.Empty);

                    // Rest API 컨테이너 체크인 요청하기 전 웹서버로 업로드 할 파일 읽어오기(File.OpenRead)
                    using (FileStream upLoadFileStream = File.OpenRead(filePath))
                    {
                        InputParamData.SetData(new RequestHeader(checkInDic[EnumCheckIn.TENANT.ToDescription()].ToString(),
                                                                 checkInDic[EnumCheckIn.AUTHORIZATION.ToDescription()].ToString(),
                                                                 checkInDic[EnumCheckIn.ACCESS.ToDescription()].ToString(),
                                                                 string.Empty),
                                               _ContainerPack,
                                               new FilePack(string.Empty, // 체크인 처리 요청시 파일 아이디 값 checkInDic[EnumCheckIn.FILEID.ToDescription()].ToString(), 인자로 전달 안해도 됨.
                                                            checkInDic[EnumCheckIn.FILENAME.ToDescription()].ToString(),
                                                            upLoadFileStream.Length.ToString(),
                                                            upLoadFileStream),
                                               new CheckInPack(checkInDic[EnumCheckIn.FOLDERID.ToDescription()].ToString(),
                                                               checkInDic[EnumCheckIn.COMMENT.ToDescription()].ToString()));

                        // 체크인 처리 중 화면 출력 
                        AppSetting.Default.WindowBase.ShowCommonLoadingWork(CobimHelper.CheckIn_LoadingWork, _Palette);

                        // 컨테이너 체크인 + 업로드 
                        var resultCheckInData = await WebView2ExplorerRestServer.PostCheckInAsync(InputParamData);

                        // Dictionary 객체 resultCheckInData 특정 키(Key) "fileId", "revsnVal"에 속하는 값(Value) 문자열로 변환 (2024.10.14 jbh)
                        // 참고 URL - https://developer-talk.tistory.com/694
                        checkInFileId = resultCheckInData[EnumCheckIn.FILEID.ToDescription()].ToString();
                        checkInRevsnVal = resultCheckInData[EnumCheckIn.REVSNVAL.ToDescription()].ToString();

                        // 체크인 처리 중 화면 종료 
                        AppSetting.Default.WindowBase.HideCommonLoadingWork(_Palette);
                    }   // Rest API 컨테이너 체크인 요청 성공후 웹서버로 업로드 할 파일 리소스 해제(Dispose()) 처리 -> 리소스 해제(Dispose()) 해야 사용자 로컬 PC에 설치된 파일 삭제 처리 가능

                    // 컨테이너 체크인 + 업로드 성공한 경우 
                    // && 파일 변환 처리 중 화면 출력 
                    // checkInFileId(체크인 처리 완료 후 파일 아이디) != fileId(체크인 처리하기 전 파일 아이디) 
                    // checkInRevsnVal(체크인 처리 완료 후 파일 버전) != revsnVal(체크인 처리하기 전 파일 버전)
                    // if(checkInFileId != fileId && checkInRevsnVal != revsnVal && false == IsTranslateWorking)
                    if(checkInFileId != fileId && checkInRevsnVal != revsnVal)
                    {
                        IsTranslateWorking = true;   // 파일 변환 처리 시작

                        // 체크인 완료된 이후 변경된 파일 아이디 값(checkInFileId)을 메서드 "InputParamData.GetFilePack().SetFileId()"에 인자 값으로 전달
                        InputParamData.GetFilePack().SetFileId(checkInFileId);

                        // HubBox 파일 변환 정보(TranslatePack) 탐색기 데이터 모델 객체 InputParamData에 값 셋팅
                        InputParamData.SetTranslatePack(new TranslatePack(_ContainerPack.GetProjectId(),
                                                                          _ContainerPack.GetTeamId(),
                                                                          _ContainerPack.GetContainerId(),
                                                                          checkInFileId));

                        // checkInContainerId = translateDatas[(int)EnumTranslate.CONTAINERID.ToDescription()].ToString();
                        // MessageBox.Show($"컨테이너 아이디 {checkInContainerId}\r\n체크인 완료!\r\n파일 변환 요청 시작합니다.");

                        AppSetting.Default.WindowBase.ShowCommonLoadingWork(CobimHelper.Translate_LoadingWork, _Palette);

                        // HubBox Rest API 3. 컨테이너에 속한 특정 파일 변환 API 호출 
                        var resultTranslateData = await HubBoxRestServer.PostTranslateAsync(InputParamData);

                        // TODO : 아래 주석친 코드 필요시 참고 (2024.10.31 jbh)
                        // TODO : Dictionary 객체 resultTranslateData에 담긴 파일 변환 API 결과 값 중 파일 변환 성공/처리 중일 경우 키 "code"에 매핑된 값으로 할당
                        //        만약 파일 변환 실패했을 경우 키 "status"에 매핑된 값으로 할당 하도록 삼항연산자 사용하여 로직 구현하기 (2024.10.14 jbh) 
                        // bool result =  resultTranslateData.TryGetValue(resultTranslateData[EnumTranslateResult.CODE.ToDescription()], out resultTranslateCode))
                        // resultTranslateCode = true == resultTranslateData.ContainsKey(EnumTranslateResult.CODE.ToDescription()) ? 
                        //                         resultTranslateData[EnumTranslateResult.CODE.ToDescription()].ToString() : 
                        //                       (true == resultTranslateData.ContainsKey(EnumTranslateResult.STATUS.ToDescription()) ? resultTranslateData[EnumTranslateResult.STATUS.ToDescription()].ToString() : null);

                        // if(true == resultTranslateData.ContainsKey(EnumTranslateResult.CODE.ToDescription()))
                        //     resultTranslateCode = resultTranslateData[EnumTranslateResult.CODE.ToDescription()].ToString();

                        // else if(true == resultTranslateData.ContainsKey(EnumTranslateResult.STATUS.ToDescription()))
                        //     resultTranslateCode = resultTranslateData[EnumTranslateResult.STATUS.ToDescription()].ToString();

                        // else resultTranslateCode = null;

                        // TryGetValue vs ContainsKey 차이
                        // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.collections.generic.dictionary-2.trygetvalue?view=net-8.0
                        // 참고 2 URL - https://stackoverflow.com/questions/38377318/contains-in-dictionary-in-c-sharp
                        // 참고 3 URL - https://ckhyeok.tistory.com/37
                        // 참고 4 URL - https://m.blog.naver.com/PostView.nhn?blogId=crusaderholy&logNo=100112263890&proxyReferer=https:%2F%2Fwww.google.com%2F
                        // 참고 5 URL - https://coding-shop.tistory.com/53
                        // 참고 6 URL - https://developer-talk.tistory.com/686

                        // 컨테이너에 속한 특정 파일 변환 Dictionary 객체 resultTranslateData 에
                        // 키값 "code", "status" 둘 다 포함되지 않은 경우 
                        // if(false == resultTranslateData.TryGetValue(EnumTranslateResult.CODE.ToDescription(), out resultTranslateCode)
                        //    && false == resultTranslateData.TryGetValue(EnumTranslateResult.STATUS.ToDescription(), out resultTranslateCode)) 
                        //      resultTranslateCode = null;

                        // resultTranslateMsg =  $"* 파일 변환 안내 *"
                        //                     + $"\r\n\r\n처리 결과 - {resultTranslateData[EnumTranslateResult.MESSAGE.ToDescription()]} ({resultTranslateCode})"
                        //                     + $"\r\n컨테이너 아이디  - {translateDatas[(int)EnumTranslate.CONTAINERID].ToUpper()}"
                        //                     + $"\r\n제목 - {translateDatas[(int)EnumTranslate.TITLE]}"
                        //                     + $"\r\n파일 이름 - {translateDatas[(int)EnumTranslate.TRANSLATE_FILENAME]}";

                        // MessageBox.Show(resultTranslateMsg);   // 파일 변환 처리 결과 메세지 출력 
                    }
                    // 컨테이너 체크인 + 업로드 실패한 경우
                    else throw new Exception("체크인 오류 발생!\r\n담당자에게 문의하세요.");

                    // MessageBox.Show("체크인 완료!");

                    // TODO : 체크인 완료(+ 컨테이너에 속한 파일 웹서버로 업로드) 후 파일 삭제 기능이 실행될 수 있도록 삭제 할 파일의 상위 폴더 경로 "deleteDirPath"에 변수 dirPath로 할당 (2024.10.14 jbh)
                    // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.environment.specialfolder?view=net-7.0
                    // 삭제할 파일의 상위 폴더 전체 경로 예시 - "C:\Users\bhjeon\Documents\COBIM\CobimPalette\bmcb\inc-001-bmcb-t005\bmcb-imbu-func_016-sptl_901-002"
                    // SpecialFolder 시스템 디렉토리(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) 사용
                    deleteDirPath = dirPath;

                    // 삭제할 파일의 상위 컨테이터 폴더(deleteDirPath)가 존재하는 경우 
                    if (true == Directory.Exists(deleteDirPath))
                    {
                        deleteContainerId = checkInDic[EnumCheckIn.CONTAINERID.ToDescription()].ToString();
                        // MessageBox.Show($"로컬에 존재하는 루트 컨테이너 폴더({deleteContainerId})\r\n다운로드 받은 파일 삭제 진행합니다.");

                        // TODO : 체크인 완료 후(+ 파일 웹서버 업로드 완료) 로컬 PC에 다운로드 받은 기존 루트 컨테이너 폴더(deleteContainerId) 및 폴더에 속한 모든 파일 삭제 
                        // TODO : 폴더(디렉토리) 클래스 "Directory", "DirectoryInfo" 차이점 비교 후 static 클래스 "Directory" 사용 (2024.10.14 jbh)
                        // 참고 URL - https://nomad-programmer.tistory.com/207
                        // TODO : Directory.Delete 메서드의 2번째 파라미터 값 설정 구현 (2024.08.30 jbh)
                        //        deleteDirPath 경로의 폴더(디렉터리), 해당 폴더의 하위 폴더(디렉터리) 및 파일을 제거하려면 true, 제거하지 않으려면 false 설정
                        //        true로 설정하면 해당 폴더의 하위 폴더(디렉터리) 및 파일을 제거할 때, 재귀적으로 (recursive) Directory.Delete 메서드 실행 
                        // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.io.directory.delete?view=net-8.0
                        Directory.Delete(deleteDirPath, true);

                        // 루트 컨테이너 폴더(deleteContainerId) + 다운로드 받은 파일 삭제 완료한 경우 
                        if(false == Directory.Exists(deleteDirPath))
                        {
                            // TODO : 아래 오류 메시지 해결하기 위해 Dispatcher.Invoke 사용 (2024.11.28 jbh)
                            // 오류 메시지 - "다른 스레드가 이 개체를 소유하고 있어 호출한 스레드가 해당 개체에 액세스할 수 없습니다"
                            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.threading.dispatcher.invoke?view=windowsdesktop-6.0
                            // 참고 2 URL - https://insurang.tistory.com/entry/WPF-C-Error-%EB%8B%A4%EB%A5%B8-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%EC%9D%B4-%EA%B0%9C%EC%B2%B4%EB%A5%BC-%EC%86%8C%EC%9C%A0%ED%95%98%EA%B3%A0-%EC%9E%88%EC%96%B4-%ED%98%B8%EC%B6%9C%ED%95%9C-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%ED%95%B4%EB%8B%B9-%EA%B0%9C%EC%B2%B4%EC%97%90-%EC%95%A1%EC%84%B8%EC%8A%A4%ED%95%A0-%EC%88%98-%EC%97%86%EC%8A%B5%EB%8B%88%EB%8B%A4-%EC%97%90%EB%9F%AC-%EC%93%B0%EB%A0%88%EB%93%9C-Thread-DispatcherInvoke
                            _CommonLoadingWorkV.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                            {
                                MessageBox.Show($"체크인 완료!\r\n루트 컨테이너 폴더 ({deleteContainerId})\r\n다운로드 받은 파일 삭제 완료");
                            }));
                        }
                        // 루트 컨테이너 폴더(deleteContainerId) + 다운로드 받은 파일 삭제 실패한 경우 
                        else throw new Exception($"[오류] 루트 컨테이너 폴더 ({deleteContainerId})\r\n삭제 오류\r\n담당자에게 문의하세요.");
                    }
                    resMsg = "success";   // 컨테이너 체크인  + (컨테이너에 속한 파일) 웹서버로 업로드 성공
                }
                else throw new Exception("[오류] 컨테이너 체크인 + 업로드 파일이 존재하지 않습니다.\r\n다시 확인 하세요.");


                // 메세지 리턴 방식
                // 1 성공
                // code : "success",
                // msg: "success"

                // 2 실패
                // code : "fail",
                // msg: 인재inc Rest API 오류 메시지
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);

                // TODO : 오류 발생시 메시지 박스 출력 구현 (2024.10.18 jbh)
                // 메시지 박스 출력 사유
                // - COBIM 웹 탐색기 JSP 파일에서 오류 메시지를 Alert 으로 대신 출력할 경우
                // AutoCAD 응용 프로그램 -> AutoCAD 문서의 백그라운드에서 돌고있는 UI쓰레드와 충돌이 발생하여
                // AutoCAD 응용 프로그램 자체가 종료(ShutDown)되는 현상이 발생함.
                // 해당 오류 해결 방법
                // 1. AutoCAD 2025 응용 프로그램 최신 버전 업데이트 
                // 2. 해당 오류를 개선하기 위해 JSP 파일에서 오류 메시지를 Alert 으로 출력 안 하고
                // 아래처럼 MessageBox.Show(ex.Message, CobimHelper.ErrorTitle); 형식으로 출력함.
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);

                resMsg = ex.Message;  // 인재 INC로 Rest API 통신 결과 오류 메시지 발생시 string 클래스 객체 resMsg에 할당
            }
            finally
            {
                // 컨테이너 체크인 + (컨테이너에 속한 파일) 웹서버로 업로드 처리 중이 아닌 경우
                // && 파일 변환 처리 중이 아닌 경우
                if (resMsg != CobimHelper.CheckIn_LoadingWork && resMsg != CobimHelper.Translate_LoadingWork)
                {
                    // 체크인 처리 중 또는 파일 변환 처리 완료
                    IsCheckInWorking = false;
                    IsTranslateWorking = false;
                    AppSetting.Default.WindowBase.HideCommonLoadingWork(_Palette);
                }
            }
            return resMsg;   // 결과 메시지 리턴
        }

        #endregion CheckInAsync

        #region CancelWorkAsync

        /// <summary>
        /// 컨테이너 작업 취소
        /// </summary>
        // TODO : WebView2 웹 기술 사용시 아래처럼 비동기 메서드 "CancelWorkAsync"의 메서드 파라미터 "object jsonObj" 선언 불가 
        //        왜냐하면 파라미터 유형으로 "object" 선언시 WebView2 웹 기술 라이브러리가 "object" 유형을 인식 못하고 
        //        자바스크립트 소스코드에서 넘어오는 입력 파라미터 값을 제대로 못 넘겨주는 오류가 발생하기 때문
        //        하여 메서드 파라미터 선언 방식을 (기존) "object jsonObj" -> (변경) "string jsonObj" 필요 (2024.11.20 jbh)
        // public async Task<string> CancelWorkAsync(object jsonObj)
        public async Task<string> CancelWorkAsync(string jsonObj)
        {
            // TODO : Json 문자열 객체 jsonObj -> Dictionary<string, object?> 객체로 복원(JsonSerializer.Deserialize) 구현 (2024.11.20 jbh)
            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.text.json.jsonserializer.deserialize?view=net-8.0#system-text-json-jsonserializer-deserialize-1(system-string-system-text-json-jsonserializeroptions)
            // 참고 2 URL - https://www.csharpstudy.com/Data/Json-SystemTextJson.aspx
            Dictionary<string, object?> cancelWorkDic = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonObj);

            string fileName = string.Empty;                      // 컨테이너 작업 취소 처리할 컨테이너에 속한 파일명

            // TODO : C# 문자열 보간 기능 사용해서 삭제 처리 할 파일 + 상위 디렉토리(폴더) 경로 문자열로 변환 (2023.12.20 jbh)
            // 참고 URL - https://gaeunhan.tistory.com/61
            string dirPath = string.Empty;                       // 컨테이너 작업 취소 처리할 파일의 상위 디렉토리(폴더) 경로
            string filePath = string.Empty;                      // 컨테이너 작업 취소 처리할 파일 전체 경로 

            string deleteContainerId = string.Empty;             // 컨테이너 작업 취소 완료 후 사용자 PC 로컬에 존재하는 삭제할 파일의 상위 컨테이너 아이디
            string deleteDirPath = string.Empty;                 // 컨테이너 작업 취소 완료 후 사용자 PC 로컬에 존재하는 삭제할 파일의 상위 디렉토리(폴더) 경로

            string resMsg = string.Empty;                        // 컨테이너 작업 취소 처리 결과(성공 또는 실패) 문자열 초기화

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                if (true == IsCancelWorking)   // 컨테이너 작업 취소 처리 중인 경우 
                    throw new Exception(CobimHelper.CancelWork_LoadingWork);

                IsCancelWorking = true;   // 컨테이너 작업 취소 처리 중 시작 

                // TODO : 다른 PC에서도 루트 컨테이너 폴더 및 폴더에 속한 모든 파일 삭제 기능이 실행될 수 있도록 업로드 할 파일 전체 경로 "dirPath"의 루트 디렉토리 값을 내문서 폴더 (Environment.SpecialFolder.MyDocuments)로 설정 (2023.12.20 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.environment.specialfolder?view=net-7.0
                // 파일 업로드 전체 경로 예시 - "C:\Users\bhjeon\Documents\COBIM\CobimPalette\bmcb\inc-001-bmcb-t005\bmcb-imbu-func_016-sptl_901-002" - (SpecialFolder 시스템 디렉토리(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) 사용)
                // dirPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\COBIM\\{Assembly.GetEntryAssembly().GetName().Name}\\{ContainerPack.projectId}\\{ContainerPack.teamId}\\{ContainerPack.containerId}";

                // TODO : StringBuilder 클래스 객체 sbDirPath 생성 -> string 클래스 객체 dirPath에 문자열 추가 (2024.10.15 jbh)
                // StringBuilder vs String 비교 
                // 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%ED%99%9C%EC%9A%A9-String-StringBuilder-%EC%84%B1%EB%8A%A5-%EC%B0%A8%EC%9D%B4
                // StringBuilder sbDirPath = new StringBuilder($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}");
                // sbDirPath.Append($"\\{CobimHelper.AssemblyName}");
                StringBuilder sbDirPath = new StringBuilder($"{_CommonDirectoryPath}");
                sbDirPath.Append($"\\{cancelWorkDic[EnumCancelWork.PROJECTID.ToDescription()]}");
                sbDirPath.Append($"\\{cancelWorkDic[EnumCancelWork.TEAMID.ToDescription()]}");
                sbDirPath.Append($"\\{cancelWorkDic[EnumCancelWork.CONTAINERID.ToDescription()]}");

                dirPath = sbDirPath.ToString();

                fileName = cancelWorkDic[EnumCancelWork.FILENAME.ToDescription()].ToString();

                // TODO : 컨테이너 작업 취소 처리할 파일 전체 경로 구현 (2023.12.04 jbh)
                // 참고 URL - https://dlgnlfus.tistory.com/129
                filePath = dirPath + $"\\{fileName}";   // 컨테이너 작업 취소 처리할 파일 전체 경로 

                if(true == File.Exists(filePath) && true == IsOpenedFile(filePath))   // AutoCAD 파일(확장자 .dwg, .dxf)이 존재하고 이미 열려있는 경우 
                    throw new Exception($"파일명 : {fileName}\r\n\r\n해당 문서가 열려있는 상태입니다.\r\n문서를 닫은 후 다시 작업취소 요청하세요.");

                // 컨테이너 리스트 생성(재조회 - Refresh 기능 포함) 메서드 "ContainerListCreateAsync" 호출시 필요한 인자 "_ContainerPack" 객체 새로 생성 
                _ContainerPack = new ContainerPack(cancelWorkDic[EnumCancelWork.PROJECTID.ToDescription()].ToString(),
                                                   cancelWorkDic[EnumCancelWork.TEAMID.ToDescription()].ToString(),
                                                   cancelWorkDic[EnumCancelWork.CONTAINERID.ToDescription()].ToString(),
                                                   string.Empty,
                                                   string.Empty);

                // Http 웹서버 통신 요청 Header, 컨테이너 작업 취소 정보 (CancelWorkPack) 탐색기 데이터 모델 객체 InputParamData에 값 셋팅
                InputParamData.SetCancelWorkData(new RequestHeader(cancelWorkDic[EnumCancelWork.TENANT.ToDescription()].ToString(),
                                                                   cancelWorkDic[EnumCancelWork.AUTHORIZATION.ToDescription()].ToString(),
                                                                   cancelWorkDic[EnumCancelWork.ACCESS.ToDescription()].ToString(),
                                                                   string.Empty),
                                                 new CancelWorkPack(cancelWorkDic[EnumCancelWork.PROJECTID.ToDescription()].ToString(),
                                                                    cancelWorkDic[EnumCancelWork.TEAMID.ToDescription()].ToString(),
                                                                    cancelWorkDic[EnumCancelWork.CONTAINERID.ToDescription()].ToString()));

                // 컨테이너 작업 취소 처리 중 화면 출력 
                AppSetting.Default.WindowBase.ShowCommonLoadingWork(CobimHelper.CancelWork_LoadingWork, _Palette);

                // TODO : Rest API 17. 컨테이너 작업 취소 Rest API 비동기 메서드 "WebView2ExplorerRestServer.PostCancelWorkAsync" 호출시
                //        출력되는 오류 메시지 "오류 메시지 "{"status":"V000","message":"필수 입력항목이 누락 되었습니다.","error":"com.bim.common.exception.ServiceRuleException"}""
                //        오류 원인 파악 및 해결 하기 (2024.10.31 jbh)
                // 1 단계 : Rest API 17. 컨테이너 작업 취소 호출 
                var resultData = await WebView2ExplorerRestServer.PostCancelWorkAsync(InputParamData);


                // TODO : 컨테이너 작업 취소 결과 확인 로직 오류 발생시 수정 예정 (2023.12.21 jbh)
                // 2 단계 : 컨테이너 작업 취소 결과 확인
                // 조건 1 - resultData 값이 null 이 아님
                // 조건 2 - resultData[ContainerHelper.workUserId] 값이 null임.
                // 조건 3 - resultData[ContainerHelper.readYn] 문자열 변환시 'N'과 같아야 함.
                // TODO : if 조건절에 != null 보다 빠른 is not null 연산자 사용 (2023.12.20 jbh)
                // 참고 URL - https://husk321.tistory.com/405
                bool bCancelWork = (resultData is not null)
                                   && (resultData[ContainerHelper.workUserId] is null)
                                   && (resultData[ContainerHelper.readYn].ToString().Equals(ContainerHelper.no));

                // 컨테이너 작업 취소가 성공한 경우 
                if(true == bCancelWork)
                {
                    // MessageBox.Show("컨테이너 작업 취소 완료!");

                    deleteDirPath = dirPath;

                    // 삭제 처리 할 파일 + 상위 디렉토리(폴더)가 존재하는 경우 
                    if (true == Directory.Exists(deleteDirPath))
                    {
                        deleteContainerId = cancelWorkDic[EnumCancelWork.CONTAINERID.ToDescription()].ToString();

                        // MessageBox.Show($"로컬에 존재하는 루트 컨테이너 폴더({deleteContainerId})\r\n다운로드 받은 파일 삭제 진행합니다.");

                        // 3 단계 : 컨테이너 작업 취소 완료 후 로컬 PC에 다운로드 받은 기존 루트 컨테이너 폴더(deleteContainerId) 및 폴더에 속한 모든 파일 삭제 
                        // TODO : 폴더(디렉토리) 클래스 "Directory", "DirectoryInfo" 차이점 비교 후 static 클래스 "Directory" 사용 (2023.12.20 jbh)
                        // 참고 URL - https://nomad-programmer.tistory.com/207
                        // TODO : Directory.Delete 메서드의 2번째 파라미터 값 설정 구현 (2023.12.20 jbh)
                        //        deleteDirPath 경로의 폴더(디렉터리), 해당 폴더의 하위 폴더(디렉터리) 및 파일을 제거하려면 true, 제거하지 않으려면 false 설정
                        //        true로 설정하면 해당 폴더의 하위 폴더(디렉터리) 및 파일을 제거할 때, 재귀적으로 (recursive) Directory.Delete 메서드 실행 
                        // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.io.directory.delete?view=net-8.0
                        Directory.Delete(deleteDirPath, true);

                        // 루트 컨테이너 폴더(deleteContainerId) + 다운로드 받은 파일 삭제 완료한 경우 
                        if(false == Directory.Exists(deleteDirPath))
                        {
                            // TODO : 아래 오류 메시지 해결하기 위해 Dispatcher.Invoke 사용 (2024.09.27 jbh)
                            // 오류 메시지 - "다른 스레드가 이 개체를 소유하고 있어 호출한 스레드가 해당 개체에 액세스할 수 없습니다"
                            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.threading.dispatcher.invoke?view=windowsdesktop-6.0
                            // 참고 2 URL - https://insurang.tistory.com/entry/WPF-C-Error-%EB%8B%A4%EB%A5%B8-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%EC%9D%B4-%EA%B0%9C%EC%B2%B4%EB%A5%BC-%EC%86%8C%EC%9C%A0%ED%95%98%EA%B3%A0-%EC%9E%88%EC%96%B4-%ED%98%B8%EC%B6%9C%ED%95%9C-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%ED%95%B4%EB%8B%B9-%EA%B0%9C%EC%B2%B4%EC%97%90-%EC%95%A1%EC%84%B8%EC%8A%A4%ED%95%A0-%EC%88%98-%EC%97%86%EC%8A%B5%EB%8B%88%EB%8B%A4-%EC%97%90%EB%9F%AC-%EC%93%B0%EB%A0%88%EB%93%9C-Thread-DispatcherInvoke
                            _CommonLoadingWorkV.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                            {
                                MessageBox.Show($"컨테이너 작업 취소 완료!\r\n루트 컨테이너 폴더 ({deleteContainerId})\r\n다운로드 받은 파일 삭제 완료");
                            }));
                        }
                        else throw new Exception($"[오류] 루트 컨테이너 폴더 ({deleteContainerId})\r\n삭제 오류\r\n담당자에게 문의하세요.");
                    }
                    resMsg = "success";   // 컨테이너 작업 취소 성공
                }
                // 컨테이너 작업 취소 실패한 경우 
                else throw new Exception($"[오류] 컨테이너 작업 취소 실패\r\n담당자에게 문의 하세요.");
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);

                // TODO : 오류 발생시 메시지 박스 출력 구현 (2024.10.18 jbh)
                // 메시지 박스 출력 사유
                // - COBIM 웹 탐색기 JSP 파일에서 오류 메시지를 Alert 으로 대신 출력할 경우
                // AutoCAD 응용 프로그램 -> AutoCAD 문서의 백그라운드에서 돌고있는 UI쓰레드와 충돌이 발생하여
                // AutoCAD 응용 프로그램 자체가 종료(ShutDown)되는 현상이 발생함.
                // 해당 오류 해결 방법
                // 1. AutoCAD 2025 응용 프로그램 최신 버전 업데이트
                // 2. 해당 오류를 개선하기 위해 JSP 파일에서 오류 메시지를 Alert 으로 출력 안 하고
                // 아래처럼 MessageBox.Show(ex.Message, CobimHelper.ErrorTitle); 형식으로 출력함.
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);

                resMsg = ex.Message;  // 인재 INC로 Rest API 통신 결과 오류 메시지 발생시 string 클래스 객체 resMsg에 할당
            }
            finally
            {
                // 컨테이너 작업 취소 처리 중이 아닌 경우 
                if(resMsg != CobimHelper.CancelWork_LoadingWork)
                {
                    IsCancelWorking = false;  // 컨테이너 작업 취소 처리 완료  
                    AppSetting.Default.WindowBase.HideCommonLoadingWork(_Palette);
                }
            }
            return resMsg;   // 결과 메시지 리턴
        }

        #endregion CancelWorkAsync

        #region OpenFileAsync

        /// <summary>
        /// 컨테이너에 속한 AutoCAD 파일(확장자 .dwg, .dxf) 열기
        /// </summary>
        // TODO : WebView2 웹 기술 사용시 아래처럼 비동기 메서드 "OpenFileAsync"의 메서드 파라미터 "object jsonObj" 선언 불가 
        //        왜냐하면 파라미터 유형으로 "object" 선언시 WebView2 웹 기술 라이브러리가 "object" 유형을 인식 못하고 
        //        자바스크립트 소스코드에서 넘어오는 입력 파라미터 값을 제대로 못 넘겨주는 오류가 발생하기 때문
        //        하여 메서드 파라미터 선언 방식을 (기존) "object jsonObj" -> (변경) "string jsonObj" 필요 (2024.11.20 jbh)
        // public async Task<string> OpenFileAsync(object jsonObj)
        public async Task<string> OpenFileAsync(string jsonObj)
        {
            // TODO : Json 문자열 객체 jsonObj -> Dictionary<string, object?> 객체로 복원(JsonSerializer.Deserialize) 구현 (2024.11.20 jbh)
            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.text.json.jsonserializer.deserialize?view=net-8.0#system-text-json-jsonserializer-deserialize-1(system-string-system-text-json-jsonserializeroptions)
            // 참고 2 URL - https://www.csharpstudy.com/Data/Json-SystemTextJson.aspx
            Dictionary<string, object?> openFileDic = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonObj);

            string fileName = string.Empty;                      // 파일(확장자 .dwg, .dxf) 열기 처리할 컨테이너에 속한 파일명

            string extensionName = string.Empty;                 // 파일열기 처리할 파일 확장자 이름(.dwg, .dxf 등등...)

            // TODO : C# 문자열 보간 기능 사용해서 파일열기 처리할 파일 + 상위 디렉토리(폴더) 경로 문자열로 변환 (2024.10.15 jbh)
            // 참고 URL - https://gaeunhan.tistory.com/61
            string dirPath = string.Empty;                       // 파일(확장자 .dwg, .dxf) 열기 처리할 상위 디렉토리(폴더) 경로
            string filePath = string.Empty;                      // 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 

            DocumentCollection docManager = null;                // AutoCAD 문서(확장자 .dwg, .dxf) 관리 클래스 객체 초기화
            // Document doc = Application.DocumentManager.MdiActiveDocument;   // 현재 활성화 된(열려있는) AutoCAD 문서(확장자 .dwg, .dxf)
            // Database db = doc.Database;                                     // 현재 활성화 된(열려있는) AutoCAD 문서(확장자 .dwg, .dxf)에 속한 데이터베이스 객체 

            string resMsg = string.Empty;                        // 컨테이너에 속한 파일(확장자 .dwg, .dxf) AutoCAD 응용 프로그램 열기 처리 결과(성공 또는 실패) 문자열 초기화

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                if(true == IsOpenFileWorking)  // 파일열기 처리 중인 경우
                    throw new Exception(CobimHelper.OpenFile_LoadingWork);

                IsOpenFileWorking = true;   // 컨테이너에 속한 파일(확장자 .dwg, .dxf) 열기 처리 시작

                // TODO : 다른 PC에서도 파일(확장자 .dwg, .dxf) 열기 기능이 실행될 수 있도록 파일 전체 경로 "dirPath"의 루트 디렉토리 값을 내문서 폴더 (Environment.SpecialFolder.MyDocuments)로 설정 (2024.10.15 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.environment.specialfolder?view=net-7.0
                // 파일(확장자 .dwg, .dxf) 열기 처리할 상위 디렉토리 폴더 전체 경로 예시 - "C:\Users\bhjeon\Documents\CobimPalette\bmcb\inc-001-bmcb-t005\bmcb-imbu-func_016-sptl_901-002"

                // TODO : StringBuilder 클래스 객체 sbDirPath 생성 -> string 클래스 객체 dirPath에 문자열 추가 (2024.10.15 jbh)
                // StringBuilder vs String 비교 
                // 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%ED%99%9C%EC%9A%A9-String-StringBuilder-%EC%84%B1%EB%8A%A5-%EC%B0%A8%EC%9D%B4
                //StringBuilder sbDirPath = new StringBuilder($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}");
                //sbDirPath.Append($"\\{CobimHelper.AssemblyName}");
                StringBuilder sbDirPath = new StringBuilder($"{_CommonDirectoryPath}");
                sbDirPath.Append($"\\{openFileDic[EnumCheckIn.PROJECTID.ToDescription()].ToString()}");
                sbDirPath.Append($"\\{openFileDic[EnumCheckIn.TEAMID.ToDescription()].ToString()}");
                sbDirPath.Append($"\\{openFileDic[EnumCheckIn.CONTAINERID.ToDescription()].ToString()}");

                dirPath = sbDirPath.ToString();

                // AutoCAD 파일(확장자 .dwg, .dxf) 열기 처리 하기 전 파일 이름 문자열 할당 
                fileName = openFileDic[EnumCheckIn.FILENAME.ToDescription()].ToString();

                // TODO : AutoCAD 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 구현 (2024.10.15 jbh)
                // 참고 URL - https://dlgnlfus.tistory.com/129
                //filePath = @"C:\Users\bhjeon\Desktop\상상진화 회사 업무\RevitBox\RevitBox 버전업\RevitBox 2025\Revitbox 템플릿\Angles.rfa";
                filePath = dirPath + $"\\{fileName}";   // 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 

                // TODO : AutoCAD 파일(확장자 .dwg, .dxf) 열기 처리할 파일 정보 불러 오도록 구현 (2024.10.15 jbh)
                // 참고 URL   - https://chashtag.tistory.com/37
                // 참고 2 URL - https://dlgnlfus.tistory.com/129
                if(true == File.Exists(filePath))
                {
                    if(true == IsOpenedFile(filePath))   // AutoCAD 파일(확장자 .dwg, .dxf) 이미 열려있는 경우 
                        throw new Exception($"파일명 : {fileName}\r\n\r\n해당 문서가 이미 열려있는 상태입니다.");

                    // TODO : 메서드 "Path.GetExtension" 사용해서
                    //        파일열기 처리 하고자 하는 파일의 확장자 이름(.dwg, .dxf 등등...) 문자열로 구하기 (2024.10.21 jbh)
                    // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.io.path.getextension?view=net-8.0
                    extensionName = Path.GetExtension(filePath);

                    // 파일 확장자가 .dwg, .dxf 가 아닌 경우
                    if(extensionName != CobimHelper.DWGExtensionName && extensionName != CobimHelper.DXFExtensionName)
                        throw new Exception($"확장자({CobimHelper.DWGExtensionName}, {CobimHelper.DXFExtensionName}) 제외 " +
                                            $"\r\n다른 형식 파일열기 불가!" +
                                            $"\r\n확인 부탁드립니다.");

                    // 파일(확장자 .dwg, .dxf) 열기 처리 중 화면 출력 
                    AppSetting.Default.WindowBase.ShowCommonLoadingWork(CobimHelper.OpenFile_LoadingWork, _Palette);

                    Log.Information(Logger.GetMethodPath(currentMethod) + $"파일명 - {fileName} 열기 시작!");

                    // TODO : 아래 코드 실행해서 Civil3D 기반 AutoCAD 도면 열기 기능 실행시 오류 발생하여 AutoCAD 응용 프로그램 강제 종료 처리 발생. 해당 오류 원인 파악 예정 (2024.11.20 jbh)
                    // TODO : string 클래스 객체 filePath에 들어있는 파일 전체 경로에 존재하는 AutoCAD 도면 열기 기능 구현 (2024.11.20 jbh)
                    // 참고 URL - https://forums.autodesk.com/t5/net/how-to-open-an-autocad-file-update-and-close-with-save-using-c/td-p/8281653
                    docManager = Application.DocumentManager;
                    // DocumentCollectionExtension.Open(docManager, filePath, false);   // 파일열기 처리
                    docManager.Open(filePath, false);

                    // TODO : 아래 오류 메시지 해결하기 위해 Dispatcher.Invoke 사용 (2024.09.27 jbh)
                    // 오류 메시지 - "다른 스레드가 이 개체를 소유하고 있어 호출한 스레드가 해당 개체에 액세스할 수 없습니다"
                    // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.threading.dispatcher.invoke?view=windowsdesktop-6.0
                    // 참고 2 URL - https://insurang.tistory.com/entry/WPF-C-Error-%EB%8B%A4%EB%A5%B8-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%EC%9D%B4-%EA%B0%9C%EC%B2%B4%EB%A5%BC-%EC%86%8C%EC%9C%A0%ED%95%98%EA%B3%A0-%EC%9E%88%EC%96%B4-%ED%98%B8%EC%B6%9C%ED%95%9C-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%ED%95%B4%EB%8B%B9-%EA%B0%9C%EC%B2%B4%EC%97%90-%EC%95%A1%EC%84%B8%EC%8A%A4%ED%95%A0-%EC%88%98-%EC%97%86%EC%8A%B5%EB%8B%88%EB%8B%A4-%EC%97%90%EB%9F%AC-%EC%93%B0%EB%A0%88%EB%93%9C-Thread-DispatcherInvoke
                    _CommonLoadingWorkV.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        resMsg = "success";   // 파일(확장자 .dwg, .dxf) 열기 성공
                        Log.Information(Logger.GetMethodPath(currentMethod) + $"파일명 - {fileName} 열기 완료!");
                        MessageBox.Show($"파일명 - {fileName}\r\n열기 완료!");
                    }));

                    // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.11.20 jbh)
                    // using(DocumentLock lockDoc = doc.LockDocument())
                    // {
                    //     using(Transaction tr = db.TransactionManager.StartTransaction())
                    //     {
                    //         // 파일(확장자 .dwg, .dxf) 열기 처리 중 화면 출력 
                    //         AppSetting.Default.WindowBase.ShowCommonLoadingWork(CobimHelper.OpenFile_LoadingWork, _Palette);

                    //         Log.Information(Logger.GetMethodPath(currentMethod) + $"파일명 - {fileName} 열기 시작!");

                    //         DocumentCollection docMgr = Application.DocumentManager;
                    //         DocumentCollectionExtension.Open(docMgr, filePath, false);   // 파일열기 처리

                    //         resMsg = "success";   // 파일(확장자 .dwg, .dxf) 열기 성공
                    //         Log.Information(Logger.GetMethodPath(currentMethod) + $"파일명 - {fileName} 열기 완료!");

                    //         tr.Commit();
                    //     }
                    // }
                }
                else throw new Exception($"파일명 - {fileName}\r\n\r\n" +
                                         $"파일 경로\r\n{filePath}\r\n\r\n" +
                                         $"해당 파일은 존재하지 않습니다.\r\n" +
                                         $"다시 확인 바랍니다.");
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);

                // TODO : 오류 발생시 메시지 박스 출력 구현 (2024.10.18 jbh)
                // 메시지 박스 출력 사유
                // - COBIM 웹 탐색기 JSP 파일에서 오류 메시지를 Alert 으로 대신 출력할 경우
                // AutoCAD 응용 프로그램 -> AutoCAD 문서의 백그라운드에서 돌고있는 UI쓰레드와 충돌이 발생하여
                // AutoCAD 응용 프로그램 자체가 종료(ShutDown)되는 현상이 발생함.
                // 해당 오류 해결 방법
                // 1. AutoCAD 2025 응용 프로그램 최신 버전 업데이트 
                // 2. 해당 오류를 개선하기 위해 JSP 파일에서 오류 메시지를 Alert 으로 출력 안 하고
                // 아래처럼 MessageBox.Show(ex.Message, CobimHelper.ErrorTitle); 형식으로 출력함.
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);

                resMsg = ex.Message;   // 파일(확장자 .dwg, .dxf) 열기 처리 결과 오류 메시지 발생시 string 클래스 객체 resMsg에 할당
            }
            finally
            {
                // 파일열기 처리 중이 아닌 경우 
                if(resMsg != CobimHelper.OpenFile_LoadingWork)
                {
                    IsOpenFileWorking = false;   // 파일열기 처리 중 종료
                    AppSetting.Default.WindowBase.HideCommonLoadingWork(_Palette);   // 파일(확장자 .dwg, .dxf) 열기 처리 중 화면 숨기기
                }
            }
            return resMsg;   // 결과 메시지 리턴
        }

        #endregion OpenFileAsync

        #region Test

        /// <summary>
        /// 테스트용 메서드 
        /// 테스트 웹 어플리케이션 Git URL 주소 - https://github.com/bonghakjeon/WebView2WebApp
        /// </summary>
        public void Test(int a, string b)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                Log.Information(Logger.GetMethodPath(currentMethod) + "메서드 Test 시작");

                // Debug.WriteLine($"{a} {b}");
                // TaskDialog.Show(CobimHelper.TestTitle, "테스트...");
                MessageBox.Show("테스트...", CobimHelper.TestTitle);
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion Test

        #region Sample

        // TODO : 아래 테스트 코드 필요시 참고 (2024.11.20 jbh)
        //[ClassInterface(ClassInterfaceType.AutoDual)]
        //[ComVisible(true)]
        //public class WebView2ExplorerVM : PageBase, IPage
        //{
        //    #region 프로퍼티

        //    /// <summary>
        //    /// Rest API 입력 파라미터 데이터 객체 
        //    /// 체크아웃, 체크인, 다운로드, 작업취소 등등...
        //    ///// </summary>
        //    public WebView2ExplorerView InputParamData { get => _InputParamData; set { SetAndNotify(ref _InputParamData, value); } }
        //    private WebView2ExplorerView _InputParamData = new();


        //    #endregion 프로퍼티 

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

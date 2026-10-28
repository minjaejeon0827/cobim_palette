using Serilog;

using System.IO;
using System.Windows;
using System.Reflection;
using Exception = System.Exception;
using MessageBox = System.Windows.MessageBox;

using CobimPalette.Common.CobimBase;
using CobimPalette.Common.LogBase;
using CobimPalette.Common.RibbonBase;

using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.ApplicationServices;
using CobimPalette.Settings;

// TODO : AutoCAD API 개발 커뮤니티 사이트 필요시 참고 (2024.11.08 jbh)
// 참고 URL - https://forums.autodesk.com/t5/net/bd-p/152

// This line is not mandatory, but improves loading performances
// 필수 아니지만 로딩 성능 향상 시켜주는 기능
[assembly: ExtensionApplication(typeof(CobimPalette.App))]

namespace CobimPalette
{
    // TODO : 필요시 메서드 "Initialize" 추후 구현 예정 (2024.11.07 jbh)
    /// <summary>
    /// Revit 애드인 프로그램 Application.cs와 비슷한 기능 
    /// 해당 App 클래스는 AutoCAD 응용 프로그램에서 
    /// 한 번 인스턴스화되고 세션 기간 동안 활성 상태로 유지
    /// 한 번 초기화를 수행하지 않으면 해당 App 클래스 제거 필수 
    /// </summary>
    public class App : IExtensionApplication
    {
        #region 기본 메소드 

        /// <summary>
        /// 애드인 프로그램(CobimPalette) 시작(초기화)
        /// Revit 애드인 프로그램 메서드 OnStartup과 비슷한 기능 
        /// </summary>
        public void Initialize()
        {
            // TODO : AppDomain.CurrentDomain 속성 사용하여 현재 실행 중인 주 응용 프로그램(AutoCAD 2025 버전) 도메인(어셈블리 + 메인 스레드) 가져오기 구현 (2024.10.10 jbh)
            // 참고 URL - https://learn.microsoft.com/en-us/dotnet/api/system.appdomain.currentdomain?view=net-8.0
            // 참고 2 URL - https://developer-kus.tistory.com/11
            AppDomain currentDomain = AppDomain.CurrentDomain;   // 현재 실행 중인 주 응용 프로그램(AutoCAD 2025 버전) 도메인(어셈블리 + 스레드) 가져오기

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : 오류 메시지 "System.Windows.Markup.XamlParseException: 'Could not load file or assembly 'AnimatedImage.Wpf, Culture=neutral, PublicKeyToken=75a6352eaa886748'. 지정된 파일을 찾을 수 없습니다.'
                //                       FileNotFoundException: Could not load file or assembly 'AnimatedImage.Wpf, Culture=neutral, PublicKeyToken=75a6352eaa886748'.지정된 파일을 찾을 수 없습니다."  
                //        오류 원인 파악 결과
                //        AutoCAD 2024 버전 까지는 이 문제가 그렇게 발생한 것을 본 적이 없는데,
                //        AutoCAD 2025 버전 부터는 DotNet Core로 바뀐 여파인지,
                //        이런저런 상황에서 위와 같은 오류 메시지가 꽤 발생하는 것으로 확인됨.
                //        해당 오류 메세지가 출력되는 주요 원인은 3rdparty dll(CobimPalette.dll) 파일을
                //        주 응용 프로그램(acad.exe) 실행시 바로 읽지 않고,
                //        주 응용 프로그램(acad.exe) 실행 후 도중에 3rdparty dll(CobimPalette.dll) 파일을 읽게 되는 상황에서는
                //        주 응용 프로그램(acad.exe) 자체에 관련한 폴더만 검색하게 되는 상황이 발생하기 때문이다.  
                // 오류 개선 방법 참고 URL -https://stackoverflow.com/questions/1373100/how-to-add-folder-to-assembly-search-path-at-runtime-in-net  
                currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromCobimPalette);

                Logger.ConfigureLogger(CobimHelper.LogFileCountLimit, CobimHelper.AssemblyName, CobimHelper.LogDirectoryPath);   // Serilog 로그 초기 설정 

                Log.Information(Logger.GetMethodPath(currentMethod) + "Serilog 로그 초기 설정 완료");

                ConfigureAppSetting();   // 애드인 프로그램 CobimPalette 기본 설정에 필요한 객체 구성(생성 또는 초기화)  

                Log.Information(Logger.GetMethodPath(currentMethod) + $"{CobimHelper.AssemblyName} 프로그램 시작");

                // static 클래스 ComponentManager에 속한 리본 아이템이 존재하지 않는 경우 
                if(ComponentManager.Ribbon is null) 
                    ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;   // 이벤트 메서드 ComponentManager_ItemInitialized 가입
                // static 클래스 ComponentManager에 속한 리본 아이템이 존재하는 경우 
                else Ribbon.CreateRibbonControl(ComponentManager.Ribbon);   // 리본 메뉴 등록


                Application.DocumentManager.DocumentCreated += App_DocumentCreated;
                Application.DocumentManager.DocumentActivated += App_DocumentActivated;
                Application.DocumentManager.DocumentBecameCurrent += App_DocumentBecameCurrent;
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }


        /// <summary>
        /// 애드인 프로그램(CobimPalette) 종료(정리)
        /// </summary>
        public void Terminate()
        {
            Application.DocumentManager.DocumentCreated -= App_DocumentCreated;
            Application.DocumentManager.DocumentActivated -= App_DocumentActivated;
            Application.DocumentManager.DocumentBecameCurrent -= App_DocumentBecameCurrent;
        }

        #endregion 기본 메소드

        #region ConfigureAppSetting

        /// <summary>
        /// 애드인 프로그램 CobimPalette 기본 설정에 필요한 객체 구성(생성 또는 초기화)
        /// </summary>
        private void ConfigureAppSetting()
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                AppSetting.Default.DirectoryBase.SetCommonDirectoryPath(CobimHelper.CommonDirectoryPath);

                // TODO : 메서드 "AppSetting.Default.DirectoryBase.SetParentDirectoryPath" 호출 -> 메서드 Directory.GetParent 호출시 변수 parentDirectoryPath에 담긴 파일의 루트(부모) 폴더가 없는 경우 오류가 발생함.
                //        하여 해당 메서드 "AppSetting.Default.DirectoryBase.SetParentDirectoryPath" 호출문 주석 처리 완료 (2024.10.11 jbh)
                // AppSetting.Default.DirectoryBase.SetParentDirectoryPath(CobimHelper.AssemblyFilePath);   // dll 파일(CobimPalette.dll)이 속한 부모 폴더 경로 가져오기 
                AppSetting.Default.DirectoryBase.SetLogDirectoryPath(CobimHelper.LogDirectoryPath);

                AppSetting.Default.HttpClientBase.CreateHttpClient();   // HttpClient 싱글톤 객체 생성

                // TODO : 필요시 AutoCAD 애드인 프로그램(CobimPalette) 로그인 화면 출력 메서드 OpenLogin() 구현 예정 (2024.08.20 jbh)
                // AppSetting.Default.LoginBase.OpenLogin();

                AppSetting.Default.WindowBase.CreateWindow();   // Window 화면 객체 생성

                AppSetting.Default.PaletteBase.CreatePalette();   // Palette 화면 싱글톤 객체 생성

                // Palette (WebView2 탐색기 화면) 등록
                AppSetting.Default.PaletteBase.RegisterPalette(CobimHelper.WebView2ExplorerTitle, CobimHelper.WebView2ExplorerGuId);

                // TODO : 필요시 Palette (HubBox 화면) 등록 메서드 "RegisterPalette" 사용 예정 (2024.12.04 jbh)
                // Palette (HubBox 화면) 등록 
                // AppSetting.Default.PaletteBase.RegisterPalette(CobimHelper.HubBoxTitle, CobimHelper.HubBoxGuId);
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달 throw
            }
        }

        #endregion ConfigureAppSetting

        #region App_DocumentCreated

        /// <summary>
        /// AutoCAD 응용 프로그램에서 사용자가 새로 생성한 문서(Document)를 열고 난 직후 실행되는 이벤트 메소드
        /// 새로 생성 된 문서(Document) 정보 가져오기 가능 
        /// </summary>
        private void App_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // Document createdDoc = e.Document;   // 새로 생성 된 문서(Document) 정보 가져오기

                // Palette - WebView2 탐색기 화면 출력
                AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.WebView2ExplorerTitle, CobimHelper.WebView2ExplorerGuId);
                // Palette - HubBox 화면 출력
                // AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.HubBoxTitle, CobimHelper.HubBoxGuId);
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion App_DocumentCreated

        #region App_DocumentActivated

        /// <summary>
        /// AutoCAD 응용 프로그램에서 사용자가 기존에 생성한 문서(Document)가 활성화된 직후 실행되는 이벤트 메소드 
        /// 활성화 된 문서(Document) 정보 가져오기 가능 
        /// </summary>
        private void App_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // Document activatedDoc = e.Document;   // 활성화 된 문서(Document) 정보 가져오기

                // Palette - WebView2 탐색기 화면 출력
                AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.WebView2ExplorerTitle, CobimHelper.WebView2ExplorerGuId);
                // Palette - HubBox 화면 출력
                // AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.HubBoxTitle, CobimHelper.HubBoxGuId);
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion App_DocumentActivated

        #region App_DocumentBecameCurrent

        /// <summary>
        /// AutoCAD 응용 프로그램에서 기존에 열려있는 문서(Document)를 현재 문서로 설정하고 
        /// 이 현재 문서가 다른 문서로 변경(다른 문서 새로 생성, 열기 등등...)될 때마다 실행되는 이벤트 메소드
        /// 현재로 설정 된 문서(Document) 정보 가져오기 가능
        /// </summary>
        private void App_DocumentBecameCurrent(object sender, DocumentCollectionEventArgs e)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // Document currentDoc = e.Document;   // 현재로 설정 된 문서(Document) 정보 가져오기

                // Palette - WebView2 탐색기 화면 출력
                AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.WebView2ExplorerTitle, CobimHelper.WebView2ExplorerGuId);
                // Palette - HubBox 화면 출력
                // AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.HubBoxTitle, CobimHelper.HubBoxGuId);
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion App_DocumentBecameCurrent

        #region LoadFromCobimPalette

        /// <summary>
        /// 애드인 프로그램 CobimPalette에 속한 
        /// dll 파일(Nuget 패키지 참조된 dll 파일 포함) 읽어오기
        /// </summary>
        private Assembly LoadFromCobimPalette(object sender, ResolveEventArgs args)
        {
            string folderPath = string.Empty;     // 애드인 프로그램 dll 파일(CobimPalette.dll) 속한 상위 폴더 경로 
            string assemblyPath = string.Empty;   // 애드인 프로그램 dll 파일(CobimPalette.dll) 경로 
            Assembly assembly = null;             // 애드인 프로그램 dll 파일(CobimPalette.dll)

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
                // assemblyPath = Path.Combine(folderPath, Assembly.GetExecutingAssembly().GetName().Name + ".dll");
                // if (!File.Exists(assemblyPath)) return null;
                if (false == File.Exists(assemblyPath)) return null;
                assembly = Assembly.LoadFrom(assemblyPath);
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
            return assembly;
            // return loadingAssembly;
        }

        #endregion LoadFromCobimPalette

        #region ComponentManager_ItemInitialized

        /// <summary>
        /// static 클래스 ComponentManager에 속한 리본 아이템 초기화 이벤트 메서드 
        /// </summary>
        private void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {
            // static 클래스 ComponentManager에 속한 리본 아이템이 존재하는 경우 
            if(ComponentManager.Ribbon is not null)
            {
                Ribbon.CreateRibbonControl(ComponentManager.Ribbon);   // 리본 메뉴 등록
                ComponentManager.ItemInitialized -= ComponentManager_ItemInitialized;   // 이벤트 메서드 ComponentManager_ItemInitialized 해제
            }
        }

        #endregion ComponentManager_ItemInitialized

        #region Sample

        #endregion Sample
    }

}

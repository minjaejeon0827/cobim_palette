using Serilog;

using System.Reflection;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Interop;

using CobimPalette.Common.LogBase;
using CobimPalette.Views.Windows.LoadingWork;
using CobimPalette.ViewModels.Windows.LoadingWork;

using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CobimPalette.Settings
{
    /// <summary>
    /// Window 기본 설정 (로그인 화면 Window 제외)
    /// </summary>
    public class WindowSetting
    {
        #region 프로퍼티

        /// <summary>
        /// AutoCAD 응용 프로그램 MainWindow Handle
        /// </summary>
        private IntPtr _MainWindowHandle { get; set; }

        /// <summary>
        /// Window - 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면
        /// </summary>
        private CommonLoadingWorkV _CommonLoadingWorkV { get; set; }

        // TODO : Window - 체크인 팝업 화면 프로퍼티 "_CheckInPartV" 필요시 사용 예정 (2024.10.10 jbh)
        /// <summary>
        /// Window - 체크인 팝업 화면 
        /// </summary>
        //private CheckInPartV _CheckInPartV { get; set; }

        /// <summary>
        /// Window - 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면 출력 여부 
        /// (출력 - true / 출력안함 - false)
        /// </summary>
        private bool _IsShownLoadingWork { get; set; }

        // TODO : Window - 체크인 팝업 화면 출력 여부 프로퍼티 "_IsShownCheckIn" 필요시 사용 예정 (2024.10.10 jbh)
        /// <summary>
        /// Window - 체크인 팝업 화면 출력 여부 
        /// </summary>
        // private bool _IsShownCheckIn { get ; set; }

        #endregion 프로퍼티

        #region 생성자

        public WindowSetting()
        {
            _IsShownLoadingWork = false;
            //_IsShownCheckIn = false;

            // AutoCAD 응용 프로그램 MainWindow Handle 가져오기 
            Process process = Process.GetCurrentProcess();
            _MainWindowHandle = process.MainWindowHandle;
        }

        #endregion 생성자

        #region CreateWindow

        /// <summary>
        /// Window 화면 객체 생성
        /// </summary>
        public void CreateWindow()
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : "파일 다운로드", "체크아웃" 처리 후 파일열기 처리시
                //        이전에 작업한 "파일 다운로드", "체크아웃" 처리 중 대기 화면이 출력되지 않도록
                //        아래 처럼 Window - 로그인, 체크아웃, 체크인, 다운로드 시
                //        처리 중 화면 객체 프로퍼티 "_CommonLoadingWorkV"를 싱글톤 패턴을 사용하지 않고 
                //        해당 객체를 새로 생성하는 로직 "_CommonLoadingWorkV = new CommonLoadingWorkV();" 구현 (2024.10.22 jbh)
                // 싱글톤 패턴 참고 URL - https://velog.io/@jhbae0420/%EC%8B%B1%EA%B8%80%ED%86%A4-%ED%8C%A8%ED%84%B4%EC%9D%98-%EC%82%AC%EC%9A%A9-%EC%9D%B4%EC%9C%A0%EC%99%80-%EB%AC%B8%EC%A0%9C%EC%A0%90
                _CommonLoadingWorkV = new CommonLoadingWorkV();
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion CreateWindow

        #region GetCommonLoadingWorkV

        /// <summary>
        /// Window - 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면 가져오기 
        /// </summary>
        public CommonLoadingWorkV GetCommonLoadingWorkV()
        {
            return _CommonLoadingWorkV;
        }

        #endregion GetCommonLoadingWorkV

        #region ShowCommonLoadingWork

        /// <summary>
        /// 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면 출력 
        /// </summary>
        public void ShowCommonLoadingWork(string message, Page palette)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                _IsShownLoadingWork = _CommonLoadingWorkV.IsVisible;

                // 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면이 출력되지 않은 경우 
                if (false == _IsShownLoadingWork)
                {
                    // TODO : 오류 메시지 "System.InvalidOperationException: '많은 UI 구성 요소에서 호출 스레드가 필요하므로 해당 스레드는 STA여야 합니다.'"
                    //        출력시 해당 오류 해결하기 위해 아래처럼 메서드 Dispatcher.Invoke 사용해서 구현 (2024.10.02 jbh)
                    // 참고 URL - https://giveme-happyending.tistory.com/208 
                    // 다른 쓰레드(Window 화면 뷰모델 CommonLoadingWorkVM.cs 소스파일)에서 UI 쓰레드 접근하기
                    // => 윈폼과 마찬가지로 메서드 Dispatcher.Invoke 사용하여 작업
                    palette.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        // TODO : "파일 다운로드", "체크아웃" 처리 후 파일열기 처리시
                        //        이전에 작업한 "파일 다운로드", "체크아웃" 처리 중 대기 화면이 출력되지 않도록
                        //        아래 처럼 Window - 로그인, 체크아웃, 체크인, 다운로드 시
                        //        처리 중 화면 객체 프로퍼티 "_CommonLoadingWorkV"를 싱글톤 패턴을 사용하지 않고 
                        //        해당 객체를 새로 생성하는 로직 "_CommonLoadingWorkV = new CommonLoadingWorkV();" 구현 (2024.10.22 jbh)
                        // 싱글톤 패턴 참고 URL - https://velog.io/@jhbae0420/%EC%8B%B1%EA%B8%80%ED%86%A4-%ED%8C%A8%ED%84%B4%EC%9D%98-%EC%82%AC%EC%9A%A9-%EC%9D%B4%EC%9C%A0%EC%99%80-%EB%AC%B8%EC%A0%9C%EC%A0%90
                        _CommonLoadingWorkV = new CommonLoadingWorkV();
                        //_CommonLoadingWorkV.Visibility = Visibility.Visible;
                        var commonLoadingWorkVM = (CommonLoadingWorkVM)_CommonLoadingWorkV.DataContext;
                        commonLoadingWorkVM.SetMessage(message);

                        // TODO : 클래스 WindowInteropHelper 사용해서 WPF 화면 뷰(Window) 프로퍼티 _CommonLoadingWorkV의 소유자 설정 구현 (2024.11.28 jbh)
                        //        소유자 설정 하기 위해 Owner 속성에 Revit 응용 프로그램 MainWindow Handle 프로퍼티 _MainWindowHandle  할당 
                        // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.interop.windowinterophelper?view=windowsdesktop-8.0
                        // 참고 2 URL - https://mydevnote.tistory.com/299
                        // 참고 3 URL - https://thebuildingcoder.typepad.com/blog/2010/06/revit-parent-window.html
                        // 참고 4 URL - https://www.codeproject.com/Articles/37278/WPF-Center-Child-Window 
                        // _CommonLoadingWorkV.Owner = Application.Current.MainWindow;
                        new WindowInteropHelper(_CommonLoadingWorkV).Owner = _MainWindowHandle;
                        _CommonLoadingWorkV.Show();   // Window 화면 객체 _CommonLoadingWorkV 출력 

                        // TODO : 위의 클래스 WindowInteropHelper 사용해서 WPF 화면 뷰(Window) 프로퍼티 _CommonLoadingWorkV의 소유자 설정하는 방식 대신에
                        //        다른 방식으로 할 수 있는 테스트 코드 필요시 참고 (2024.11.28 jbh)
                        // 참고 URL - https://forums.autodesk.com/t5/net/show-a-wpf-window-in-the-center-of-the-autocad-window/td-p/8241703
                        // 참고 2 URL - https://forums.autodesk.com/t5/net/mainwindow-to-system-windows-window/td-p/4922284
                        // Application.ShowModalWindow(Application.MainWindow.Handle, _CommonLoadingWorkV, false);
                        // var nativeWindow = new NativeWindow();
                        // nativeWindow.AssignHandle(_MainWindowHandle);
                    }));
                }
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion ShowCommonLoadingWork

        #region HideCommonLoadingWork

        // TODO : 메서드 "CloseCommonLoadingWork" 실행 -> _CommonLoadingWorkV.Close(); 한번 닫은 Window 객체는 위의 메서드 "ShowCommonLoadingWork" 다시 호출시 
        //        아래처럼 오류 메시지 출력 되므로 _CommonLoadingWorkV.Close(); 대신 _CommonLoadingWorkV.Hide(); 가 출력 되도록 메서드 "HideCommonLoadingWork" 구현 (2024.10.07 jbh)
        // 오류 메시지 "System.InvalidOperationException: '창이 닫힌 후에는 Visibility를 설정하거나 Show, ShowDialog 또는 WindowInteropHelper.EnsureHandle을 호출할 수 없습니다.'"
        // 참고 URL - https://jooji815.tistory.com/62

        /// <summary>
        /// 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면 숨기기 
        /// </summary>
        public void HideCommonLoadingWork(Page palette)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                _IsShownLoadingWork = _CommonLoadingWorkV.IsVisible;
                // 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면이 출력된 경우 
                if (true == _IsShownLoadingWork)
                {
                    // TODO : 오류 메시지 "System.InvalidOperationException: '많은 UI 구성 요소에서 호출 스레드가 필요하므로 해당 스레드는 STA여야 합니다.'"
                    //        출력시 해당 오류 해결하기 위해 아래처럼 메서드 Dispatcher.Invoke 사용해서 구현 (2024.10.02 jbh)
                    // 참고 URL - https://giveme-happyending.tistory.com/208 
                    // 다른 쓰레드(Window 화면 뷰모델 CommonLoadingWorkVM.cs 소스파일)에서 UI 쓰레드 접근하기
                    // => 윈폼과 마찬가지로 메서드 Dispatcher.Invoke 사용하여 작업
                    palette.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        //_CommonLoadingWorkV.Visibility = Visibility.Collapsed;
                        _CommonLoadingWorkV.Hide();   // Window 화면 객체 _CommonLoadingWorkV 숨기기 
                    }));
                }
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion HideCommonLoadingWork

        #region ShowCheckInPart

        // TODO : 체크인 팝업 화면 출력 메서드 "ShowCheckInPart" 필요시 사용 예정 (2024.10.10 jbh) 
        /// <summary>
        /// 체크인 팝업 화면 출력 
        /// </summary>
        //public void ShowCheckInPart(Page palette)
        //{
        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        _IsShownCheckIn = _CheckInPartV.IsVisible;

        //        // 체크인 팝업 화면이 출력되지 않은 경우 
        //        if(false == _IsShownCheckIn)
        //        {
        //            // TODO : 오류 메시지 "System.InvalidOperationException: '많은 UI 구성 요소에서 호출 스레드가 필요하므로 해당 스레드는 STA여야 합니다.'"
        //            //        출력시 해당 오류 해결하기 위해 아래처럼 메서드 Dispatcher.Invoke 사용해서 구현 (2024.10.10 jbh)
        //            // 참고 URL - https://giveme-happyending.tistory.com/208 
        //            // 다른 쓰레드(Window 화면 뷰모델 CheckInPartVM.cs 소스파일)에서 UI 쓰레드 접근하기
        //            // => 윈폼과 마찬가지로 메서드 Dispatcher.Invoke 사용하여 작업
        //            palette.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
        //            {
        //                _CheckInPartV.Show();   // Window 화면 객체 _CheckInPartV 출력 
        //            }));
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        throw;   // 오류 발생시 상위 호출자 예외처리 전달
        //    }
        //}

        #endregion ShowCheckInPart

        #region HideCheckInPart

        // TODO : 체크인 팝업 화면 숨기기 메서드 "HideCheckInPart" 필요시 사용 예정 (2024.10.10 jbh)
        /// <summary>
        /// 체크인 팝업 화면 숨기기
        /// </summary>
        //public void HideCheckInPart(Page palette)
        //{
        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        _IsShownCheckIn = _CheckInPartV.IsVisible;

        //        // 체크인 팝업 화면이 출력된 경우 
        //        if(true == _IsShownCheckIn)
        //        {
        //            // TODO : 오류 메시지 "System.InvalidOperationException: '많은 UI 구성 요소에서 호출 스레드가 필요하므로 해당 스레드는 STA여야 합니다.'"
        //            //        출력시 해당 오류 해결하기 위해 아래처럼 메서드 Dispatcher.Invoke 사용해서 구현 (2024.10.10 jbh)
        //            // 참고 URL - https://giveme-happyending.tistory.com/208 
        //            // 다른 쓰레드(Window 화면 뷰모델 CheckInPartVM.cs 소스파일)에서 UI 쓰레드 접근하기
        //            // => 윈폼과 마찬가지로 메서드 Dispatcher.Invoke 사용하여 작업
        //            palette.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
        //            {
        //                _CheckInPartV.Hide();   // Window 화면 객체 _CheckInPartV 숨기기 
        //            }));
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        throw;   // 오류 발생시 상위 호출자 예외처리 전달
        //    }
        //}

        #endregion HideCheckInPart

        #region Sample

        // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.10.10 jbh)
        /// <summary>
        /// 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면 종료 
        /// </summary>
        //public void CloseCommonLoadingWork(Page palette)
        //{
        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        _IsShownLoadingWork = _CommonLoadingWorkV.IsVisible;
        //        // 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면이 출력된 경우 
        //        if(true == _IsShownLoadingWork) 
        //        {
        //            // TODO : 오류 메시지 "System.InvalidOperationException: '많은 UI 구성 요소에서 호출 스레드가 필요하므로 해당 스레드는 STA여야 합니다.'"
        //            //        출력시 해당 오류 해결하기 위해 아래처럼 메서드 Dispatcher.Invoke 사용해서 구현 (2024.10.02 jbh)
        //            // 참고 URL - https://giveme-happyending.tistory.com/208 
        //            // 다른 쓰레드(Window 화면 뷰모델 CommonLoadingWorkVM.cs 소스파일)에서 UI 쓰레드 접근하기
        //            // => 윈폼과 마찬가지로 메서드 Dispatcher.Invoke 사용하여 작업
        //            palette.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
        //            {
        //                _CommonLoadingWorkV.Visibility = Visibility.Collapsed;
        //                _CommonLoadingWorkV.Close();   // Window 화면 객체 _CommonLoadingWorkV 종료 
        //            }));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        throw;   // 오류 발생시 상위 호출자 예외처리 전달
        //    }
        //}

        // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.10.07 jbh)
        /// <summary>
        /// Window 화면 객체 가져오기 
        /// </summary>
        //public Window GetWindow(string pWindowType)
        //{
        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        switch (pWindowType)
        //        {
        //            case CobimHelper.TypeOfCommonLoadingWork:   // 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면
        //                return _CommonLoadingWorkV;   // Window 화면 객체 _CommonLoadingWorkV 리턴  
        //            default:
        //                throw new Exception($"[오류]\r\n화면 유형({pWindowType}) 출력 오류!\r\n관리자에게 문의하세요.");
        //        }

        //        Log.Information(Logger.GetMethodPath(currentMethod) + $"[Window] {pWindowType} 화면 가져오기 완료");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        throw;   // 오류 발생시 상위 호출자 예외처리 전달
        //    }
        //}

        /// <summary>
        /// Window 화면 출력 
        /// </summary>
        //public void ShowWindow(string pWindowType)
        //{
        //    bool isShown = false;                                // Window 화면 출력 여부 (출력 - true / 출력안함 - false)

        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        //Log.Information(Logger.GetMethodPath(currentMethod) + "[Window] 화면 출력 시작");

        //        // TODO : Window 클래스 속성 "IsVisible", "IsEnabled", "IsActive" 사용해서 Window 화면 출력 여부 확인 로직 구현 (2024.10.02 jbh)
        //        // 참고 URL - https://forum.dotnetdev.kr/t/wpf-show/816/5
        //        // 참고 2 URL - https://stackoverflow.com/questions/30431133/wpf-detecting-window-close-event-and-coupling-with-a-button-event
        //        // 속성 IsVisible 참고 URL - https://learn.microsoft.com/en-us/dotnet/api/system.windows.uielement.isvisible?view=windowsdesktop-8.0
        //        // 속성 IsEnabled 참고 URL - https://learn.microsoft.com/en-us/dotnet/api/system.windows.uielement.isenabled?view=windowsdesktop-8.0
        //        // 속성 IsActive 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.window.isactive?view=windowsdesktop-8.0
        //        // _CommonLoadingWorkV.IsEnabled

        //        switch (pWindowType)
        //        {
        //            case CobimHelper.TypeOfCommonLoadingWork:   // 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면
        //                isShown = _CommonLoadingWorkV.IsVisible && _CommonLoadingWorkV.IsActive;

        //                // 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면이 출력되지 않은 경우 
        //                if (false == isShown) _CommonLoadingWorkV.Show();   // Window 화면 객체 _CommonLoadingWorkV 출력 
        //                break;
        //            default:
        //                throw new Exception($"[오류]\r\n화면 유형({pWindowType}) 출력 오류!\r\n관리자에게 문의하세요.");
        //        }

        //        Log.Information(Logger.GetMethodPath(currentMethod) + $"[Window] {pWindowType} 화면 출력 완료");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        throw;   // 오류 발생시 상위 호출자 예외처리 전달
        //    }
        //}

        /// <summary>
        /// Window 화면 종료 
        /// </summary>
        //public void CloseWindow(string pWindowType)
        //{
        //    bool isShown = false;                                // Window 화면 출력 여부 (출력 - true / 출력안함 - false)

        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        switch (pWindowType)
        //        {
        //            case CobimHelper.TypeOfCommonLoadingWork:   // 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면
        //                isShown = _CommonLoadingWorkV.IsVisible && _CommonLoadingWorkV.IsActive;
        //                // 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면이 출력된 경우 
        //                if (true == isShown) _CommonLoadingWorkV.Close();   // Window 화면 객체 _CommonLoadingWorkV 종료 
        //                break;
        //            default:
        //                throw new Exception($"[오류]\r\n화면 유형({pWindowType}) 출력 오류!\r\n관리자에게 문의하세요.");
        //        }

        //        Log.Information(Logger.GetMethodPath(currentMethod) + $"[Window] {pWindowType} 화면 종료 완료");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        throw;   // 오류 발생시 상위 호출자 예외처리 전달
        //    }
        //}

        /// <summary>
        /// Window 화면 객체 생성(싱글톤 패턴 사용)
        /// </summary>
        //public void CreateWindowSingleton()
        //{
        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        if (_CommonLoadingWorkV is null) _CommonLoadingWorkV = new CommonLoadingWorkV();
        //        // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.10.23 jbh)
        //        // 싱글톤 패턴 사용해서 처음 한번만 프로퍼티 "_CommonLoadingWorkV", "_CheckInPartV" 객체 생성 (로그인 화면 Window 제외)
        //        // if(_CommonLoadingWorkV is null) _CommonLoadingWorkV = new CommonLoadingWorkV();
        //        // if(_CheckInPartV is null) _CheckInPartV = new CheckInPartV();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        throw;   // 오류 발생시 상위 호출자 예외처리 전달
        //    }
        //}

        #endregion Sample
    }
}

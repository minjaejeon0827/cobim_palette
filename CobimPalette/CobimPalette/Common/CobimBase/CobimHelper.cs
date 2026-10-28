using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CobimPalette.Common.CobimBase
{
    public class CobimHelper
    {
        #region 공통

        #region 어셈블리

        /// <summary>
        /// 어셈블리(.dll 파일) 이름 - CobimPalette
        /// </summary>
        public static string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// 어셈블리(.dll 파일) 경로 - CobimPalette
        /// </summary>
        public static string AssemblyFilePath = Assembly.GetExecutingAssembly().Location;

        #endregion 어셈블리

        #region 로그 파일

        /// <summary>
        /// 로그 파일 갯수 설정 (테스트용 30일 이전)
        /// </summary>
        public const int LogFileCountLimit = 30;

        #endregion 로그 파일

        #region 메소드 타입

        /// <summary>
        /// 반환형 void 메소드 타입
        /// </summary>
        public const string VoidMethodType = "void";

        /// <summary>
        /// 비동기 메소드 타입
        /// </summary>
        public const string AsyncMethodType = "async";

        #endregion 메소드 타입

        #region 폴더(디렉토리) 경로 

        /// <summary>
        /// 상위 폴더 (디렉토리) 이름
        /// </summary>
        public static string CobimBaseDirectory = "COBIM";

        /// <summary>
        /// 폴더 (디렉토리) 공통 경로 
        /// (응용 프로그램 - CobimExplorer.exe, Revit 애드인 프로그램 - CobimDockableDialogs.dll, AutoCAD 애드인 프로그램 - CobimPalette.dll)
        /// </summary>
        public static string CommonDirectoryPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{CobimBaseDirectory}\\{AssemblyName}";

        /// <summary>
        /// 로그(Logs) 폴더(디렉토리) 경로 
        /// </summary>
        public static string LogDirectoryPath = $"{CommonDirectoryPath}\\Logs";

        /// <summary>
        /// 테스트 Json 파일 경로  
        /// </summary>
        public const string TestJsonFilePath = @"\Json\Test.json";

        #endregion 폴더(디렉토리) 경로

        #region 트랜잭션

        /// <summary>
        /// 트랜잭션 Start
        /// </summary>
        public const string Start = "Start";

        #endregion 트랜잭션 

        #region MessageBox 타이틀

        /// <summary>
        /// 타이틀 - 테스트
        /// </summary>
        public const string TestTitle = "테스트";

        /// <summary>
        /// 타이틀 - 완료 
        /// </summary>
        public const string CompletedTitle = "완료";

        /// <summary>
        /// 타이틀 - 오류  
        /// </summary>
        public const string ErrorTitle = "오류";

        /// <summary>
        /// 타이틀 - 알림
        /// </summary>
        public const string NoticeTitle = "알림";

        #endregion MessageBox 타이틀

        #region Window

        /// <summary>
        /// Window - 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면
        /// </summary>
        public const string TypeOfCommonLoadingWork = "CommonLoadingWorkV";

        #endregion Window

        #endregion 공통

        #region Palette

        #region ExplorerVM

        /// <summary>
        /// Palette - 탐색기 화면 뷰 제목
        /// </summary>
        public const string ExpTitle = "COBIM 탐색기";

        /// <summary>
        /// Palette - 탐색기 화면 뷰 유형
        /// </summary>
        public const string TypeOfExplorer = "ExplorerV";

        // TODO : 추후 Guid 스터디 진행하기 (2024.08.20 minjae)
        // 참고 URL - https://velog.io/@ci___der/GUID%EA%B0%80-%EB%AC%B4%EC%97%87%EC%9D%BC%EA%B9%8C
        /// <summary>
        /// Palette - 탐색기 화면 뷰 객체 Guid
        /// </summary>
        public static readonly Guid ExplorerGuId = new Guid("{9D50D85E-12F2-4F42-B265-4263474AB95A}");

        #endregion ExplorerVM

        #region WebView2ExplorerVM

        /// <summary>
        /// COBIM 웹 탐색기 브라우저에 존재하는 바인딩 할 메서드 객체 이름 
        /// </summary>
        public const string boundAsync = "boundAsync";

        /// <summary>
        /// 파일 다운로드
        /// </summary>
        // public const string DownLoadAsync = "DownLoadAsync";

        /// <summary>
        /// 컨테이너 체크아웃 + (참조/연관) 관계 컨테이너 파일 다운로드  
        /// </summary>
        // public const string CheckOutAsync = "CheckOutAsync";

        /// <summary>
        /// 컨테이너 체크인 + (컨테이너에 속한 파일) 웹서버로 업로드  
        /// </summary>
        // public const string CheckInAsync = "CheckInAsync";

        /// <summary>
        /// 컨테이너 작업 취소
        /// </summary>
        // public const string CancelWorkAsync = "CancelWorkAsync";

        /// <summary>
        /// Palette - WebView2 탐색기 화면 뷰 제목
        /// </summary>
        //public const string WebView2ExplorerTitle = "COBIM 웹-탐색기";
        public const string WebView2ExplorerTitle = "COBIM WebExplorer";

        /// <summary>
        /// Palette - WebView2 탐색기 화면 뷰 이름
        /// </summary>
        public const string TypeOfWebView2Explorer = "WebView2ExplorerV";

        // TODO : 추후 Guid 스터디 진행하기 (2024.09.19 minjae)
        // 참고 URL - https://velog.io/@ci___der/GUID%EA%B0%80-%EB%AC%B4%EC%97%87%EC%9D%BC%EA%B9%8C
        /// <summary>
        /// Palette - WebView2 탐색기 화면 뷰 객체 Guid
        /// </summary>
        // public static readonly Guid WebView2ExplorerGuId = new Guid("{EE99E94C-EF99-4E61-A094-A8711330ACC7}");
        public static readonly Guid WebView2ExplorerGuId = new Guid("{5342BC37-E817-42F0-AB46-878D3664259E}");

        /// <summary>
        /// 파일 다운로드 대기 처리 
        /// </summary>
        public const string DownLoad_LoadingWork = "파일 다운로드 처리 중입니다...";

        /// <summary>
        /// 테스트 대기 처리
        /// </summary> 
        public const string Test_LoadingWork = "테스트 처리 중입니다...";

        /// <summary>
        /// 체크아웃 대기 처리
        /// </summary>
        public const string CheckOut_LoadingWork = "체크아웃 처리 중입니다...";

        /// <summary>
        /// 체크인 대기 처리
        /// </summary>
        public const string CheckIn_LoadingWork = "체크인 처리 중입니다...";

        /// <summary>
        /// 체크인 Comment 메시지 공백 알림
        /// </summary>
        public const string CheckIn_CommentWhiteSpace = "Comment 메세지가 비었습니다." +
                                                        "\r\nComment 메세지 입력 후 다시 시도하시기 바랍니다.";

        /// <summary>
        /// 연관 컨테이너에 속한 파일 다운로드 대기 처리 
        /// </summary>
        public const string LinkedContainer_LoadingWork = "연관 컨테이너에 속한\r\n파일 다운로드 처리 중입니다...";

        /// <summary>
        /// 파일 변환 요청 대기 처리
        /// </summary>
        public const string Translate_LoadingWork = "파일 변환 요청 중입니다...";

        /// <summary>
        /// 컨테이너 작업 취소 대기 처리 
        /// </summary>
        public const string CancelWork_LoadingWork = "컨테이너 작업 취소\r\n처리 중입니다...";

        /// <summary>
        /// 파일열기 대기 처리 
        /// </summary>
        public const string OpenFile_LoadingWork = "파일 열기 처리 중입니다...";

        /// <summary>
        /// 파일열기 가능한 Revit 파일 확장자 이름
        /// </summary>
        public const string RVTExtensionName = ".rvt";

        /// <summary>
        /// 파일열기 가능한 AutoCAD 파일 확장자 이름
        /// </summary>
        public const string DWGExtensionName = ".dwg";

        /// <summary>
        /// 파일열기 가능한 AutoCAD 파일 확장자 이름
        /// </summary>
        public const string DXFExtensionName = ".dxf";

        #endregion WebView2ExplorerVM

        #region CheckInPartVM

        /// <summary>
        /// 체크인 팝업화면 Title Text   
        /// </summary>
        public const string CheckIn_Title = "체크인_업로드";

        #endregion CheckInPartVM

        #region HubBox

        /// <summary>
        /// Palette - HubBox 화면 뷰 제목
        /// </summary>
        public const string HubBoxTitle = "HubBox 파일 변환 리스트";

        /// <summary>
        /// Palette - HubBox 화면 뷰 이름
        /// </summary>
        public const string TypeOfHubBox = "HubBoxV";

        // TODO : 추후 Guid 스터디 진행하기 (2024.09.19 minjae)
        // 참고 URL - https://velog.io/@ci___der/GUID%EA%B0%80-%EB%AC%B4%EC%97%87%EC%9D%BC%EA%B9%8C
        /// <summary>
        /// Palette - HubBox 화면 뷰 객체 Guid
        /// </summary>
        public static readonly Guid HubBoxGuId = new Guid("{52B8E66B-B3BF-40BE-AEB6-CA3FEB78EE74}");

        #endregion HubBox

        #endregion Palette

        #region ImagineBuilder

        /// <summary>
        /// (주)상상진화 기업 홈페이지 URL 주소
        /// </summary>
        public const string ImagineBuilderURL = "https://imbu.co.kr";

        #endregion ImagineBuilder

        #region Sample

        #endregion Sample
    }
}

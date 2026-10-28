using System.IO;
using System.Net.Http;
using System.ComponentModel;

using CobimPalette.Data.Models.CobimBase.Login;

namespace CobimPalette.Data.Models.CobimBase.WebView2Explorer
{
    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    public class WebView2ExplorerView<TData> : WebView2ExplorerView where TData : WebView2ExplorerView<TData> { }

    /// <summary>
    /// CobimPalette
    /// WebView2 탐색기 화면 뷰모델(WebView2ExplorerVM.cs) 데이터 모델 
    /// ** 데이터 모델 기본 원리 ** 
    /// - 의존성 주입 DI (수동 주입) 방식이며 
    /// 사용자가 요청하는 특정 기능을 수행하기 위해
    /// 필요한 입력 파라미터 객체 값을 
    /// 아래 예시처럼 외부(WebView2 탐색기 화면 뷰모델(WebView2ExplorerVM.cs))에서
    /// 수동 주입 받는 형식(new RequestHeader(...) / new ContainerPack(...) / new FilePack(...))으로 구현된다.
    /// (예시) - WebView2ExplorerVM.cs -> 파일 다운로드 비동기 메서드 "DownLoadAsync"
    /// 
    /// COBIM 웹 탐색기 데이터 모델 객체 InputParamData에 데이터셋 새로 할당 
    /// InputParamData.SetData(new RequestHeader(downLoadDic[EnumDownLoad.TENANT.ToDescription()].ToString(),
    ///                                          downLoadDic[EnumDownLoad.AUTHORIZATION.ToDescription()].ToString()),
    ///                        new ContainerPack(downLoadDic[EnumDownLoad.PROJECTID.ToDescription()].ToString(),
    ///                                          downLoadDic[EnumDownLoad.TEAMID.ToDescription()].ToString(),
    ///                                          downLoadDic[EnumDownLoad.CONTAINERID.ToDescription()].ToString(),
    ///                                          string.Empty),
    ///                        new FilePack(downLoadDic[EnumDownLoad.FILEID.ToDescription()].ToString(),
    ///                                     downLoadDic[EnumDownLoad.FILENAME.ToDescription()].ToString()));
    /// </summary>
    public class WebView2ExplorerView : TWebView2ExplorerView<WebView2ExplorerView>
    {
        #region 프로퍼티

        /// <summary>
        /// HttpClient client (계속 호출하지 말고 싱글톤으로 사용해야함.)
        /// </summary>
        private HttpClient _Client { get; set; }

        /// <summary>
        /// 폴더 (디렉토리) 공통 경로 
        /// (응용 프로그램 - CobimExplorer.exe, Revit 애드인 프로그램 - CobimDockableDialogs.dll, AutoCAD 애드인 프로그램 - CobimPalette.dll)
        /// </summary>
        private string _CommonDirectoryPath { get; set; }

        /// <summary>
        /// Http 서버 통신 요청 Header
        /// </summary>
        private RequestHeader _RequestHeader { get; set; }

        /// <summary>
        /// 컨테이너 정보 
        /// </summary>
        private ContainerPack _ContainerPack { get; set; }

        /// <summary>
        /// 다운로드할 파일 정보
        /// </summary>
        private FilePack _FilePack { get; set; }

        /// <summary>
        /// 컨테이너 체크인 + 파일 웹서버 업로드 정보
        /// </summary>
        private CheckInPack _CheckInPack { get; set; }

        /// <summary>
        /// 컨테이너 작업 취소 정보
        /// </summary>
        private CancelWorkPack _CancelWorkPack { get; set; }

        /// <summary>
        /// HubBox 컨테이너에 속한 파일 웹뷰어로 출력시 필수 정보 
        /// </summary>
        private WebViewerPack _WebViewerPack { get; set; }

        /// <summary>
        /// 컨테이너 체크인 + 파일 웹서버 업로드 성공시 
        /// HubBox 파일 변환 Rest API 요청시 필수 정보
        /// </summary>
        private TranslatePack _TranslatePack { get; set; }

        // TODO : 아래 주석친 코드 필요시 참고 (2024.11.20 minjae)
        /// <summary>
        /// 컨테이너에 속한 파일(확장자 .dwg, .dxf) 
        /// AutoCAD 응용 프로그램 열기 작업시 필수 정보 
        /// </summary>
        // private OpenFilePack _OpenFilePack { get; set; }

        #endregion 프로퍼티

        #region 생성자

        // TODO : 아래 주석친 2가지 오류 메시지 출력으로 인해 Default 생성자 "public WebView2ExplorerView() : base() { }" 구현 (2024.10.07 minjae)
        // WebView2ExplorerVM.cs 소스 파일 -> 1번 오류 메시지 "오류 CS0310	'제네릭 형식 또는 메서드 'PageBase<TData>'에서 'TData' 매개 변수로 사용하려면 'WebView2ExplorerView'이(가) 매개 변수가 없는 public 생성자를 사용하는 비추상 형식이어야 합니다." 
        // WebView2ExplorerView.cs 소스 파일 -> 2번 오류 메시지 "오류 CS7036	'WebView2ExplorerView.WebView2ExplorerView(HttpClient, RequestHeader, ContainerPack, FilePack, CheckInPack)'의 필수 매개 변수 'client'에 해당하는 인수가 없습니다."
        //public WebView2ExplorerView() { }
        public WebView2ExplorerView() : base() { }


        // TODO : 아래 주석친 생성자 "public WebView2ExplorerView(HttpClient client, RequestHeader requestHeader, ContainerPack containerPack, FilePack filePack, CheckInPack checkInPack = null) : base()"
        //        WebView2 탐색기 화면 뷰모델(WebView2ExplorerVM.cs)에서 필요시 참고 (2024.09.24 minjae)
        // public WebView2ExplorerView(HttpClient client, RequestHeader requestHeader, ContainerPack containerPack, FilePack filePack, CheckInPack checkInPack = null) : base() 
        // {
        //     _Client = client;
        //     _RequestHeader = requestHeader;
        //     _ContainerPack = containerPack;
        //     _FilePack = filePack;
        //     _CheckInPack = checkInPack;
        // }

        #endregion 생성자

        #region SetData

        /// <summary>
        /// 데이터 모델 클래스 WebView2ExplorerView.cs에 속하는
        /// 데이터 프로퍼티에 값 셋팅(또는 값 복제)
        /// </summary>
        public void SetData(RequestHeader requestHeader, ContainerPack? containerPack = null, FilePack? filePack = null, CheckInPack? checkInPack = null)
        {
            // this.SetHttpClient(client);
            this.SetRequestHeader(requestHeader);
            this.SetContainerPack(containerPack);
            this.SetFilePack(filePack);
            this.SetCheckInPack(checkInPack);
        }

        #endregion SetData

        #region SetCancelWorkData

        /// <summary>
        /// WebView2 웹 탐색기 화면 데이터 모델 클래스 WebView2ExplorerView.cs에 속하는
        /// 데이터 프로퍼티 값 셋팅 (또는 값 복제)
        /// "_RequestHeader - (Http 서버 통신 요청 Header)",
        /// "_CancelWorkPack - (컨테이너 작업 취소 정보)" 
        /// </summary>
        public void SetCancelWorkData(RequestHeader requestHeader, CancelWorkPack cancelWorkPack)
        {
            this.SetRequestHeader(requestHeader);
            this.SetCancelWorkPack(cancelWorkPack);
        }

        #endregion SetCancelWorkData

        #region HttpClient client

        /// <summary>
        /// HttpClient client 값 셋팅 
        /// </summary>
        public void SetHttpClient(HttpClient client)
        {
            _Client = client;
        }

        /// <summary>
        /// HttpClient client 값 가져오기 
        /// </summary>
        public HttpClient GetHttpClient()
        {
            return _Client;
        }

        #endregion HttpClient client

        #region 폴더 (디렉토리) 공통 경로

        /// <summary>
        /// 폴더 (디렉토리) 공통 경로 값 셋팅 
        /// (응용 프로그램 - CobimExplorer.exe, Revit 애드인 프로그램 - CobimDockableDialogs.dll, AutoCAD 애드인 프로그램 - CobimPalette.dll)
        /// </summary>
        public void SetCommonDirectoryPath(string commonDirectoryPath)
        {
            _CommonDirectoryPath = commonDirectoryPath;
        }

        /// <summary>
        /// 폴더 (디렉토리) 공통 경로 가져오기
        /// (응용 프로그램 - CobimExplorer.exe, Revit 애드인 프로그램 - CobimDockableDialogs.dll, AutoCAD 애드인 프로그램 - CobimPalette.dll)
        /// </summary>
        public string GetCommonDirectoryPath()
        {
            return _CommonDirectoryPath;
        }

        #endregion 폴더 (디렉토리) 공통 경로

        #region Http 웹서버 통신 요청 Header

        /// <summary>
        /// Http 웹서버 통신 요청 Header 값 셋팅 
        /// </summary>
        public void SetRequestHeader(RequestHeader requestHeader)
        {
            _RequestHeader = requestHeader;
        }

        /// <summary>
        /// Http 웹서버 통신 요청 Header 값 가져오기 
        /// </summary>
        public RequestHeader GetRequestHeader()
        {
            return _RequestHeader;
        }

        #endregion Http 웹서버 통신 요청 Header

        #region 컨테이너 정보 

        /// <summary>
        /// 컨테이너 정보 값 셋팅 
        /// </summary>
        public void SetContainerPack(ContainerPack containerPack)
        {
            _ContainerPack = containerPack;
        }

        /// <summary>
        /// 컨테이너 정보 값 가져오기 
        /// </summary>
        public ContainerPack GetContainerPack()
        {
            return _ContainerPack;
        }

        #endregion 컨테이너 정보 

        #region 다운로드할 파일 정보

        /// <summary>
        /// 다운로드할 파일 정보 값 셋팅 
        /// </summary>
        public void SetFilePack(FilePack filePack)
        {
            _FilePack = filePack;
        }

        /// <summary>
        /// 다운로드할 파일 정보 값 가져오기 
        /// </summary>
        public FilePack GetFilePack()
        {
            return _FilePack;
        }

        #endregion 다운로드할 파일 정보

        #region 컨테이너 체크인 + 파일 웹서버 업로드 정보

        /// <summary>
        /// 컨테이너 체크인 + 파일 웹서버 업로드 정보 값 셋팅 
        /// </summary>
        public void SetCheckInPack(CheckInPack checkInPack)
        {
            _CheckInPack = checkInPack;
        }

        /// <summary>
        /// 컨테이너 체크인 + 파일 웹서버 업로드 정보 값 가져오기 
        /// </summary>
        public CheckInPack GetCheckInPack()
        {
            return _CheckInPack;
        }

        #endregion 컨테이너 체크인 + 파일 웹서버 업로드 정보

        #region 컨테이너 작업 취소 정보

        /// <summary>
        /// 컨테이너 작업 취소 정보 값 셋팅 
        /// </summary>
        public void SetCancelWorkPack(CancelWorkPack cancelWorkPack)
        {
            _CancelWorkPack = cancelWorkPack;
        }

        /// <summary>
        /// 컨테이너 작업 취소 정보 가져오기 
        /// </summary>
        public CancelWorkPack GetCancelWorkPack()
        {
            return _CancelWorkPack;
        }


        #endregion 컨테이너 작업 취소 정보 

        #region HubBox 컨테이너에 속한 파일 웹뷰어로 출력시 필수 정보

        /// <summary>
        /// HubBox 컨테이너에 속한 파일 웹뷰어 정보 값 세팅 
        /// </summary>
        public void SetWebViewerPack(WebViewerPack webViewerPack)
        {
            _WebViewerPack = webViewerPack;
        }

        /// <summary>
        /// HubBox 컨테이너에 속한 파일 웹뷰어 정보 가져오기 
        /// </summary>
        public WebViewerPack GetWebViewerPack()
        {
            return _WebViewerPack;
        }

        #endregion HubBox 컨테이너에 속한 파일 웹뷰어로 출력시 필수 정보


        #region HubBox 파일 변환 Rest API 요청시 필수 정보

        /// <summary>
        /// HubBox 파일 변환 정보 값 셋팅 
        /// </summary>
        public void SetTranslatePack(TranslatePack translatePack)
        {
            _TranslatePack = translatePack;
        }

        /// <summary>
        /// HubBox 파일 변환 정보 가져오기
        /// </summary>
        public TranslatePack GetTranslatePack()
        {
            return _TranslatePack;
        }

        #endregion HubBox 파일 변환 Rest API 요청시 필수 정보

        #region 컨테이너에 속한 파일(확장자 .dwg, .dxf) AutoCAD 응용 프로그램 열기 작업시 필수 정보 

        // TODO : 아래 주석친 코드 필요시 참고 (2024.11.20 minjae)
        /// <summary>
        /// 컨테이너에 속한 파일(확장자 .dwg, .dxf) AutoCAD 응용 프로그램 열기 작업시 필수 정보 값 셋팅
        /// </summary>
        // public void SetOpenFilePack(OpenFilePack openFilePack)
        // {
        //     _OpenFilePack = openFilePack;
        // }

        /// <summary>
        /// 컨테이너에 속한 파일(확장자 .dwg, .dxf) AutoCAD 응용 프로그램 열기 작업시 필수 정보 가져오기 
        /// </summary>
        // public OpenFilePack GetOpenFilePack()
        // {
        //     return _OpenFilePack;
        // }

        #endregion 컨테이너에 속한 파일(확장자 .dwg, .dxf) AutoCAD 응용 프로그램 열기 작업시 필수 정보 
    }

    #region EnumDownLoad

    public enum EnumDownLoad : int
    {
        [Description("projectId")]   // 프로젝트_아이디
        PROJECTID = 0,
        [Description("teamId")]      // 팀_아이디
        TEAMID = 1,
        [Description("containerId")] // 컨테이너_아이디
        CONTAINERID = 2,
        [Description("fileId")]      // 파일_아이디
        FILEID = 3,
        [Description("fileNm")]      // 파일_이름
        FILENAME = 4,
        [Description("Tenant")]      // 테넌트 아이디
        TENANT = 5,
        [Description("Authorization")]   // 로그인 토큰 키   
        AUTHORIZATION = 6,
        [Description("Access")]    // 접속자 아이디 (Access Id, accessKey 같은 값을 의미, 로그인 아이디와는 다름)
        ACCESS = 7,
        [Description("Refresh")]   // Refresh Token - 로그인 토큰 만료시 재발급을 위한 토큰 키 
        REFRESH = 8
    }

    #endregion EnumDownLoad

    #region EnumLinkedRelation

    /// <summary>
    /// 참조/연관 컨테이너 관계 표현 
    /// </summary>
    public enum EnumLinkedRelation : int
    {
        [Description("참조_연관컨테이너_부재")]
        NONE = 0,

        [Description("참조_연관컨테이너_존재")]
        EXISTENCE = 1
    }

    #endregion EnumLinkedRelations

    #region EnumCheckOut

    public enum EnumCheckOut : int
    {
        [Description("projectId")]   // 프로젝트_아이디
        PROJECTID = 0,
        [Description("teamId")]      // 팀_아이디
        TEAMID = 1,
        [Description("containerId")] // 체크아웃_컨테이너_아이디
        CONTAINERID = 2,

        // TODO : 탐색기 화면 뷰(ExplorerV.xaml) -> 컨텍스트 메뉴 "체크아웃" 클릭시 멀티 바인딩 대상 중 "exp_FolderId" 제외함.
        //        제외한 사유는 "exp_FolderId" 설정시 탐색기 뷰모델(ExplorerVM.cs) -> 컨테이너 리스트 생성 비동기 메서드 "ContainerListCreateAsync"
        //        실행 결과 데이터 정보가 변경되어 컨테이너 리스트 화면이 바뀌므로 해당 프로퍼티는 멀티 바인딩 대상에서 제외 처리함. (2024.10.11 minjae) 
        //        하여 아래 enum 열거형 구조체 EnumCheckOut 멤버 변수 FOLDERID 또한 사용 제외 (2024.10.11 minjae)
        // [Description("folderId")]   // 폴더_아이디
        // FOLDERID = 3,
        [Description("fileId")]   // 파일_아이디
        FILEID = 3,
        [Description("fileNm")]   // 파일명
        FILENAME = 4,
        [Description("Tenant")]   // 테넌트 아이디
        TENANT = 5,
        [Description("Authorization")]   // 로그인 토큰 키   
        AUTHORIZATION = 6,
        [Description("Access")]    // 접속자 아이디 (Access Id, accessKey 같은 값을 의미, 로그인 아이디와는 다름)
        ACCESS = 7,
        [Description("Refresh")]   // Refresh Token - 로그인 토큰 만료시 재발급을 위한 토큰 키 
        REFRESH = 8
    }


    #endregion EnumCheckOut

    #region EnumCheckOutResult

    /// <summary>
    /// 체크아웃 실행 결과
    /// </summary>
    public enum EnumCheckOutResult : int
    {
        [Description("체크아웃_실패")]
        FAILURE = 0,
        [Description("체크아웃_성공")]
        SUCCESS = 1
    }

    #endregion EnumCheckOutResult

    #region EnumCheckIn

    public enum EnumCheckIn : int
    {
        [Description("projectId")]   // 프로젝트_아이디
        PROJECTID = 0,
        [Description("teamId")]      // 팀_아이디
        TEAMID = 1,
        [Description("containerId")] // 컨테이너_아이디
        CONTAINERID = 2,
        [Description("folderId")]    // 폴더_아이디
        FOLDERID = 3,
        // TODO : enum 열거형 구조체 EnumCheckIn 멤버 변수 "UPLOADFILE" 필요시 사용 예정 (2024.10.11 minjae)
        //[Description("uploadFiles")]  // 업로드_파일
        //UPLOADFILES = 4,
        [Description("fileId")]      // 파일_아이디
        FILEID = 4,
        // TODO : 추후 체크인 구현시 업로드 하려는 파일명(UPLOAD_FILENAME)과 기존 체크인 된 파일명(FILENAME)이 다를 경우
        //        enum 열거형 구조체 EnumCheckIn 멤버 변수 "FILENAME" 사용 예정 (2024.10.11 minjae)
        [Description("fileNm")] // 파일명
        FILENAME = 5,
        // TODO : enum 열거형 구조체 EnumCheckIn 멤버 변수 "FILESIZE" 필요시 사용 예정 (2024.10.11 minjae)
        // [Description("fileSize")]   // 파일_사이즈
        // FILESIZE = 6
        [Description("revsnVal")]      // 업로드_전_파일_버전
        REVSNVAL = 6,
        [Description("title")]         // 제목
        TITLE = 7,
        [Description("Tenant")]          // 테넌트 아이디
        TENANT = 8,
        [Description("Authorization")]   // 로그인 토큰 키   
        AUTHORIZATION = 9,
        [Description("comment")]   // comment
        COMMENT = 10,
        [Description("Access")]    // 접속자 아이디 (Access Id, accessKey 같은 값을 의미, 로그인 아이디와는 다름)
        ACCESS = 11,
        [Description("Refresh")]   // Refresh Token - 로그인 토큰 만료시 재발급을 위한 토큰 키 
        REFRESH = 12
    }

    #endregion EnumCheckIn

    #region EnumCancelWork

    public enum EnumCancelWork : int
    {
        [Description("projectId")]   // 프로젝트_아이디
        PROJECTID = 0,
        [Description("teamId")]      // 팀_아이디
        TEAMID = 1,
        [Description("containerId")] // 작업취소_컨테이너_아이디
        CONTAINERID = 2,
        [Description("fileNm")]   // 파일명
        FILENAME = 3,
        // TODO : enum 열거형 구조체 EnumCancelWork 멤버 변수 "WORKUSERID" 필요시 사용 예정 (2024.10.11 minjae)
        //[Description("작업중인_사용자_아이디")]
        //WORKUSERID = 3,
        [Description("Tenant")]   // 테넌트 아이디
        TENANT = 4,
        [Description("Authorization")]   // 로그인 토큰 키   
        AUTHORIZATION = 5,
        [Description("Access")]    // 접속자 아이디 (Access Id, accessKey 같은 값을 의미, 로그인 아이디와는 다름)
        ACCESS = 6,
        [Description("Refresh")]   // Refresh Token - 로그인 토큰 만료시 재발급을 위한 토큰 키 
        REFRESH = 7
    }

    #endregion EnumCancelWork

    #region ContainerPack

    /// <summary>
    /// 컨테이너 정보
    /// </summary>
    public class ContainerPack
    {
        /// <summary>
        /// 프로젝트 아이디 
        /// </summary>
        private string _ProjectId { get; set; }

        /// <summary>
        /// 팀 아이디
        /// </summary>
        private string _TeamId { get; set; }

        /// <summary>
        /// 컨테이너 아이디
        /// </summary>
        private string _ContainerId { get; set; }

        /// <summary>
        /// 컨테이너 목록 조회 - 사용자가 선택한 폴더 아이디
        /// </summary>
        private string _SelectedFolderId { get; set; }

        /// <summary>
        /// 체크아웃 - 연관 컨테이너 아이디
        /// </summary>
        private string _LinkedContainerId { get; set; }

        #region 생성자

        public ContainerPack(string projectId, string teamId, string? containerId = null, string? selectedFolderId = null, string? linkedContainerId = null)
        {
            _ProjectId = projectId;
            _TeamId = teamId;
            _ContainerId = containerId;
            _SelectedFolderId = selectedFolderId;
            _LinkedContainerId = linkedContainerId;
        }

        #endregion 생성자 

        #region 프로젝트 아이디 

        /// <summary>
        /// 프로젝트 아이디 값 셋팅
        /// </summary>
        public void SetProjectId(string projectId)
        {
            _ProjectId = projectId;
        }

        /// <summary>
        /// 프로젝트 아이디 가져오기
        /// </summary>
        public string GetProjectId()
        {
            return _ProjectId;
        }

        #endregion 프로젝트 아이디 

        #region 팀 아이디

        /// <summary>
        /// 팀 아이디 값 셋팅
        /// </summary>
        public void SetTeamId(string teamId)
        {
            _TeamId = teamId;
        }

        /// <summary>
        /// 팀 아이디 가져오기
        /// </summary>
        public string GetTeamId()
        {
            return _TeamId;
        }

        #endregion 팀 아이디

        #region 컨테이너 아이디

        /// <summary>
        /// 컨테이너 아이디 값 셋팅
        /// </summary>
        public void SetContainerId(string containerId)
        {
            _ContainerId = containerId;
        }

        /// <summary>
        /// 컨테이너 아이디 가져오기
        /// </summary>
        public string GetContainerId()
        {
            return _ContainerId;
        }

        #endregion 컨테이너 아이디

        #region 사용자가 선택한 폴더 아이디

        /// <summary>
        /// 사용자가 선택한 폴더 아이디 값 셋팅
        /// </summary>
        public void SetSelectedFolderId(string selectedFolderId)
        {
            _SelectedFolderId = selectedFolderId;
        }

        /// <summary>
        /// 사용자가 선택한 폴더 아이디 가져오기 
        /// </summary>
        public string GetSelectedFolderId()
        {
            return _SelectedFolderId;
        }

        #endregion 사용자가 선택한 폴더 아이디

        #region 연관 컨테이너 아이디

        /// <summary>
        /// 체크아웃 - 연관 컨테이너 아이디 값 셋팅
        /// </summary>
        public void SetLinkedContainerId(string linkedContainerId)
        {
            _LinkedContainerId = linkedContainerId;
        }

        /// <summary>
        /// 체크아웃 - 연관 컨테이너 아이디 가져오기
        /// </summary>
        public string GetLinkedContainerId()
        {
            return _LinkedContainerId;
        }

        #endregion 연관 컨테이너 아이디

        #region Sample

        #endregion Sample
    }

    #endregion ContainerPack

    #region FilePack

    /// <summary>
    /// 파일 정보
    /// </summary>
    public class FilePack
    {
        /// <summary>
        /// 파일 아이디 
        /// </summary>
        private string _FileId { get; set; }

        /// <summary>
        /// 파일명
        /// </summary>
        private string _FileName { get; set; }

        // TODO : 파일 사이즈 프로퍼티 "_FileSize" 필요시 사용 예정 (2024.10.11 minjae)
        /// <summary>
        /// 파일 사이즈
        /// </summary>
        private string _FileSize { get; set; }

        /// <summary>
        /// 업로드 할 파일
        /// </summary>
        private FileStream _UploadFiles { get; set; }

        #region 생성자

        public FilePack(string fileId, string fileName, string? fileSize = null, FileStream? uploadFiles = null)
        {
            _FileId = fileId;
            _FileName = fileName;
            _FileSize = fileSize;
            _UploadFiles = uploadFiles;
        }

        #endregion 생성자 

        #region 파일 아이디 

        /// <summary>
        /// 파일 아이디 값 셋팅 
        /// </summary>
        public void SetFileId(string fileId)
        {
            _FileId = fileId;
        }

        /// <summary>
        /// 파일 아이디 가져오기 
        /// </summary>
        public string GetFileId()
        {
            return _FileId;
        }

        #endregion 파일 아이디 

        #region 파일명

        /// <summary>
        /// 파일명 값 셋팅 
        /// </summary>
        public void SetFileName(string fileName)
        {
            _FileName = fileName;
        }

        /// <summary>
        /// 파일명 가져오기 
        /// </summary>
        public string GetFileName()
        {
            return _FileName;
        }

        #endregion 파일명

        #region 파일 사이즈

        /// <summary>
        /// 파일 사이즈 값 셋팅 
        /// </summary>
        public void SetFileSize(string fileSize)
        {
            _FileSize = fileSize;
        }

        /// <summary>
        /// 파일 사이즈 가져오기
        /// </summary>
        public string GetFileSize()
        {
            return _FileSize;
        }

        #endregion 파일 사이즈

        #region 업로드 할 파일

        /// <summary>
        /// 업로드 할 파일 값 셋팅 
        /// </summary>
        public void SetUploadFiles(FileStream uploadFiles)
        {
            _UploadFiles = uploadFiles;
        }

        /// <summary>
        /// 업로드 할 파일 가져오기
        /// </summary>
        public FileStream GetUploadFiles()
        {
            return _UploadFiles;
        }

        #endregion 업로드 할 파일

        #region Sample

        #endregion Sample
    }

    #endregion FilePack

    #region CheckInPack

    /// <summary>
    /// 컨테이너 체크인 + 파일 웹서버 업로드 정보
    /// </summary>
    public class CheckInPack
    {
        /// <summary>
        /// 폴더 아이디
        /// </summary>
        private string _FolderId { get; set; }

        /// <summary>
        /// comment
        /// </summary>
        private string _Comment { get; set; }

        // TODO : 파일 정보 프로퍼티 "_FilePack" 필요시 사용 예정 (2024.10.14 minjae)
        /// <summary>
        /// 파일 정보
        /// </summary>
        // private FilePack _FilePack { get; set; }

        #region 생성자

        //public CheckInPack(string folderId, string comment, FilePack filePack)
        public CheckInPack(string folderId, string comment)
        {
            _FolderId = folderId;
            _Comment = comment;
            // _FilePack = filePack;
        }

        #endregion 생성자 

        #region 폴더 아이디

        /// <summary>
        /// 폴더 아이디 값 셋팅
        /// </summary>
        public void SetFolderId(string folderId)
        {
            _FolderId = folderId;
        }

        /// <summary>
        /// 폴더 아이디 가져오기
        /// </summary>
        public string GetFolderId()
        {
            return _FolderId;
        }

        #endregion 폴더 아이디

        #region comment

        /// <summary>
        /// comment 값 셋팅
        /// </summary>
        public void SetComment(string comment)
        {
            _Comment = comment;
        }

        /// <summary>
        /// comment 가져오기
        /// </summary>
        public string GetComment()
        {
            return _Comment;
        }

        #endregion comment

        #region 파일 정보

        // TODO : 파일 정보 값 셋팅 메서드 "SetFilePack" 필요시 사용 예정 (2024.10.14 minjae)
        /// <summary>
        /// 파일 정보 값 셋팅 
        /// </summary>
        // public void SetFilePack(FilePack filePack)
        // {
        //     _FilePack = filePack;
        // }

        // TODO : 파일 정보 가져오기 메서드 "GetFilePack" 필요시 사용 예정 (2024.10.14 minjae)
        /// <summary>
        /// 파일 정보 가져오기
        /// </summary>
        // public FilePack GetFilePack()
        // {
        //     return _FilePack;
        // }

        #endregion 파일 정보

        #region Sample

        #endregion Sample
    }

    #endregion CheckInPack

    #region CancelWorkPack

    /// <summary>
    /// 컨테이너 작업 취소 정보
    /// </summary>
    public class CancelWorkPack
    {
        // TODO : ExplorerView.cs 소스파일에 속한
        //        컨테이너 작업 취소 정보 클래스 "CancelWorkPack"안에 속하는 프로퍼티 "projectId", "teamId", "containerId" 의 접근 제한자를 
        //        private로 설정하면 오류 발생하여 접근 제한자를 public으로 수정함. (2024.11.04 minjae)
        //        오류 발생 위치 - ExplorerRestServer.cs -> 비동기 메서드 "PostCancelWorkAsync" 
        //        -> 메서드 "JsonSerializer.Serialize" 호출하여 json 문자열(content)을 제대로 리턴 받지 못하고 오류 발생함. 

        // TODO : ExplorerView.cs 소스파일에 속한
        //        컨테이너 작업 취소 정보 클래스 "CancelWorkPack"안에 속하는 프로퍼티 "projectId", "teamId", "containerId" 프로퍼티 이름 
        //        변경하면 오류 발생하여 수정 안 하고 그대로 유지함.(2024.11.04 minjae)
        //        오류 발생 위치 - ExplorerRestServer.cs -> 비동기 메서드 "PostCancelWorkAsync" 
        //        서버로 부터 제대로 리턴 받지 못하고 오류 발생함. 

        /// <summary>
        /// 프로젝트 아이디 
        /// </summary>
        public string projectId { get; set; }

        /// <summary>
        /// 팀 아이디
        /// </summary>
        public string teamId { get; set; }

        /// <summary>
        /// 컨테이너 아이디
        /// </summary>
        public string containerId { get; set; }

        #region 생성자

        public CancelWorkPack(string projectId, string teamId, string containerId)
        {
            this.projectId = projectId;
            this.teamId = teamId;
            this.containerId = containerId;
        }

        #endregion 생성자 

        #region Sample

        #endregion Sample
    }

    #endregion CancelWorkPack

    #region WebViewerPack

    /// <summary>
    /// 컨테이너에 속한 파일 웹뷰어로 출력시 필수 정보 
    /// </summary>
    public class WebViewerPack
    {
        #region 프로퍼티

        /// <summary>
        /// 프로젝트 아이디 
        /// </summary>
        private string _ProjectId { get; set; }

        /// <summary>
        /// 팀 아이디
        /// </summary>
        private string _TeamId { get; set; }

        /// <summary>
        /// 컨테이너 아이디
        /// </summary>
        private string _ContainerId { get; set; }

        /// <summary>
        /// 컨테이너에 속한 파일 아이디
        /// </summary>
        private string _FileId { get; set; }

        #endregion 프로퍼티

        #region 생성자

        public WebViewerPack(string projectId, string teamId, string containerId, string fileId)
        {
            _ProjectId = projectId;
            _TeamId = teamId;
            _ContainerId = containerId;
            _FileId = fileId;
        }

        #endregion 생성자 

        #region 프로젝트 아이디 

        /// <summary>
        /// 프로젝트 아이디 값 셋팅
        /// </summary>
        public void SetProjectId(string projectId)
        {
            _ProjectId = projectId;
        }

        /// <summary>
        /// 프로젝트 아이디 가져오기
        /// </summary>
        public string GetProjectId()
        {
            return _ProjectId;
        }

        #endregion 프로젝트 아이디 

        #region 팀 아이디

        /// <summary>
        /// 팀 아이디 값 셋팅
        /// </summary>
        public void SetTeamId(string teamId)
        {
            _TeamId = teamId;
        }

        /// <summary>
        /// 팀 아이디 가져오기
        /// </summary>
        public string GetTeamId()
        {
            return _TeamId;
        }

        #endregion 팀 아이디

        #region 컨테이너 아이디

        /// <summary>
        /// 컨테이너 아이디 값 셋팅
        /// </summary>
        public void SetContainerId(string containerId)
        {
            _ContainerId = containerId;
        }

        /// <summary>
        /// 컨테이너 아이디 가져오기
        /// </summary>
        public string GetContainerId()
        {
            return _ContainerId;
        }

        #endregion 컨테이너 아이디

        #region 컨테이너에 속한 파일 아이디

        /// <summary>
        /// 컨테이너에 속한 파일 아이디 값 셋팅
        /// </summary>
        public void SetFileId(string fileId)
        {
            _FileId = fileId;
        }

        /// <summary>
        /// 컨테이너에 속한 파일 아이디 가져오기
        /// </summary>
        public string GetFileId()
        {
            return _FileId;
        }

        #endregion 컨테이너에 속한 파일 아이디

        #region Sample

        #endregion Sample
    }

    #endregion WebViewerPack

    #region TranslatePack

    /// <summary>
    /// 컨테이너 체크인 + 파일 웹서버 업로드 성공시 
    /// HubBox 파일 변환 Rest API 요청시 필수 정보 
    /// </summary>
    public class TranslatePack
    {
        #region 프로퍼티

        /// <summary>
        /// 프로젝트 아이디 
        /// </summary>
        private string _ProjectId { get; set; }

        /// <summary>
        /// 팀 아이디
        /// </summary>
        private string _TeamId { get; set; }

        /// <summary>
        /// 컨테이너 아이디
        /// </summary>
        private string _ContainerId { get; set; }

        /// <summary>
        /// 컨테이너에 속한 파일 아이디
        /// </summary>
        private string _FileId { get; set; }

        #endregion 프로퍼티

        #region 생성자

        public TranslatePack(string projectId, string teamId, string containerId, string fileId)
        {
            _ProjectId = projectId;
            _TeamId = teamId;
            _ContainerId = containerId;
            _FileId = fileId;
        }

        #endregion 생성자 

        #region 프로젝트 아이디 

        /// <summary>
        /// 프로젝트 아이디 값 셋팅
        /// </summary>
        public void SetProjectId(string projectId)
        {
            _ProjectId = projectId;
        }

        /// <summary>
        /// 프로젝트 아이디 가져오기
        /// </summary>
        public string GetProjectId()
        {
            return _ProjectId;
        }

        #endregion 프로젝트 아이디 

        #region 팀 아이디

        /// <summary>
        /// 팀 아이디 값 셋팅
        /// </summary>
        public void SetTeamId(string teamId)
        {
            _TeamId = teamId;
        }

        /// <summary>
        /// 팀 아이디 가져오기
        /// </summary>
        public string GetTeamId()
        {
            return _TeamId;
        }

        #endregion 팀 아이디

        #region 컨테이너 아이디

        /// <summary>
        /// 컨테이너 아이디 값 셋팅
        /// </summary>
        public void SetContainerId(string containerId)
        {
            _ContainerId = containerId;
        }

        /// <summary>
        /// 컨테이너 아이디 가져오기
        /// </summary>
        public string GetContainerId()
        {
            return _ContainerId;
        }

        #endregion 컨테이너 아이디

        #region 파일 아이디 

        /// <summary>
        /// 파일 아이디 값 셋팅 
        /// </summary>
        public void SetFileId(string fileId)
        {
            _FileId = fileId;
        }

        /// <summary>
        /// 파일 아이디 가져오기 
        /// </summary>
        public string GetFileId()
        {
            return _FileId;
        }

        #endregion 파일 아이디 

        #region Sample

        #endregion Sample
    }

    #endregion TranslatePack

    #region Sample

    // TODO : 아래 주석친 코드 필요시 참고 (2024.11.20 minjae)
    /// <summary>
    /// 컨테이너에 속한 파일(확장자 .dwg, .dxf) 
    /// AutoCAD 응용 프로그램 열기 작업시 필수 정보 
    /// </summary>
    //public class OpenFilePack
    //{
    //    #region 프로퍼티

    //    /// <summary>
    //    /// palette 화면 객체 (WebView2 탐색기 뷰 - WebView2ExplorerV.xaml)
    //    /// </summary>
    //    private Page _Palette { get; set; }


    //    /// <summary>
    //    /// 파일(확장자 .dwg, .dxf) 열기 처리할 파일 이름
    //    /// </summary>
    //    private string _FileName { get; set; }

    //    /// <summary>
    //    /// 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 
    //    /// </summary>
    //    private string _FilePath { get; set; }

    //    /// <summary>
    //    /// Revit API 사용한 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 객체
    //    /// </summary>
    //    // private ModelPath _ModelPath { get; set; }

    //    /// <summary>
    //    /// Revit API 사용한 파일(확장자 .dwg, .dxf) 열기할 때 사용할 수 있는 옵션 클래스 객채
    //    /// </summary>
    //    // private OpenOptions _OpenOptions { get; set; }

    //    #endregion 프로퍼티

    //    #region 생성자

    //    //public OpenFilePack(Page palette, string fileName, string filePath, ModelPath modelPath, OpenOptions openOptions)
    //    //{
    //    //    _Palette = palette;
    //    //    _FileName = fileName;
    //    //    _FilePath = filePath;
    //    //    _ModelPath = modelPath;
    //    //    _OpenOptions = openOptions;
    //    //}

    //    #endregion 생성자

    //    #region Palette 화면 객체

    //    /// <summary>
    //    /// Palette 화면 객체 (WebView2 탐색기 뷰 - WebView2ExplorerV.xaml) 값 셋팅
    //    /// </summary>
    //    public void SetPalette(Page palette)
    //    {
    //        _Palette = palette;
    //    }

    //    /// <summary>
    //    /// Palette 화면 객체 (WebView2 탐색기 뷰 - WebView2ExplorerV.xaml) 가져오기
    //    /// </summary>
    //    public Page GetPalette()
    //    {
    //        return _Palette;
    //    }

    //    #endregion Palette 화면 객체

    //    #region 파일(확장자 .dwg, .dxf) 열기 처리할 파일 이름

    //    /// <summary>
    //    /// 파일(확장자 .dwg, .dxf) 열기 처리할 파일이름 값 셋팅
    //    /// </summary>
    //    public void SetFileName(string fileName)
    //    {
    //        _FileName = fileName;
    //    }

    //    /// <summary>
    //    /// 파일(확장자 .dwg, .dxf) 열기 처리할 파일 이름 가져오기
    //    /// </summary>
    //    public string GetFileName()
    //    {
    //        return _FileName;
    //    }

    //    #endregion 파일(확장자 .dwg, .dxf) 열기 처리할 파일 이름 

    //    #region 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 

    //    /// <summary>
    //    /// 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 값 셋팅
    //    /// </summary>
    //    public void SetFilePath(string filePath)
    //    {
    //        _FilePath = filePath;
    //    }

    //    /// <summary>
    //    /// 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 가져오기
    //    /// </summary>
    //    public string GetFilePath()
    //    {
    //        return _FilePath;
    //    }

    //    #endregion 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 

    //    #region Revit API 사용한 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로

    //    /// <summary>
    //    /// Revit API 사용한 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 객체 값 셋팅
    //    /// </summary>
    //    // public void SetModelPath(ModelPath modelPath)
    //    // {
    //    //     _ModelPath = modelPath;
    //    // }

    //    /// <summary>
    //    /// Revit API 사용한 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로 객체 가져오기
    //    /// </summary>
    //    // public ModelPath GetModelPath()
    //    // {
    //    //     return _ModelPath;
    //    // }

    //    #endregion Revit API 사용한 파일(확장자 .dwg, .dxf) 열기 처리할 파일 전체 경로

    //    #region Revit API 사용한 파일(확장자 .dwg, .dxf) 열기할 때 사용할 수 있는 옵션 클래스 

    //    /// <summary>
    //    /// Revit API 사용한 파일(확장자 .dwg, .dxf) 열기할 때 사용할 수 있는 옵션 클래스 객체 값 셋팅
    //    /// </summary>
    //    // public void SetOpenOptions(OpenOptions openOptions)
    //    // {
    //    //     _OpenOptions = openOptions;
    //    // }

    //    /// <summary>
    //    /// Revit API 사용한 파일(확장자 .dwg, .dxf) 열기할 때 사용할 수 있는 옵션 클래스 객체 가져오기
    //    /// </summary>
    //    // public OpenOptions GetOpenOptions()
    //    // {
    //    //     return _OpenOptions;
    //    // }

    //    #endregion Revit API 사용한 파일(확장자 .dwg, .dxf) 열기할 때 사용할 수 있는 옵션 클래스 

    //    #region Sample

    //    #endregion Sample
    //}

    #endregion Sample
}

using System.ComponentModel;

namespace CobimPalette.Data.Models.CobimBase.HubBox
{

    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    public class HubBoxView<TData> : HubBoxView where TData : HubBoxView<TData> { }

    // TODO : 추후 필요시 HubBox 전용 데이터 모델 클래스 HubBoxView 구현 예정 (2024.10.30 minjae) 
    /// <summary>
    /// CobimPalette
    /// HubBox 뷰모델(HubBoxVM.cs) HubBox 전용 데이터 모델
    /// </summary>
    public class HubBoxView : THubBoxView<HubBoxView>
    {
        #region 프로퍼티

        // TODO : Http 웹서버 통신 응답 데이터 프로퍼티 "_ResponseData" 필요시 사용/수정 예정 (2024.10.30 minjae)
        /// <summary>
        /// Http 웹서버 통신 응답 데이터
        /// </summary>
        private static ResponseData _ResponseData { get; set; }
        //public static ResponseData ResponseDatas
        //{
        //    // 싱글톤 패턴 
        //    get => _ResponseDatas ?? (_ResponseDatas = new ResponseData());

        //    set
        //    {
        //        _ResponseDatas = value;
        //        StaticChanged(nameof(ResponseDatas));
        //    }
        //}
        //private static ResponseData _ResponseDatas;

        #endregion 프로퍼티

        #region 생성자

        // TODO : 생성자 "HubBoxView()" 탐색기 화면 뷰모델(ExplorerVM.cs)에서 필요시 호출해서 HubBoxView 클래스 객체 생성 예정 (2024.09.25 minjae)
        public HubBoxView() : base() { }

        #endregion 생성자 

        #region Http 웹서버 통신 응답 데이터

        /// <summary>
        /// Http 웹서버 통신 응답 데이터 값 셋팅
        /// </summary>
        public static void SetResponseData(ResponseData responseData)
        {
            _ResponseData = responseData;
        }

        /// <summary>
        /// Http 웹서버 통신 응답 데이터 값 가져오기 
        /// </summary>
        public static ResponseData GetResponseData()
        {
            return _ResponseData;
        }

        #endregion Http 웹서버 통신 응답 데이터
    }

    #region HubBox - Rest API 2. 파일 뷰어 요청

    /// <summary>
    /// Http 웹서버 통신 응답 데이터
    /// </summary>
    public class ResponseData
    {
        #region 프로퍼티

        /// <summary>
        /// HttpStatusCode 상태 코드 
        /// </summary>
        private string _Code { get; set; }

        /// <summary>
        /// 응답 메세지 
        /// </summary>
        private string _Message { get; set; }

        /// <summary>
        /// 파일 뷰어 Url 주소
        /// </summary>
        private string _Data { get; set; }

        #endregion 프로퍼티

        #region 생성자

        public ResponseData(string code, string message, string data)
        {
            _Code = code;
            _Message = message;
            _Data = data;
        }

        #endregion 생성자

        #region HttpStatusCode 상태 코드 

        /// <summary>
        /// HttpStatusCode 상태 코드 값 셋팅
        /// </summary>
        public void SetCode(string code)
        {
            _Code = code;
        }

        /// <summary>
        /// HttpStatusCode 상태 코드 가져오기
        /// </summary>
        public string GetCode()
        {
            return _Code;
        }

        #endregion HttpStatusCode 상태 코드 

        #region 응답 메세지 

        /// <summary>
        /// 응답 메세지 값 셋팅
        /// </summary>
        public void SetMessage(string message)
        {
            _Message = message;
        }

        /// <summary>
        /// 응답 메세지 가져오기
        /// </summary>
        public string GetMessage()
        {
            return _Message;
        }

        #endregion 응답 메세지 

        #region 파일 뷰어 Url 주소

        /// <summary>
        /// 파일 뷰어 Url 주소 값 셋팅
        /// </summary>
        public void SetData(string data)
        {
            _Data = data;
        }

        /// <summary>
        /// 파일 뷰어 Url 주소 가져오기 
        /// </summary>
        public string GetData()
        {
            return _Data;
        }

        #endregion 파일 뷰어 Url 주소

        #region Sample

        #endregion Sample
    }

    #endregion HubBox - Rest API 2. 파일 뷰어 요청

    #region EnumWebViewerUrn

    /// <summary>
    /// HubBox - Rest API 2. 파일 뷰어 요청 응답 데이터 
    /// </summary>
    public enum EnumWebViewerUrn : int
    {
        [Description("파일_뷰어_요청_응답_데이터_부재")]
        NONE = 0,

        [Description("파일_뷰어_요청_응답_데이터_존재")]
        EXISTENCE = 1
    }

    #endregion EnumWebViewerUrn

    #region EnumWebViewer

    public enum EnumWebViewer : int
    {
        [Description("프로젝트_아이디")]
        PROJECTID = 0,
        [Description("팀_아이디")]
        TEAMID = 1,
        [Description("컨테이너_아이디")]
        CONTAINERID = 2,
        [Description("파일_아이디")]
        FILEID = 3,
        [Description("제목")]
        TITLE = 4,
        [Description("파일_웹뷰어_보기")]
        WEBVIEWER = 5
    }

    #endregion EnumWebViewer

    #region EnumWebViewerResult

    public enum EnumWebViewerResult : int
    {
        [Description("code")]
        CODE = 0,
        [Description("data")]
        DATA = 1,
        [Description("message")]
        MESSAGE = 2
    }

    #endregion EnumWebViewerResult

    #region EnumTranslate

    public enum EnumTranslate : int
    {
        [Description("projectId")]   // 프로젝트_아이디
        PROJECTID = 0,
        [Description("teamId")]      // 팀_아이디
        TEAMID = 1,
        [Description("containerId")] // 컨테이너_아이디
        CONTAINERID = 2,
        [Description("folderId")]    // 폴더_아이디
        FOLDERID = 3,
        // TODO : enum 열거형 구조체 EnumTranslate 멤버 변수 "UPLOADFILE" 필요시 사용 예정 (2024.09.25 minjae)
        //[Description("uploadFiles")]  // 업로드_파일
        //UPLOADFILES = 4,
        [Description("fileId")]      // 파일_아이디
        FILEID = 4,
        // TODO : 추후 체크인 구현시 업로드 하려는 파일명(UPLOAD_FILENAME)과 기존 체크인 된 파일명(FILENAME)이 다를 경우
        //        enum 열거형 구조체 EnumCheckIn 멤버 변수 "FILENAME" 사용 예정 (2024.10.11 minjae)
        // [Description("fileNm")]   // 파일명
        // FILENAME = 5,
        [Description("translate_fileNm")]   // "변환_파일명"
        TRANSLATE_FILENAME = 5,
        // TODO : enum 열거형 구조체 EnumCheckIn 멤버 변수 "FILESIZE" 필요시 사용 예정 (2024.10.11 minjae)
        // [Description("fileSize")]   // 파일_사이즈
        // FILESIZE    = 6
        [Description("revsnval")]      // 업로드_전_파일_버전
        REVSNVAL = 6,
        [Description("title")]         // 제목
        TITLE = 7
    }

    #endregion EnumTranslate

    #region EnumTranslateResult

    public enum EnumTranslateResult : int
    {
        [Description("code")]
        CODE = 0,
        [Description("status")]
        STATUS = 1,
        [Description("message")]
        MESSAGE = 2,
    }

    #endregion EnumTranslateResult

    #region Sample

    // TODO : 추후 필요시 enum 열거형 구조체 EnumTranslate 참고 (2024.10.11 minjae)
    //public enum EnumTranslate : int
    //{
    //    [Description("프로젝트_아이디")]
    //    PROJECTID = 0,
    //    [Description("팀_아이디")]
    //    TEAMID = 1,
    //    [Description("컨테이너_아이디")]
    //    CONTAINERID = 2,
    //    [Description("폴더_아이디")]
    //    FOLDERID = 3,
    //    // TODO : enum 열거형 구조체 EnumTranslate 멤버 변수 "UPLOADFILE" 필요시 사용 예정 (2024.09.25 minjae)
    //    //[Description("업로드_파일")]
    //    //UPLOADFILE = 4,
    //    [Description("파일_아이디")]
    //    FILEID = 4,
    //    // TODO : 추후 체크인 구현시 업로드 하려는 파일명(UPLOAD_FILENAME)과 기존 체크인 된 파일명(FILENAME)이 다를 경우
    //    //        enum 열거형 구조체 EnumCheckIn 멤버 변수 "FILENAME" 사용 예정 (2024.10.11 minjae)
    //    // [Description("파일명")]
    //    // FILENAME = 5,
    //    [Description("변환_파일명")]
    //    TRANSLATE_FILENAME = 5,
    //    // TODO : enum 열거형 구조체 EnumCheckIn 멤버 변수 "FILESIZE" 필요시 사용 예정 (2024.10.11 minjae)
    //    // [Description("파일_사이즈")]
    //    // FILESIZE    = 6
    //    [Description("업로드_전_파일_버전")]
    //    REVSNVAL = 6,
    //    [Description("제목")]
    //    TITLE = 7
    //}

    // TODO : 추후 필요시 enum 열거형 구조체 EnumTranslateResult 참고 (2024.10.11 minjae)
    //public enum EnumTranslateResult : int
    //{
    //    [Description("code")]
    //    CODE = 0,
    //    [Description("status")]
    //    STATUS = 1,
    //    [Description("message")]
    //    MESSAGE = 2
    //}

    // TODO : ResponseData 클래스 필요시 참고 (2024.10.30 minjae)
    /// <summary>
    /// Http 웹서버 통신 응답 데이터
    /// </summary>
    //public class ResponseData
    //{
    //    /// <summary>
    //    /// HttpStatusCode 상태 코드 
    //    /// </summary>
    //    public string Code { get; set; }

    //    /// <summary>
    //    /// 응답 메세지 
    //    /// </summary>
    //    public string Message { get; set; }

    //    /// <summary>
    //    /// 파일 뷰어 Url 주소
    //    /// </summary>
    //    public string Data { get; set; }
    //}

    // TODO : WebViewerPack 클래스 필요시 참고 (2024.10.14 minjae)
    /// <summary>
    /// 컨테이너에 속한 파일 웹뷰어로 출력시 필수 정보 
    /// </summary>
    //public class WebViewerPack
    //{
    //    #region 프로퍼티

    //    /// <summary>
    //    /// 요청 파라미터 - 프로젝트 아이디 
    //    /// </summary>
    //    private string _ProjectId { get; set; }

    //    /// <summary>
    //    /// 요청 파라미터 - 팀 아이디
    //    /// </summary>
    //    private string _TeamId { get; set; }

    //    /// <summary>
    //    /// 요청 파라미터 - 컨테이너 아이디
    //    /// </summary>
    //    private string _ContainerId { get; set; }

    //    /// <summary>
    //    /// 요청 파라미터 - 컨테이너에 속한 파일 아이디
    //    /// </summary>
    //    private string _FileId { get; set; }

    //    #endregion 프로퍼티

    //    #region 생성자

    //    public WebViewerPack(string projectId, string teamId, string containerId, string fileId)
    //    {
    //        _ProjectId = projectId;
    //        _TeamId = teamId;
    //        _ContainerId = containerId;
    //        _FileId = fileId;
    //    }

    //    #endregion 생성자 

    //    #region 프로젝트 아이디 

    //    /// <summary>
    //    /// 요청 파라미터 -  프로젝트 아이디 값 셋팅
    //    /// </summary>
    //    public void SetProjectId(string projectId)
    //    {
    //        _ProjectId = projectId;
    //    }

    //    /// <summary>
    //    /// 요청 파라미터 - 프로젝트 아이디 가져오기
    //    /// </summary>
    //    public string GetProjectId()
    //    {
    //        return _ProjectId;
    //    }

    //    #endregion 프로젝트 아이디 

    //    #region 팀 아이디

    //    /// <summary>
    //    /// 요청 파라미터 - 팀 아이디 값 셋팅
    //    /// </summary>
    //    public void SetTeamId(string teamId)
    //    {
    //        _TeamId = teamId;
    //    }

    //    /// <summary>
    //    /// 요청 파라미터 - 팀 아이디 가져오기
    //    /// </summary>
    //    public string GetTeamId()
    //    {
    //        return _TeamId;
    //    }

    //    #endregion 팀 아이디

    //    #region 컨테이너 아이디

    //    /// <summary>
    //    /// 요청 파라미터 - 컨테이너 아이디 값 셋팅
    //    /// </summary>
    //    public void SetContainerId(string containerId)
    //    {
    //        _ContainerId = containerId;
    //    }

    //    /// <summary>
    //    /// 요청 파라미터 - 컨테이너 아이디 가져오기
    //    /// </summary>
    //    public string GetContainerId()
    //    {
    //        return _ContainerId;
    //    }

    //    #endregion 컨테이너 아이디

    //    #region 파일 아이디 

    //    /// <summary>
    //    /// 요청 파라미터 - 파일 아이디 값 셋팅 
    //    /// </summary>
    //    public void SetFileId(string fileId)
    //    {
    //        _FileId = fileId;
    //    }

    //    /// <summary>
    //    /// 요청 파라미터 - 파일 아이디 가져오기 
    //    /// </summary>
    //    public string GetFileId()
    //    {
    //        return _FileId;
    //    }

    //    #endregion 파일 아이디 

    //    #region Sample

    //    #endregion Sample
    //}

    // TODO : TranslatePack 클래스 필요시 참고 (2024.10.14 minjae)
    /// <summary>
    /// 컨테이너 체크인 + 파일 웹서버 업로드 성공시 
    /// HubBox 파일 변환 Rest API 요청시 필수 정보 
    /// </summary>
    //public class TranslatePack
    //{
    //    #region 프로퍼티

    //    // TODO : 컨테이너 정보 프로퍼티 "_ContainerPack" 필요시 사용 예정 (2024.10.14 minjae)
    //    /// <summary>
    //    /// 컨테이너 정보
    //    /// </summary>
    //    // private ContainerPack _ContainerPack { get; set; }

    //    // TODO : 파일 정보 프로퍼티 "_FilePack" 필요시 사용 예정 (2024.10.14 minjae)
    //    /// <summary>
    //    /// 파일 정보
    //    /// </summary>
    //    // private FilePack _FilePack { get; set; }

    //    /// <summary>
    //    /// 프로젝트 아이디 
    //    /// </summary>
    //    private string _ProjectId { get; set; }

    //    /// <summary>
    //    /// 팀 아이디
    //    /// </summary>
    //    private string _TeamId { get; set; }

    //    /// <summary>
    //    /// 컨테이너 아이디
    //    /// </summary>
    //    private string _ContainerId { get; set; }

    //    /// <summary>
    //    /// 컨테이너에 속한 파일 아이디
    //    /// </summary>
    //    private string _FileId { get; set; }

    //    #endregion 프로퍼티

    //    #region 생성자

    //    // TODO : 생성자 TranslatePack(ContainerPack containerPack, FilePack filePack) 필요시 참고 (2024.10.14 minjae)
    //    //public TranslatePack(ContainerPack containerPack, FilePack filePack)
    //    //{
    //    //    _ContainerPack = containerPack;
    //    //    _FilePack = filePack;
    //    //}

    //    public TranslatePack(string projectId, string teamId, string containerId, string fileId)
    //    {
    //        _ProjectId = projectId;
    //        _TeamId = teamId;
    //        _ContainerId = containerId;
    //        _FileId = fileId;
    //    }

    //    #endregion 생성자 

    //    #region 컨테이너 정보 

    //    // TODO : 컨테이너 정보 값 셋팅 메서드 "SetContainerPack" 필요시 참고 (2024.10.14 minjae)
    //    /// <summary>
    //    /// 컨테이너 정보 값 셋팅 
    //    /// </summary>
    //    //public void SetContainerPack(ContainerPack containerPack)
    //    //{
    //    //    _ContainerPack = containerPack;
    //    //}

    //    // TODO : 컨테이너 정보 가져오기 메서드 "GetContainerPack" 필요시 참고 (2024.10.14 minjae)
    //    /// <summary>
    //    /// 컨테이너 정보 가져오기
    //    /// </summary>
    //    //public ContainerPack GetContainerPack()
    //    //{
    //    //    return _ContainerPack;
    //    //}

    //    #endregion 컨테이너 정보

    //    #region 파일 정보

    //    // TODO : 파일 정보 값 셋팅 메서드 "SetFilePack" 필요시 참고 (2024.10.14 minjae)
    //    /// <summary>
    //    /// 파일 정보 값 셋팅 
    //    /// </summary>
    //    //public void SetFilePack(FilePack filePack)
    //    //{
    //    //    _FilePack = filePack;
    //    //}

    //    // TODO : 파일 정보 가져오기 메서드 "GetFilePack" 필요시 참고 (2024.10.14 minjae)
    //    /// <summary>
    //    /// 파일 정보 가져오기
    //    /// </summary>
    //    //public FilePack GetFilePack()
    //    //{
    //    //    return _FilePack;
    //    //}

    //    #endregion 파일 정보

    //    #region 프로젝트 아이디 

    //    /// <summary>
    //    /// 프로젝트 아이디 값 셋팅
    //    /// </summary>
    //    public void SetProjectId(string projectId)
    //    {
    //        _ProjectId = projectId;
    //    }

    //    /// <summary>
    //    /// 프로젝트 아이디 가져오기
    //    /// </summary>
    //    public string GetProjectId()
    //    {
    //        return _ProjectId;
    //    }

    //    #endregion 프로젝트 아이디 

    //    #region 팀 아이디

    //    /// <summary>
    //    /// 팀 아이디 값 셋팅
    //    /// </summary>
    //    public void SetTeamId(string teamId)
    //    {
    //        _TeamId = teamId;
    //    }

    //    /// <summary>
    //    /// 팀 아이디 가져오기
    //    /// </summary>
    //    public string GetTeamId()
    //    {
    //        return _TeamId;
    //    }

    //    #endregion 팀 아이디

    //    #region 컨테이너 아이디

    //    /// <summary>
    //    /// 컨테이너 아이디 값 셋팅
    //    /// </summary>
    //    public void SetContainerId(string containerId)
    //    {
    //        _ContainerId = containerId;
    //    }

    //    /// <summary>
    //    /// 컨테이너 아이디 가져오기
    //    /// </summary>
    //    public string GetContainerId()
    //    {
    //        return _ContainerId;
    //    }

    //    #endregion 컨테이너 아이디

    //    #region 파일 아이디 

    //    /// <summary>
    //    /// 파일 아이디 값 셋팅 
    //    /// </summary>
    //    public void SetFileId(string fileId)
    //    {
    //        _FileId = fileId;
    //    }

    //    /// <summary>
    //    /// 파일 아이디 가져오기 
    //    /// </summary>
    //    public string GetFileId()
    //    {
    //        return _FileId;
    //    }

    //    #endregion 파일 아이디 

    //    #region Sample

    //    #endregion Sample
    //}
    /// <summary>
    /// 컨테이너 체크인 + 업로드 성공시 
    /// HubBox 파일 변환 Rest API 요청시 필수 정보 
    /// </summary>
    //public class TranslatePack
    //{
    //    #region 프로퍼티

    //    /// <summary>
    //    /// 요청 파라미터 - 프로젝트 아이디 
    //    /// </summary>
    //    private string _ProjectId { get; set; }

    //    /// <summary>
    //    /// 요청 파라미터 - 팀 아이디
    //    /// </summary>
    //    private string _TeamId { get; set; }

    //    /// <summary>
    //    /// 요청 파라미터 - 컨테이너 아이디
    //    /// </summary>
    //    private string _ContainerId { get; set; }

    //    /// <summary>
    //    /// 요청 파라미터 - 컨테이너에 속한 파일 아이디
    //    /// </summary>
    //    private string _FileId { get; set; }

    //    #endregion 프로퍼티

    //    #region 생성자

    //    public TranslatePack(string projectId, string teamId, string containerId, string fileId)
    //    {
    //        _ProjectId = projectId;
    //        _TeamId = teamId;
    //        _ContainerId = containerId;
    //        _FileId = fileId;
    //    }

    //    #endregion 생성자 

    //    #region 프로젝트 아이디 

    //    /// <summary>
    //    /// 요청 파라미터 -  프로젝트 아이디 값 셋팅
    //    /// </summary>
    //    public void SetProjectId(string projectId)
    //    {
    //        _ProjectId = projectId;
    //    }

    //    /// <summary>
    //    /// 요청 파라미터 - 프로젝트 아이디 가져오기
    //    /// </summary>
    //    public string GetProjectId()
    //    {
    //        return _ProjectId;
    //    }

    //    #endregion 프로젝트 아이디 

    //    #region 팀 아이디

    //    /// <summary>
    //    /// 요청 파라미터 - 팀 아이디 값 셋팅
    //    /// </summary>
    //    public void SetTeamId(string teamId)
    //    {
    //        _TeamId = teamId;
    //    }

    //    /// <summary>
    //    /// 요청 파라미터 - 팀 아이디 가져오기
    //    /// </summary>
    //    public string GetTeamId()
    //    {
    //        return _TeamId;
    //    }

    //    #endregion 팀 아이디

    //    #region 컨테이너 아이디

    //    /// <summary>
    //    /// 요청 파라미터 - 컨테이너 아이디 값 셋팅
    //    /// </summary>
    //    public void SetContainerId(string containerId)
    //    {
    //        _ContainerId = containerId;
    //    }

    //    /// <summary>
    //    /// 요청 파라미터 - 컨테이너 아이디 가져오기
    //    /// </summary>
    //    public string GetContainerId()
    //    {
    //        return _ContainerId;
    //    }

    //    #endregion 컨테이너 아이디

    //    #region 파일 아이디 

    //    /// <summary>
    //    /// 요청 파라미터 - 파일 아이디 값 셋팅 
    //    /// </summary>
    //    public void SetFileId(string fileId)
    //    {
    //        _FileId = fileId;
    //    }

    //    /// <summary>
    //    /// 요청 파라미터 - 파일 아이디 가져오기 
    //    /// </summary>
    //    public string GetFileId()
    //    {
    //        return _FileId;
    //    }

    //    #endregion 파일 아이디 

    //    #region Sample

    //    #endregion Sample
    //}

    #endregion Sample
}

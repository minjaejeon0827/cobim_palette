using System;

namespace CobimPalette.Rest.Api.CobimBase.WebView2Explorer
{
    public class ContainerHelper
    {
        #region Rest API 요청 URL

        // TODO : 6. 컨테이너 목록 조회 (파일 리스트 조회 API) const string 변수 "requestFolderURL" 필요시 수정 예정 (2023.09.20 jbh)
        /// <summary>
        /// 6. 컨테이너 목록 조회 (파일 리스트 조회 API)
        /// 요청 URL
        /// </summary>
        // public const string requestFolderURL = "https://cobim.io/api/folder/list/jc-constrct/inc-001-jc-constrct-t001";
        // public const string requestFolderURL = "https://cobim.io/api/folder/list/";
        // public const string requestFileURL = "https://cobim.io/api/container/list/:projectId/:teamId/:folderId";
        public const string requestContainerURL = "https://cobim.io/api/explorer/container/list/";

        /// <summary>
        /// 8. 연관 컨테이너 목록 조회 API
        /// </summary>
        public const string requestLinkedContainerURL = "https://cobim.io/api/explorer/container/linked/list/";

        /// <summary>
        /// 9. 컨테이너 상세 조회 API
        /// </summary>
        public const string requestDetailURL = "https://cobim.io/api/explorer/container/detail/";

        /// <summary>
        /// 10. 컨테이너 체크아웃 + 다운로드 API
        /// </summary>
        public const string requestCheckOutURL = "https://cobim.io/api/explorer/container/checkout/";


        /// <summary>
        /// 11. 컨테이너 체크인 + 업로드 API
        /// </summary>
        public const string requestCheckInURL = "https://cobim.io/api/explorer/container/checkin/";


        // TODO : 15. 컨테이너 변경(파일 변경) - (1) 파일 업로드 API const string 객체 requestCreateURL 필요시 사용 예정 (2023.11.28 jbh)
        /// <summary>
        /// 15. 컨테이너 변경(파일 변경) - (1) 파일 업로드 API
        /// </summary>
        public const string requestCreateURL = "https://cobim.io/api/explorer/file/container/create/";


        // TODO : 15. 컨테이너 변경(파일 변경) - (2) 컨테이너 변경 API const string 객체 requestUpdateURL 필요시 사용 예정 (2023.11.28 jbh)
        /// <summary>
        /// 15. 컨테이너 변경(파일 변경) - (2) 컨테이너 변경 API
        /// </summary>
        public const string requestUpdateURL = "https://cobim.io/api/explorer/container/update/";


        /// <summary>
        /// 17. 컨테이너 작업 취소
        /// </summary>
        public const string requestCancelWorkURL = "https://cobim.io/api/explorer/container/work/cancel/";

        #endregion Rest API 요청 URL

        #region 공통 - Rest API 요청 데이터

        /// <sumary>
        /// 요청 헤더 - 로그인 토큰
        /// </sumary>
        public const string authorization = "Authorization";

        /// <summary>
        /// 요청 헤더 - 테넌트 아이디
        /// </summary>
        public const string tenant = "Tenant";

        /// <summary>
        /// 요청 헤더 - 사용자 아이디
        /// </summary>
        public const string access = "Access";

        /// <summary>
        /// 컨테이너 잠금 상태 ( "Y": 잠금 ON, "N": 잠금 OFF)
        /// </summary>
        public const string yes = "Y";

        /// <summary>
        /// 컨테이너 잠금 상태 ( "Y": 잠금 ON, "N": 잠금 OFF)
        /// </summary>
        public const string no = "N";

        /// <summary>
        /// 컨테이너가 속한 상위 프로젝트 아이디
        /// </summary>
        public const string projectId = "projectId";

        /// <summary>
        /// 컨테이너가 속한 상위 팀 아이디
        /// </summary>
        public const string teamId = "teamId";

        /// <summary>
        /// 컨테이너에 속한 파일 이름
        /// </summary>
        public const string fileNm = "fileNm";

        /// <summary>
        /// 파일 아이디
        /// </summary>
        public const string fileId = "fileId";

        /// <summary>
        /// 컨테이너 ID
        /// </summary>
        public const string containerId = "containerId";

        /// <summary>
        /// 제목
        /// </summary>
        public const string title = "title";

        /// <summary>
        /// 작성자
        /// </summary>
        public const string createUserId = "createUserId";

        /// <summary>
        /// 파일 버전
        /// </summary>
        public const string revsnVal = "revsnVal";

        /// <summary>
        /// 컨테이너에 속한 파일 유형 정보
        /// </summary>
        public const string fileTypeCodeInfo = "fileTypeCodeInfo";

        /// <summary>
        /// 컨테이너가 속한 상위 폴더 아이디
        /// </summary>
        public const string folderId = "folderId";

        #endregion 공통 - Rest API 요청 데이터

        /// <summary>
        /// Http 통신 결과 값 
        /// </summary>
        public const string resultData = "resultData";

        /// <summary>
        /// 연관된 부모 컨테이너 정보 
        /// </summary>
        public const string linkedNodeInfo = "linkedNodeInfo";

        /// <summary>
        /// 컨테이너 정보, 파일정보
        /// </summary>
        public const string detail = "detail";

        /// <summary>
        /// 메타데이터 정보
        /// </summary>
        public const string metaDataList = "metaDataList";

        /// <summary>
        /// 마일스톤 업무
        /// </summary>
        public const string taskDetail = "taskDetail";

        /// <summary>
        /// 특정 마일스톤 업무 정보
        /// </summary>
        public const string milestoneTaskInfo = "milestoneTaskInfo";

        /// <summary>
        /// 마일스톤 - 업무 인덱스
        /// </summary>
        public const string idx = "idx";

        /// <summary>
        /// 파일유형 systemCode
        /// </summary>
        public const string systemCode = "systemCode";

        /// <summary>
        /// 파일유형 codeName
        /// </summary>
        public const string codeName = "codeName";

        /// <summary>
        /// 메타데이터 - 소프트웨어
        /// </summary>
        public const string soft_ifo = "soft-ifo";

        /// <summary>
        /// 메타데이터 - 소프트웨어 metaName
        /// </summary>
        public const string soft_ifo_metaName = "소프트웨어";


        /// <summary>
        /// 메타데이터 - 소프트웨어버전
        /// </summary>
        public const string soft_ver = "soft-ver";

        /// <summary>
        /// 메타데이터 - 키워드/태그
        /// </summary>
        public const string find_tag = "find-tag";

        /// <summary>
        /// 메타 데이터 - metaCode
        /// </summary>
        public const string metaCode = "metaCode";

        /// <summary>
        /// 메타 데이터 - metaComment
        /// </summary>
        public const string metaComment = "metaComment";

        /// <summary>
        /// 메타 데이터 - parentMetaCode
        /// </summary>
        public const string parentMetaCode = "parentMetaCode";

        /// <summary>
        /// 메타 데이터 - metaName
        /// </summary>
        public const string metaName = "metaName";

        /// <summary>
        /// 파일유형 - 모델 systemCode
        /// </summary>
        public const string systemCodeModel = "typmodel";

        /// <summary>
        /// 파일유형 - 모델 codeName
        /// </summary>
        public const string codeNameModel = "모델";

        /// <summary>
        /// 파일유형 - 보고서 systemCode
        /// </summary>
        public const string systemCodeReprt = "typreprt";

        /// <summary>
        /// 파일유형 - 보고서 codeName
        /// </summary>
        public const string codeNameReprt = "보고서";

        // TODO : Rest API 9. 컨테이너 상세 조회 실행시 -> 서버를 통해 오는 JSON 데이터 중 "taskDetail"의 값이 null인 경우에 사용하는 인덱스 None "taskIdxNone" 구현 (2023.10.24 jbh)
        /// <summary>
        /// 마일스톤 - 업무 인덱스 None
        /// </summary>
        public const string taskIdxNone = "0";

        // TODO : Rest API 9. 컨테이너 상세 조회 실행시 -> 서버를 통해 오는 JSON 데이터 중 "taskDetail"의 값이 null인 경우에 사용하는 업무 유형 None "taskDetailTypeNone" 구현 (2023.10.24 jbh)
        /// <summary>
        /// 마일스톤 - 업무 유형 None
        /// </summary>
        public const string taskTypeNone = "관련업무선택";

        /// <summary>
        /// 마일스톤 - 업무 인덱스 요구사항정의
        /// </summary>
        public const string taskIdxDefine = "20";

        /// <summary>
        /// 마일스톤 - 업무 유형 요구사항정의
        /// </summary>
        public const string taskTypeDefine = "요구사항정의";

        /// <summary>
        /// 마일스톤 - 업무 인덱스 엔터티도출
        /// </summary>
        public const string taskIdxEntity = "21";

        /// <summary>
        /// 마일스톤 - 업무 유형 엔터티도출
        /// </summary>
        public const string taskTypeEntity = "엔터티도출";

        /// <summary>
        /// 마일스톤 - 업무 인덱스 요구사항정의서 작성
        /// </summary>
        public const string taskIdxCreation = "22";

        /// <summary>
        /// 마일스톤 - 업무 유형 요구사항정의서 작성
        /// </summary>
        public const string taskTypeCreation = "요구사항정의서 작성";

        /// <summary>
        /// 마일스톤 - 업무 인덱스 UI설계서 작성
        /// </summary>
        public const string taskIdxUI = "23";


        /// <summary>
        /// 마일스톤 - 업무 유형 UI설계서 작성
        /// </summary>
        public const string taskTypeUI = "UI설계서 작성";

        #region 기본 정보 탭 - Label

        /// <summary>
        /// 공통 (컨테이너 정보, 파일정보) - 구분 
        /// </summary>
        public const string divLabel = "구분";

        /// <summary>
        /// 공통 (컨테이너 정보, 파일정보) - 내용
        /// </summary>
        // contents 의미
        // 참고 URL - https://confusingtimes.tistory.com/1821
        public const string contentsLabel = "내용";

        /// <summary>
        /// 컨테이너 정보 - 작성자
        /// </summary>
        public const string createUserLabel = "작성자";

        /// <summary>
        /// 컨테이너 정보 - 버전
        /// </summary>
        public const string versionLabel = "버전";

        /// <summary>
        /// 컨테이너 정보 - 제목
        /// </summary>
        public const string titleLabel = "제목";

        // TODO : 컨테이너 정보 Label const string 문자열 객체 "soft_ifo_Label", "soft_ver_Label", "find_tag_Label" 필요시 사용 예정 (2023.10.19 jbh)
        /// <summary>
        /// 컨테이너 정보 - 소프트웨어
        /// </summary>
        public const string soft_ifo_Label = "소프트웨어";

        /// <summary>
        /// 컨테이너 정보 - 소프트웨어버전
        /// </summary>
        public const string soft_ver_Label = "소프트웨어버전";

        /// <summary>
        /// 컨테이너 정보 - 키워드/태그
        /// </summary>
        public const string find_tag_Label = "키워드/태그";

        /// <summary>
        /// 컨테이너 정보 - 파일유형
        /// </summary>
        public const string fileTypeCodeLabel = "파일유형";

        /// <summary>
        /// 컨테이너 정보 - 마일스톤
        /// </summary>
        public const string taskDetailLabel = "마일스톤";

        /// <summary>
        /// 파일정보 - 파일명
        /// </summary>
        public const string fileNameLabel = "파일명";

        #endregion 기본 정보 탭 - Label

        #region 컨테이너 

        /// <summary>
        /// 컨테이너 그룹
        /// </summary>
        public const string containerGroup = "containerGroup";

        /// <summary>
        /// 등록일시
        /// </summary>
        public const string createDateTime = "createDateTime";

        /// <summary>
        /// 컨테이너에 속한 파일 유형 이름 
        /// </summary>
        public const string fileTypeCodeName = "codeName";

        /// <summary>
        /// 컨테이너에 속한 파일 유형
        /// </summary>
        public const string fileTypeCode = "fileTypeCode";


        /// <summary>
        /// 컨테이너에 속한 파일 단계 정보
        /// </summary>
        public const string stepCodeInfo = "stepCodeInfo";

        /// <summary>
        /// 컨테이너에 속한 파일 단계 이름 
        /// </summary>
        public const string stepCodeName = "codeName";

        /// <summary>
        /// 컨테이너에 속한 파일 상태 정보
        /// </summary>
        public const string statusCodeInfo = "statusCodeInfo";

        /// <summary>
        /// 컨테이너에 속한 파일 체크 정보
        /// </summary>
        public const string checkCodeInfo = "checkCodeInfo";

        /// <summary>
        /// 컨테이너에 속한 파일 상태 이름
        /// </summary>
        public const string statusCodeName = "codeName";

        /// <summary>
        /// 컨테이너에 속한 파일 정보 웹뷰어로 출력 가능 여부  
        /// </summary>
        public const string convertState = "convertState";

        /// <summary>
        /// 파일 리스트
        /// </summary>
        public const string fileList = "fileList";

        /// <summary>
        /// 파일 컨테이너 리스트 (파일 속성 정보 + 메타 데이터 포함) 
        /// </summary>
        public const string containerList = "containerList";

        #endregion 컨테이너 

        #region 연관 컨테이너 

        /// <summary>
        /// 부모 컨테이너 아이디
        /// </summary>
        public const string targetId = "targetId";

        /// <summary>
        /// 자식 컨테이너 아이디(또는 자기 자신 컨테이너 ID)
        /// </summary>
        public const string sourceId = "sourceId";

        #endregion 연관 컨테이너

        #region 체크아웃 - 체크인 공통 

        /// <summary>
        /// 컨테이너 작업중인 사용자 아이디
        /// </summary>
        public const string workUserId = "workUserId";

        /// <summary>
        /// 상태
        /// </summary>
        public const string statusCode = "statusCode";

        /// <summary>
        /// 컨테이너 잠금 상태 ( "Y": 잠금 ON, "N": 잠금 OFF)
        /// </summary>
        public const string readYn = "readYn";

        /// <summary>
        /// 컨테이너 검토상태  ("comstt10": 미진행, "comstt20": 진행, "comstt30": 완료)
        /// </summary>
        public const string checkStatusCode = "checkStatusCode";

        #endregion 체크아웃 - 체크인 공통

        #region 체크아웃

        /// <summary>
        /// 체크아웃 가능 조건 - 컨테이너 readYn 필드 값이 'N' 일 때
        /// </summary>
        public const string containerLockOff = "N";

        /// <summary>
        /// 체크아웃 가능 조건 2 - 검토 요청 중인 컨테이너가 아닐 때(statusCode != 'comstt20' ) 
        /// </summary>
        public const string containerReview = "comstt20";

        #endregion 체크아웃

        #region 체크인 

        /// <summary>
        /// 11. 컨테이너 체크인 + 업로드
        /// 요청 Body - Content-Type - form-data 형식
        /// </summary>
        public const string formDataContentType = "multipart/form-data";

        /// <summary>
        /// 업로드 할 파일
        /// </summary>
        public const string uploadFiles = "uploadFiles";

        /// <summary>
        /// 파일명
        /// </summary>
        public const string fileName = "fileName";

        /// <summary>
        /// 파일사이즈
        /// </summary>
        public const string fileSize = "fileSize";

        /// <summary>
        /// 변경사유
        /// </summary>
        public const string comment = "comment";

        #endregion 체크인

        #region 컨테이너 작업 취소 

        /// <summary>
        /// 17. 컨테이너 작업 취소
        /// 요청 Body - Content-Type
        /// </summary>
        public const string contentType = "application/json";

        #endregion 컨테이너 작업 취소

        #region Sample

        #endregion Sample
    }
}

using Serilog;

using System.Reflection;
using CobimUtil;

namespace CobimPalette.Rest.Api.CobimBase.HubBox
{
    public class HubBoxHelper
    {
        #region 어셈블리

        /// <summary>
        /// 응용 프로그램 유형 - CobimExplorer
        /// </summary>
        public const string cobimExplorer = "CobimExplorer";

        /// <summary>
        /// 응용 프로그램 유형 - CobimDockableDialogs
        /// </summary>
        public const string cobimDockableDialogs = "CobimDockableDialogs";

        /// <summary>
        /// 응용 프로그램 유형 - CobimPalette
        /// </summary>
        public const string cobimPalette = "CobimPalette";

        /// <summary>
        /// 어셈블리(.dll 파일) 이름 
        /// (예) CobimExplorer.Rest.Api / CobimDockableDialogs.Rest.Api / CobimPalette.Rest.Api
        /// </summary>
        public static string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// 실행 중인 응용 프로그램 이름 
        /// </summary>
        public static string appName = GetAppName(assemblyName);

        /// <summary>
        /// 어셈블리 이름 문자열 배열 중 
        /// 응용 프로그램 이름이 존재하는 인덱스 번호 
        /// </summary>
        public const int appNameIdx = 0;

        #endregion 어셈블리 

        #region Rest API Port 번호 

        /// <summary>
        /// Revit 애드인 프로그램(CobimDockableDialogs.dll)
        /// AutoCAD 애드인 프로그램(CobimPalette.dll)
        /// 전용 웹 탐색기 - cobim_web_explorer Port 번호 
        /// </summary>
        public const string web_explorerPort = "8060";

        /// <summary>
        /// 탐색기 응용프로그램 (CobimExplorer.exe) 전용 
        /// 탐색기 - cobim_translator Port 번호 
        /// </summary>
        public const string translatorPort = "8100";

        #endregion Rest API Port 번호  

        #region Rest API 요청 URL

        // TODO : 로컬 서버, 개발 서버, 운영 서버 용어 정의 참고 (2024.08.26 minjae)
        // 참고 URL - https://velog.io/@ryucherry/Server-%EB%A1%9C%EC%BB%AC%EC%84%9C%EB%B2%84-%EA%B0%9C%EB%B0%9C%EC%84%9C%EB%B2%84-%EC%8A%A4%ED%85%8C%EC%9D%B4%EC%A7%95%EC%84%9C%EB%B2%84-%EC%9A%B4%EC%98%81%EC%84%9C%EB%B2%84-%EB%9E%80

        /// <summary>
        /// HubBox Dev 로컬 서버 Rase URL
        /// </summary>
        // public const string localBaseURL = "http://localhost:";

        // TODO : AutoCAD 애드인 프로그램(CobimPalette.dll)에서 Rest API 요청시 (HTTP GET / POST)
        //        HTTP URL 주소에 문자열 "/cad" 추가해서 보낼 수 있도록 아래처럼 코드 "(appName.Equals(cobimPalette) ? "/cad" : string.Empty)" 추가 (2024.11.13 minjae)
        /// <summary>
        /// HubBox 개발 서버 Base URL
        /// </summary>
        // public const string DevBaseURL = "http://convert.cobim.kr:";
        // public const string devBaseURL = "http://explorer.cobim.kr:";
        public static string devBaseURL = "http://explorer.cobim.kr:" + (appName.Equals(cobimExplorer) ? translatorPort : web_explorerPort) + (appName.Equals(cobimPalette) ? "/cad" : string.Empty);

        // TODO : 아래 주석친 코드 public static string devBaseURL = "http://convert.cobim.kr:" + (appName.Equals(cobimExplorer) ? translatorPort : web_explorerPort); 필요시 참고 (2024.10.14 minjae)
        // public static string devBaseURL = "http://convert.cobim.kr:" + (appName.Equals(cobimExplorer) ? translatorPort : web_explorerPort);


        // TODO : AutoCAD 애드인 프로그램(CobimPalette.dll)에서 Rest API 요청시 (HTTP GET / POST)
        //        HTTP URL 주소에 문자열 "/cad" 추가해서 보낼 수 있도록 아래처럼 코드 "(appName.Equals(cobimPalette) ? "/cad" : string.Empty)" 추가 (2024.11.13 minjae)
        // TODO : 아래 3가지 프로그램은 Port 번호가 서로 다르지만 (translatorPort, web_explorerPort)
        //        체크인 처리 후 POST - 3. 컨테이너에 속한 특정 파일 변환 API 요청시에는
        //        Port 번호는 탐색기 응용 프로그램(CobimExplorer.exe)인 translatorPort = 8100 으로 통일해야
        //        3가지 프로그램 모두 체크인 처리 후 ACC 파일 변환 요청이 정상적으로 처리 된다.

        //        1. 탐색기 응용 프로그램(CobimExplorer.exe)
        //        2. Revit 애드인 프로그램(CobimDockableDialogs.dll)
        //        3. AutoCAD 애드인 프로그램(CobimPalette.dll) 
        /// <summary>
        /// HubBox Translate Rest API Base URL 
        /// </summary>
        public static string devTranslateBaseURL = "http://explorer.cobim.kr:" + translatorPort + (appName.Equals(cobimPalette) ? "/cad" : string.Empty);


        // TODO : 추후 필요시 HubBox 운영 서버 Base URL const string 객체 requestProBaseURL 구현 예정 (2024.08.26 minjae)
        /// <summary>
        /// HubBox 운영 서버 Base URL
        /// </summary>
        // public const string proBaseURL = "http://explorer.cobim.kr:";

#if DEBUG   // Debug 모드인 경우

        /// <summary>
        /// Revit 애드인 프로그램(CobimDockableDialogs.dll) 전용 
        /// AutoCAD 애드인 프로그램(CobimPalette.dll)
        /// 웹 탐색기 - 로그인 URL 주소 
        /// </summary>
        public static string requestLoginURL = devBaseURL;


        /// <summary>
        /// GET - 1. HubBox ACC 변환 내역 화면 조회 API 주소
        /// 개발 서버에서 사용하는 
        /// Http URL 주소 예시 - http://explorer.cobim.kr:8100/web/convert/list
        /// </summary>
        //public const string requestWebConvertListURL = localBaseURL + "/web/convert/list";
        public static string requestWebConvertListURL = devBaseURL + "/web/convert/list";

        /// <summary>
        /// GET - 2. 컨테이너에 속한 특정 파일 변환 후 해당 파일 웹뷰어 조회 API
        /// </summary>
        //public const string requestWebViewerURL = localBaseURL + "/acc/viewer/";
        public static string requestWebViewerURL = devBaseURL + "/acc/viewer/";

        // TODO : 아래 3가지 프로그램은 Port 번호가 서로 다르지만 (translatorPort, web_explorerPort)
        //        체크인 처리 후 POST - 3. 컨테이너에 속한 특정 파일 변환 API 요청시에는
        //        Port 번호는 탐색기 응용 프로그램(CobimExplorer.exe)인 translatorPort = 8100 으로 통일해야
        //        3가지 프로그램 모두 체크인 처리 후 ACC 파일 변환 요청이 정상적으로 처리 된다.

        //        1. 탐색기 응용 프로그램(CobimExplorer.exe)
        //        2. Revit 애드인 프로그램(CobimDockableDialogs.dll)
        //        3. AutoCAD 애드인 프로그램(CobimPalette.dll) 
        // TODO : const string 객체 requestTranslateURL 사용해서 추후 체크인 API 호출하기 전에
        //        해당 Rest API 연동 기능 구현 및 호출해서 사용할 예정 (2024.08.23 minjae) 
        /// <summary>
        /// POST - 3. 컨테이너에 속한 특정 파일 변환 API
        /// </summary>
        //public const string requestTranslateURL = localBaseURL + "/acc/convert/";
        public static string requestTranslateURL = devTranslateBaseURL + "/acc/convert/";

#else   // Release 모드인 경우

        /// <summary>
        /// GET - 1. HubBox 웹사이트 변환 목록 화면 조회 API 주소
        /// 개발 서버에서 사용하는 
        /// Http URL 주소 예시 - http://explorer.cobim.kr:8100/web/convert/list
        /// </summary>
        // public const string requestWebConvertListURL = "http://explorer.cobim.kr:8100/web/convert/list";
        public static string requestWebConvertListURL = devBaseURL + "/web/convert/list";

        /// <summary>
        /// GET - 2. 컨테이너에 속한 특정 파일 변환 후 해당 파일 웹뷰어 조회 API
        /// </summary>
        // public const string requestWebViewerURL = "http://explorer.cobim.kr:8100/acc/viewer/";
        public static string requestWebViewerURL = devBaseURL + "/acc/viewer/";

        // TODO : const string 객체 requestTranslateURL 사용해서 추후 체크인 API 호출하기 전에
        //        해당 Rest API 연동 기능 구현 및 호출해서 사용할 예정 (2024.08.23 minjae) 
        /// <summary>
        /// POST - 3. 컨테이너에 속한 특정 파일 변환 API
        /// </summary>
        // public const string requestTranslateURL = "http://explorer.cobim.kr:8100/acc/convert/";
        public static string requestTranslateURL = devBaseURL + "/acc/convert/";

#endif

        #endregion Rest API 요청 URL

        #region Rest API 응답 데이터 (1. 파일 변환 요청 / 2. 파일 뷰어 요청)

        #region 공통

        /// <summary>
        /// HttpStatusCode 상태 코드 키
        /// </summary>
        public const string code = "code";

        /// <summary>
        /// HttpStatusCode 오류 상태 코드 키
        /// </summary>
        //public const string statusCode = "statusCode"; 

        /// <summary>
        /// 응답 메세지 키 
        /// </summary>
        public const string message = "message";

        /// <summary>
        /// 응답 데이터 - 파일 뷰어 또는 변환 요청 성공 응답 메세지
        /// </summary>
        public const string successMessage = "success";

        /// <summary>
        /// 응답 데이터 - 변환 처리된 파일 뷰어 Url 주소
        /// </summary>
        public const string data = "data";

        #endregion 공통

        #region 1. 파일 변환 요청


        #endregion 1. 파일 변환 요청

        #region 2. 파일 뷰어 요청

        /// <summary>
        /// (파일) 변환 성공(success) - 파일 (웹)뷰어 보기 가능
        /// </summary>
        public const string successWebViewerVisibility = "Visible";

        #region HttpStatusCode 상태 코드 값

        /// <summary>
        /// (파일) 변환 성공(success) 
        /// </summary>
        public const string successStatusCode = "200";

        /// <summary>
        /// (파일) 변환중입니다.(ACC에서 파일 변환 진행 중)
        /// </summary>
        public const string translateStatusCode = "202";

        /// <summary>
        /// (파일) 변환에 실패 한 파일입니다.(ACC에서 파일 변환 실패)
        /// </summary>
        public const string failureStatusCode = "204";

        /// <summary>
        /// (파일) 변환 내역이 없습니다.(해당 프로젝트ID, 팀ID, 컨테이너ID, 파일ID로 이미 변환 된 파일이 없는 경우)
        /// </summary>
        public const string notExistTranslateStatusCode = "404";

        #endregion HttpStatusCode 상태 코드 값

        /// <summary>
        /// GET - 2. 파일 변환 요청 실패 (지원하지 않는 파일 형식 또는 동일한 프로젝트ID, 팀ID, 컨테이너ID, 파일ID로 이미 변환 된 파일)
        /// HttpStatusCode 상태 코드 값
        /// </summary>
        // public const string failureStatusCode = "422";

        /// <summary>
        /// GET - 2. 파일 변환 요청 응답 메세지 (동일한 프로젝트ID, 팀ID, 컨테이너ID, 파일ID로 이미 변환 된 파일) 
        /// </summary>
        // public const string convertMessage = "This file has already been converted.";


        #endregion 2. 파일 뷰어 요청

        #endregion Rest API 응답 데이터 (1. 파일 변환 요청 / 2. 파일 뷰어 요청) 

        #region GetAssemblyName

        /// <summary>
        /// 실행 중인 응용 프로그램 이름 구하기 
        /// 메서드 파라미터(pAssemblyName)에 할당되는 문자열 
        /// (예) CobimExplorer.Rest.Api / CobimDockableDialogs.Rest.Api / CobimPalette.Rest.Api
        /// </summary>
        private static string GetAppName(string pAssemblyName)
        {
            string appName = string.Empty;                       // 현재 실행 중인 응용 프로그램 이름 초기화 

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : 실행 중인 응용 프로그램 이름 구하기 위해 아래 방법으로 구현 (2024.09.26 minjae)
                // 참고 URL - https://gent.tistory.com/502
                //        1. Split 메서드를 사용하여 문자열 "pAssemblyName"을 특정 구분자('.')로 나눠서 배열로 변환
                //        2. 특정 구분자('.') 기준으로 나눈 문자열 배열에서 인덱스 appNameIdx(0 번지)에 존재하는
                //           실행 중인 응용 프로그램 이름(assemblyArray[appNameIdx])을 string 클래스 객체 appName에 할당 
                //        3. 2번에서 구한 실행 중인 응용 프로그램 이름(appName) 리턴  
                string[] assemblyArray = pAssemblyName.Split('.');
                appName = assemblyArray[appNameIdx];
            }
            catch(Exception ex)
            {
                Log.Logger.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message);
            }
            return appName;
        }

        #endregion GetAssemblyName

        #region Sample

        #endregion Sample
    }
}

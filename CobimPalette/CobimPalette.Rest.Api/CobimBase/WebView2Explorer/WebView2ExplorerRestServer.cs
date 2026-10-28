using Serilog;

using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Diagnostics;

using CobimPalette.Data.Models.CobimBase.WebView2Explorer;
using CobimPalette.Data.Models.CobimBase.Login;
using CobimUtil;

namespace CobimPalette.Rest.Api.CobimBase.WebView2Explorer
{
    /// <summary>
    /// 성과품(CDE) 조회 Rest API 클래스 
    /// </summary>
    public class WebView2ExplorerRestServer
    {

        #region GetFileDownLoadAsync

        // TODO : Rest API 파일 다운로드 (Get) 메서드 "GetFileDownLoadAsync" 구현 (2024.10.07 jbh)
        // 참고 URL - https://asecurity.dev/entry/NET-Download-file-from-HttpClientWebClient
        /// <summary>
        /// 7. 파일 다운로드 (Get)
        /// Rest API 메서드 파라미터 "WebView2ExplorerView pInputParamData"
        /// </summary>
        public static async Task GetFileDownLoadAsync(WebView2ExplorerView pInputParamData)
        {
            HttpClient client = pInputParamData.GetHttpClient();                     // HttpClient client 객체 가져오기 
            string commonDirectoryPath = pInputParamData.GetCommonDirectoryPath();   // 폴더 (디렉토리) 공통 경로 값 가져오기 
            RequestHeader requestHeader = pInputParamData.GetRequestHeader();        // Http 서버 통신 요청 Header 값 가져오기 
            ContainerPack containerPack = pInputParamData.GetContainerPack();        // 컨테이너 정보 값 가져오기 
            FilePack filePack = pInputParamData.GetFilePack();                       // 다운로드할 파일 정보 값 가져오기 

            string downloadFilePath = string.Empty;        // 다운로드 받으려고 하는 최종 파일 경로 (파일 전체 경로 + 이름 중복일 때 변경된 파일 이름)

            // TODO : C# 문자열 보간 기능 사용해서 로그파일 생성할 상위 폴더 경로 문자열로 변환 (2024.10.07 jbh)
            // 참고 URL - https://gaeunhan.tistory.com/61
            // TODO : 다른 PC에서도 파일 다운로드 기능이 실행될 수 있도록 다운로드 받을 파일 전체 경로 "dirPath"의 루트 디렉토리 값을 내문서 폴더 (Environment.SpecialFolder.MyDocuments)로 설정 (2024.10.07 jbh)
            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.environment.specialfolder?view=net-7.0
            // 파일 다운로드 전체 경로 예시 - "C:\Users\bhjeon\Documents\COBIM\CobimPalette\bmcb\inc-001-bmcb-t005\bmcb-imbu-func_016-sptl_901-002" - (SpecialFolder 시스템 디렉토리(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) 사용)
            // TODO : 메서드 Assembly.GetCallingAssembly() 사용하여 비동기 메서드 "GetFileDownLoadAsync" 호출한 어셈블리 (CobimPalette) 문자열 가져올 때 실패한 원인(문자열 "System.Private.CoreLib" 으로 리턴) 파악 예정 (2024.10.07 jbh)
            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.reflection.assembly.getcallingassembly?view=net-8.0
            // string dirPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\COBIM\\{Assembly.GetCallingAssembly().GetName().Name}\\{containerPack.GetProjectId()}\\{containerPack.GetTeamId()}\\{containerPack.GetContainerId()}";
            string dirPath = $"{commonDirectoryPath}\\{containerPack.GetProjectId()}\\{containerPack.GetTeamId()}\\{containerPack.GetContainerId()}";
            string filePath = dirPath + $"\\{filePack.GetFileName()}";

            DirectoryInfo di = new DirectoryInfo(dirPath);

            FileInfo fi = new FileInfo(filePath);

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : Http 통신 Get 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 메시지 출력 기능 구현 예정 (2024.10.07 jbh)

                // HttpClient 클래스 객체 "client"의 요청헤더(DefaultRequestHeaders)에 데이터 추가 하기 전 초기화
                client.DefaultRequestHeaders.Clear();

                // 요청 헤더 (requestHeader) HttpClient 클래스 객체 "client"에 추가 
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.authorization, requestHeader.GetAuthorization());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.tenant, requestHeader.GetTenant());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.access, requestHeader.GetAccess());

                // TODO : StringBuilder 클래스 객체 requestSb 생성 -> 7. 파일 다운로드 (파일 다운로드 Rest API) URL 주소에 문자열 추가 (2024.10.07 jbh)
                // StringBuilder vs String 비교 
                // 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%ED%99%9C%EC%9A%A9-String-StringBuilder-%EC%84%B1%EB%8A%A5-%EC%B0%A8%EC%9D%B4

                // Http 통신(Get) 테스트용 URL 주소 - requestFileDownLoadURL = "https://cobim.io/api/explorer/file/container/download/jc-constrct/inc-001-jc-constrct-t001";
                StringBuilder requestSb = new StringBuilder(FileHelper.requestFileDownLoadURL);
                requestSb.Append(containerPack.GetProjectId());
                requestSb.Append("/");
                requestSb.Append(containerPack.GetTeamId());
                requestSb.Append("/");
                requestSb.Append(containerPack.GetContainerId());
                requestSb.Append("/");
                requestSb.Append(filePack.GetFileId());

                var requestFileDownLoadURL = requestSb.ToString();

                // TODO : HttpClient Get 방식으로 서버에 요청할 때, using 문 사용 (2024.10.07 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/fundamentals/networking/http/httpclient
                using HttpResponseMessage response = await client.GetAsync(requestFileDownLoadURL);

                string resMsg = await response.Content.ReadAsStringAsync();

                // Rest API 7. 파일 다운로드 Http 통신 결과가 성공할 경우 
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    var ms = await response.Content.ReadAsStreamAsync();

                    // 디렉터리(di)가 존재하지 않는 경우
                    if (false == di.Exists) Directory.CreateDirectory(dirPath);   // 디렉터리 새로 생성

                    // 새로 다운로드 받으려는 파일 전체 경로 - dirPath + 파일이름 - fi.Name 구하기
                    downloadFilePath = FileHelper.FileDownLoadPath(dirPath, containerPack, fi.Name);


                    // TODO : 해당 using 문장을 벗어나면 FileStream 클래스 객체 fs Dipose 처리 (리소스 해제 및 자원 반납) 구현 (2024.10.07 jbh)
                    //        아래 소스코드를 using문으로 구현 안하고 실행시 파일을 생성되나 FileStream 클래스 객체 fs가 close 처리가 자동으로 되지 않아서 
                    //        다운로드 받은 이미지 파일(.jpg, .png 등등...)을 열어도 손상된 상태라고 하며 이미지를 볼 수 없으니 주의해야 한다.
                    // 참고 URL - https://hongjinhyeon.tistory.com/92
                    // TODO : httpClient 객체 client 사용 URL을 이용한 파일 다운로드 구현 (2024.10.07 jbh)
                    // 참고 URL - https://asecurity.dev/entry/NET-Download-file-from-HttpClientWebClient
                    // 참고 2 URL - https://stackoverflow.com/questions/45711428/download-file-with-webclient-or-httpclient
                    using (var fs = File.Create(downloadFilePath))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        await ms.CopyToAsync(fs);
                        await ms.FlushAsync();
                        await fs.FlushAsync();

                        // fs.Close();
                    }

                    Log.Logger.Information(Logger.GetMethodPath(currentMethod) + "7. 파일 다운로드 성공");

                    return;
                }

                // Rest API 7. 파일 다운로드 Http 통신 결과가 실패할 경우 
                else throw new Exception(resMsg);
            }
            catch (Exception ex)
            {
                // TODO : Http 통신 Get 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 메시지 출력 기능 구현 예정 (2024.10.07 jbh)
                Log.Logger.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달 throw
            }
        }

        #endregion GetFileDownLoadAsync

        #region GetLinkedContainerListAsync

        /// <summary>
        /// 8. 연관 컨테이너 목록 조회 (Get) - 연관 컨테이너 정보(참조관계 - 부모/자식 컨테이너)
        /// Rest API 메서드 파라미터 "WebView2ExplorerView pInputParamData"
        /// </summary>
        public static async Task<List<Dictionary<string, object>>> GetLinkedContainerListAsync(WebView2ExplorerView pInputParamData)
        {
            HttpClient client = pInputParamData.GetHttpClient();                     // HttpClient client 객체 가져오기 
            // string commonDirectoryPath = pInputParamData.GetCommonDirectoryPath();   // 폴더 (디렉토리) 공통 경로 값 가져오기
            RequestHeader requestHeader = pInputParamData.GetRequestHeader();        // Http 서버 통신 요청 Header 값 가져오기 
            ContainerPack containerPack = pInputParamData.GetContainerPack();        // 컨테이너 정보 값 가져오기 
            FilePack filePack = pInputParamData.GetFilePack();                       // 다운로드할 파일 정보 값 가져오기 

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : Http 통신 Get 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 메시지 출력 기능 구현 예정 (2024.10.08 jbh)

                // HttpClient 클래스 객체 "client"의 요청헤더(DefaultRequestHeaders)에 데이터 추가 하기 전 초기화
                client.DefaultRequestHeaders.Clear();

                // TODO : 로그인 JWT 토큰 값 들어있는 string 클래스 객체 "authorization"를 bearer 토큰으로 변환하기 위해 문자열 "bearer " 추가해서
                // HttpClient 요청 헤더 client.DefaultRequestHeaders에 추가(TryAddWithoutValidation) (2024.10.08 jbh)
                // 추후 인재 INC와 소통 진행 -> JWT 토큰 또는 bearer 토큰 둘 중에 하나로 통일 예정 
                // 최종 결과 - client.DefaultRequestHeaders.Clear(); 사용하면 bearer 토큰 쓸 필요 없이 JWT 토큰만으로 사용 가능

                // 요청 헤더 (requestHeader) HttpClient 클래스 객체 "client"에 추가 
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.authorization, requestHeader.GetAuthorization());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.tenant, requestHeader.GetTenant());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.access, requestHeader.GetAccess());

                // TODO : StringBuilder 클래스 객체 requestSb 생성 -> 8. 연관 컨테이너 목록 조회 URL 주소에 문자열 추가 (2024.10.08 jbh)
                // StringBuilder vs String 비교 
                // 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%ED%99%9C%EC%9A%A9-String-StringBuilder-%EC%84%B1%EB%8A%A5-%EC%B0%A8%EC%9D%B4
                StringBuilder requestSb = new StringBuilder(ContainerHelper.requestLinkedContainerURL);
                requestSb.Append(containerPack.GetProjectId());
                requestSb.Append("/");
                requestSb.Append(containerPack.GetTeamId());
                requestSb.Append("/");
                requestSb.Append(containerPack.GetContainerId());

                var requestLinkedNodeInfoURL = requestSb.ToString();

                // TODO : HttpClient Get 방식으로 서버에 요청할 때, using 문 사용 (2024.10.08 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/fundamentals/networking/http/httpclient
                using HttpResponseMessage response = await client.GetAsync(requestLinkedNodeInfoURL);

                string resMsg = await response.Content.ReadAsStringAsync();

                // Rest API 8. 연관 컨테이너 목록 조회 Http 통신 결과가 성공할 경우 
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    var jtmp = (JsonObject.Parse(resMsg)[ContainerHelper.resultData] as JsonArray).Select(x => x);

                    // TODO : 연관 컨테이너 리스트에 속한 컨테이너 데이터 Dictionary 객체의 Key "containerId"의 값(Value)을 기준으로 오름차순(OrderBy) 정렬하여 리스트 구현 (2024.10.08 jbh)
                    // 참고 URL   - https://codechacha.com/ko/csharp-sort-list/
                    // 참고 2 URL - https://developer-talk.tistory.com/694
                    // 참고 3 URL - https://developer-talk.tistory.com/210
                    var linkedNodeInfoList = jtmp.Select(x => JsonSerializer.Deserialize<Dictionary<string, object>>(x)).OrderBy(x => x[ContainerHelper.containerId].ToString()).ToList();

                    Log.Logger.Information(Logger.GetMethodPath(currentMethod) + "8. 연관 컨테이너 목록 조회 성공");

                    return linkedNodeInfoList;
                }

                // Rest API 8. 연관 컨테이너 목록 조회 Http 통신 결과가 실패할 경우 
                else throw new Exception(resMsg);

            }
            catch (Exception ex)
            {
                // TODO : Http 통신 Get 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 메시지 출력 기능 구현 예정 (2024.10.08 jbh)
                Log.Logger.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달 throw 
            }
        }

        #endregion GetLinkedContainerListAsync

        #region GetCheckOutAsync

        /// <summary>
        /// 10. 컨테이너 체크아웃 + (파일)다운로드 (Get)
        /// Rest API 메서드 파라미터 "WebView2ExplorerView pInputParamData"
        /// </summary>
        public static async Task GetCheckOutAsync(WebView2ExplorerView pInputParamData)
        {
            // string createUserId  = string.Empty;     // 컨테이너 생성자
            // string workUserId    = string.Empty;     // 컨테이너 작업중인 사용자 아이디
            // string readYn        = string.Empty;     // 컨테이너 잠금 상태 ( "Y": 잠금 ON, "N": 잠금 OFF)
            // string statusCode    = string.Empty;     // 컨테이너 검토상태 ("comstt10": 미진행, "comstt20": 진행, "comstt30": 완료)

            HttpClient client = pInputParamData.GetHttpClient();                 // HttpClient client 객체 가져오기 
            string commonDirectoryPath = pInputParamData.GetCommonDirectoryPath();   // 폴더 (디렉토리) 공통 경로 값 가져오기
            RequestHeader requestHeader = pInputParamData.GetRequestHeader();   // Http 서버 통신 요청 Header 값 가져오기 
            ContainerPack containerPack = pInputParamData.GetContainerPack();    // 컨테이너 정보 값 가져오기 
            FilePack filePack = pInputParamData.GetFilePack();                   // 다운로드할 파일 정보 값 가져오기 

            string downloadFilePath = string.Empty;     // 다운로드 받으려고 하는 최종 파일 경로 (파일 전체 경로 + 이름 중복일 때 변경된 파일 이름)

            // TODO : 다른 PC에서도 파일 다운로드 기능이 실행될 수 있도록 다운로드 받을 파일 경로 "dirPath"의 루트 디렉토리 값을 내문서 폴더 (Environment.SpecialFolder.MyDocuments)로 설정 (2024.10.08 jbh)
            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.environment.specialfolder?view=net-7.0
            // TODO : 메서드 Assembly.GetCallingAssembly() 사용하여 비동기 메서드 "GetCheckOutAsync" 호출한 어셈블리 (CobimPalette) 문자열 가져올 때 실패한 원인(문자열 "System.Private.CoreLib" 으로 리턴) 파악 예정 (2024.10.08 jbh)
            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.reflection.assembly.getcallingassembly?view=net-8.0
            // string dirPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\COBIM\\{Assembly.GetCallingAssembly().GetName().Name}\\{containerPack.GetProjectId()}\\{containerPack.GetTeamId()}\\{containerPack.GetContainerId()}";
            string dirPath = $"{commonDirectoryPath}\\{containerPack.GetProjectId()}\\{containerPack.GetTeamId()}\\{containerPack.GetContainerId()}";
            string filePath = dirPath + $"\\{filePack.GetFileName()}";
            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo fi = new FileInfo(filePath);

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : Http 통신 Get 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 메시지 출력 기능 구현 예정 (2024.10.08 jbh)

                // HttpClient 클래스 객체 "client"의 요청헤더(DefaultRequestHeaders)에 데이터 추가 하기 전 초기화
                client.DefaultRequestHeaders.Clear();

                // 요청 헤더 (requestHeader) HttpClient 클래스 객체 "client"에 추가 
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.authorization, requestHeader.GetAuthorization());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.tenant, requestHeader.GetTenant());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.access, requestHeader.GetAccess());

                // TODO : StringBuilder 클래스 객체 requestSb 생성 -> 10. 컨테이너 체크아웃 + (파일)다운로드 Rest API URL 주소에 문자열 추가 (2024.10.08 jbh)
                // StringBuilder vs String 비교 
                // 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%ED%99%9C%EC%9A%A9-String-StringBuilder-%EC%84%B1%EB%8A%A5-%EC%B0%A8%EC%9D%B4

                // TODO : 서버에 Http 통신(Get) 테스트 진행시 아래 URL 주소에 테스트 프로젝트 아이디 "jc-constrct", 팀아이디를 "inc-001-jc-constrct-t001" 메서드 파라미터로 넘겨 받아서 
                // Http Get 방식으로 호출해서 10. 컨테이너 체크아웃 + (파일)다운로드 Rest API 테스트 진행 (2024.10.08 jbh)
                // 테스트용 URL 주소 - requestCheckOutURL = "https://cobim.io/api/explorer/container/checkout/jc-constrct/inc-001-jc-constrct-t001/jc-constrct-ijinc-sptl_902-func_010-006";
                StringBuilder requestSb = new StringBuilder(ContainerHelper.requestCheckOutURL);
                requestSb.Append(containerPack.GetProjectId());
                requestSb.Append("/");
                requestSb.Append(containerPack.GetTeamId());
                requestSb.Append("/");
                requestSb.Append(containerPack.GetContainerId());

                var requestCheckOutURL = requestSb.ToString();

                // TODO : HttpClient Get 방식으로 서버에 요청할 때, using 문 사용 (2024.10.08 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/fundamentals/networking/http/httpclient
                using HttpResponseMessage response = await client.GetAsync(requestCheckOutURL);

                string resMsg = await response.Content.ReadAsStringAsync();

                // Rest API 10. 컨테이너 체크아웃 + (파일)다운로드 Http 통신 결과가 성공할 경우 
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    var ms = await response.Content.ReadAsStreamAsync();

                    // 디렉터리(di)가 존재하지 않는 경우
                    if (false == di.Exists) Directory.CreateDirectory(dirPath);   // 디렉터리 새로 생성

                    // 새로 다운로드 받으려는 파일 전체 경로 - dirPath + 파일이름 - fi.Name 구하기
                    downloadFilePath = FileHelper.FileDownLoadPath(dirPath, containerPack, fi.Name);

                    // TODO : 해당 using 문장을 벗어나면 FileStream 클래스 객체 fs Dipose 처리 (리소스 해제 및 자원 반납) 구현 (2024.10.08 jbh)
                    //        아래 소스코드를 using문으로 구현 안하고 실행시 파일을 생성되나 FileStream 클래스 객체 fs가 close 처리가 자동으로 되지 않아서 
                    //        다운로드 받은 이미지 파일(.jpg, .png 등등...)을 열어도 손상된 상태라고 하며 이미지를 볼 수 없으니 주의해야 한다.
                    // 참고 URL - https://hongjinhyeon.tistory.com/92
                    // TODO : HttpClient 클래스 객체 client 사용 URL을 이용한 파일 다운로드 구현 (2024.10.08 jbh)
                    // 참고 URL - https://asecurity.dev/entry/NET-Download-file-from-HttpClientWebClient
                    // 참고 2 URL - https://stackoverflow.com/questions/45711428/download-file-with-webclient-or-httpclient

                    using (FileStream fs = File.Create(downloadFilePath))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        await ms.CopyToAsync(fs);
                        await ms.FlushAsync();
                        await fs.FlushAsync();

                        // fs.Close();
                    }

                    Log.Logger.Information(Logger.GetMethodPath(currentMethod) + "10. 컨테이너 체크아웃 + (파일)다운로드 성공");

                    return;
                }

                // Rest API 10. 컨테이너 체크아웃 + (파일)다운로드 Http 통신 결과가 실패할 경우 
                else throw new Exception(resMsg);

            }
            catch (Exception ex)
            {
                // TODO : Http 통신 Get 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 메시지 출력 기능 구현 예정 (2024.10.08 jbh)
                Log.Logger.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달 throw 
            }
        }

        #endregion GetCheckOutAsync

        #region PostCheckInAsync

        /// <summary>
        /// 11. 컨테이너 체크인 + 업로드 (POST)
        /// Rest API 메서드 파라미터 "WebView2ExplorerView pInputParamData"
        /// </summary>
        public static async Task<Dictionary<string, object>> PostCheckInAsync(WebView2ExplorerView pInputParamData)
        {
            HttpClient client = pInputParamData.GetHttpClient();                // HttpClient client 객체 가져오기 
            // string commonDirectoryPath = pInputParamData.GetCommonDirectoryPath();   // 폴더 (디렉토리) 공통 경로 값 가져오기
            RequestHeader requestHeader = pInputParamData.GetRequestHeader();   // Http 서버 통신 요청 Header 값 가져오기 
            ContainerPack containerPack = pInputParamData.GetContainerPack();   // 컨테이너 정보 값 가져오기 
            FilePack filePack = pInputParamData.GetFilePack();                  // 컨테이너 체크인 + 파일 웹서버 업로드 할 파일 정보 값 가져오기
            CheckInPack checkInPack = pInputParamData.GetCheckInPack();         // 컨테이너 체크인 + 파일 웹서버 업로드 정보 값 가져오기 

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : 2024년 10월 COBIM 시연회 종료후 Http 통신 Get 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 로그 기록 및 Rest API 메서드 종료 기능 구현 예정 (2024.10.11 jbh)

                // HttpClient 클래스 객체 "client"의 요청헤더(DefaultRequestHeaders)에 데이터 추가 하기 전 초기화
                client.DefaultRequestHeaders.Clear();

                // 요청 헤더 (requestHeader) HttpClient 클래스 객체 "client"에 추가 
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.authorization, requestHeader.GetAuthorization());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.tenant, requestHeader.GetTenant());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.access, requestHeader.GetAccess());

                // TODO : HttpClient Post 방식으로 서버에 요청할 때, using 문 사용 (2024.10.11 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/fundamentals/networking/http/httpclient

                // TODO : HttpClient Post 방식으로 서버에 요청할 때, 요청 데이터 (form-data) 형식(Dictionary 방식)으로 객체 "var formContent" 구현 (2024.10.11 jbh)
                // 참고 URL - https://stackoverflow.com/questions/20005355/how-to-post-data-using-httpclient

                // TODO : 자바 스프링(Java Spring) 오류 메시지 "org.springframework.validation.BindException" 출력 원인 파악 (2024.10.11 jbh)
                // 참고 URL - https://jy-note.tistory.com/7

                // TODO : HttpClient Post 방식으로 요청 데이터 (form-data) 중 파일 업로드 형식(upload file form) 구현 (2024.10.11 jbh)
                // 참고 URL - https://brokul.dev/sending-files-and-additional-data-using-httpclient-in-net-core
                var strCnt = new StreamContent(filePack.GetUploadFiles());
                var fCnt = new ByteArrayContent(strCnt.ReadAsByteArrayAsync().Result);
                fCnt.Headers.ContentType = MediaTypeHeaderValue.Parse(ContainerHelper.formDataContentType);

                // TODO : HttpClient Post 방식으로 요청 데이터 (form-data) 중 파일 업로드 형식(upload file form) 구현 (2024.10.11 jbh)
                // 참고 URL - https://stackoverflow.com/questions/16416601/c-sharp-httpclient-4-5-multipart-form-data-upload
                using var formContent = new MultipartFormDataContent
                { 
                    // 요청 데이터 Body(form-data) 문자열 
                    { new StringContent(checkInPack.GetFolderId(), Encoding.UTF8), ContainerHelper.folderId },
                    { new StringContent(filePack.GetFileName(), Encoding.UTF8), ContainerHelper.fileName },
                    { new StringContent(filePack.GetFileSize(), Encoding.UTF8), ContainerHelper.fileSize },
                    { new StringContent(checkInPack.GetComment(),  Encoding.UTF8), ContainerHelper.comment  },
                    // 요청 데이터 Body(form-data) 이미지 파일 (uploadFiles)
                    { fCnt, ContainerHelper.uploadFiles, filePack.GetFileName() }
                };

                // TODO : StringBuilder 클래스 객체 requestSb 생성 -> 11. 컨테이너 체크인 + 업로드 (POST) Rest API URL 주소에 문자열 추가 (2024.10.11 jbh)
                // StringBuilder vs String 비교 
                // 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%ED%99%9C%EC%9A%A9-String-StringBuilder-%EC%84%B1%EB%8A%A5-%EC%B0%A8%EC%9D%B4

                // TODO : 서버에 Http 통신(POST) 테스트 진행시 아래 URL 주소에 테스트 프로젝트 아이디 "jc-constrct", 팀 아이디 "inc-001-jc-constrct-t001"
                //        컨테이너 아이디 "jc-constrct-ijinc-sptl_902-func_010-006" 메서드 파라미터로 넘겨 받아서 
                //        Http POST 방식으로 호출해서 11. 컨테이너 체크인 + 업로드 (POST) Rest API 테스트 진행 (2024.10.11 jbh)
                // 테스트용 URL 주소 - requestCheckOutURL = "https://cobim.io/api/explorer/container/checkin/jc-constrct/inc-001-jc-constrct-t001/jc-constrct-ijinc-sptl_902-func_010-006";
                StringBuilder requestSb = new StringBuilder(ContainerHelper.requestCheckInURL);
                requestSb.Append(containerPack.GetProjectId());
                requestSb.Append("/");
                requestSb.Append(containerPack.GetTeamId());
                requestSb.Append("/");
                requestSb.Append(containerPack.GetContainerId());

                var requestCheckInURL = requestSb.ToString();

                // TODO : HttpClient Post 방식으로 서버에 요청할 때, using 문 사용 (2024.10.11 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/fundamentals/networking/http/httpclient
                // TODO : HttpClient Post 방식으로 서버에 요청할 때, 요청 데이터 (form-data) 형식(Dictionary 방식)으로 객체 "var formContent"구현 (2024.10.11 jbh)
                // 참고 URL - https://stackoverflow.com/questions/20005355/how-to-post-data-using-httpclient
                using HttpResponseMessage response = await client.PostAsync(requestCheckInURL, formContent);

                // TODO : 인재 INC에서 개발한 서버측에서 전달 받은 오류 메시지 "{"status":"io.jsonwebtoken.MalformedJwtException: JWT strings must contain exactly 2 period characters. Found: 10","message":"JWT strings must contain exactly 2 period characters. Found: 10","error":"io.jsonwebtoken.MalformedJwtException"}"
                //        해당 오류 메시지의 원인은 JWT 토큰을 파싱하는 과정에서 발생한 오류로 확인. 해당 오류 원인 파악 후 로직 수정 예정 (2024.10.11 jbh)
                //        참고 URL - https://cano721.tistory.com/216
                //        참고 2 URL - https://stackoverflow.com/questions/53949137/jwt-strings-must-contain-exactly-2-period-characters-found-0
                //        참고 3 URL - https://velog.io/@jiu_lee/Project-%EB%B9%84%EB%B0%80%EB%B2%88%ED%98%B8-%EC%B0%BE%EA%B8%B0-with-JWT
                string resMsg = await response.Content.ReadAsStringAsync();

                // Rest API 11. 컨테이너 체크인 + 업로드 Http 통신 결과가 성공할 경우 
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    // TODO : 서버로 부터 받은 json 데이터에 속한 "resultData" 영역이 리스트가 아니기 때문에 아래처럼 (JsonObject.Parse(resMsg) 사용 -> JsonNode(JsonObject 배열) 형식으로 변환 후 JSON 문자열 변환(ToJsonString()) (2024.10.11 jbh)
                    // 참고 URL - https://stackoverflow.com/questions/76035924/c-sharp-list-field-names-in-a-jsonobject-or-jsonnode
                    // 참고 2 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.text.json.nodes.jsonnode.tojsonstring?view=net-8.0
                    var jtmp = (JsonObject.Parse(resMsg)[ContainerHelper.resultData]).ToJsonString();

                    // TODO : JsonString 객체 jtmp를 Dictionary<string, object> 클래스 객체 "resultData"로 변환할 수 있도록 JsonSerializer.Deserialize 진행 (2024.10.11 jbh)
                    // 참고 URL - https://stackoverflow.com/questions/70111672/read-from-json-file-into-ienumerable-using-system-text-json
                    var resultData = JsonSerializer.Deserialize<Dictionary<string, object>>(jtmp);

                    Log.Logger.Information(Logger.GetMethodPath(currentMethod) + "11. 컨테이너 체크인 + 업로드 성공");

                    return resultData;
                }
                // Rest API 11. 컨테이너 체크인 + 업로드 Http 통신 결과가 실패할 경우 
                else throw new Exception(resMsg);
            }
            catch (Exception ex)
            {
                // TODO : 2024년 10월 COBIM 시연회 종료후 Http 통신 Get 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 로그 기록 및 Rest API 메서드 종료 기능 구현 예정 (2024.10.11 jbh)

                Log.Logger.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달 throw
            }
        }

        #endregion PostCheckInAsync

        #region PostCancelWorkAsync

        // TODO : WebView2ExplorerView.cs 소스파일에 속한
        //        컨테이너 작업 취소 정보 클래스 "CancelWorkPack"안에 속하는 프로퍼티 "projectId", "teamId", "containerId" 의 접근 제한자를 
        //        private로 설정하면 오류 발생하여 접근 제한자를 public으로 수정함. (2024.11.04 jbh)
        //        오류 발생 위치 - ExplorerRestServer.cs -> 비동기 메서드 "PostCancelWorkAsync" 
        //        -> 메서드 "JsonSerializer.Serialize" 호출하여 json 문자열(content)을 제대로 리턴 받지 못하고 오류 발생함. 

        // TODO : WebView2ExplorerView.cs 소스파일에 속한
        //        컨테이너 작업 취소 정보 클래스 "CancelWorkPack"안에 속하는 프로퍼티 "projectId", "teamId", "containerId" 프로퍼티 이름 
        //        변경하면 오류 발생하여 수정 안 하고 그대로 유지함.(2024.11.04 jbh)
        //        오류 발생 위치 - ExplorerRestServer.cs -> 비동기 메서드 "PostCancelWorkAsync" 
        //        서버로 부터 제대로 리턴 받지 못하고 오류 발생함. 

        /// <summary>
        /// 17. 컨테이너 작업 취소 (Post)
        /// Rest API 메서드 파라미터 "WebView2ExplorerView pInputParamData"
        /// </summary>
        public static async Task<Dictionary<string, object>> PostCancelWorkAsync(WebView2ExplorerView pInputParamData)
        {
            HttpClient client = pInputParamData.GetHttpClient();                   // HttpClient client 객체 가져오기
            RequestHeader requestHeader = pInputParamData.GetRequestHeader();      // Http 서버 통신 요청 Header 값 가져오기
            CancelWorkPack cancelWorkPack = pInputParamData.GetCancelWorkPack();   // 컨테이너 작업 취소 정보 값 가져오기 

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : Http 통신 Post 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 메시지 출력 기능 구현 예정 (2023.12.01 jbh)

                // HttpClient 클래스 객체 "client"의 요청헤더(DefaultRequestHeaders)에 데이터 추가 하기 전 초기화
                client.DefaultRequestHeaders.Clear();

                // 요청 헤더 (requestHeader) HttpClient 클래스 객체 "client"에 추가 
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.authorization, requestHeader.GetAuthorization());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.tenant, requestHeader.GetTenant());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.access, requestHeader.GetAccess());

                // TODO : 컨테이너 작업 취소 정보 CancelWorkPack 클래스 객체 "cancelWorkPack" string 클래스 json 문자열(content) 구현 (2023.12.20 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0
                // json 문자열(하드코딩) 생성 코드 예시 -> (예) string requestData = "{\"id\":\"\",\"password\":\"\",\"inputTenantId\":\"inc-001\"}";
                string requestData = JsonSerializer.Serialize(cancelWorkPack);

                // TODO : HttpClient Post 방식으로 서버에 요청할 때, using 문 사용 (2023.12.06 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/fundamentals/networking/http/httpclient
                // TODO : 메서드 "StringContent"에 메서드 파라미터 Encoding 클래스 객체 "Encoding.UTF8" 제거 (2023.12.20 jbh)
                // TODO : 오류 메시지 "{"status":"org.springframework.web.HttpMediaTypeNotSupportedException: Content type 'text/plain;charset=utf-8' not supported","message":"Content type 'text/plain;charset=utf-8' not supported","error":"org.springframework.web.HttpMediaTypeNotSupportedException"}" 원인 파악 하기 (2023.12.20 jbh)
                //        오류 메시지 원인은 Content type을 추가하지 않을 경우 자동 생성되는 Content type이 "text/plain"이라서 발생한 오류이다.
                //        하여 아래처럼  Content type을 "LoginHelper.contentType(== "application/json")" 변경해야 한다.
                // 참고 URL - https://velog.io/@ds257873/Content-type-textplaincharsetUTF-8-not-supported
                using var jsonContent = new StringContent(
                                                requestData,
                                                Encoding.UTF8,
                                                ContainerHelper.contentType
                                            );

                // TODO : StringBuilder 클래스 객체 requestSb 생성 -> 17. 컨테이너 작업 취소 (POST) Rest API URL 주소에 문자열 추가 (2023.12.20 jbh)
                // StringBuilder vs String 비교 
                // 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%ED%99%9C%EC%9A%A9-String-StringBuilder-%EC%84%B1%EB%8A%A5-%EC%B0%A8%EC%9D%B4

                // TODO : 서버에 Http 통신(POST) 테스트 진행시 아래 URL 주소에 테스트 프로젝트 아이디 "jc-constrct", 팀 아이디 "inc-001-jc-constrct-t001"
                //        메서드 파라미터로 넘겨 받아서 Http POST 방식으로 호출 및
                //        17. 컨테이너 작업 취소 (POST) Rest API 테스트 진행 (2023.12.20 jbh)
                // 테스트용 URL 주소 - requestCancelWorkURL = "https://cobim.io/api/explorer/container/work/cancel/jc-constrct/inc-001-jc-constrct-t001";
                StringBuilder requestSb = new StringBuilder(ContainerHelper.requestCancelWorkURL);
                requestSb.Append(cancelWorkPack.projectId);
                requestSb.Append("/");
                requestSb.Append(cancelWorkPack.teamId);

                var requestCancelWorkURL = requestSb.ToString();

                Debug.WriteLine($"{requestCancelWorkURL}");

                // TODO : HttpClient Post 방식으로 서버에 요청할 때, using 문 사용 (2023.12.20 jbh)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/fundamentals/networking/http/httpclient   
                using HttpResponseMessage response = await client.PostAsync(requestCancelWorkURL, jsonContent);

                // TODO : 필요시 토큰 만료된 경우 요청 헤더(requestHeader) 재발급 토큰(RefreshToken) 추가 및 HttpClient 클래스 객체 "client"에 추가 구현 예정 (2024.11.12 jbh)

                // TODO : 인재 INC에서 개발한 서버측에서 전달 받은 오류 메시지 "{"status":"io.jsonwebtoken.MalformedJwtException: JWT strings must contain exactly 2 period characters. Found: 10","message":"JWT strings must contain exactly 2 period characters. Found: 10","error":"io.jsonwebtoken.MalformedJwtException"}"
                //        해당 오류 메시지의 원인은 JWT 토큰을 파싱하는 과정에서 발생한 오류로 확인. 해당 오류 원인 파악 후 로직 수정 예정 (2023.12.20 jbh)
                //        참고 URL - https://cano721.tistory.com/216
                //        참고 2 URL - https://stackoverflow.com/questions/53949137/jwt-strings-must-contain-exactly-2-period-characters-found-0
                //        참고 3 URL - https://velog.io/@jiu_lee/Project-%EB%B9%84%EB%B0%80%EB%B2%88%ED%98%B8-%EC%B0%BE%EA%B8%B0-with-JWT
                // TODO : 오류 메시지 "{"status":"org.springframework.web.HttpMediaTypeNotSupportedException: Content type 'text/plain;charset=utf-8' not supported","message":"Content type 'text/plain;charset=utf-8' not supported","error":"org.springframework.web.HttpMediaTypeNotSupportedException"}" 원인 파악 후 해결 진행 예정 (2023.12.20 jbh)
                string resMsg = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"{resMsg}\n");

                // Rest API 17. 컨테이너 작업 취소 Http 통신 결과가 성공할 경우 
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    // var jtmp = (JsonObject.Parse(resMsg)[ContainerHelper.resultData]).Select(x => x);
                    // TODO : 서버로 부터 받은 resMsg 데이터에 속한 "resultData" 영역이 리스트가 아니기 때문에 아래처럼 (JsonObject.Parse(resMsg) 사용 -> JsonNode(JsonObject 배열) 형식으로 변환 후 JSON 문자열 변환(ToJsonString()) (2023.12.20 jbh)
                    // 참고 URL - https://stackoverflow.com/questions/76035924/c-sharp-list-field-names-in-a-jsonobject-or-jsonnode
                    // 참고 2 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.text.json.nodes.jsonnode.tojsonstring?view=net-8.0
                    var jtmp = (JsonObject.Parse(resMsg)[ContainerHelper.resultData]).ToJsonString();

                    // TODO : JsonString 객체 jtmp를 Dictionary<string, object> 클래스 객체 "resultData"로 변환할 수 있도록 JsonSerializer.Deserialize 진행 (2023.12.20 jbh)
                    // 참고 URL - https://stackoverflow.com/questions/70111672/read-from-json-file-into-ienumerable-using-system-text-json
                    var resultData = JsonSerializer.Deserialize<Dictionary<string, object>>(jtmp);

                    Log.Logger.Information(Logger.GetMethodPath(currentMethod) + "17. 컨테이너 작업 취소 성공");

                    return resultData;
                }
                // Rest API 17. 컨테이너 작업 취소 Http 통신 결과가 실패할 경우 
                else throw new Exception(resMsg);

            }
            catch (Exception ex)
            {
                // TODO : Http 통신 Post 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 메시지 출력 기능 구현 예정 (2023.12.20 jbh)
                // Log.Information(LogHelper.GetMethodPath(currentMethod) + ex.Message);
                Log.Logger.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                // TODO : 오류 발생시 상위 호출자 예외처리 전달 throw 구현 (2023.12.20 jbh)
                // 참고 URL - https://devlog.jwgo.kr/2009/11/27/thrownthrowex/
                throw;
            }
        }

        #endregion PostCancelWorkAsync

        #region Sample

        #endregion Sample
    }
}

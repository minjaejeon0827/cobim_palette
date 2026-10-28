using Serilog;

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Reflection;

using CobimPalette.Rest.Api.CobimBase.WebView2Explorer;
using CobimPalette.Data.Models.CobimBase.Login;
using CobimPalette.Data.Models.CobimBase.WebView2Explorer;
using CobimUtil;

namespace CobimPalette.Rest.Api.CobimBase.HubBox
{
    /// <summary>
    /// HubBox 파일 조회/변환 Rest API 클래스 
    /// </summary>
    public class HubBoxRestServer
    {
        #region PostTranslateAsync

        /// <summary>
        /// POST - 1. 컨테이너에 속한 특정 파일 변환 API 
        /// 인재 INC - Rest API 11. 컨테이너 체크인 + 파일 웹서버 업로드 (POST) 성공한 경우에만 해당 특정 파일 변환 API 호출 
        /// Rest API 메서드 파라미터 "WebView2ExplorerView pInputParamData"
        /// </summary>
        public static async Task<Dictionary<string, object>> PostTranslateAsync(WebView2ExplorerView pInputParamData)
        {
            HttpClient client = pInputParamData.GetHttpClient();                // HttpClient client 객체 가져오기 
            RequestHeader requestHeader = pInputParamData.GetRequestHeader();   // Http 서버 통신 요청 Header 값 가져오기  
            Data.Models.CobimBase.WebView2Explorer.TranslatePack translatePack = pInputParamData.GetTranslatePack();   // HubBox 파일 변환 정보 값 가져오기 

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록 

            try
            {
                Log.Logger.Information(Logger.GetMethodPath(currentMethod) + "3. 컨테이너에 속한 특정 파일 변환 Rest API 시작");

                // TODO : 2024년 10월 COBIM 시연회 종료후 Http 통신 Get 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 로그 기록 및 Rest API 메서드 종료 기능 구현 예정 (2024.08.26 minjae)

                // HttpClient 클래스 객체 "client"의 요청헤더(DefaultRequestHeaders)에 데이터 추가 하기 전 초기화
                client.DefaultRequestHeaders.Clear();

                // 요청 헤더 (requestHeader) HttpClient 클래스 객체 "client"에 추가 
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.authorization, requestHeader.GetAuthorization());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.tenant, requestHeader.GetTenant());
                client.DefaultRequestHeaders.TryAddWithoutValidation(ContainerHelper.access, requestHeader.GetAccess());

                // TODO : TranslatePack 클래스 객체 "translatePack" string 클래스 json 문자열(content) 구현 (2024.10.11 minjae)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0
                // json 문자열(하드코딩) 생성 코드 예시 -> (예) string requestData = "{\"id\":\"\",\"password\":\"\",\"inputTenantId\":\"inc-001\"}";
                string requestData = JsonSerializer.Serialize(translatePack);

                // TODO : HttpClient Post 방식으로 서버에 요청할 때, using 문 사용 (2024.08.26 minjae)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/fundamentals/networking/http/httpclient
                using var jsonContent = new StringContent(
                                                requestData,
                                                Encoding.UTF8
                                            );

                // TODO : StringBuilder 클래스 객체 requestSb 생성 -> POST - 3. 컨테이너에 속한 특정 파일 변환 Rest API URL 주소에 문자열 추가 (2024.08.26 minjae)
                // StringBuilder vs String 비교 
                // 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%ED%99%9C%EC%9A%A9-String-StringBuilder-%EC%84%B1%EB%8A%A5-%EC%B0%A8%EC%9D%B4

                StringBuilder requestSb = new StringBuilder(HubBoxHelper.requestTranslateURL);
                requestSb.Append(translatePack.GetProjectId());
                requestSb.Append("/");
                requestSb.Append(translatePack.GetTeamId());
                requestSb.Append("/");
                requestSb.Append(translatePack.GetContainerId());
                requestSb.Append("/");
                requestSb.Append(translatePack.GetFileId());

                var requestTranslateURL = requestSb.ToString();

                // TODO : HttpClient Post 방식으로 서버에 요청할 때, using 문 사용 (2024.08.26 minjae)
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/fundamentals/networking/http/httpclient   
                using HttpResponseMessage response = await client.PostAsync(requestTranslateURL, jsonContent);

                string resMsg = await response.Content.ReadAsStringAsync();

                // 웹서버로 부터 받은 결과값(json 데이터)을 Dictionary 변환
                var translateJsonNode = JsonNode.Parse(resMsg);
                var resultData = JsonSerializer.Deserialize<Dictionary<string, object>>(translateJsonNode);

                Log.Logger.Information(Logger.GetMethodPath(currentMethod) + "3. 컨테이너에 속한 특정 파일 변환 Rest API 종료");

                return resultData;   // Dictionary 클래스 객체 resultData 리턴
            }
            catch (Exception ex)
            {
                // TODO : 2024년 10월 COBIM 시연회 종료후 Http 통신 Get 방식 작업 시 서버로 부터 결과값 리턴되지 않고 시간이 길어지면 Time Out 처리 되면서 오류 로그 기록 및 Rest API 메서드 종료 기능 구현 예정 (2024.10.11 minjae)
                Log.Logger.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달 throw
            }
        }

        #endregion PostTranslateAsync

        #region Sample

        #endregion Sample
    }
}

using Serilog;
using System.Net.Http;
using System.Reflection;

using CobimPalette.Common.LogBase;

namespace CobimPalette.Settings
{
    /// <summary>
    /// Rest API HttpClient 기본 설정 
    /// </summary>
    public class HttpClientSetting
    {
        #region 프로퍼티 

        /// <summary>
        /// HttpClient client (계속 호출하지 말고 싱글톤으로 사용해야함.)
        /// </summary>
        private HttpClient _Client { get; set; }

        #endregion 프로퍼티 

        #region 생성자

        // TODO : 필요시 생성자 "HttpClientSetting" 구현 예정 (2024.10.07 jbh)
        public HttpClientSetting()
        {

        }

        #endregion 생성자

        #region CreateHttpClient

        /// <summary>
        /// HttpClient 싱글톤 객체 생성 
        /// </summary>
        public void CreateHttpClient()
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // 싱글톤 패턴 사용해서 처음 한번만 프로퍼티 "_Client" 객체 생성
                if (_Client is null) _Client = new HttpClient();
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion CreateHttpClient

        #region GetHttpClient

        /// <summary>
        /// HttpClient 싱글톤 패턴 객체 가져오기 
        /// </summary>
        public HttpClient GetHttpClient()
        {
            return _Client;
        }

        #endregion GetHttpClient


        #region Sample

        #endregion Sample
    }
}

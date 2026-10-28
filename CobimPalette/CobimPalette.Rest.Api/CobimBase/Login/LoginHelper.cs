using System;

namespace CobimPalette.Rest.Api.CobimBase.Login
{
    public class LoginHelper
    {
        #region 프로퍼티 

        /// <summary>
        /// 1. 로그인
        /// 요청 파라미터 - inputTenantId
        /// </summary>
        public const string inputTenantId = "inc-001";

        /// <summary>
        /// 1. 로그인
        /// 요청 URL - auth_server_url
        /// </summary>
        public const string auth_server_url = "https://cobim.io/api/account/user/signin";

        /// <summary>
        /// 1. 로그인
        /// 요청 Header - Content-Type	
        /// </summary>
        public const string contentType = "application/json";

        /// <sumary>
        /// 로그인 토큰
        /// </sumary>
        public const string authorization = "Authorization";

        /// <summary>
        /// 테넌트 아이디
        /// </summary>
        public const string tenant = "Tenant";

        #endregion 프로퍼티 

        #region Sample

        #endregion Sample
    }
}

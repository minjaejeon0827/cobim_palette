using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CobimPalette.Data.Models.CobimBase.Login
{
    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    public class LoginView<TData> : LoginView where TData : LoginView<TData> { }

    /// <summary>
    /// CobimPalette
    /// 로그인 화면 뷰모델(LoginVM) 데이터 모델
    /// </summary>
    public class LoginView : TLoginView<LoginView>
    {
        #region 프로퍼티 

        // TODO : Http 서버 통신 요청 Header 프로퍼티 "RequestHeader" 필요시 사용/수정 예정 (2024.09.23 minjae)
        /// <summary>
        /// Http 서버 통신 요청 Header
        /// </summary>
        //public static RequestHeader RequestHeader
        //{
        //    // 싱글톤 패턴 
        //    get => _RequestHeader ?? (_RequestHeader = new RequestHeader());

        //    set
        //    {
        //        _RequestHeader = value;
        //        StaticChanged(nameof(RequestHeader));
        //    }
        //}
        //private static RequestHeader _RequestHeader;


        #endregion 프로퍼티 

        #region 생성자

        public LoginView() : base() { }

        #endregion 생성자
    }

    #region RequestHeader

    /// <summary>
    /// Http 웹서버 통신 요청 Header
    /// </summary>
    public class RequestHeader
    {
        #region 프로퍼티

        /// <summary>
        /// inputTenantId - 테넌트 아이디
        /// </summary>
        private string _Tenant { get; set; }

        /// <summary>
        /// Authorization - 로그인 토큰 키  
        /// </summary>
        private string _Authorization { get; set; }

        /// <summary>
        /// Access - 접속자 아이디 (Access Id, accessKey 같은 값을 의미, 로그인 아이디와는 다름)
        /// </summary>
        private string _Access { get; set; }

        /// <summary>
        /// Refresh Token - 로그인 토큰 만료시 재발급을 위한 토큰 키 
        /// </summary>
        public string _Refresh { get; set; }

        #endregion 프로퍼티

        #region 생성자

        public RequestHeader(string tenant, string authorization, string access, string? refresh = null)
        {
            _Tenant = tenant;
            _Authorization = authorization;
            _Access = access;
            _Refresh = refresh;
        }

        #endregion 생성자 

        #region 테넌트 아이디

        /// <summary>
        /// inputTenantId - 테넌트 아이디 값 셋팅
        /// </summary>
        public void SetTenant(string tenant)
        {
            _Tenant = tenant;
        }

        /// <summary>
        /// inputTenantId - 테넌트 아이디 값 가져오기
        /// </summary>
        public string GetTenant()
        {
            return _Tenant;
        }

        #endregion 테넌트 아이디

        #region 로그인 토큰 키  

        /// <summary>
        /// Authorization - 로그인 토큰 키 값 셋팅 
        /// </summary>
        public void SetAuthorization(string authorization)
        {
            _Authorization = authorization;
        }

        /// <summary>
        /// Authorization - 로그인 토큰 키 값 가져오기  
        /// </summary>
        public string GetAuthorization()
        {
            return _Authorization;
        }

        #endregion 로그인 토큰 키  

        #region 로그인 아이디

        /// <summary>
        /// Access - 로그인 아이디(Access Id) 값 셋팅 
        /// </summary>
        public void SetAccess(string access)
        {
            _Access = access;
        }

        /// <summary>
        /// Access - 로그인 아이디(Access Id) 가져오기 
        /// </summary>
        public string GetAccess()
        {
            return _Access;
        }

        #endregion 로그인 아이디

        #region 로그인 재발급 토큰 키 

        /// <summary>
        /// Refresh Token - 로그인 토큰 만료시 재발급을 위한 토큰 키 값 셋팅
        /// </summary>
        public void SetRefresh(string refresh)
        {
            _Refresh = refresh;
        }

        /// <summary>
        /// Refresh Token - 로그인 토큰 만료시 재발급을 위한 토큰 키 가져오기
        /// </summary>
        public string GetRefresh()
        {
            return _Refresh;
        }

        #endregion 로그인 재발급 토큰 키 

        #region Sample

        #endregion Sample
    }

    #endregion RequestHeader

    #region LoginPack

    /// <summary>
    /// 로그인 정보 
    /// </summary>
    public class LoginPack
    {
        #region 프로퍼티 

        // TODO : 로그인 정보 클래스 "LoginPack"안에 속하는 프로퍼티 "inputTenantId", "id", "password" 의 접근 제한자를 
        //        private로 설정하면 오류 발생하여 접근 제한자를 public으로 수정함. (2024.03.18 minjae)
        //        오류 발생 위치 - UserRestServer.cs -> 비동기 메서드 "PostUserLoginAsync" 
        //        -> 메서드 "JsonSerializer.Serialize" 호출하여 json 문자열(content)을 제대로 리턴 받지 못하고 오류 발생함. 

        /// <summary>
        /// 요청 파라미터 - inputTenantId
        /// </summary>
        public string inputTenantId { get; set; }

        /// <summary>
        /// 요청 파라미터 - 아이디
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 요청 파라미터 - 비밀번호
        /// </summary>
        public string password { get; set; }

        #endregion 프로퍼티 

        #region 생성자 

        public LoginPack(string inputTenantId, string clientId, string clientPW)
        {
            this.inputTenantId = inputTenantId;
            this.id = clientId;
            this.password = clientPW;
        }

        #endregion 생성자
    }

    #endregion LoginPack

    #region LoginResponseData

    /// <summary>
    /// 로그인 응답 결과 데이터
    /// </summary>
    public class LoginResponseData
    {
        #region 프로퍼티 

        /// <summary>
        /// 로그인 토큰 키 (Access Token)
        /// </summary>
        public string resultData { get; set; }

        /// <summary>
        /// 로그인 토큰 만료시 재발급을 위한 토큰 키 (Refresh Token)
        /// </summary>
        public string refreshToken { get; set; }

        /// <summary>
        /// 모든 API 요청시 요청 헤더(RequestHeader)에 저장하여 
        /// 필수로 전달해야하는 정보 (Access Id, accessKey 같은 값을 의미)
        /// </summary>
        public string accessKey { get; set; }

        /// <summary>
        /// 로그인 실패 횟수
        /// </summary>
        public int resultCount { get; set; }

        /// <summary>
        /// Http 통신 결과 메시지
        /// </summary>
        public string resultMessage { get; set; }

        /// <summary>
        /// 테넌트 아이디
        /// </summary>
        public string tenantId { get; set; }

        /// <summary>
        /// jwt 토큰
        /// </summary>
        public string jwt { get; set; }

        public string param { get; set; }

        public string payload { get; set; }

        public string actionUserInfo { get; set; }

        #endregion 프로퍼티 
    }

    #endregion LoginResponseData

    #region Sample

    // TODO : Http 서버 통신 요청 Header 클래스 RequestHeader 필요시 참고 (2024.10.07 minjae)
    /// <summary>
    /// Http 서버 통신 요청 Header
    /// </summary>
    // public class RequestHeader
    // {
    //     /// <summary>
    //     /// inputTenantId - 테넌트 아이디
    //     /// </summary>
    //     public string Tenant { get; set; }

    //     /// <summary>
    //     /// 로그인 토큰 키 - Authorization
    //     /// </summary>
    //     public string Authorization { get; set; }
    // }

    #endregion Sample
}

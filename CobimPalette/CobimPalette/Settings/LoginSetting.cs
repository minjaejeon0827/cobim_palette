using CobimPalette.Data.Models.CobimBase.Login;

namespace CobimPalette.Settings
{
    // TODO : 로그인 기본 설정 클래스 LoginSetting 필요시 추가 구현 예정 (2024.09.24 minjae)
    /// <summary>
    /// 로그인 기본 설정 
    /// </summary>
    public class LoginSetting
    {
        #region 프로퍼티

        /// <summary>
        /// Window - 로그인 화면 뷰 
        /// </summary>
        // private LoginV _LoginV { get; set; }

        // TODO : 로그인 서버 주소 프로퍼티 "_LoginServerPath" 필요시 사용 예정 (2024.09.23 minjae)
        /// <summary>
        /// 로그인 서버 주소 
        /// </summary>
        // private string _LoginServerPath { get; set; } //  = LoginHelper.auth_server_url;

        /// <summary>
        /// 로그인 아이디
        /// </summary>
        private string _LoginId { get; set; }

        /// <summary>
        /// 로그인 비밀번호
        /// </summary>
        private string _Password { get; set; }

        /// <summary>
        /// Http 서버 통신 요청 Header
        /// </summary>
        private RequestHeader _RequestHeader { get; set; }

        /// <summary>
        /// 로그인 (Login) 성공 여부 
        /// </summary>
        // private bool _IsLogin { get; set; }

        /// <summary>
        /// 로그인 화면 종료 (Exit) 여부 
        /// </summary>
        // private bool _IsExit { get; set; }

        #endregion 프로퍼티 

        #region 생성자

        public LoginSetting()
        {
            // 싱글톤 패턴 사용해서 처음 한번만 프로퍼티 "_RequestHeader", "_LoginV" 객체 생성
            // _RequestHeader = new RequestHeader();
            // _LoginV = new LoginV();
        }

        #endregion 생성자

        #region OpenLogin

        /// <summary>
        /// 로그인 화면 오픈 
        /// </summary>
        //public void OpenLogin()
        //{
        //    _LoginV.ShowDialog(); 
        //}

        #endregion OpenLogin

        #region CloseLogin

        /// <summary>
        /// 로그인 화면 종료 
        /// </summary>
        //public void CloseLogin() 
        //{
        //    _LoginV.Close();
        //}

        #endregion CloseLogin

        #region SetLoginServerPath

        // TODO : 필요시 메서드 SetLoginServerPath 사용 예정 (2024.09.23 minjae)
        /// <summary>
        /// 로그인 서버 URL 주소 값 셋팅
        /// </summary>
        //public void SetLoginServerPath(string loginServerPath) 
        //{
        //    _LoginServerPath = pLoginServerPath;
        //}

        #endregion SetLoginServerPath

        #region GetLoginServerPath

        // TODO : 필요시 메서드 GetLoginServerPath 사용 예정 (2024.09.23 minjae)
        /// <summary>
        /// 로그인 서버 URL 주소 값 가져오기
        /// </summary>
        //public string GetLoginServerPath()
        //{
        //    return _LoginServerPath;
        //}

        #endregion GetLoginServerPath

        #region SetLoginId

        /// <summary>
        /// 로그인 아이디 값 셋팅
        /// </summary>
        public void SetLoginId(string loginId)
        {
            _LoginId = loginId;
        }

        #endregion SetLoginId

        #region GetLoginId

        /// <summary>
        /// 로그인 아이디 값 가져오기 
        /// </summary>
        public string GetLoginId()
        {
            return _LoginId;
        }

        #endregion GetLoginId

        #region SetPassword

        /// <summary>
        /// 비밀번호 값 셋팅
        /// </summary>
        public void SetPassword(string password)
        {
            _Password = password;
        }

        #endregion SetPassword

        #region GetPassword

        /// <summary>
        /// 비밀번호 값 가져오기 
        /// </summary>
        /// <returns></returns>
        public string GetPassword()
        {
            return _Password;
        }

        #endregion GetPassword

        #region SetRequestHeader

        /// <summary>
        /// Http 서버 통신 요청 Header 값 셋팅
        /// </summary>
        public void SetRequestHeader(string tenantId, string resultData)
        {
            _RequestHeader.SetTenant(tenantId);
            _RequestHeader.SetAuthorization(resultData);
        }

        #endregion SetRequestHeader

        #region GetRequestHeader

        /// <summary>
        /// Http 서버 통신 요청 Header 가져오기
        /// </summary>
        public RequestHeader GetRequestHeader()
        {
            return _RequestHeader;
        }

        #endregion GetRequestHeader

        #region SetIsLogin

        /// <summary>
        /// 로그인 (Login) 성공 여부 값 셋팅하기
        /// </summary>
        //public void SetIsLogin(bool isLogin)
        //{
        //    _IsLogin = isLogin;
        //}

        #endregion SetIsLogin

        #region GetIsLogin

        /// <summary>
        /// 로그인 (Login) 성공 여부 값 가져오기
        /// </summary>
        //public bool GetIsLogin() 
        //{
        //    return _IsLogin;
        //}

        #endregion GetIsLogin

        #region SetIsExit

        /// <summary>
        /// 로그인 화면 종료 (Exit) 여부 값 셋팅하기  
        /// </summary>
        //public void SetIsExit(bool isExit)
        //{
        //    _IsExit = isExit;
        //}

        #endregion SetIsExit

        #region GetIsExit

        /// <summary>
        /// 로그인 화면 종료 (Exit) 여부 값 가져오기  
        /// </summary>
        //public bool GetIsExit() 
        //{
        //    return _IsExit;
        //}

        #endregion GetIsExit

        #region Sample

        #endregion Sample
    }
}

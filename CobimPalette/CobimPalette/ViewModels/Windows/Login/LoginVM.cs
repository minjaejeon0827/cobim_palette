using Serilog;
using System.Reflection;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

using CobimNet;

using CobimPalette.Settings;
using CobimPalette.Commands;
using CobimPalette.Common.LogBase;
using CobimPalette.Common.CobimBase;
using CobimPalette.Data.Models.CobimBase.Login;
using CobimPalette.Utils.String;

namespace CobimPalette.ViewModels.Windows.Login
{

    public class LoginVM : BindableBase
    {
        #region 프로퍼티

        /// <summary>
        /// 유효성 검사 - 1. 로그인 (POST) Rest API 요청 데이터 타입
        /// </summary>
        private enum EnumRequestDataType : int
        {
            [System.ComponentModel.Description("테넌트 아이디")]
            TENANTID = 0,
            [System.ComponentModel.Description("로그인 아이디")]
            LOGINID = 1,
            [System.ComponentModel.Description("비밀번호")]
            PASSWORD = 2,
        }

        /// <summary>
        /// 종료 Command
        /// </summary>
        public ICommand ExitCommand { get; set; }

        /// <summary>
        /// 로그인 Command
        /// </summary>
        public ICommand LoginCommand { get; set; }

        /// <summary>
        /// 로그인 요청 서버 주소 
        /// </summary>
        //public string LoginServerPath { get => _LoginServerPath; set { _LoginServerPath = value; NotifyOfPropertyChange(nameof(LoginServerPath)); } }
        //private string _LoginServerPath = LoginHelper.auth_server_url;

        /// <summary>
        /// 로그인 (Login)
        /// </summary>
        public bool IsLogin { get => _IsLogin; set { _IsLogin = value; NotifyOfPropertyChange(nameof(IsLogin)); } }
        private bool _IsLogin;

        /// <summary>
        /// 종료 (Exit)
        /// </summary>
        public bool IsExit { get => _IsExit; set { _IsExit = value; NotifyOfPropertyChange(nameof(IsExit)); } }
        private bool _IsExit;


        // TODO : bool 자료형 프로퍼티 "IsLoginWorking"
        //        private bool 프로퍼티 및 NotifyOfPropertyChange(nameof(~~~)) 참조(사용) 안함. (2024.09.04 jbh)
        // private bool 프로퍼티 및 NotifyOfPropertyChange(nameof(~~~)) 참조(사용) 안 하는 이유?
        // "로그인" 기능 실행시 처리 중인 상태에서 해당 기능을 중복 요청하지 못 하도록
        // 버튼 "로그인"이 클릭되지 못하도록
        // 비활성화(IsEnabled = false 또는 Visibility= Visibility.Collapsed) 처리 하기 위해서 이고
        // 해당 기능을 실행하기 위해서 바인딩한 객체(프로퍼티)에 할당된 값이
        // True면 False, False면 True로 반대의 값으로 리턴되는 Converter 클래스 "InvertBooleanConverter.cs" 구현

        /// <summary>
        /// 로그인 처리 진행 여부 
        /// </summary>
        public bool IsLoginWorking { get => _IsLoginWorking; set { _IsLoginWorking = value; NotifyOfPropertyChange(nameof(IsLoginWorking)); } }
        private bool _IsLoginWorking;

        /// <summary>
        /// 테넌트 아이디
        /// </summary>
        public string TenantID { get; set; }

        /// <summary>
        /// 로그인 아이디
        /// </summary>
        public string LoginID { get; set; }

        /// <summary>
        /// 비밀번호
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 로그인 뷰모델
        /// </summary>
        public LoginVM loginVM { get; set; }

        #endregion 프로퍼티

        #region 생성자

        public LoginVM()
        {
            ExitCommand = new ButtonCommand(CobimHelper.VoidMethodType, CloseLogin, CanExecuteMethod);
            LoginCommand = new ButtonCommand(CobimHelper.AsyncMethodType, LoginAsync, CanExecuteMethod);
        }

        #endregion 생성자

        #region 기본 메소드 

        private bool CanExecuteMethod(object obj)
        {
            return true;
        }

        #endregion 기본 메소드

        #region SaveOption

        /// <summary>
        /// 테스트용 로그인 설정값 임시 저장
        /// </summary>
        private void SaveOption(LoginResponseData responseData)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                AppSetting.Default.LoginBase.SetLoginId(this.LoginID);
                AppSetting.Default.LoginBase.SetPassword(this.Password);

                // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.09.23 jbh)
                // LoginView.SetRequestHeader(new RequestHeader(this.TenantId, responseData.resultData, responseData.accessKey, responseData.refreshToken));
                // AppSetting.Default.LoginBase.SetRequestHeader(this.TenantId, responseData.resultData);
                // AppSetting.Default.LoginBase.LoginServerPath = this.LoginServerPath;
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + "SaveOption Error = " + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
            return;
        }

        #endregion SaveOption

        #region IsValidator

        /// <summary>
        /// 유효성 검사 - 테넌트 아이디(TenantId), 로그인 아이디(LoginID), 비밀번호(Password) 문자열 입력 여부 확인
        /// </summary>
        private bool IsValidator(EnumRequestDataType pEnumRequestDataType)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                switch (pEnumRequestDataType)
                {
                    case EnumRequestDataType.TENANTID:      // 테넌트 아이디
                        if (true == string.IsNullOrWhiteSpace(TenantID)) throw new Exception($"[{EnumRequestDataType.TENANTID.ToDescription()}]\r\n입력하지 않았습니다!");
                        break;

                    case EnumRequestDataType.LOGINID:       // 로그인 아이디
                        if (true == string.IsNullOrWhiteSpace(LoginID)) throw new Exception($"[{EnumRequestDataType.LOGINID.ToDescription()}]\r\n입력하지 않았습니다!");
                        break;

                    case EnumRequestDataType.PASSWORD:      // 비밀번호
                        if (true == string.IsNullOrWhiteSpace(Password)) throw new Exception($"[{EnumRequestDataType.PASSWORD.ToDescription()}]\r\n입력하지 않았습니다!");
                        break;

                    default:
                        throw new Exception("유효성 검사 오류!!\r\n관리자에게 문의 하세요.");
                        // break;
                }
                return true;
            }
            catch (Exception ex)
            {
                // TODO : 필요시 에러 로그 기록 사용 예정 (2024.07.03 jbh)
                // Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
                return false;
            }
        }

        #endregion IsValidator

        #region LoginAsync

        // TODO : 필요시 로그인 처리 비동기 메서드 "LoginAsync" 구현 예정 (2024.09.25 jbh)
        /// <summary>
        /// 로그인 처리 
        /// </summary>
        private async Task LoginAsync(object obj)
        {
            var currentMethod = MethodBase.GetCurrentMethod();    // TODO : 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {

            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
            finally
            {

            }
        }

        #endregion LoginAsync

        #region CloseLogin

        // TODO : 필요시 로그인 화면 종료 메서드 "CloseLogin" 구현 예정 (2024.09.25 jbh)
        /// <summary>
        /// 로그인 화면 종료 
        /// </summary>
        private void CloseLogin(object obj)
        {
            // AppSetting.Default.LoginBase.SetIsExit(true);

            // AppSetting.Default.LoginBase.CloseLogin();
        }

        #endregion CloseLogin

        #region Sample

        #endregion Sample
    }
}

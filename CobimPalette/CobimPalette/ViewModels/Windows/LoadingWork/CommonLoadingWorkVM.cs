using CobimPalette.Interface.Page;
using CobimPalette.Services.Page;

namespace CobimPalette.ViewModels.Windows.LoadingWork
{
    // TODO : 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면 뷰모델(CommonLoadingWorkVM.cs) 구현 (2024.10.02 minjae)  
    // 참고 URL - https://standard-stereotype.tistory.com/13  
    // 참고 2 URL - https://rhkdrmfh.tistory.com/135  
    // 참고 3 URL - https://www.sysnet.pe.kr/3/0/5107?pageno=0#google_vignette  
    public class CommonLoadingWorkVM : PageBase, IPage
    {
        #region 프로퍼티

        /// <summary>
        /// 화면 출력 메세지 
        /// </summary>
        public string Message { get => _Message; set { _Message = value; NotifyOfPropertyChange(); } }
        private string _Message = string.Empty;

        #endregion 프로퍼티

        #region 생성자

        public CommonLoadingWorkVM() : base()
        {

        }

        #endregion 생성자 

        #region SetMessage

        // TODO : 메서드 "SetMessage"에 존재하는 메서드 파라미터 "IsAutoNewLine" 메세지 멀티 라인 기능 필요시 사용 예정 (2024.01.03 minjae)
        /// <summary>
        /// 화면 출력 메세지 셋팅 
        /// </summary>
        public void SetMessage(string pMessage, bool IsAutoNewLine = false)
        {
            this.Message = pMessage;
        }

        #endregion SetMessage

        #region Sample

        #endregion Sample
    }
}

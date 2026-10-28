using CobimPalette.Interface.Page;
using CobimPalette.Services.Page;
using CobimPalette.Data.Models.CobimBase.HubBox;
using CobimPalette.Rest.Api.CobimBase.HubBox;

namespace CobimPalette.ViewModels.Pages.Palettes.Project.BIM360.HubBox
{
    // TODO : Palette로 출력된 [2]번 HubBox 웹사이트 관리자(Admin-1) 화면 종료시
    // 구글 크롬 전용 <CefSharp> 화면
    // 뷰(HubBoxV.xaml)
    // 뷰모델(HubBoxVM.cs)에서
    // 사용하고 있는 메모리를 정리하는 로직 구현하기 (2024.11.12 minjae)

    // 참고 URL - https://stackoverflow.com/questions/48398628/c-sharp-cefsharp-memory-management
    // 참고 2 URL - https://blog.naver.com/iconms1/222033637762
    // 참고 3 URL - https://github.com/cefsharp/CefSharp/blob/master/CefSharp.WinForms.Example/BrowserForm.cs#L137

    // TODO : 구글 크롬 기반 웹브라우저 화면을 WPF에서 출력할 수 있도록 CefSharp(구글 크롬 웹브라우저 컨트롤) 구현 (2024.11.12 minjae) 
    // 참고 URL - https://blog.naver.com/codeknight2/222210163649  
    // 참고 2 URL - https://github.com/cefsharp/CefSharp/wiki/Quick-Start  
    // 유튜브 참고 URL - https://youtu.be/si8wemHOMYA?si=FiL-r-_5MoF1EwU_
    public class HubBoxVM : PageBase<HubBoxView>, IPage
    {
        #region 프로퍼티

        /// <summary>
        /// HubBox 웹사이트 파일 변환 목록 화면 조회 API 주소
        /// </summary>
        public string WebConvertListURL { get => _WebConvertListURL; set { _WebConvertListURL = value; NotifyOfPropertyChange(); } }
        private string _WebConvertListURL;

        #endregion 프로퍼티

        #region 생성자

        public HubBoxVM() : base()
        {
            this._WebConvertListURL = HubBoxHelper.requestWebConvertListURL;   // HubBox 웹사이트 변환 목록 화면 조회 API 주소
        }

        #endregion 생성자

        #region 기본 메소드 

        #endregion 기본 메소드 

        #region Sample

        // ** 구글 크롬 전용 <CefSharp> 참고 자료
        // 참고 URL - https://blog.naver.com/codeknight2/222210163649

        // 참고 2 URL - https://github.com/cefsharp/CefSharp/wiki/Quick-Start

        #endregion Sample
    }
}

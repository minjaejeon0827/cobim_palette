using Serilog;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

using CobimPalette.Commands;
using CobimPalette.Common.CobimBase;
using CobimPalette.Common.LogBase;
using CobimPalette.Interface.Page;
using CobimPalette.Services.Part;
using CobimPalette.Data.Models.CobimBase.WebView2Explorer;


namespace CobimPalette.ViewModels.Part.Palettes.Project.WebView2CDE.WebView2Explorer
{
    // TODO : 체크인 화면(CheckInPartV.xaml) -> 버튼 "파일열기" 클릭시 팝업화면 "열기" 출력되면서 폴더 및 파일목록이 출력되도록
    //        체크인 뷰모델 (CheckInPartVM.cs)에서 메서드 OpenFile 몸체 안에서 -> 파일 열기 메서드 "OpenFileDialog" 사용해서 구현하기 (2024.10.11 minjae)
    // 참고 URL   - https://rhkdrmfh.tistory.com/119
    // 참고 2 URL - https://afsdzvcx123.tistory.com/entry/C-%EB%AC%B8%EB%B2%95-C-%ED%8F%B4%EB%8D%94-%EC%97%B4%EA%B8%B0-%E2%80%93-Process-Start
    // 참고 3 URL - https://insurang.tistory.com/entry/WPF-C-OpenFileDialog-%ED%8C%8C%EC%9D%BC%EC%97%B4%EA%B8%B0-%EC%83%81%EC%84%B8-%ED%8C%8C%EC%9D%BC%EC%98%A4%ED%94%88-%EC%98%A4%ED%94%88%EB%8B%A4%EC%9D%B4%EC%96%BC%EB%A1%9C%EA%B7%B8-CustomPlaces-InitialDirectory-Path-%ED%8F%B4%EB%8D%94-folder-Filter-%ED%95%84%ED%84%B0-%ED%99%95%EC%9E%A5%EC%9E%90-%EC%9C%84%EC%B9%98-%EC%A7%80%EC%A0%95-ofdlg-CheckFileExists-FileOk-ShowDialog

    public class CheckInPartVM : PartBase<WebView2ExplorerView>, IPage
    {
        #region 프로퍼티

        /// <summary>
        /// 체크인 업로드(UpLoad) Command
        /// </summary>
        public ICommand UpLoadCommand { get; set; }

        /// <summary>
        /// 종료 Command
        /// </summary>
        public ICommand ExitCommand { get; set; }

        // TODO : 체크인 팝업화면 제목 프로퍼티 "CheckIn_Title" 필요 없을 시 삭제 예정 (2024.10.11 minjae)
        /// <summary>
        /// 체크인 팝업화면 제목
        /// </summary>
        public string CheckIn_Title { get => _CheckIn_Title; set { _CheckIn_Title = value; NotifyOfPropertyChange(); } }
        private string _CheckIn_Title;


        /// <summary>
        /// 체크인 Comment 입력 - Description 
        /// </summary>
        public string TxtComment { get => _TxtComment; set { _TxtComment = value; NotifyOfPropertyChange(); } }
        private string _TxtComment = string.Empty;

        /// <summary>
        /// 종료 (Exit)
        /// </summary>
        public bool IsExit { get => _IsExit; set { _IsExit = value; NotifyOfPropertyChange(nameof(_IsExit)); } }
        private bool _IsExit;


        #endregion 프로퍼티

        #region 생성자

        public CheckInPartVM() : base()
        {
            this._CheckIn_Title = CobimHelper.CheckIn_Title;

            UpLoadCommand = new ButtonCommand(CobimHelper.AsyncMethodType, UpLoadFileAsync, CanExecuteMethod);
            ExitCommand = new ButtonCommand(CobimHelper.VoidMethodType, ExitCheckIn, CanExecuteMethod);
        }

        #endregion 생성자

        #region 기본 메소드 

        // TODO : 메서드 "CanExecuteMethod" 필요시 수정 예정 (2024.10.10 minjae)
        private bool CanExecuteMethod(object obj)
        {
            return true;
        }

        #endregion 기본 메소드 

        #region UpLoadFileAsync

        /// <summary>
        /// 컨테이너 체크인 + 업로드
        /// </summary>
        private async Task UpLoadFileAsync(object obj)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {

            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message);
            }
            return;
        }

        #endregion UpLoadFileAsync

        #region ExitCheckIn

        /// <summary>
        /// 종료 버튼 
        /// </summary>
        private void ExitCheckIn(object obj)
        {

        }

        #endregion ExitCheckIn

        #region Sample

        #endregion Samples
    }
}

using Serilog;

using System.Reflection;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

using AnimatedImage.Wpf;

using CobimPalette.Common.CobimBase;
using CobimPalette.Common.LogBase;
using CobimPalette.ViewModels.Windows.LoadingWork;
using System.Windows.Media.Imaging;

namespace CobimPalette.Views.Windows.LoadingWork
{
    /// <summary>
    /// CommonLoadingWorkV.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommonLoadingWorkV : Window
    {
        #region 생성자

        public CommonLoadingWorkV()
        {
            InitializeComponent();

            // TODO : 대기 처리중 화면 뷰(CommonLoadingWorkV.xaml) 호출시 DataContext 객체 생성 구현 (2024.10.02 minjae)
            // 참고 URL - https://yeko90.tistory.com/entry/wpf-DataContext-%EB%8B%A4%EC%96%91%ED%95%9C-%EB%B0%A9%EB%B2%95
            this.DataContext = new CommonLoadingWorkVM();

            InitSetting();
        }

        #endregion 생성자

        #region InitSetting

        /// <summary>
        /// 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 
        /// 화면 초기 셋팅
        /// </summary>
        private void InitSetting()
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : 오류 메시지 "Invalid URI: The format of the URI could not be determined" 출력 해결하기 위해
                //        메서드 "InitSetting"안에 아래처럼 속성 "UriSource"에 할당할 이미지 파일 경로를 절대경로 사용해서 구현 (2024.10.10 minjae)
                // commonProgressImg.UriSource = new Uri("pack://application:,,,/CobimPalette;component/Image/CommonProgress.gif");
                // commonProgressImg.UriSource = new Uri("pack://application:,,,/CobimPalette;component/Resources/CommonProgress.gif");
                // 참고 URL - https://github.com/XamlAnimatedGif/WpfAnimatedGif
                // 참고 2 URL - https://github.com/XamlAnimatedGif/XamlAnimatedGif?tab=readme-ov-file
                // 참고 3 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.media.imaging.bitmapimage.urisource?view=windowsdesktop-8.0
                // 참고 4 URL - https://stackoverflow.com/questions/22576588/invalid-uri-the-format-of-the-uri-could-not-be-determined
                var commonProgressImg = new BitmapImage();
                commonProgressImg.BeginInit();

                // 속성 "UriSource"에 이미지 파일 할당 방법 1
                // commonProgressImg.UriSource = new Uri("pack://application:,,,/CobimPalette;component/Image/CommonProgress.gif");
                // 속성 "UriSource"에 이미지 파일 할당 방법 2
                commonProgressImg.UriSource = new Uri("pack://application:,,,/CobimPalette;component/Resources/CommonProgress.gif");
                commonProgressImg.EndInit();
                ImageBehavior.SetAnimatedSource(CommonProgressImg, commonProgressImg);
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion InitSetting

        #region Sample

        #endregion Sample
    }
}

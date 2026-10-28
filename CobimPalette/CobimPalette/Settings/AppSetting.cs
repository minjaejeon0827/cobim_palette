using CobimNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CobimPalette.Settings
{
    // TODO : AutoCAD 애드인 프로그램의 경우 자립(Stand Alone) 방식으로 독자적으로 프로그램을 실행할 수 없고, 
    //        AutoCAD 응용 프로그램에 애드인 프로그램 dll 파일을 연동 후 AutoCAD 응용 프로그램을 선실행 해야만
    //        AutoCAD 애드인 프로그램 또한 실행할 수 있다. (2024.09.30 minjae)
    // 자립(Stand Alone) 방식
    // 참고 URL - https://www.inflearn.com/community/questions/24062/%EC%8A%A4%ED%83%A0%EB%93%9C-%EC%96%BC%EB%A1%A0-stand-alone-%EB%B0%A9%EC%8B%9D?srsltid=AfmBOorEtdzMSrxQfZKvOSz4jCs-JRyw3MeBsyAZZ30PhiuS5MJboVSD
    /// <summary>
    /// 애드인 프로그램 CobimPalette 기본 설정 
    /// </summary>
    public class AppSetting : BindableBase
    {
        /// <summary>
        /// 애드인 프로그램 CobimPalette 기본 설정 객체 자신 
        /// </summary>
        public static AppSetting Default
        {
            get => _Default ?? (_Default = new AppSetting());
            set
            {
                _Default = value;
                BindableBase.StaticChanged(nameof(Default));
            }
        }
        private static AppSetting _Default;

        /// <summary>
        /// Rest API HttpClient 기본 설정 
        /// </summary>
        public HttpClientSetting HttpClientBase
        {
            get => _HttpClientBase ?? (_HttpClientBase = new HttpClientSetting());
            set
            {
                _HttpClientBase = value;
                this.Changed(nameof(HttpClientBase));
            }
        }
        private HttpClientSetting _HttpClientBase;

        /// <summary>
        /// 로그인 기본 설정 
        /// </summary>
        public LoginSetting LoginBase
        {
            get => _LoginBase ?? (_LoginBase = new LoginSetting());
            set
            {
                _LoginBase = value;
                this.Changed(nameof(LoginBase));
            }
        }
        private LoginSetting _LoginBase;

        /// <summary>
        /// 디렉토리(폴더) 기본 설정
        /// </summary>
        public DirectorySetting DirectoryBase
        {
            get => _DirectoryBase ?? (_DirectoryBase = new DirectorySetting());
            set
            {
                _DirectoryBase = value;
                this.Changed(nameof(DirectoryBase));
            }
        }
        private DirectorySetting _DirectoryBase;

        /// <summary>
        /// (주)상상진화 기본 설정
        /// </summary>
        public ImagineBuilderSetting ImagineBuilderBase
        {
            get => _ImagineBuilderBase ?? (_ImagineBuilderBase = new ImagineBuilderSetting());
            set
            {
                _ImagineBuilderBase = value;
                this.Changed(nameof(ImagineBuilderBase));
            }
        }
        private ImagineBuilderSetting _ImagineBuilderBase;

        /// <summary>
        /// Palette 기본 설정 
        /// </summary>
        public PaletteSetting PaletteBase
        {
            get => _PaletteBase ?? (_PaletteBase = new PaletteSetting());
            set
            {
                _PaletteBase = value;
                this.Changed(nameof(PaletteBase));
            }
        }
        private PaletteSetting _PaletteBase;


        /// <summary>
        /// Window 기본 설정
        /// </summary>
        public WindowSetting WindowBase
        {
            get => _WindowBase ?? (_WindowBase = new WindowSetting());
            set
            {
                _WindowBase = value;
                this.Changed(nameof(WindowBase));
            }
        }
        private WindowSetting _WindowBase;

        #region Sample

        #endregion Sample
    }
}

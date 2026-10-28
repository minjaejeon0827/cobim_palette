using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CobimPalette.Common.RibbonBase
{
    public class RibbonHelper
    {
        #region Ribbon

        /// <summary>
        /// 리본 탭 - COBIM
        /// </summary>
        public const string TabName = "COBIM";

        /// <summary>
        /// 리본 탭 아이디 - cobimTab
        /// </summary>
        public const string TabId = "cobimTab";

        /// <summary>
        /// 리본 패널 - COBIM
        /// </summary>
        public const string PanName_COBIM = "COBIM";

        /// <summary>
        /// 리본 패널 아이디 - COBIM
        /// </summary>
        public const string PanId_COBIM = "panCOBIM";

        /// <summary>
        /// 리본 패널 - 성과품
        /// </summary>
        public const string PanName_CDE = "성과품";

        /// <summary>
        /// 리본 패널 아이디 - 성과품
        /// </summary>
        public const string PanId_CDE = "panCDE";

        /// <summary>
        /// 리본 패널 - HubBox
        /// </summary>
        public const string PanName_HubBox = "HubBox";

        /// <summary>
        /// 리본 패널 아이디 - HubBox
        /// </summary>
        public const string PanId_HubBox = "panHubBox";

        /// <summary>
        /// 리본 패널 - (주)상상진화
        /// </summary>
        public const string PanName_ImagineBuilder = "(주)상상진화";

        /// <summary>
        /// 리본 패널 아이디 - (주)상상진화
        /// </summary>
        public const string PanId_ImagineBuilder = "panImagineBuilder";

        /// <summary>
        /// 리본 버튼 - WebExplorer
        /// </summary>
        public const string BtnName_COBIM = "WebExplorer";

        /// <summary>
        /// 리본 버튼 아이디 - WebExplorer
        /// </summary>
        public const string BtnId_COBIM = "btnCOBIM";

        /// <summary>
        /// 리본 버튼 - 성과품(CDE)
        /// </summary>
        public const string BtnName_CDE = "성과품(CDE)";

        /// <summary>
        /// 리본 버튼 아이디 - 성과품(CDE)
        /// </summary>
        public const string BtnId_CDE = "btnCDE";

        /// <summary>
        /// 리본 버튼 - HubBox
        /// </summary>
        public const string BtnName_HubBox = "HubBox";

        /// <summary>
        /// 리본 버튼 아이디 - HubBox
        /// </summary>
        public const string BtnId_HubBox = "btnHubBox";

        /// <summary>
        /// 리본 버튼 - (주)상상진화 (기업 홈페이지)
        /// </summary>
        public const string BtnName_ImagineBuilder = "Home Page";

        /// <summary>
        /// 리본 버튼 아이디 - (주)상상진화 (기업 홈페이지)
        /// </summary>
        public const string BtnId_ImagineBuilder = "btnImagineBuilder";

        /// <summary>
        /// CommandParameter - WebExplorer 
        /// </summary>
        public const string CommandParam_CmdWebExplorer = "CmdWebExplorer ";

        // TODO : 아래 주석친 코드 필요시 참고 (2024.12.10 jbh)
        // TODO : CommandParameter - 성과품(CDE), HubBox, (주)상상진화 (기업 홈페이지) 문자열 끝에 공백 문자(" ") 포함 안 하면
        //        AutoCAD2025 애드인 프로그램 -> 리본 버튼 클릭시 Command.cs 소스 파일
        //        메서드 "WebView2CDE", "CmdHubBox", "CmdImagineBuilder" 정상적으로 실행 안 함.
        //        하여 문자열 끝에 공백 문자(" ") 포함 하도록 구현 (2024.11.07 jbh)
        //        공백 문자(" ") 포함 예시 - "WebView2CDE ", "CmdHubBox ", "CmdImagineBuilder "
        /// <summary>
        /// CommandParameter - 성과품(CDE)
        /// </summary>
        // public const string CommandParam_WebView2CDE = "CmdWebView2CDE ";

        /// <summary>
        /// CommandParameter - HubBox 
        /// </summary>
        // public const string CommandParam_HubBox = "CmdHubBox ";

        /// <summary>
        /// CommandParameter - (주)상상진화 (기업 홈페이지)
        /// </summary>
        // public const string CommandParam_ImagineBuilder = "CmdImagineBuilder ";


        #endregion Ribbon

        #region Sample

        #endregion Sample
    }
}

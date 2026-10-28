using Serilog;

using System.IO;
using System.Reflection;
using System.ComponentModel;
using Exception = System.Exception;

using CobimPalette.Common.CobimBase;
using CobimPalette.Common.Converters;
using CobimPalette.Common.LogBase;
using CobimPalette.Commands;

using Autodesk.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

// TODO : AutoCAD API 개발 커뮤니티 사이트 필요시 참고 (2024.11.08 minjae)
// 참고 URL - https://forums.autodesk.com/t5/net/bd-p/152

namespace CobimPalette.Common.RibbonBase
{
    // TODO : AutoCAD2025 애드인 프로그램 "CobimPalette" 리본 메뉴 구현 (2024.11.07 minjae)
    // 참고 URL - https://help.autodesk.com/view/OARX/2024/ENU/?guid=GUID-01548FB4-7DE1-4025-B4C6-5E33D3530E15
    // 참고 2 URL - https://2nd-deck.tistory.com/entry/CAutoCAD-AutoCAD-%EA%B5%AC%EB%8F%99%EC%8B%9C-Ribbon-Menu-%EC%83%9D%EC%84%B1%ED%95%98%EB%8A%94-%EB%B0%A9%EB%B2%95
    // 참고 3 URL - https://chatgpt.com/c/672be836-7450-8011-b580-ab5674c28f08
    // 참고 4 URL - https://forums.autodesk.com/t5/net/ribbonbar-namespace/td-p/7153055

    /// <summary>
    /// 리본 메뉴 
    /// </summary>
    public class Ribbon
    {
        #region 프로퍼티 

        /// <summary>
        /// 리본 패널 열거형 구조체 
        /// </summary>
        private enum EnumRibbonPanel : int
        {
            [Description("COBIM")]
            COBIM = 0,
            // [Description("성과품")]
            // WEBVIEW2CDE = 0,

            // TODO : 리본 패널 열거형 구조체 멤버 HUBBOX 필요시 사용 예정 (2024.12.04 minjae)
            // [Description("HubBox")]
            // HUBBOX = 1,
            [Description("(주)상상진화")]
            IMAGINE_BUILDER = 1
        }

        #endregion 프로퍼티 

        #region CreateRibbonControl

        /// <summary>
        /// 리본 컨트롤(메뉴) 등록(생성)
        /// </summary>
        public static void CreateRibbonControl(RibbonControl ribbon)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                Log.Information(Logger.GetMethodPath(currentMethod) + "리본 메뉴 등록 시작");

                // 1. 애드인 프로그램 dll 파일 로드
                PreDllLoad();

                // 2. 리본 탭 추가 
                RibbonTab tab = new RibbonTab();
                tab.Title = RibbonHelper.TabName;
                tab.Id = RibbonHelper.TabId;
                ribbon.Tabs.Add(tab);

                // 3. 리본 패널 추가 
                List<RibbonPanelSource> panelList = new List<RibbonPanelSource>();
                panelList.Add(new RibbonPanelSource());

                panelList[(int)EnumRibbonPanel.COBIM].Title = RibbonHelper.PanName_COBIM;
                panelList[(int)EnumRibbonPanel.COBIM].Id = RibbonHelper.PanId_COBIM;

                // TODO : 리본 버튼 "Home Page" 필요시 사용 예정 (2024.12.10 minjae)
                // panelList.Add(new RibbonPanelSource());

                // TODO : 리본 버튼 "HubBox" 필요시 사용 예정 (2024.12.04 minjae)
                // panelList.Add(new RibbonPanelSource());

                // TODO : 리본 버튼 "성과품(CDE)" 필요시 사용 예정 (2024.12.10 minjae)
                // panelList[(int)EnumRibbonPanel.WEBVIEW2CDE].Title = RibbonHelper.PanName_CDE;
                // panelList[(int)EnumRibbonPanel.WEBVIEW2CDE].Id = RibbonHelper.PanId_CDE;

                // TODO : 리본 버튼 "HubBox" 필요시 사용 예정 (2024.12.04 minjae)
                // panelList[(int)EnumRibbonPanel.HUBBOX].Title = RibbonHelper.PanName_HubBox;
                // panelList[(int)EnumRibbonPanel.HUBBOX].Id = RibbonHelper.PanId_HubBox;

                // TODO : 리본 버튼 "Home Page" 필요시 사용 예정 (2024.12.10 minjae)
                // panelList[(int)EnumRibbonPanel.IMAGINE_BUILDER].Title = RibbonHelper.PanName_ImagineBuilder;
                // panelList[(int)EnumRibbonPanel.IMAGINE_BUILDER].Id = RibbonHelper.PanId_ImagineBuilder;

                // foreach 반복문 실행하여 리본 탭에 패널 목록 추가 
                foreach(RibbonPanelSource panelSource in panelList)
                {
                    RibbonPanel rp = new RibbonPanel();
                    rp.Source = panelSource;
                    tab.Panels.Add(rp);
                }

                // TODO : 필요시 리본 버튼 - COBIM 텍스트 "WebExplorer" 숨김 처리 되도록 구현 예정 (2024.12.09 minjae)
                // 참고 URL - https://stackoverflow.com/questions/76601888/button-with-image-and-empty-string-in-revit-api

                // TODO : 리소스 이미지 파일 "COBIMLogo.png" 추가시 아래와 같은 오류 메시지가 출력하여
                //        리소스 파일(Resources.resx) 마우스 우클릭 -> 컨텍스트 메뉴 "다른 프로그램으로 열기(N)..." 클릭
                //        -> 팝업화면 "연결 프로그램 - Resources.resx" 출력
                //        -> 텍스트 "이 파일을 열 때 사용할 프로그램을 선택하세요(C)." 아래 연결할 수 있는 프로그램들 중에 "관리되는 리소스 편집기(레거시)" 클릭
                //        -> 버튼 "확인" 클릭 -> 예전에 리소스 이미지 파일 추가하는 방식으로 리소스 이미지 파일 "COBIMLogo.png" 추가하기 (2024.12.10 minjae)
                // 오류 코드 "MSB3554"
                // 오류 메시지 - 출력 파일 "D:\bhjeon\Cobim_CobimPalette\CobimPalette\CobimPalette\obj\x64\Debug\net8.0-windows\CobimPalette.Properties.Resources.resources"에 쓸 수 없습니다.
                //               'D:\bhjeon\Cobim_CobimPalette\CobimPalette\CobimPalette\Properties\Resources\COBIMLogo.png' 경로의 일부를 찾을 수 없습니다.
                // 참고 URL - https://stackoverflow.com/questions/79176337/visual-studio-2022-resource-explorer-add-file
                // 참고 2 URL - https://stackoverflow.com/questions/78873310/vs2022-updates-broke-resource-file/78924871#78924871
                // 4. 리본 버튼 추가
                RibbonButton btnCOBIM = new RibbonButton();
                btnCOBIM.Text = RibbonHelper.BtnName_COBIM;
                btnCOBIM.Id = RibbonHelper.BtnId_COBIM;
                btnCOBIM.ShowText = true;
                btnCOBIM.ShowImage = true;
                // 리본 버튼에 Command 바인딩 
                btnCOBIM.CommandParameter = RibbonHelper.CommandParam_CmdWebExplorer;
                btnCOBIM.CommandHandler = new RibbonCommand();
                btnCOBIM.LargeImage = BitmapConverter.ConvertFromBitmap(Properties.Resources.COBIMLogo);
                btnCOBIM.Size = RibbonItemSize.Large;   // 리본 버튼 이미지 사이즈 Large (32 X 32 픽셀) 설정
                btnCOBIM.Orientation = System.Windows.Controls.Orientation.Vertical;   // 리본 버튼 이미지 + 텍스트 수직 설정 

                // 5. 4번에서 구현한 리본 버튼 추가 
                panelList[(int)EnumRibbonPanel.COBIM].Items.Add(btnCOBIM);

                Log.Information(Logger.GetMethodPath(currentMethod) + "리본 메뉴 등록 완료");

                // TODO : 아래 주석친 코드 필요시 참고 
                // TODO : AutoCAD2025 테마 다크 모드 설정시 리본 버튼 이미지 화질이 좋지 않게 출력됨.
                //        하여 리본 버튼 이미지 화질이 깔끔하게 출력되기 위해 AutoCAD2025 테마 라이트 모드로 설정함.(2024.11.07 minjae)
                // 4. 리본 버튼 추가 
                // RibbonButton btnCDE = new RibbonButton();
                // btnCDE.Text = RibbonHelper.BtnName_CDE;
                // btnCDE.Id = RibbonHelper.BtnId_CDE;

                // TODO : 리본 버튼 - COBIM 텍스트 "COBIM" 숨김 처리 되도록 구현 (2024.12.09 minjae)
                // btnCDE.ShowText = true;
                // btnCDE.ShowText = false;
                // btnCDE.ShowImage = true;
                // 리본 버튼에 Command 바인딩 
                // btnCDE.CommandParameter = RibbonHelper.CommandParam_WebView2CDE;
                // btnCDE.CommandHandler = new RibbonCommand();
                // btnCDE.LargeImage = BitmapConverter.ConvertFromBitmap(Properties.Resources.WebView2CDE);
                // btnCDE.Size = RibbonItemSize.Large;   // 리본 버튼 이미지 사이즈 Large (32 X 32 픽셀) 설정
                // TODO : 리본 버튼 - COBIM 텍스트 "COBIM" 숨김 처리 되려면 리본 버튼 이미지 + 텍스트 수직 설정 안 해야 해서 주석처리 진행 (2024.12.09 minjae)
                // btnCDE.Orientation = System.Windows.Controls.Orientation.Vertical;   // 리본 버튼 이미지 + 텍스트 수직 설정 

                // TODO : 리본 버튼 "HubBox" 필요시 사용 예정 (2024.12.04 minjae)
                // RibbonButton btnHubBox = new RibbonButton();
                // btnHubBox.Text = RibbonHelper.BtnName_HubBox;
                // btnHubBox.Id = RibbonHelper.BtnId_HubBox;
                // btnHubBox.ShowText = true;
                // btnHubBox.ShowImage = true;
                // // 리본 버튼에 Command 바인딩 
                // btnHubBox.CommandParameter = RibbonHelper.CommandParam_HubBox;
                // btnHubBox.CommandHandler = new RibbonCommand();
                // btnHubBox.LargeImage = BitmapConverter.ConvertFromBitmap(Properties.Resources.HubBox);
                // btnHubBox.Size = RibbonItemSize.Large;   // 리본 버튼 이미지 사이즈 Large (32 X 32 픽셀) 설정
                // btnHubBox.Orientation = System.Windows.Controls.Orientation.Vertical;   // 리본 버튼 이미지 + 텍스트 수직 설정

                // TODO : 리본 버튼 "Home Page" 필요시 사용 예정 (2024.12.10 minjae)
                //RibbonButton btnImagineBuilder = new RibbonButton();
                //btnImagineBuilder.Text = RibbonHelper.BtnName_ImagineBuilder;
                //btnImagineBuilder.Id = RibbonHelper.BtnId_ImagineBuilder;
                //btnImagineBuilder.ShowText = true;
                //btnImagineBuilder.ShowImage = true;
                //// 리본 버튼에 Command 바인딩 
                //btnImagineBuilder.CommandParameter = RibbonHelper.CommandParam_ImagineBuilder;
                //btnImagineBuilder.CommandHandler = new RibbonCommand();
                //btnImagineBuilder.LargeImage = BitmapConverter.ConvertFromBitmap(Properties.Resources.ImagineBuilder);
                //btnImagineBuilder.Size = RibbonItemSize.Large;   // 리본 버튼 이미지 사이즈 Large (32 X 32 픽셀) 설정
                //btnImagineBuilder.Orientation = System.Windows.Controls.Orientation.Vertical;   // 리본 버튼 이미지 + 텍스트 수직 설정

                // 5. 4번에서 구현한 리본 버튼 추가 
                // panelList[(int)EnumRibbonPanel.WEBVIEW2CDE].Items.Add(btnCDE);
                // TODO : 리본 버튼 "HubBox" 필요시 사용 예정 (2024.12.04 minjae)
                // panelList[(int)EnumRibbonPanel.HUBBOX].Items.Add(btnHubBox);
                // TODO : 리본 버튼 "Home Page" 필요시 사용 예정 (2024.12.10 minjae)
                // panelList[(int)EnumRibbonPanel.IMAGINE_BUILDER].Items.Add(btnImagineBuilder);

                // TODO : 아래 주석친 리본 버튼을 리본 패널에 Stack 방식(수직)으로 추가하는 방법 필요시 참고 (2024.11.07 minjae)
                // RibbonRowPanel rrp = new RibbonRowPanel();
                // rrp.Items.Add(btnCDE);
                // rrp.Items.Add(new RibbonRowBreak());  // 버튼 목록을 수직으로 나누는 경계
                // rrp.Items.Add(btnHubBox);
                // rrp.Items.Add(new RibbonRowBreak());  // 버튼 목록을 수직으로 나누는 경계
                // rrp.Items.Add(btnImagineBuilder);
                // RibbonPanel rp = new RibbonPanel();
                // rp.Source.Items.Add(rrp);


            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 예외처리 throw 
            }
        }

        #endregion CreateRibbonControl

        #region PreDllLoad

        /// <summary>
        /// AutoCAD2025 애드인 프로그램 CobimPalette dll 파일(CobimPalette.dll) 로드 
        /// </summary>
        private static void PreDllLoad()
        {
            string dllPath = string.Empty;       // dll 파일(CobimPalette.dll) 경로 초기화 
            string commandPath = string.Empty;   // 리본 버튼에 바인딩할 Command 명령 dll 파일(CobimPalette.dll) 경로 초기화

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                dllPath = CobimHelper.AssemblyFilePath;
                commandPath = $"(command \"netload\" \"{dllPath}\") ";

                // dll 파일(CobimPalette.dll)이 존재하는 경우 
                if(true == File.Exists(dllPath))
                {
                    // AutoCAD2025 애드인 프로그램 CobimPalette의 리본 버튼 Command 바인딩 처리 
                    // 참고 URL - https://help.autodesk.com/view/OARX/2023/ENU/?guid=OARX-ManagedRefGuide-Autodesk_AutoCAD_ApplicationServices_Document_SendStringToExecute_string__MarshalAsUnmanagedType_U1__bool__MarshalAsUnmanagedType_U1__bool__MarshalAsUnmanagedType_U1__bool
                    Application.DocumentManager.MdiActiveDocument.SendStringToExecute(commandPath, true, false, false);
                }
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 예외처리 throw 
            }
        }

        /// <summary>
        /// AutoCAD2025 애드인 프로그램 CobimPalette dll 파일(CobimPalette.dll)과 리본 버튼 Command 바인딩  
        /// </summary>
        private static void PreDllLoad(string dllPath, string command)
        {
            if (System.IO.File.Exists(dllPath))
            {
                // DLL Load

                // 참고 URL - https://help.autodesk.com/view/OARX/2023/ENU/?guid=OARX-ManagedRefGuide-Autodesk_AutoCAD_ApplicationServices_Document_SendStringToExecute_string__MarshalAsUnmanagedType_U1__bool__MarshalAsUnmanagedType_U1__bool__MarshalAsUnmanagedType_U1__bool
                Application.DocumentManager.MdiActiveDocument.SendStringToExecute(command, true, false, false);
            }
        }

        #endregion PreDllLoad

        #region Sample

        // TODO : 아래 주석친 리본 컨트롤(메뉴) 등록(생성) static 메서드 "CreateRibbonControl" 필요시 참고 (2024.11.07 minjae)
        /// <summary>
        /// 리본 컨트롤(메뉴) 등록(생성)
        /// </summary>
        //public static void CreateRibbonControl(RibbonControl ribbon)
        //{
        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        Log.Information(Logger.GetMethodPath(currentMethod) + "리본 메뉴 등록 시작");

        //        // 1. 리본 버튼 객체 "button1", "button2" 생성 및 버튼 텍스트 값 할당 
        //        RibbonButton button1 = new RibbonButton();
        //        button1.Text = "Button1";
        //        button1.ShowText = true;

        //        RibbonButton button2 = new RibbonButton();
        //        button2.Text = "Button2";
        //        button2.ShowText = true;

        //        // 2. 리본 패널 객체 rowPanel 생성 및 Stack 방식으로 리본 버튼 객체 "button1", "button2" 추가 
        //        RibbonRowPanel rowPanel = new RibbonRowPanel();
        //        rowPanel.Items.Add(button1);
        //        rowPanel.Items.Add(new RibbonRowBreak());
        //        rowPanel.Items.Add(button2);

        //        // 3. 리본 패널 Source 객체 panelSource 생성 및 2번에서 생성한 리본 패널 객체 rowPanel 추가 
        //        RibbonPanelSource panelSource = new RibbonPanelSource();
        //        panelSource.Title = "Panel1";
        //        panelSource.Items.Add(rowPanel);

        //        // 4. 리본 패널 객체 panel 생성 및 속성 Source에 값 할당 
        //        RibbonPanel panel = new RibbonPanel();
        //        panel.Source = panelSource;

        //        // 5. 리본 탭 객체 tab 셍상 및 4번에서 생성한 리본 패널 객체 panel 추가 
        //        RibbonTab tab = new RibbonTab();
        //        tab.Title = "Tab1";
        //        tab.Panels.Add(panel);

        //        //RibbonControl ribbon = new RibbonControl();
        //        ribbon.Tabs.Add(tab);

        //        Log.Information(Logger.GetMethodPath(currentMethod) + "리본 메뉴 등록 완료");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        throw;   // 오류 발생시 예외처리 throw 
        //    }
        //}

        #endregion Sample
    }
}

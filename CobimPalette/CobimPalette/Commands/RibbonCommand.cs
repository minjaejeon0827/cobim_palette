using System.Windows.Input;

using Autodesk.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace CobimPalette.Commands
{
    // TODO : AutoCAD2025 애드인 프로그램 "CobimPalette" 리본 버튼 Command 클래스 "RibbonCommand" 구현 (2024.11.07 jbh)
    // 참고 URL - https://help.autodesk.com/view/OARX/2024/ENU/?guid=GUID-01548FB4-7DE1-4025-B4C6-5E33D3530E15
    // 참고 2 URL - https://2nd-deck.tistory.com/entry/CAutoCAD-AutoCAD-%EA%B5%AC%EB%8F%99%EC%8B%9C-Ribbon-Menu-%EC%83%9D%EC%84%B1%ED%95%98%EB%8A%94-%EB%B0%A9%EB%B2%95
    // 참고 3 URL - https://chatgpt.com/c/672be836-7450-8011-b580-ab5674c28f08
    // 참고 4 URL - https://forums.autodesk.com/t5/net/ribbonbar-namespace/td-p/7153055

    /// <summary>
    /// 리본 버튼 Command
    /// </summary>
    public class RibbonCommand : ICommand
    {
        #region 프로퍼티 

        public event EventHandler CanExecuteChanged;

        #endregion 프로퍼티 

        #region 기본 메소드 

        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// AutoCAD2025 애드인 프로그램 CobimPalette 속한 리본 버튼 클릭시 실행 이벤트 
        /// </summary>
        public void Execute(object parameter)
        {
            RibbonButton ribbonButton = parameter as RibbonButton;
            Application.DocumentManager.MdiActiveDocument.SendStringToExecute((string)ribbonButton.CommandParameter, true, false, true);
        }

        #endregion 기본 메소드

        #region Sample

        #endregion Sample
    }
}

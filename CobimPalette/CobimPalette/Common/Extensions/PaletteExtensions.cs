using System.Windows;

using Autodesk.AutoCAD.Windows;

namespace CobimPalette.Common.Extensions
{
    public static class PaletteExtensions
    {
        // TODO : 생성된 CustomPaletteSet 객체 관리자 클래스 PaletteSetManager 구현 (2024.11.12 jbh)
        // 참고 URL - https://chatgpt.com/c/6732eff1-98a8-8011-844a-96c169d9efa5
        /// <summary>
        /// 생성된 CustomPaletteSet 객체 관리자 클래스
        /// </summary>
        public class PaletteSetManager
        {
            #region 프로퍼티 

            /// <summary>
            /// PaletteSet 클래스 객체 Dictionary 
            /// </summary>
            //private Dictionary<Guid, PaletteSet> _PaletteSetDic;

            /// <summary>
            /// CustomPaletteSet 클래스 객체 Dictionary 
            /// </summary>
            private Dictionary<Guid, CustomPaletteSet> _CustomPaletteSetDic;

            #endregion 프로퍼티

            #region 생성자

            // TODO : 필요시 생성자 PaletteSetManager 수정 예정 (2024.11.12 jbh)
            public PaletteSetManager()
            {
                //_PaletteSetDic = new Dictionary<Guid, PaletteSet>();
                _CustomPaletteSetDic = new Dictionary<Guid, CustomPaletteSet>();
            }

            #endregion 생성자 

            #region PaletteIsRegistered

            /// <summary>
            /// 특정 Guid(paletteGuId) 값을 가진 CustomPaletteSet 객체 등록여부 확인
            /// </summary>
            public bool PaletteIsRegistered(Guid paletteGuId)
            {
                // 특정 paletteGuId 값을 가진 CustomPaletteSet 객체 등록 여부 삼항 연산자 사용해서 확인 
                // bool IsRegistered = (true == _PaletteSetDic.ContainsKey(paletteGuId)) ? true : false;
                // return IsRegistered;
                bool IsRegistered = (true == _CustomPaletteSetDic.ContainsKey(paletteGuId)) ? true : false;
                return IsRegistered;
            }

            #endregion PaletteIsRegistered

            #region RegisterPalette

            /// <summary>
            /// CustomPaletteSet 등록 
            /// </summary>
            public void RegisterPalette(Guid paletteGuId, string title, FrameworkElement paletteElement, DockSides paletteDockSides)
            {
                var paletteSet = new PaletteSet(title, paletteGuId);
                CustomPaletteSet customPaletteSet = new CustomPaletteSet(paletteSet, paletteElement, paletteDockSides);
                RegisterPalette(paletteGuId, customPaletteSet);
            }

            /// <summary>
            /// CustomPaletteSet 객체 Dictionary 데이터 추가 
            /// </summary>
            private void RegisterPalette(Guid paletteGuId, CustomPaletteSet customPaletteSet)
            {
                _CustomPaletteSetDic.Add(paletteGuId, customPaletteSet);
            }

            #endregion RegisterPalette

            #region GetPalette

            // TODO : 필요시 특정 GUID를 가진 CustomPaletteSet 객체 가져오기 메서드 "GetPalette" 수정 예정 (2024.11.12 jbh)
            /// <summary>
            /// 특정 Guid(paletteGuId) 값을 가진 CustomPaletteSet 객체 가져오기 
            /// </summary>
            public CustomPaletteSet GetPalette(Guid paletteGuId)
            {
                // 특정 paletteGuId 값을 가진 CustomPaletteSet 객체가 등록된 경우 
                if(true == PaletteIsRegistered(paletteGuId))
                    return _CustomPaletteSetDic[paletteGuId];
                // 특정 paletteGuId 값을 가진 CustomPaletteSet 객체가 등록된 적이 없는 경우
                else return null;
            }

            #endregion GetPalette

            #region Sample

            #endregion Sample
        }

        /// <summary>
        /// AutoCAD 응용프로그램에서 호출되며, 
        /// AutoCAD 응용프로그램 화면(UI 인터페이스)에 추가될 
        /// Palette(Palette Window / Palette Page)에 대한 정보(데이터)를 제공하는 제공자 클래스  
        /// </summary>
        public class CustomPaletteSet
        {
            #region 프로퍼티

            /// <summary>
            /// AutoCAD 응용 프로그램 화면(UI 인터페이스)에 추가되는 새로운 PaletteSet 클래스 객체 
            /// </summary>
            private PaletteSet _PaletteSet { get; set; }

            #endregion 프로퍼티

            #region 생성자


            public CustomPaletteSet(PaletteSet paletteSet, FrameworkElement paletteElement, DockSides paletteDockSides)
            {
                SetUpPalette(paletteSet, paletteElement, paletteDockSides);
            }

            #endregion 생성자

            #region SetUpPalette

            /// <summary>
            /// Palette 셋업 (커스터마이징 처리)
            /// </summary>
            private void SetUpPalette(PaletteSet paletteSet, FrameworkElement paletteElement, DockSides paletteDockSides)
            {
                _PaletteSet = paletteSet;

                _PaletteSet.Style = PaletteSetStyles.ShowAutoHideButton |
                                    PaletteSetStyles.ShowCloseButton |
                                    PaletteSetStyles.ShowPropertiesMenu;

                _PaletteSet.MinimumSize = new System.Drawing.Size(250, 150);

                _PaletteSet.AddVisual(paletteSet.Name, paletteElement);
                
                _PaletteSet.Visible = false;
                _PaletteSet.Dock = paletteDockSides;
                _PaletteSet.RolledUp = false;
            }

            #endregion SetUpPalette

            #region SetData

            /// <summary>
            /// 데이터 모델 클래스 WebView2ExplorerView.cs에 속하는
            /// 데이터 프로퍼티에 값 셋팅(또는 값 복제)
            /// </summary>
            public void SetData(bool visible, DockSides paletteDockSides, bool rolledUp)
            {
                this.SetVisible(visible);
                this.SetDockSides(paletteDockSides);
                this.SetRolledUp(rolledUp);
            }

            #endregion SetData

            #region SetVisible

            /// <summary>
            /// Palette 화면 출력 여부 값 셋팅 
            /// </summary>
            public void SetVisible(bool visible)
            {
                _PaletteSet.Visible = visible;
            }

            #endregion SetVisible

            #region GetVisible

            /// <summary>
            /// Palette 화면 출력 여부 값 가져오기 
            /// </summary>
            public bool GetVisible() 
            {
                return _PaletteSet.Visible;
            }

            #endregion GetVisible

            #region SetDockSides

            /// <summary>
            /// Palette 화면 DockSides 상태 값 셋팅 
            /// </summary>
            public void SetDockSides(DockSides paletteDockSides)
            {
                _PaletteSet.Dock = paletteDockSides;
            }

            #endregion SetDockSides

            #region GetDockSides

            /// <summary>
            /// Palette 화면 DockSides 상태 값 가져오기 
            /// </summary>
            public DockSides GetDockSides()
            {
                return _PaletteSet.Dock;
            }

            #endregion GetDockSides

            #region SetRolledUp

            /// <summary>
            /// Palette 화면이 정상 출력 되지 않고 AutoCAD 응용 프로그램 
            /// 한쪽 모서리 쪽으로 말려 들어 갔는지 여부 값 셋팅 
            /// </summary>
            public void SetRolledUp(bool rolledUp)
            {
                _PaletteSet.RolledUp = rolledUp;
            }

            #endregion SetRolledUp

            #region GetRolledUp

            /// <summary>
            /// Palette 화면이 정상 출력 되지 않고 AutoCAD 응용 프로그램 
            /// 한쪽 모서리 쪽으로 말려 들어 갔는지 여부 값 가져오기  
            /// </summary>
            public bool GetRolledUp()
            {
                return _PaletteSet.RolledUp;
            }

            #endregion GetRolledUp
        }

        #region Sample

        #endregion Sample
    }
}

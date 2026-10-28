using System;

namespace CobimPalette.Data.Models.CobimBase.AutoCAD
{
    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    public class AutoCADView<TData> : AutoCADView where TData : AutoCADView<TData> { }

    // TODO : 추후 필요시 AutoCAD API 전용 데이터 모델 클래스 AutoCADView 구현 예정 (2024.10.30 jbh) 
    /// <summary>
    /// CobimPalette
    /// AutoCAD API 전용 데이터 모델
    /// </summary>
    public class AutoCADView : TAutoCADView<AutoCADView>
    {
        #region 프로퍼티 

        #endregion 프로퍼티 

        #region 생성자

        public AutoCADView() : base() { }

        #endregion 생성자
    }

    #region Sample

    #endregion Sample
}

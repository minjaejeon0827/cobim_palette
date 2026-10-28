using System;
using System;

namespace CobimPalette.Data.Models.CobimBase.AutoCAD
{

    // TODO : 필요시 TAutoCADView.cs 클래스 파일 구현 예정 (2024.09.23 jbh)
    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    public class TAutoCADView<TData> : TAutoCADView where TData : TAutoCADView<TData> { }

    public class TAutoCADView : CDEModelBase<TAutoCADView>
    {
        #region 프로퍼티 

        #endregion 프로퍼티 
    }

    #region Sample

    #endregion Sample
}

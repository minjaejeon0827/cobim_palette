using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CobimPalette.Data.Models.CobimBase.Login
{
    // TODO : 필요시 TLoginView.cs 클래스 파일 구현 예정 (2024.09.23 jbh)
    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    public class TLoginView<TData> : TLoginView where TData : TLoginView<TData> { }

    public class TLoginView : CDEModelBase<TLoginView>
    {
        #region 프로퍼티 

        #endregion 프로퍼티 
    }

    #region Sample

    #endregion Sample
}

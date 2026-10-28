using System;

namespace CobimPalette.Data.Models.CobimBase.HubBox
{
    // TODO : 필요시 THubBoxView.cs 클래스 파일 구현 예정 (2024.09.25 jbh)
    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    public class THubBoxView<TData> : THubBoxView where TData : THubBoxView<TData> { }

    // TODO : 일단 임시로 아래 처럼 구현해서 사용 추후 필요시 CDEModelBase 클래스 구현 예정 (2024.09.25 jbh)    
    public class THubBoxView : CDEModelBase<THubBoxView>
    {
        #region 프로퍼티 

        #endregion 프로퍼티
    }

    #region Sample

    #endregion Sample
}

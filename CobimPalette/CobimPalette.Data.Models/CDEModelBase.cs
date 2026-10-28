using CobimNet;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CobimPalette.Data.Models
{
    // TODO : 추후 필요시 "CDEModelBase" 클래스 구현 예정 (2024.09.23 minjae)

    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    public class CDEModelBase<TData> : CDEModelBase where TData : CDEModelBase<TData> { }

    // TODO : 추후 필요시 "CDEClass" 클래스, "ICloneable" 인터페이스 구현 예정 (2024.09.23 minjae)
    public class CDEModelBase : BindableBase, ICloneable
    {
        #region  프로퍼티 

        public enum DataCodeType
        {
            [Description("-데이터-")]
            Unknown = 0,
            [Description("성과품(CDE)")]
            CDE = 1,
            [Description("HubBox")]
            HubBox = 2
        }

        /// <summary>
        /// 데이터코드 타입 - TableCodeType과 비슷한 역할 
        /// </summary>
        [JsonIgnore]
        public DataCodeType DataCode
        {
            get => this._DataCode;
            set => this._DataCode = value;
        }
        private DataCodeType _DataCode;

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion 프로퍼티 

        #region 생성자

        //public CDEModelBase()
        //{

        //}

        #endregion 생성자

        #region Sample

        #endregion Sample
    }
}

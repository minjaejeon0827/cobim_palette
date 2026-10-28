using Serilog;

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace CobimPalette.Utils.String
{
    public static class StrHelper
    {
        /// <summary>
        /// Enum 열거형 구조체 멤버 변수의 Description 문자열 리턴
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum source)
        {
            try
            {
                DescriptionAttribute[] customAttributes = (DescriptionAttribute[])source.GetType().GetField(source.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
                // TODO : if 조건절에 != null 보다 빠른 is not null 연산자 사용 (2024.10.11 minjae)
                // 참고 URL - https://husk321.tistory.com/405
                return customAttributes is not null && customAttributes.Length != 0 ? customAttributes[0].Description : source.ToString();
            }
            catch (Exception ex)
            {
                Log.Debug("-----------------------------------\n 메소드 : " + MethodBase.GetCurrentMethod().ReflectedType.Name + "==>" + MethodBase.GetCurrentMethod().Name + "\n" + string.Format(" LINE : {0} 행\n", (object)new StackFrame(0, true).GetFileLineNumber()) + "-----------------------------------\n 내용 : " + ex.Message + "\n-----------------------------------\n");
            }
            return source.ToString();
        }
    }
}

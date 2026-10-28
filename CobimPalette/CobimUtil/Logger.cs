using System.Reflection;

namespace CobimUtil
{
    public class Logger
    {
        #region 프로퍼티

        // TODO : 로그 타입 유형 문자열 (const string) 객체 다른 유형 필요시 추가 예정 (2024.09.25 jbh)
        public const string debugMessage = "디버그 - ";

        public const string errorMessage = "오류 - ";

        public const string warningMessage = "경고 - ";

        #endregion 프로퍼티

        #region 생성자

        // TODO : 필요시 생성자 Logger 구현 예정 (2024.09.25 jbh)
        public Logger(string loggerName)
        {

        }

        #endregion 생성자

        #region 기본 메소드 

        #endregion 기본 메소드 

        #region GetMethodPath

        /// <summary>
        /// 로그 실행한 Namespace 및 메서드(비동기 메서드 포함) 경로 추출
        /// </summary>
        public static string GetMethodPath(MethodBase pCurrentMethod)
        {
            var fullMethodPath = string.Empty;

            var currentUtilMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                fullMethodPath = "[" + pCurrentMethod.DeclaringType.FullName + " | " + pCurrentMethod.Name + "] : ";

                return fullMethodPath;
            }
            catch (Exception ex)
            {
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion GetMethodPath

        #region Sample

        #endregion Sample
    }
}

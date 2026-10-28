using Serilog;

using System.IO;
using System.Reflection;
using System.Text;

namespace CobimPalette.Common.LogBase
{
    public class Logger
    {
        #region 프로퍼티 

        // TODO : 로그 타입 유형 문자열 (const string) 객체 다른 유형 필요시 추가 예정 (2024.08.14 jbh)
        public const string debugMessage = "디버그 - ";

        public const string infoMessage = "정보 - ";

        public const string errorMessage = "오류 - ";

        public const string warningMessage = "경고 - ";

        #endregion 프로퍼티

        #region ConfigureLogger

        // TODO : C# Serilog 사용해서 로그 남기도록 static 메서드 "ConfigureLogger" 구현 (2024.08.14 jbh)
        // 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%EB%AC%B8%EB%B2%95-C-Serilog-%EC%82%AC%EC%9A%A9%ED%95%98%EC%97%AC-%EB%A1%9C%EA%B7%B8-%EB%82%A8%EA%B8%B0%EA%B8%B0

        /// <summary>
        /// Serilog 로그 초기 설정 
        /// </summary>
        /// <param name="pAssemblyName">로그 기록 남기려는 어셈블리 이름</param>
        public static void ConfigureLogger(int pLogFileCountLimit, string pAssemblyName, string pLogDirectoryPath)
        {
            string logDirectoryPath = string.Empty;    // 로그파일 상위 디렉토리(폴더) 경로 
            string logFilePath = string.Empty;   // 로그파일 생성 경로

            try
            {
                // 로그파일 상위 디렉토리(폴더) 경로 구하기
                StringBuilder sbLogDirectoryPath = new StringBuilder(pLogDirectoryPath);
                logDirectoryPath = sbLogDirectoryPath.ToString();

                // 상위 디렉토리(폴더) 하위 로그파일 경로 구하기 
                StringBuilder sbLogFilePath = new StringBuilder(logDirectoryPath);
                sbLogFilePath.Append($"\\{pAssemblyName}_.log");
                logFilePath = sbLogFilePath.ToString();

                // TODO : Stylet Logger 클래스 사용 안하고 Serilog 이용해서 로그 기록 및 로그 파일 생성 (2024.08.14 jbh)
                // 참고 URL - https://m.blog.naver.com/wolfre/221713399852
                // 참고 2 URL - https://github.com/serilog/serilog/wiki/Getting-Started
                // 참고 3 URL - https://afsdzvcx123.tistory.com/entry/C-%EB%AC%B8%EB%B2%95-C-Serilog-%EC%82%AC%EC%9A%A9%ED%95%98%EC%97%AC-%EB%A1%9C%EA%B7%B8-%EB%82%A8%EA%B8%B0%EA%B8%B0
                // 참고 4 URL - https://blog.naver.com/PostView.naver?blogId=wolfre&logNo=221713399852&parentCategoryNo=&categoryNo=26&viewDate=&isShowPopularPosts=true&from=search
                // 참고 5 URL - https://www.reddit.com/r/dotnet/comments/qhwa4i/serilog_remove_files_older_than_30_days/
                // 참고 6 URL - https://github.com/serilog/serilog-sinks-file#rolling-policies

                // TODO : 에러 로그를 메일로 보내는 로직(.WriteTo.Email) 필요시 추후 구현 예정 (2024.08.14 jbh)
                // 참고 URL - https://github.com/serilog/serilog-sinks-email
                var log = new LoggerConfiguration()
                              .MinimumLevel.Verbose()   // 로그 남기는 기준 ex) Verbose 이상 다 남겨야 함.
                                                        // Verbose(0) => Debug => Information => Warning => Error => Fatal(5)
                              .WriteTo.Console()        // 콘솔 파일에도 로그 출력
                              .WriteTo.Debug()
                              // 로그 파일 이름을 날짜 형식(예)"CobimPalette_20240814.log" 으로 로그 파일 생성
                              .WriteTo.File(
                                  // $"Logs\\{Assembly.GetEntryAssembly().GetName().Name}\\{Assembly.GetEntryAssembly().GetName().Name}_{DateTime.Now.ToString("yyyyMMdd")}.log",
                                  path: logFilePath,
                                  Serilog.Events.LogEventLevel.Verbose,
                                  // TODO : 로그 파일에 날짜를 수동으로 지정할 필요 없이 rollingInterval: RollingInterval.Day, 사용해서 로그파일 이름 언더바(_)옆에 로그 생성된 날짜가 추가되도록 구현
                                  //        (이렇게 구현해야 아래 프로퍼티 "retainedFileCountLimit" 사용해서 날짜가 지난 로그파일 삭제 처리 할 수 있음.) (2024.08.14 jbh) 
                                  // 참고 URL - https://stackoverflow.com/questions/32108148/serilog-rollingfile
                                  rollingInterval: RollingInterval.Day,
                                  rollOnFileSizeLimit: true,   // 로그 파일 최대 사이즈 1GB  null 옵션 가능
                                                               // TODO : 테스트 용 로그 파일 갯수 2개(2일치 로그) 설정(2일 이상 지난 오래된 로그는 삭제 처리)
                                                               //        2일 지난 로그 파일 정상적으로 삭제 처리시 아래 주석 처리된 소스코드 "retainedFileCountLimit: 62,"로 다시 구현해야 함. (2024.08.14 jbh)
                                                               // 참고 URL - https://stackoverflow.com/questions/44577336/how-do-i-automatically-tail-delete-older-logs-using-serilog-in-a-net-wpf-appl
                                  retainedFileCountLimit: pLogFileCountLimit,
                                  // retainedFileCountLimit: 62,  // 로그 파일 갯수 62개(2달치 로그) 설정(2달 지난 오래된 로그는 삭제 처리) - 기본 31개 설정 가능, null 옵션 가능
                                  encoding: Encoding.UTF8
                              )
                              .CreateLogger();
                Log.Logger = log;             // Serilog를 애드인 프로그램 CobimPalette에서 사용할 수 있도록 설정 (전역 로그)

                // 기간 지난 로그 파일 삭제 (new LoggerConfiguration() -> .WriteTo.File(retainedFileCountLimit)에 할당된 값(기간) 기준)
                DeleteOldLogFiles(pLogFileCountLimit, logDirectoryPath);

                // 로그 레벨 기록 예시
                // Log.Information($"Info name = {System.Reflection.Assembly.GetEntryAssembly().GetName().Name}");
                // Log.Debug($"Debug name = {System.Reflection.Assembly.GetEntryAssembly().GetName().Name}");
                // Log.Verbose($"Verbose name = {System.Reflection.Assembly.GetEntryAssembly().GetName().Name}");
                // Log.Warning($"Warning name = {System.Reflection.Assembly.GetEntryAssembly().GetName().Name}");
                // Log.Error($"Error name = {System.Reflection.Assembly.GetEntryAssembly().GetName().Name}");
                // Log.Fatal($"Fatal name = {System.Reflection.Assembly.GetEntryAssembly().GetName().Name}");
            }
            catch(Exception ex)
            {
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion ConfigureLogger

        #region DeleteOldLogFiles

        // TODO : 기간 지난 로그 파일 삭제 ((new LoggerConfiguration() -> .WriteTo.File(retainedFileCountLimit)에 할당된 값(기간) 기준)) 처리 메서드 "DeleteOldLogFiles" 구현 (2024.08.14 jbh)
        // 참고 URL - https://chat.openai.com/c/8791804a-55ca-4bf1-b54e-1a34a68f39b5
        /// <summary>
        /// 기간 지난 로그 파일 삭제
        /// </summary>
        public static void DeleteOldLogFiles(int pLogFileCountLimit, string pLogDirectoryPath)
        {
            string logDirectoryPath = string.Empty;   // 로그 파일이 저장된 디렉토리 경로 
            int daysToKeep = 0;                       // 로그 파일 삭제할 기간 설정 (예: 30일 이전 파일 삭제 : -30으로 음수로 값 할당 처리 필요)

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                logDirectoryPath = pLogDirectoryPath;     // 1. 로그 파일이 저장된 디렉토리 경로 할당

                daysToKeep = (-1) * pLogFileCountLimit;   // 2. 로그 파일 삭제할 기간 설정 (로그파일 삭제하려면 음수로 값 할당 처리 필요)

                DirectoryInfo directory = new DirectoryInfo(logDirectoryPath);

                // 3. 디렉터리(directory) 존재 여부 확인
                if (false == directory.Exists) Directory.CreateDirectory(logDirectoryPath);   // 디렉터리 새로 생성

                var fileList = directory.GetFiles().ToList();   // 4. 해당 디렉터리(di) 안에 로그 파일 존재 여부 확인 

                // 5. 오래된 파일을 찾아서 삭제 
                // TODO : Linq 확장 메서드 "FindAll" "ForEach" "DateTime.Compare" 사용 기간(날짜) 지난 로그 파일 존재여부 확인
                // DateTime.Compare 메서드
                // 참고 URL - https://jinwooking.tistory.com/22

                // DateTime.CompareTo 메서드
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.datetime.compareto?view=net-7.0

                // List 의 FindAll()
                // 참고 URL - https://jwidaegi.blogspot.com/2019/05/unity-list-findall.html

                // C# LINQ List 의 All(), Any()
                // 참고 URL - https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=empty_wagon&logNo=20148303261

                // C# LINQ List 의 ForEach()
                // 참고 URL - https://www.codegrepper.com/code-examples/csharp/linq+foreach+c%23

                // 날짜 지난 로그 파일이 존재하고
                // 로그 파일 생성 날짜가 금일 날짜 보다 30일 이전(DateTime.Now.AddDays(-30))이면 로그 파일 삭제
                // 참고 URL - https://bigenergy.tistory.com/entry/C-%ED%83%80%EC%9D%B4%EB%A8%B8%EB%A5%BC-%EC%9D%B4%EC%9A%A9%ED%95%9C-%EC%98%A4%EB%9E%98%EB%90%9C-%ED%8C%8C%EC%9D%BC-%EC%82%AD%EC%A0%9C-%EB%A1%9C%EA%B7%B8-%EC%82%AD%EC%A0%9C
                // 참고 2 URL - https://medialink.tistory.com/44
                // TODO : 아래 소스코드 실행시 당일 날짜(DateTime.Now) 30일 전일(-30 = daysToKeep) 이전 로그파일 찾아서 오래된 로그 파일 삭제 처리 구현 (2024.08.14 jbh)
                fileList.FindAll(file => file.CreationTime.CompareTo(DateTime.Now.AddDays(daysToKeep)) < 0)
                        .ForEach(file => {
                            if (Path.GetExtension(file.FullName) == ".log") File.Delete(file.FullName);
                        });
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                Console.WriteLine($"오류 발생: {ex.Message}");
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion DeleteOldLogFiles

        #region GetMethodPath

        /// <summary>
        /// 로그 실행한 Namespace 및 메서드(비동기 메서드 포함) 경로 추출
        /// </summary>
        public static string GetMethodPath(MethodBase pCurrentMethod)
        {
            var fullMethodPath = string.Empty;                  // 실행된 메소드 전체 경로  
            var LoggerMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : 테스트 코드 - 강제로 오류 발생하도록 Exception 생성 하도록 구현 (필요시 사용) (2024.08.14 jbh)
                // 참고 URL - https://morm.tistory.com/187
                // 참고 2 URL - https://learn.microsoft.com/ko-kr/dotnet/csharp/fundamentals/exceptions/creating-and-throwing-exceptions
                // throw new Exception();

                fullMethodPath = "[" + pCurrentMethod.DeclaringType.FullName + " | " + pCurrentMethod.Name + "] : ";

                return fullMethodPath;
            }
            catch (Exception ex)
            {
                // fullMethodPath = "[" + LoggerMethod.DeclaringType.FullName + " | " + LoggerMethod.Name + "] : ";
                // Log.Error(fullMethodPath + Logger.errorMessage + ex.Message);
                Log.Error(LoggerMethod.Name + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion GetMethodPath

        #region Sample

        #endregion Sample
    }
}

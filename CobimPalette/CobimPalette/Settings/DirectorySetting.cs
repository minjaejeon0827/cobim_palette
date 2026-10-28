using System;

namespace CobimPalette.Settings
{
    /// <summary>
    /// 디렉토리(폴더) 기본 설정
    /// </summary>
    public class DirectorySetting
    {
        #region 프로퍼티

        /// <summary>
        /// DLL 파일(CobimPalette.dll)의 부모 폴더 경로
        /// </summary>
        private string _ParentDirectoryPath { get; set; }

        /// <summary>
        /// 폴더 (디렉토리) 공통 경로 
        /// (응용 프로그램 - CobimExplorer.exe, Revit 애드인 프로그램 - CobimDockableDialogs.dll, AutoCAD 애드인 프로그램 - CobimPalette.dll)
        /// </summary>
        private string _CommonDirectoryPath { get; set; }

        /// <summary>
        /// 로그(Logs) 폴더(디렉토리) 경로 
        /// </summary>
        private string _LogDirectoryPath { get; set; }

        #endregion 프로퍼티 

        #region 생성자

        public DirectorySetting()
        {
            _ParentDirectoryPath = string.Empty;
            _LogDirectoryPath = string.Empty;
        }

        #endregion 생성자 

        #region SetParentDirectoryPath

        // TODO : 메서드 "SetParentDirectoryPath" 호출 -> 메서드 Directory.GetParent 호출시 변수 parentDirectoryPath에 담긴 파일의 루트(부모) 폴더가 없는 경우 오류가 발생함.
        //        하여 해당 메서드 SetParentDirectoryPath 호출문 주석 처리 완료 (2024.10.11 jbh)
        /// <summary>
        /// DLL 파일(CobimPalette.dll)의 부모 폴더 경로 값 구한 후 셋팅 
        /// </summary>
        //public void SetParentDirectoryPath(string pAssemblyFilePath)
        //{
        //    string parentDirectoryPath = string.Empty;                 // DLL 파일(CobimPalette.dll)의 부모 폴더 경로
        //    var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

        //    try
        //    {
        //        parentDirectoryPath = Path.GetDirectoryName(pAssemblyFilePath);  // DLL 파일(CobimPalette.dll)의 바로 위 상위 디렉토리(폴더명 - "Debug" 또는 "dll") 경로 가져오기

        //        // 2. DLL 파일의 상위 디렉토리(폴더명 - "CobimPalette") 가져오기 
        //        // Path.GetDirectoryName 참고 URL - https://afsdzvcx123.tistory.com/entry/C-%EB%AC%B8%EB%B2%95-%ED%8C%8C%EC%9D%BC-%EA%B2%BD%EB%A1%9C%EC%97%90%EC%84%9C-%EB%94%94%EB%A0%89%ED%86%A0%EB%A6%AC-%EA%B2%BD%EB%A1%9C-%EA%B0%80%EC%A0%B8%EC%98%A4%EA%B8%B0
        //        // Directory.GetParent 참고 URL - https://chat.openai.com/c/0ed1e911-9425-4b35-9008-8e4a0a4493db
        //        while(true)
        //        {
        //            // parentDirName = Path.GetDirectoryName(dllPath);
        //            // dllPath = parentDirName;

        //            // TODO : 메서드 Directory.GetParent 호출시 변수 parentDirectoryPath에 담긴 파일의 루트(부모) 폴더가 없는 경우 오류가 발생함.
        //            //        하여 해당 메서드 Directory.GetParent 호출하는 상위 메서드 "SetParentDirectoryPath" 주석 처리 완료 (2024.10.11 jbh)
        //            DirectoryInfo parentDirectory = Directory.GetParent(parentDirectoryPath);
        //            parentDirectoryPath = parentDirectory.FullName;

        //            if(parentDirectory.Name.Equals(CobimHelper.AssemblyName)) break;
        //        }

        //        //_ParentDirectoryPath = string.Empty;
        //        _ParentDirectoryPath = parentDirectoryPath;
        //    }
        //    catch(Exception ex)
        //    {
        //        Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
        //        throw;   // 오류 발생시 상위 호출자 예외처리 전달 throw 
        //    }
        //}

        #endregion SetParentDirectoryPath

        #region GetParentDirectoryPath

        /// <summary>
        /// DLL 파일(CobimPalette.dll)의 부모 폴더 경로 값 가져오기
        /// </summary>
        public string GetParentDirectoryPath()
        {
            return _ParentDirectoryPath;
        }

        #endregion GetParentDirectoryPath

        #region SetCommonDirectoryPath

        /// <summary>
        /// 폴더 (디렉토리) 공통 경로 값 셋팅 
        /// (응용 프로그램 - CobimExplorer.exe, Revit 애드인 프로그램 - CobimDockableDialogs.dll, AutoCAD 애드인 프로그램 - CobimPalette.dll)
        /// </summary>
        public void SetCommonDirectoryPath(string commonDirectoryPath)
        {
            _CommonDirectoryPath = commonDirectoryPath;
        }

        #endregion SetCommonDirectoryPath

        #region GetCommonDirectoryPath

        /// <summary>
        /// 폴더 (디렉토리) 공통 경로 가져오기
        /// (응용 프로그램 - CobimExplorer.exe, Revit 애드인 프로그램 - CobimDockableDialogs.dll, AutoCAD 애드인 프로그램 - CobimPalette.dll)
        /// </summary>
        public string GetCommonDirectoryPath()
        {
            return _CommonDirectoryPath;
        }

        #endregion GetCommonDirectoryPath

        #region SetLogDirectoryPath

        /// <summary>
        /// 로그(Logs) 폴더(디렉토리) 경로 구한 후 값 셋팅
        /// </summary>
        public void SetLogDirectoryPath(string pLogDirectoryPath)
        {
            _LogDirectoryPath = pLogDirectoryPath;
        }

        #endregion SetLogDirectoryPath

        #region GetLogDirectoryPath

        /// <summary>
        /// 로그(Logs) 폴더(디렉토리) 경로 가져오기
        /// </summary>
        public string GetLogDirectoryPath()
        {
            return _LogDirectoryPath;
        }

        #endregion GetLogDirectoryPath

        #region Sample

        #endregion Sample
    }
}

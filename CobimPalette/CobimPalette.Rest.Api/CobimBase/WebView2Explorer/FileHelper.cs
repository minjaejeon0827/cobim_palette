using Serilog;

using System.IO;
using System.Reflection;
using MessageBox = System.Windows.MessageBox;

using CobimPalette.Data.Models.CobimBase.WebView2Explorer;
using CobimUtil;

namespace CobimPalette.Rest.Api.CobimBase.WebView2Explorer
{
    public class FileHelper
    {
        #region 프로퍼티

        /// <summary>
        /// 7. 파일 다운로드 (파일 다운로드 API)
        /// </summary>
        public const string requestFileDownLoadURL = "https://cobim.io/api/explorer/file/container/download/";

        #endregion 프로퍼티

        #region FileDownLoadName

        /// <summary>
        /// 새로 다운로드 받으려는 파일 이름(파일 전체 경로 - pDirPath + 파일이름 - fi.Name)이 기존에 다운로드 받은 파일(파일 전체 경로 - pDirPath + 파일이름 - fi.Name)과 이름 중복일 때 (N)으로 파일명 생성
        /// </summary>
        /// <param name="pDirPath"></param>
        /// <param name="pFileName"></param>
        /// <returns></returns>
        public static string FileDownLoadPath(string pDirPath, ContainerPack pContainerPack, string pFileName)
        {
            string containerId = string.Empty;    // 다운로드 받으려고 하는 파일의 상위 컨테이너 아이디 폴더
            string linkedContainerId = string.Empty;    // 연관 컨테이너 아이디
            string fileName = string.Empty;    // 파일 이름 (파일 전체 경로 + 파일이름 - fi.Name)
            string dirMapPath = string.Empty;    // 다운로드 받으려고 하는 파일 중복 여부 체크할 때 필요한 파일 경로 
            string downloadFilePath = string.Empty;    // 다운로드 받으려고 하는 최종 파일 경로 (파일 전체 경로 + 이름 중복일 때 변경된 파일 이름)

            // TODO : 로그 기록시 현재 실행 중인 메서드 위치 기록하기 (2023.10.10 jbh)
            // 참고 URL - https://slaner.tistory.com/73
            // 참고 2 URL - https://stackoverflow.com/questions/4132810/how-can-i-get-a-method-name-with-the-namespace-class-name
            // 참고 3 URL - https://stackoverflow.com/questions/44153/can-you-use-reflection-to-find-the-name-of-the-currently-executing-method
            var currentMethod = MethodBase.GetCurrentMethod();

            try
            {
                // TODO : 다운로드 받으려고 하는 파일의 상위 컨테이너 아이디 폴더 아이디 값(pContainerPack.containerId)을 string 클래스 객체 containerId에 할당 (2023.11.24 jbh)
                containerId = pContainerPack.GetContainerId();

                // TODO : 체크아웃 진행시 체크아웃 요청한 컨테이너 아이디와 연관된 컨테이너 아이디 문자열 값 추출 구현 (2023.12.15 jbh)
                linkedContainerId = pContainerPack.GetLinkedContainerId();

                // TODO : 다운로드 받을 파일명이 중복일 때, 기존 파일명 +N 으로 생성하도록 구현(2023.10.25 jbh)
                // 참고 URL - https://pseudo-code.tistory.com/166
                fileName = pFileName;

                if (fileName.Length > 0)
                {
                    int startIndex = 0;               // 파일이름 문자열 시작
                    int indexOfDot = fileName.LastIndexOf(".");
                    string fileNameWithoutExtension = fileName.Substring(startIndex, indexOfDot);  // 파일 이름 (파일 확장자 제외)
                    string extensionName = fileName.Substring(indexOfDot);                         // 파일 확장자 이름(.png, .jpg, .gif 등등...)

                    bool bExist = true;               // 중복된 파일 존재여부 확인
                    int fileCnt = 0;                  // 중복된 파일 다운로드시 파일 이름에 붙일 숫자(N) 

                    // string dirMapPath = string.Empty;    

                    while (bExist)
                    {
                        dirMapPath = pDirPath;           // 파일 전체 경로 - pDirPath 문자열 값을 string 클래스 객체 dirMapPath에 할당
                        string pathCombine = Path.Combine(dirMapPath, fileName);

                        // 해당 파일 경로에 똑같은 이름의 파일이 존재하는 경우 
                        if (true == File.Exists(pathCombine))
                        {
                            if (fileCnt == 0)
                            {
                                // 연관 컨테이너 아이디가 존재하는 경우 
                                if (false == string.IsNullOrWhiteSpace(linkedContainerId))
                                    MessageBox.Show($"대상 폴더 ({containerId})\r\n연관 컨테이너 아이디 ({linkedContainerId})\r\n동일 이름 파일 ({fileName}) 존재\r\n다운로드 받으시겠습니까?");
                                // 연관 컨테이너 아이디가 존재하지 않는 경우 
                                else MessageBox.Show($"대상 폴더 ({containerId})\r\n동일 이름 파일 ({fileName}) 존재\r\n다운로드 받으시겠습니까?");
                            }
                            fileCnt++;
                            // 기존 파일 + (N)으로 파일명 변경 
                            fileName = fileNameWithoutExtension + "(" + fileCnt + ")" + extensionName;
                        }
                        // 똑같은 이름의 파일이 존재하지 않는 경우 
                        else bExist = false;
                    }
                }

                downloadFilePath = Path.Combine(dirMapPath, fileName);
                return downloadFilePath;   // 다운로드 받으려고 하는 최종 파일 경로 (파일 전체 경로 + 이름 중복일 때 변경된 파일 이름) 리턴
                // return fileName;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                // TODO : 오류 발생시 상위 호출자 예외처리 전달 throw 구현 (2023.10.6 jbh)
                // 참고 URL - https://devlog.jwgo.kr/2009/11/27/thrownthrowex/
                throw;
            }
        }

        #endregion FileDownLoadName

        #region Sample

        #endregion Sample
    }
}

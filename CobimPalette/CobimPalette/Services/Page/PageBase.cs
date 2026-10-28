using Serilog;
using MessageBox = System.Windows.MessageBox;

namespace CobimPalette.Services.Page
{
    // TODO : 아래 유튜브 영상의 ViewModelBase.cs 클래스를 응용해서 PageBase.cs 클래스 구현 (2024.09.24 minjae)
    // 참고 유튜브 영상 URL -  https://youtu.be/2FPFgW0xVB0 (2024.09.24 minjae)

    // TODO : 필요시 바로 아래 주석친 코드처럼 (PageBase -> PageBase<TData>) 수정 예정 (2024.09.24 minjae)
    // public abstract class PageBase<TData> : PageBase {}

    // public abstract class PageBase : TestPageBase // Screen// , ICommand
    // public abstract class PageBase<TData> : PageBase

    // TODO : 제네릭 클래스 PageBase<TData>에 속하는 프로퍼티 "SelectedData"가 null로 할당 안 하고 객체 생성(new()) 해주기 위해서 소스코드 "where TData : new()" 추가 (2024.09.24 minjae)
    // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/csharp/programming-guide/generics/generic-classes

    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    [HiddenVm]
    public abstract class PageBase<TData> : PageBase where TData : new()
    {
        #region 프로퍼티

        // TODO : 데이터 타입 프로퍼티 "DataType" 구현 시 오류 발생
        //        오류 코드 "CS8370" - 오류 메시지 "'병합 할당' 기능(??= 연산)은 C# 7.3에서 사용할 수 없습니다. 8.0 이상의 언어 버전을 사용하세요."
        //        해당 오류 해결하기 위해 프로젝트 파일 "CobimPalette"의 언어 버전 (기존) 7.3 -> (변경) 9.0  변경 완료 (2024.10.11 minjae)
        // '병합 할당' 기능 (??=) 연산 
        // null 병합 연산자 ??는 null이 아닌 경우 왼쪽 피연산자의 값을 반환한다. 그렇지 않으면 오른쪽 피연산자를 평가하고 그 결과를 반환한다.
        // 참고 URL - https://developstudy.tistory.com/69
        // 참고 2 URL - https://velog.io/@jinuku/C-%EB%B0%8F-.-%EC%97%B0%EC%82%B0%EC%9E%90
        /// <summary>
        /// 데이터 타입
        /// </summary>
        public Type DataType => _DataType ??= typeof(TData);
        private Type _DataType;

        /// <summary>
        /// Rest API 입력 파라미터 데이터 객체 
        /// 체크아웃, 체크인, 다운로드, 작업취소 등등...
        ///// </summary>
        public TData InputParamData { get => _InputParamData; set { SetAndNotify(ref _InputParamData, value); } }
        private TData _InputParamData = new();

        // TODO : 추후 필요시 프로퍼티 "SelectedData" 싱글톤 객체로 구현 예정 (2024.09.24 minjae)
        // public TData SelectedData { get => _SelectedData; set { SetAndNotify(ref _SelectedData, value); } }
        // private TData _SelectedData = new();

        #endregion 프로퍼티

        #region 생성자

        public PageBase() : base()
        {
        }

        #endregion 생성자
    }

    [HiddenVm]
    public abstract class PageBase : CobimScreen
    {
        #region 프로퍼티 

        // TODO : ListView 데이터 로우 갯수 프로퍼티 "DisplaySearchCount" 필요시 사용 예정 (2024.10.11 minjae)
        /// <summary>
        /// ListView 데이터 로우 갯수 
        /// </summary>
        public int DisplaySearchCount { get => _DisplaySearchCount; set { _DisplaySearchCount = value; NotifyOfPropertyChange(); NotifyOfPropertyChange(nameof(DisplaySearchCountDesc)); } }
        private int _DisplaySearchCount = 0;

        // TODO : ListView 데이터 로우 갯수 Description 프로퍼티 "DisplaySearchCountDesc" 필요시 사용 예정 (2024.10.11 minjae)
        /// <summary>
        /// ListView 데이터 로우 갯수 Description
        /// </summary>
        public string DisplaySearchCountDesc => $"[{DisplaySearchCount.ToString("#,##0")}] 건 조회";

        // TODO : 즐겨찾기 - 북마크(노란색 별표) 프로퍼티 "IsBookMark" 필요시 사용 예정 (2024.09.24 minjae)
        /// <summary>
        /// 즐겨찾기 - 북마크
        /// </summary>
        public bool IsBookMark { get => _IsBookMark; set { _IsBookMark = value; NotifyOfPropertyChange(); } }
        private bool _IsBookMark = false;


        #endregion 프로퍼티

        #region 생성자

        // TODO :  필요시 아래 생성자에 주석 처리한 : base() 사용 예정 (2024.10.11 minjae)
        public PageBase() : base()
        {

        }

        #endregion 생성자

        #region 기본 메소드


        #endregion 기본 메소드    

        #region RegBookMarkAsync

        // TODO : 즐겨찾기 - 북마크 등록 메서드 "RegBookMarkAsync" 필요시 구현 예정 (2024.09.24 minjae)
        /// <summary>
        /// 즐겨찾기 - 북마크 등록 
        /// </summary>
        public async Task RegBookMarkAsync()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        #endregion RegBookMarkAsync
    }

    #region Sample

    #endregion Sample
}

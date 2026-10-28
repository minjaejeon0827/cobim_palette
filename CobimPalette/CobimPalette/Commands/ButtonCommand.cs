using System.Windows.Input;

using CobimPalette.Common.CobimBase;

namespace CobimPalette.Commands
{
    // TODO: RelayCommand 추후 로직 변경 예정 (2024.09.23 jbh)
    // 참고 URL - https://www.codeproject.com/Tips/813345/Basic-MVVM-and-ICommand-Usage-Example
    // 참고 2 URL - https://youtu.be/s7pt3EkDyq4
    // 참고 3 URL - https://blog.naver.com/PostView.naver?blogId=goldrushing&logNo=221243250136 
    // Command 참고 URL - https://blog.naver.com/goldrushing/221243250136
    public class ButtonCommand : ICommand
    {
        #region 프로퍼티 

        /// <summary>
        /// 메소드 타입
        /// </summary>
        public string _methodType { get; set; }

        // TODO : 비동기 메소드 + 반환 타입이 void인 메소드 둘 다 구별해서 실행해야할 경우 추후 Action<object> _executeMethod; 사용 예정 (2024.09.23 jbh)
        // Action은 반환 타입이 void인 메소드를 위해 특별히 설계된 제네릭 델리게이트 의미
        // 참고 URL - https://blog.joe-brothers.com/csharp-delegate-func-action/
        Action<object> _executeVoidMethod;            // 반환 타입 void 메소드

        Func<object, Task> _executeAsyncMethod;   // 비동기 메소드

        Func<object, bool> _canexecuteMethod;

        // TODO : 추후 필요 시 이벤트 핸들러 "CanExecuteChanged" 사용 예정(2024.09.23 jbh)
        public event EventHandler CanExecuteChanged;

        #endregion 프로퍼티

        #region 생성자

        // TODO : 비동기 메소드 + 반환 타입이 void인 메소드 둘 다 구별해서 실행해야할 경우 추후 아래  반환 타입이 void인 메소드와 바인딩하는 "ButtonCommand" 생성자 사용 예정 (2024.09.23 jbh)
        // 반환 타입이 void인 메소드와 바인딩하는 "ButtonCommand" 생성자
        public ButtonCommand(string voidMethodType, Action<object> executeMethod, Func<object, bool> canexecuteMethod)
        {
            this._methodType = voidMethodType;
            this._executeVoidMethod = executeMethod;
            this._canexecuteMethod = canexecuteMethod;
        }

        // 비동기 메소드(async)와 바인딩하는 "ButtonCommand" 생성자
        public ButtonCommand(string asyncMethodType, Func<object, Task> executeAsyncMethod, Func<object, bool> canexecuteMethod)
        {
            this._methodType = asyncMethodType;
            this._executeAsyncMethod = executeAsyncMethod;
            this._canexecuteMethod = canexecuteMethod;
        }

        #endregion 생성자

        #region 기본 메소드

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            // 비동기 메소드인 경우 
            if (_methodType.Equals(CobimHelper.AsyncMethodType)) _executeAsyncMethod(parameter);

            // 반환 타입이 void인 메소드인 경우
            else _executeVoidMethod(parameter);
        }

        #endregion 기본 메소드
    }

    #region Sample

    #endregion Sample
}

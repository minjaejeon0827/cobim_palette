using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using TextBox = System.Windows.Controls.TextBox;

namespace CobimPalette.Utils.Text
{
    // TODO : Attached Property 기능 "TextBoxHelper.cs" 구현 (2024.10.10 jbh)
    // 유튜브 참고 URL - https://youtu.be/An7kwDYt3OQ
    public class TextBoxHelper
    {
        /// <summary>
        /// 콜백(CallBack) 메서드 OnUseOnPropertyChanged - 이벤트 메서드 "TextBox_TextChanged" 실행하는 기능 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnUseOnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            // TODO : if 조건절에 == null 보다 빠른 is null 연산자 사용 (2024.10.10 jbh)
            // 참고 URL - https://husk321.tistory.com/405
            if (textBox is null) return;

            // 이벤트 메서드 "TextBox_TextChanged" 가입
            if ((bool)e.NewValue) textBox.TextChanged += TextBox_TextChanged;

            // 이벤트 메서드 "TextBox_TextChanged" 해제
            else textBox.TextChanged -= TextBox_TextChanged;
        }

        /// <summary>
        /// TextChanged 이벤트 메서드 
        /// TextBox에 Text가 변경될 때마다 바인딩된 프로퍼티(속성)을 계속적으로 업데이트 해주는 기능
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpression?.UpdateSource();
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        // 로그인 화면 뷰(LoginV.xaml), 체크인 화면 뷰(CheckInPartV.xaml)
        // -> 아이디 <TextBox> 영역에 프로퍼티 (utilsText:TextBoxHelper.UseOnPropertyChanged="True") 추가
        // -> 프로그램 실행 -> 로그인 화면 뷰(LoginV.xaml)에서 아이디 텍스트 문자열 입력시 
        // -> TextBoxHelper.cs 소스파일 이동 -> 의존 프로퍼티 "UseOnPropertyChangedProperty" -> 메서드 " DependencyProperty.RegisterAttached" 실행
        // -> 해당 메서드 파라미터 "new PropertyMetadata(false, OnUseOnPropertyChanged)" 안에 false 값이 -> true로 변경 -> 콜백(CallBack) 메서드 "OnUseOnPropertyChanged" 실행 
        public static readonly DependencyProperty UseOnPropertyChangedProperty =
            DependencyProperty.RegisterAttached("UseOnPropertyChanged", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, OnUseOnPropertyChanged));

        public static bool GetUseOnPropertyChanged(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseOnPropertyChangedProperty);
        }

        public static void SetUseOnPropertyChanged(DependencyObject obj, bool value)
        {
            obj.SetValue(UseOnPropertyChangedProperty, value);
        }
    }

    #region Sample

    #endregion Sample
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tyxy
{
    /// <summary>
    /// MyCheckbox.xaml 的交互逻辑
    /// </summary>
    public partial class MyCheckbox : UserControl
    {
        public MyCheckbox()
        {
            InitializeComponent();
        }
        //private bool _check { get; set; }
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(MyCheckbox), new PropertyMetadata(false, OnIsCheckedChanged));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MyCheckbox;
            if (control != null)
            {
                bool isChecked = (bool)e.NewValue;
                if (isChecked)
                {
                    Storyboard storyboard1 = (Storyboard)control.Resources["Storyboard1"];
                    storyboard1.Begin();
                }
                else
                {
                    Storyboard storyboard2 = (Storyboard)control.Resources["Storyboard2"];
                    storyboard2.Begin();
                }
            }
        }
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        // P/Invoke 声明
        const int GWL_STYLE = -16;
        const int WS_OVERLAPPEDWINDOW = 0x00CF0000;

        void ShowConsole()
        {
            IntPtr hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, SW_SHOW);
                Window parentWindow = Window.GetWindow(this);
                parentWindow.Focus();
            }
        }
        static void HideConsole()
        {
            IntPtr hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, SW_HIDE);
            }
        }
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;
            if (IsChecked)
            {
                ShowConsole();
            }
            else
            {
                HideConsole();
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HideConsole();
        }
    }
}

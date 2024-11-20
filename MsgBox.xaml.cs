using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tyxy
{
    /// <summary>
    /// MsgBox.xaml 的交互逻辑
    /// </summary>
    public partial class MsgBox : Window
    {
        // 添加属性以便在关闭窗口时返回用户的选择
        public bool Result { get; private set; } = false;

        public MsgBox(string title, string msg)
        {
            InitializeComponent();
            Lable_title.Content = title;
            Lable_msg.Content = msg;
        }

        // 添加事件处理程序以响应"是"按钮的点击事件
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        // 添加事件处理程序以响应"否"按钮的点击事件
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }


    }
}

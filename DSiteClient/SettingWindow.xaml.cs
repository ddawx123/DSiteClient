using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DSiteClient
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public SettingWindow()
        {
            InitializeComponent();
        }

        public bool isFirstRun()
        {
            if (!File.Exists(System.Windows.Forms.Application.StartupPath + @"\configure.json"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isFirstRun())
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
                label_notice.Content = "首次运行必须配置应用属性（点此退出）";
            }
            else
            {
                FileStream fs = new FileStream(System.Windows.Forms.Application.StartupPath + @"\configure.json", FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(fs);
                string confstr = sr.ReadToEnd();
                if (String.IsNullOrEmpty(confstr))
                {
                    System.Windows.MessageBox.Show("配置文件异常，请重新设置应用属性！", "配置文件异常", MessageBoxButton.OK, MessageBoxImage.Error);
                    SettingWindow config_form = new SettingWindow();
                    config_form.ShowDialog();
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(confstr);
                textDBSrv.Text = jo["sqlsrv"].ToString();
                textDBUsr.Text = jo["sqlusr"].ToString();
                textDBPwd.Password = jo["sqlpwd"].ToString();
                textDBName.Text = jo["sqldbn"].ToString();
                textSSOUrl.Text = jo["ssoUrl"].ToString();
                fs.Close();
                fs.Dispose();
                sr.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(textDBSrv.Text) || String.IsNullOrEmpty(textDBUsr.Text) || String.IsNullOrEmpty(Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(textDBPwd.SecurePassword))) || String.IsNullOrEmpty(textDBName.Text) || String.IsNullOrEmpty(textSSOUrl.Text))
            {
                MessageBox.Show("抱歉，请填完所有属性项目后再次尝试应用。","警告",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                return;
            }
            string json_configure_text = "{\"ssoUrl\":\"" + textSSOUrl.Text + "\",\"sqlsrv\":\"" + textDBSrv.Text + "\",\"sqlusr\":\"" + textDBUsr.Text + "\",\"sqlpwd\":\"" + Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(textDBPwd.SecurePassword)) + "\",\"sqldbn\":\"" + textDBName.Text + "\"}";
            File.WriteAllText(System.Windows.Forms.Application.StartupPath + @"\configure.json", json_configure_text);
            MessageBox.Show("设置保存成功！","系统消息",MessageBoxButton.OK,MessageBoxImage.Information);
            Close();
        }

        private void label_notice_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

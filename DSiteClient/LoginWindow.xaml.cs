using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Interop;

namespace DSiteClient
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string login_apiurl;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(MainWindow.Sec.token))
            {
                //System.Windows.Application.Current.MainWindow.Close();
                System.Windows.Application.Current.Shutdown(); //强制卸载对象
            } 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = textUserName.Text;
            string password = Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(textPassword.SecurePassword));
            DoLogin(username, password);
        }

        //  <summary>
        /// 小丁工作室统一身份认证平台登录模型构造
        /// </summary>
        /// <param name="username">用户账号</param>
        /// <param name="password">用户密码</param>
        /// <param name="context">会话句柄</param>
        /// <returns>json格式的字符串</returns>
        public void DoLogin(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                System.Windows.MessageBox.Show("用户账号或密码不能为空，请补全后再次尝试。", "系统警告",MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(login_apiurl);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.UTF8.GetByteCount("username=" + username + "&userpwd=" + password + "&cors_domain=localapp");
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                String post_data = "username=" + username + "&userpwd=" + password + "&cors_domain=localapp";
                using (StreamWriter dataStream = new StreamWriter(request.GetRequestStream()))
                {
                    dataStream.Write(post_data);
                    dataStream.Close();
                    //Fetch Response Content
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //设置UTF-8为默认编码  
                    //142
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                JObject jo = (JObject)JsonConvert.DeserializeObject(retString);
                string login_result = jo["data"]["authcode"].ToString();
                string requestId = jo["requestID"].ToString();
                if (login_result.Equals("1"))
                {
                    MainWindow.Sec.token = requestId;
                    System.Windows.Application.Current.MainWindow.Show();
                    System.Windows.Application.Current.MainWindow.Title = "小丁工作室-个人网站信息管理系统";
                    this.Close();
                }
                else
                {
                    textPassword.Clear();
                    System.Windows.MessageBox.Show("远程服务器反馈用户账号或密码不正确，请更正后再次尝试。", "凭据无效", MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(!File.Exists(System.Windows.Forms.Application.StartupPath + @"\configure.json"))
            {
                SettingWindow config_form = new SettingWindow();
                config_form.ShowDialog();
            }
            else
            {
                System.Windows.Application.Current.MainWindow.Hide();
                FileStream fs = new FileStream(System.Windows.Forms.Application.StartupPath + @"\configure.json", FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(fs);
                string confstr = sr.ReadToEnd();
                if(String.IsNullOrEmpty(confstr))
                {
                    System.Windows.MessageBox.Show("配置文件异常，请重新设置应用属性！", "配置文件异常", MessageBoxButton.OK, MessageBoxImage.Error);
                    SettingWindow config_form = new SettingWindow();
                    config_form.ShowDialog();
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(confstr);
                login_apiurl = jo["ssoUrl"].ToString();
                if(String.IsNullOrEmpty(login_apiurl))
                {
                    System.Windows.MessageBox.Show("认证服务器信息读取异常，请重新设置应用属性！", "配置文件异常", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                textUserName.Focus();
                fs.Close();
                fs.Dispose();
                sr.Close();
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow config_form = new SettingWindow();
            config_form.ShowDialog();
        }
    }
}

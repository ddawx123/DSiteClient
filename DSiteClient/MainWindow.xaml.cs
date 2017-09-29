using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DSiteClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
        public static class db
        {
            public static FileStream fs = new FileStream(System.Windows.Forms.Application.StartupPath + @"\configure.json", FileMode.OpenOrCreate);
            public static StreamReader sr = new StreamReader(fs);
            public static string confstr = sr.ReadToEnd();
            public static JObject jo = (JObject)JsonConvert.DeserializeObject(confstr);
            public static string connStr = "Server=" + jo["sqlsrv"].ToString() + ";Database=" + jo["sqldbn"].ToString() + ";uid=" + jo["sqlusr"].ToString() + ";pwd=" + jo["sqlpwd"].ToString() + ";";
            //fs.Close();
            //fs.Dispose();
            //sr.Close();
        }
        */

        SqlConnection sqlcon = new SqlConnection("Server=localarea.dingstudio.cn;Database=labindex;uid=usr1;pwd=123@abc;"); //创建一个新的数据库连接会话
        SqlCommand sqlcmd; //构造一个空的数据库语句模型
        SqlDataAdapter sqlda; //构造一个全新的数据库适配器用于后期处理数据
        DataSet ds; //构造一个数据对象存储用于后期拉取数据

        public static class Sec
        {
            public static string token;
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Sec.token))
            {
                LoginWindow login_form = new LoginWindow();
                Title = "【正在等待用户响应】小丁工作室-个人网站信息管理系统";
                login_form.ShowDialog();
            }
            //sqlcmd = new SqlCommand("select * from siteform", sqlcon);
            LoadData();
            Title = "小丁工作室-个人网站信息管理系统";
        }

        public void LoadData()
        {
            Title = "【正在等待服务器响应】小丁工作室-个人网站信息管理系统";
            //拉取站点
            sqlda = new SqlDataAdapter("select * from siteform", sqlcon);
            ds = new DataSet();
            sqlda.Fill(ds);
            dg_siteListBox.ItemsSource = ds.Tables[0].DefaultView;
            dg_siteListBox.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid_LoadingRow);
            //拉取留言
            sqlda = new SqlDataAdapter("select * from msgform", sqlcon);
            ds = new DataSet();
            sqlda.Fill(ds);
            dg_msgListBox.ItemsSource = ds.Tables[0].DefaultView;
            dg_msgListBox.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid_LoadingRow);
        }

        public void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            Title = "小丁工作室-个人网站信息管理系统";
        }

        private void btnReLock_Click(object sender, RoutedEventArgs e)
        {
            Sec.token = "";
            LoginWindow login_form = new LoginWindow();
            Title = "【正在等待用户响应】小丁工作室-个人网站信息管理系统";
            login_form.ShowDialog();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
//using System.Data.Objects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using System.Windows.Interop;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using FirstFloor.ModernUI.Windows.Controls;
using System.Runtime.InteropServices;

namespace Manage_your_Life
{
    /// <summary>
    /// DataGridRowEdit.xaml の相互作用ロジック
    /// </summary>
    public partial class DataGridRowEdit : Window
    {

        /// <summary>
        /// Window間データ共有用クラス
        /// </summary>
        DataBanker context;


        public DataGridRowEdit()
        {
            InitializeComponent();

            context = DataBanker.GetInstance();
        }



        /// <summary>
        /// 文字列がTimeSpanに変換出来るか
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Boolean IsTimeSpanFormat(string value)
        {
            try
            {
                TimeSpan.Parse(value);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// フォームの値でデータベースを更新
        /// </summary>
        /// <param name="appId">更新するレコードのID</param>
        private void UpdataDatabase(int appId)
        {
            //データベース接続
            string basePath = Directory.GetCurrentDirectory() + @"\ApplicationDatabase.mdf";
            string connStr = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=""" + basePath + @""";Integrated Security=True";
            ApplicationDataClassesDataContext database = null;

            try
            {
                database = new ApplicationDataClassesDataContext(connStr);

                var q =
                    from p in database.DatabaseApplication
                    where p.Id == appId
                    select p;

                foreach (var r in q)
                {
                    r.Title = this.textBox_Title.Text;
                    r.DatabaseDate.UsageTime = this.textBox_UsageTime.Text;
                    r.Memo = this.textBox_Memo.Text;
                }

                database.SubmitChanges();

                MessageBox.Show("データベースが更新されました。", "情報"
                    ,MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                database.Dispose();
            }            
        }


//-----------------------------------------------------イベントハンドラ

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //データを取得する
            this.textBox_Title.Text = (string)context["Title"];
            this.textbox_ProcName.Text = (string)context["ProcName"];
            this.textbox_Path.Text = (string)context["ProcPath"];
            this.textBox_Memo.Text = (string)context["Memo"];
            this.label_AddDate.Content = (DateTime)context["AddDate"];
            this.label_LastDate.Content = (DateTime)context["LastDate"];            
            this.textBox_UsageTime.Text = (string)context["UsageTime"];            
        }


        //保存ボタン押下
        private void button_Save_Click(object sender, RoutedEventArgs e)
        {
            //使用時間の構文チェック
            if (IsTimeSpanFormat(this.textBox_UsageTime.Text))
            {
                MessageBoxResult result = MessageBox.Show("本当に保存しますか?", "確認",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.Yes)
                {
                    int appId = (int)context["Id"];
                    UpdataDatabase(appId);

                    DialogResult = true;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("使用時間の構文エラーです。", "エラー", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }         
        }


        //キャンセルボタン押下
        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }



//-----------------------------------------------------

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// ウィンドウをドラッグで動かせるようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;            
            PostMessage(hwnd, 0xA1, (IntPtr)2, IntPtr.Zero);
        }

       
    }
}

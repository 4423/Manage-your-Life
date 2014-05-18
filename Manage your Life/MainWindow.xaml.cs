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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Manage_your_Life
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region メンバ
        /// <summary>
        /// 前回のProcess
        /// </summary>
        Process previousProcess;

        /// <summary>
        /// WinAPIを叩くProcessInformationクラスのインスタンス
        /// </summary>
        ProcessInformation pInfo;

        /// <summary>
        /// 登録アプリ同士の計測スルーバグ回避用
        /// </summary>
        bool preTitleCheck = false;

        #endregion



        public MainWindow()
        {
            InitializeComponent();

            pInfo = new ProcessInformation();           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DatabaseOperation(pInfo.GetActiveProcess());
        }


        internal void DatabaseOperation(Process proc)
        {
            //TODO 接続文字列。環境によって変更可にしよう
            string baseDir = @"""c:\users\yuhi\documents\visual studio 2013\Projects\2nd\Manage your Life\Manage your Life\ApplicationDatabase.mdf""";
            string connStr = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + baseDir + ";Integrated Security=True";

            using (var db = new ApplicationDataClassesDataContext(connStr))
            {
                //既に存在しているか？
                //TODO ここでは実行ファイルのパスで判別しているが…要検討
                //see: http://bit.ly/1jAFEn3
                bool exist = db.DatabaseProcess.Any(p => p.Path.Contains(proc.MainModule.FileName));
                
				//存在していなければ新規にデータを挿入
                if (!exist)
                {
                    DatabaseApplication app = new DatabaseApplication();
                    app.Title = proc.MainWindowTitle;
                    db.DatabaseApplication.InsertOnSubmit(app);
                    db.SubmitChanges();

                    //SubmitChanges()すると挿入したIDが取得できるようになる
                    //see: http://bluestick.jp/tech/index.php/archives/50
                    var id = app.Id;

                    DatabaseProcess pro = new DatabaseProcess();
                    pro.AppId = id;
                    pro.Name = proc.ProcessName;
                    pro.Path = proc.MainModule.FileName;
                    db.DatabaseProcess.InsertOnSubmit(pro);

                    DatabaseDate date = new DatabaseDate();
                    date.AppId = id;
                    date.AddDate = date.LastDate = DateTime.Now;
                    //date.UsageTime = DateTime.Parse("0:00:00:00");
                    db.DatabaseDate.InsertOnSubmit(date);                    

                    db.SubmitChanges();
                }
            }

        }
    }
}

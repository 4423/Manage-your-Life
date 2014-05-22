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
using System.Windows.Threading;
using System.IO;

namespace Manage_your_Life
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region メンバ
        /// <summary>
        /// WPFでタイマーを使う
        /// </summary>
        DispatcherTimer timer;

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
            //タイマーの作成
            timer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(DispatcherTimer_Tick);
            
            //タイマーの実行開始
            timer.Start();
            
            DatabaseOperation(pInfo.GetActiveProcess());
        }


        /// <summary>
        /// タイマー間隔が経過すると呼び出される
        /// </summary>
        /// <see cref="http://ari-it.doorblog.jp/archives/28684231.html"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            //一時的にタイマー停止(処理に時間がかかるかも)
            timer.Stop();

            //アクティブプロセス取得
            Process activeProcess = pInfo.GetActiveProcess();

            //初回起動などnullだったら現在のプロセスを代入
            if (previousProcess == null) previousProcess = activeProcess;

            //前回と同じプロセス名だったら何もしない
            //TODO ブラウザ等はページ遷移毎に処理されない！！
            if ((activeProcess.ProcessName == previousProcess.ProcessName) && !preTitleCheck)
            {
                timer.Start();
                return;
            }

            //TODO 最前面のアプリケーションが変わった時にしたい処理
            Hoge(activeProcess);


            //キャッシュ
            previousProcess = activeProcess;
            timer.Start();
        }


        private void Hoge(Process activeProcess)
        {
            statusBarItem_label.Content = activeProcess.MainWindowTitle;

        }




        /// <summary>
        /// 新たなプログラム情報をデータベースに追記する
        /// </summary>
        /// <param name="proc"></param>
        internal void DatabaseOperation(Process proc)
        {
            string basePath = Directory.GetCurrentDirectory() + @"\ApplicationDatabase.mdf";
            string connStr = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=""" + basePath + @""";Integrated Security=True";

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

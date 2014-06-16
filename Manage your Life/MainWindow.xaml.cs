using System;
using System.Collections.Generic;
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
        /// 最初に最前面になった時の日時
        /// </summary>
        DateTime firstActiveDate;

        /// <summary>
        /// データベースを操作
        /// </summary>
        DatabaseOperation dbOperator;

        TimeUtility timeUtil;

        /// <summary>
        /// 登録アプリ同士の計測スルーバグ回避用
        /// </summary>
        bool preTitleCheck = false; //TODO 改名したいけど何やってるのか分からない

        /// <summary>
        /// アプリケーションが最前面から外れた時の検出
        /// false: 最前面
        /// true: 背面(最前面から外れた初回)
        /// </summary>
        bool isRearApplication = false;


        #endregion


        public MainWindow()
        {
            InitializeComponent();

            pInfo = new ProcessInformation();
            dbOperator = new DatabaseOperation();
            timeUtil = new TimeUtility();

            //タイマーの作成
            timer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(DispatcherTimer_Tick);
            //タイマーの実行開始
            timer.Start();            
        }


        /// <summary>
        /// タイマー間隔が経過すると呼び出される
        /// </summary>
        /// <see cref="http://ari-it.doorblog.jp/archives/28684231.html"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
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

            //最前面のアプリケーションが変わった時にしたい処理
            ApplicationChanged(activeProcess);
            
            //キャッシュ
            previousProcess = activeProcess;
            timer.Start();
        }



        /// <summary>
        /// 最前面のアプリケーションが変わった時にする処理
        /// </summary>
        /// <param name="activeProcess">新たな最前面のProcess</param>
        private void ApplicationChanged(Process activeProcess)
        {
            //最初に最前面になった時
            if (!isRearApplication)
            {
                //DBに存在していなければ新規にデータ挿入
                if (!dbOperator.IsExist(activeProcess))
                {
                    dbOperator.Register(activeProcess);
                    SetDataGrid();
                }

                //最初にアクティブになった時間を取得
                firstActiveDate = DateTime.Now;

                isRearApplication = true;
                preTitleCheck = false;
            }
            else //最前面解除
            {
                //計測時間追記の為にDBから該当Idを取得
                int appId = dbOperator.GetCorrespondingAppId(previousProcess);

                //DBから使用時間を取得し、今回の使用時間を加算してDB更新
                var activeInterval = timeUtil.GetInterval(firstActiveDate);
                TimeSpan usageTime = dbOperator.UpdateUsageTime(appId, activeInterval);

                //TODO 更新された時間のDataGridへの表示処理

                isRearApplication = false;
                preTitleCheck = true;
            }

            statusBarItem_label.Content = activeProcess.MainWindowTitle;
        }




        private void SetDataGrid()
        {
            dataGrid1.ItemsSource = null;

          
            dataGrid1.ItemsSource = dbOperator.GetAllData();
            //TODO DGVへの登録はDB登録毎に1つずつ行って、プレ版同様使用中Appを選択→使用後にそこのみ値を変更の方が良いかも

        }


//-----------------------------------------------------------------イベントハンドラ

        //DataGridの選択行が変更された時
        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listView1.Items.Clear();

            try
            {
                //選択された行のデータを取得
                int selectedIndex = dataGrid1.SelectedIndex;
                var row = dataGrid1.Items[selectedIndex];

                //上のItemsより生成するObjectのプロパティの中からパスの値を取り出す
                string procPath = row.GetType().GetProperty("ProcPath").GetValue(row).ToString();

                //パスからアイコン生成
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(procPath);

                //see: http://bit.ly/1i44IJo Icon を ImageSource に変換する 
                iconImage.Source = Imaging.
                    CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ex)
            {
                //何もしないんだよ
            }
        }


        private void DeleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ref_Click(object sender, RoutedEventArgs e)
        {
            SetDataGrid();
        }


        //Datagridの列非表示
        //see: http://ameblo.jp/shirokoma55/entry-11561024241.html
        //dataGrid1.ColumnFromDisplayIndex(0).Visibility = Visibility.Collapsed;

        //Listviewへのバインディングで参考になりそう
        //see: http://gushwell.ldblog.jp/archives/52333865.html
    }
}

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

namespace Manage_your_Life
{

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : ModernWindow
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
            dbOperator = DatabaseOperation.Instance;
            timeUtil = new TimeUtility();

            //イベントハンドラの追加
            dbOperator.TimelineLog_Updated += new EventHandler(this.TimelineLog_Changed);
            

            //タイマーの作成
            timer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(DispatcherTimer_Tick);
            //タイマーの実行開始
            timer.Start();
        }


        /// <summary>
        /// タイマー指定時間が経過すると呼び出される
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
            //DrawChart();
            SetDataGrid();

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
                }

                //最初にアクティブになった時間を取得
                firstActiveDate = DateTime.Now;

                isRearApplication = true;
                preTitleCheck = false;
            }
            else //最前面解除
            {
                //計測時間追記の為にDBから該当Idを取得
                int appId = dbOperator.GetCorrespondingAppId(previousProcess.MainModule.FileName);

                //DBから使用時間を取得し、今回の使用時間を加算してDB更新
                var activeInterval = timeUtil.GetInterval(firstActiveDate);
                TimeSpan usageTime = dbOperator.UpdateUsageTime(appId, activeInterval);

                //TODO 更新された時間のDataGridへの表示処理

                isRearApplication = false;
                preTitleCheck = true;
            }
        }



        //CAUTION レコード新規登録時にするSetDataGridはイベントとして実装App.xaml側で実装した
        private void SetDataGrid()
        {

            //pageApplication.dataGrid1.ItemsSource = null;
            //pageApplication.dataGrid1.ItemsSource = dbOperator.GetAllData();

            //pageApplication.SetDataGrid(dbOperator.GetAllData());

            //dataGrid1.ItemsSource = null;


            //dataGrid1.ItemsSource = dbOperator.GetAllData();
            //TODO DGVへの登録はDB登録毎に1つずつ行って、プレ版同様使用中Appを選択→使用後にそこのみ値を変更の方が良いかも

        }


        //-----------------------------------------------------------------イベントハンドラ


        /// <summary>
        /// TimelineDBに変更を加えられた時の通知を受け取る(自作)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimelineLog_Changed(object sender, System.EventArgs e)
        {
            
        }

    }      
}
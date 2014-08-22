using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Manage_your_Life
{

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        #region Field
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

        /// <summary>
        /// 登録アプリ同士の計測スルーバグ回避用
        /// </summary>
        bool preTitleCheck = false;

        /// <summary>
        /// アプリケーションが最前面から外れた時の検出
        /// false: 最前面
        /// true: 背面(最前面から外れた初回)
        /// </summary>
        bool isRearApplication = false;

        /// <summary>
        /// バルーン通知
        /// </summary>
        private NotifyIcon notifyIcon;
        
        /// <summary>
        /// 使いすぎ警告の対象Dictionary
        /// </summary>
        Dictionary<int, TimeSpan> overuseWarningItems;

        DataBanker dataBanker;

        /// <summary>
        /// プロセスが死ぬ前に予め今回のFileNameをとっておく
        /// </summary>
        string previousProcessFileName = "";
        string previousProcessName = "";

        #endregion

        
        public MainWindow()
        {
            InitializeComponent();
            
            pInfo = new ProcessInformation();
            dbOperator = DatabaseOperation.Instance;
            overuseWarningItems = dbOperator.GetOveruseWarningCollection();
            dataBanker = DataBanker.Instance;
            dataBanker["WarningNotAgain"] = new List<int>();
            dataBanker["WarningCount"] = 0;

            //バルーン通知の設定
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = "Manage your Life";
            notifyIcon.Icon = Properties.Resources.originalIconTray;
            notifyIcon.Visible = true;
            //コンテキストメニュー追加
            ContextMenuStrip menuStrip = new ContextMenuStrip();
            ToolStripMenuItem exitItem = new ToolStripMenuItem();
            ToolStripMenuItem openItem = new ToolStripMenuItem();
            exitItem.Text = "終了(&E)";
            openItem.Text = "開く(&O)";
            menuStrip.Items.Add(openItem);
            menuStrip.Items.Add(new ToolStripSeparator());
            menuStrip.Items.Add(exitItem);
            exitItem.Click += new EventHandler(exitItem_Click);
            openItem.Click += new EventHandler(openItem_Click);

            notifyIcon.ContextMenuStrip = menuStrip;            
            notifyIcon.MouseDoubleClick += new MouseEventHandler(notifyIcon_MouseDoubleClick);


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
                string activeProcessFileName = "";
                try
                {
                    activeProcessFileName = activeProcess.MainModule.FileName;
                }
                catch (System.ComponentModel.Win32Exception ex) { }
                catch (Exception ex) { }

                //フラグが変わらないので、新しいプロセスが前面になった時から正常に時間計算可能
                if (activeProcessFileName == "") return;


                //DBに存在していなければ新規にデータ挿入
                if (!dbOperator.IsExist(activeProcessFileName))
                {
                    dbOperator.Register(activeProcess);
                }

                //使用時間の警告
                int appId = dbOperator.GetCorrespondingAppId(activeProcessFileName);
                DoOveruseWarining(appId, activeProcess.ProcessName);
                

                //最初にアクティブになった時間を取得
                firstActiveDate = DateTime.Now;

                previousProcessFileName = activeProcess.MainModule.FileName;
                previousProcessName = activeProcess.ProcessName;
                isRearApplication = true;
                preTitleCheck = false;
            }
            //最前面解除
            else 
            {
                //計測時間追記の為にDBから該当Idを取得
                int appId = dbOperator.GetCorrespondingAppId(previousProcessFileName);

                //DBから使用時間を取得し、今回の使用時間を加算してDB更新
                var activeInterval = Utility.GetInterval(firstActiveDate);
                dbOperator.UpdateUsageTime(appId, activeInterval);

                //バルーンで通知
                ShowBalloonTip(activeInterval, previousProcessName);                

                isRearApplication = false;
                preTitleCheck = true;
            }            
        }


        /// <summary>
        /// 使用時間の警告を実行する
        /// </summary>
        /// <param name="appId"></param>
        private void DoOveruseWarining(int appId, string processName)
        {
            //警告機能がオフであれば
            if (!Properties.Settings.Default.checkBox_IsOveruseWarining) return;
            //警告対象に現在のIDが含まれていなければ
            if (!overuseWarningItems.ContainsKey(appId)) return;
            //もう警告しない対象に含まれていれば
            if (((List<int>)dataBanker["WarningNotAgain"]).Contains(appId) == true) return;

            //今日の使用時間と警告時間を取得
            TimeSpan warningTime = overuseWarningItems[appId];
            TimeSpan todayUsageTime = dbOperator.GetTodayPrpcessUsageTime(appId);

            //今日の使用時間が警告時間よりも大きい場合
            if (todayUsageTime > warningTime)
            {   
                DoWarning window = new DoWarning(processName, appId, warningTime);
                window.ShowDialog();
                dataBanker["WarningCount"] = (int)dataBanker["WarningCount"] + 1;
            }           
        }


        /// <summary>
        /// 今まで最前面にあったプロセスの使用時間を通知する
        /// </summary>
        /// <param name="activeInterval">今回の使用時間</param>
        private void ShowBalloonTip(TimeSpan activeInterval, string procName)
        {
            if (Properties.Settings.Default.checkBox_IsBalloonEnable)
            {
                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon.BalloonTipTitle = "\"" + procName + "\"" + "の計測終了";
                notifyIcon.BalloonTipText = "使用時間: " + activeInterval.ToString(@"hh\:mm\:ss");
                notifyIcon.ShowBalloonTip(1000);
            }
        }



//-----------------------------------------------ウィンドウボタン

        //最小化ボタン押下
        private void ModernWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
                notifyIcon.Visible = true;
            }
        }

        //閉じるボタン押下
        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowClosingProcess();
        }

        
        /// <summary>
        /// ウィンドウを復元する
        /// </summary>
        private void WindowOpen()
        {
            if (!this.IsVisible) this.Show();

            this.WindowState = System.Windows.WindowState.Normal;
            this.Activate();
            this.Focus();
        }


        /// <summary>
        /// ウィンドウを閉じる時の処理
        /// </summary>
        private void WindowClosingProcess()
        {
            timer.Stop();
            this.Hide();
            notifyIcon.Dispose();

            Properties.Settings.Default.Save();

            //TodayReportの表示
            if (Properties.Settings.Default.checkBox_IsReportEnable)
            {
                TodayReport window = new TodayReport();
                window.ShowDialog();
            }
        }

//-----------------------------------------------通知領域

        //アイコンダブルクリックで復元
        void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WindowOpen();
            }
        }


        //メニューの終了を選択
        private void exitItem_Click(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            WindowClosingProcess();

            System.Windows.Application.Current.Shutdown();
        }


        //メニューの開くを選択
        private void openItem_Click(object sender, EventArgs e)
        {
            WindowOpen();
        }


    }      
}
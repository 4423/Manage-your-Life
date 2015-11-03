using FirstFloor.ModernUI.Windows.Controls;
using Manage_your_Life.Models;
using Manage_your_Life.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Manage_your_Life.Views
{

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        #region Field

        private ActiveProcessMonitor procMonitor;
        private ProcessCache previousProcess = new ProcessCache();

        /// <summary>
        /// 最初に最前面になった時の日時
        /// </summary>
        DateTime firstActiveDate;

        /// <summary>
        /// データベースを操作
        /// </summary>
        DatabaseOperation dbOperator;
        
        /// <summary>
        /// バルーン通知
        /// </summary>
        private NotifyIcon notifyIcon;
        
        /// <summary>
        /// 使いすぎ警告の対象Dictionary
        /// </summary>
        Dictionary<int, TimeSpan> overuseWarningItems;

        DataBanker dataBanker;

        #endregion

        
        public MainWindow()
        {
            InitializeComponent();
            
            dbOperator = DatabaseOperation.Instance;
            overuseWarningItems = dbOperator.GetOveruseWarningCollection();
            dataBanker = DataBanker.Instance;
            dataBanker["WarningNotAgain"] = new List<int>();
            dataBanker["WarningCount"] = 0;

            //コンテキストメニュー追加            
            ToolStripMenuItem exitItem = new ToolStripMenuItem() { Text = "終了(&E)" };
            ToolStripMenuItem openItem = new ToolStripMenuItem() { Text = "開く(&O)" };
            exitItem.Click += new EventHandler(exitItem_Click);
            openItem.Click += new EventHandler(openItem_Click);

            ContextMenuStrip menuStrip = new ContextMenuStrip();
            menuStrip.Items.Add(openItem);
            menuStrip.Items.Add(new ToolStripSeparator());
            menuStrip.Items.Add(exitItem);            

            //バルーン通知の設定
            notifyIcon = new NotifyIcon()
            {
                Text = "Manage your Life",
                Icon = Properties.Resources.originalIconTray,
                Visible = true,
                BalloonTipIcon = ToolTipIcon.Info,
                ContextMenuStrip = menuStrip
            };         
            notifyIcon.MouseDoubleClick += new MouseEventHandler(notifyIcon_MouseDoubleClick);

            this.procMonitor = new ActiveProcessMonitor();
            this.procMonitor.OnActiveProcessChanged += OnActiveProcessChanged;
            this.procMonitor.Start();
        }


        private void OnActiveProcessChanged(Process activeProcess)
        {
            if (previousProcess.Process != null)
            {
                WindowDeactivated();
            }

            WindowActivated(activeProcess);
            
            //キャッシュ
            this.previousProcess.Register(activeProcess);
        }


        /// <summary>
        /// ウィンドウが最前面になったとき
        /// </summary>
        /// <param name="activeProcess"></param>
        private void WindowActivated(Process activeProcess)
        {
            //DBに存在していなければ新規にデータ挿入
            if (!dbOperator.IsExist(activeProcess.MainModule.FileName))
            {
                dbOperator.Register(activeProcess);
            }

            //使用時間の警告
            int appId = dbOperator.GetMatchedId(activeProcess.MainModule.FileName);
            ShowOveruseWarining(appId, activeProcess.ProcessName);
            
            //最初にアクティブになった時間を取得
            firstActiveDate = DateTime.Now;
        }


        /// <summary>
        /// 今まで最前面だったウィンドウが最前面でなくなったとき
        /// </summary>
        private void WindowDeactivated()
        {
            //計測時間追記の為にDBから該当Idを取得
            int appId = dbOperator.GetMatchedId(this.previousProcess.FileName);

            //DBから使用時間を取得し、今回の使用時間を加算してDB更新
            var activeInterval = Utils.GetInterval(firstActiveDate);
            dbOperator.UpdateUsageTime(appId, activeInterval);

            //バルーンで通知
            ShowBalloonTip(activeInterval, this.previousProcess.ProcessName);
        }
        

        /// <summary>
        /// 使用時間の警告を実行する
        /// </summary>
        /// <param name="appId"></param>
        private void ShowOveruseWarining(int appId, string processName)
        {
            if (!CanExecuteOveruseWarining(appId)) return;

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
        /// 以下の条件で真を返します。
        /// ・警告機能がユーザ設定で有効
        /// ・警告対象に現在のAppIDが含まれている
        /// ・もう警告しない対象に含まれていない
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        private bool CanExecuteOveruseWarining(int appId)
        {
            return Settings.Default.checkBox_IsOveruseWarining == true
                && overuseWarningItems.ContainsKey(appId) == true
                && (dataBanker["WarningNotAgain"] as List<int>).Contains(appId) == false;
        }


        /// <summary>
        /// 今まで最前面にあったプロセスの使用時間を通知する
        /// </summary>
        /// <param name="activeInterval">今回の使用時間</param>
        private void ShowBalloonTip(TimeSpan activeInterval, string procName)
        {
            if (Properties.Settings.Default.checkBox_IsBalloonEnable)
            {
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
            this.procMonitor.Stop();
            this.Hide();
            notifyIcon.Visible = false;
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
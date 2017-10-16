using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using System.Windows.Forms;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Manage_your_Life.Models;

namespace Manage_your_Life.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ActiveProcessUsageTimeLogger logger;
        private ActiveProcessMonitor procMonitor;
        private DatabaseOperation db;        
        // 使いすぎ警告の対象Dictionary
        private Dictionary<int, TimeSpan> overuseWarningItems;
        private DataBanker dataBanker;


        public MainWindowViewModel()
        {
            // アクティブなプロセスが変わるたびに使用時間をバルーン通知する
            this.logger = ActiveProcessUsageTimeLogger.Instance;
            this.logger.Logged += (proc, timeSpan) =>
            {
                if (Properties.Settings.Default.checkBox_IsBalloonEnable)
                {
                    var title = $"\"{ proc.ProcessName }\"の計測終了";
                    var message = $"使用時間: { timeSpan.ToString(@"hh\:mm\:ss") }";
                    this.Messenger.Raise(new BalloonTipMessage(title, message, BalloonIcon.Info, "BalloonTip"));
                }
            };


            this.db = DatabaseOperation.Instance;
            this.overuseWarningItems = this.db.GetOveruseWarningCollection();
            this.dataBanker = DataBanker.Instance;
            this.dataBanker["WarningNotAgain"] = new List<int>();
            this.dataBanker["WarningCount"] = 0;

            // 使いすぎ警告のダイアログを表示する
            this.procMonitor = ActiveProcessMonitor.Instance;
            this.procMonitor.OnActiveProcessChanged += (proc, _) =>
            {
                // 警告機能がユーザ設定で有効
                if (!Properties.Settings.Default.checkBox_IsOveruseWarining) return;

                int appId = this.db.GetMatchedId(proc.MainModule.FileName);
                if (!CanExecuteOveruseWarining(appId)) return;

                //今日の使用時間と警告時間を取得
                TimeSpan warningTime = this.overuseWarningItems[appId];
                TimeSpan todayUsageTime = this.db.GetTodayPrpcessUsageTime(appId);

                //今日の使用時間が警告時間よりも小さいなら警告しない
                if (todayUsageTime < warningTime) return;
                
                var vm = new AlertDialogViewModel(proc.ProcessName, warningTime);
                this.Messenger.Raise(new TransitionMessage(vm, "AlertDialogMessageKey"));

                //これ以上表示しない
                if (vm.IsNotDialogShowAgain)
                {
                    //IDをセット
                    var noWarningId = (List<int>)this.dataBanker["WarningNotAgain"] ?? new List<int>();
                    noWarningId.Add(appId);
                    this.dataBanker["WarningNotAgain"] = noWarningId;
                }

                this.dataBanker["WarningCount"] = (int)this.dataBanker["WarningCount"] + 1;
            };            
        }


        /// <summary>
        /// 以下の条件で真を返します
        /// ・警告対象に現在のAppIDが含まれている
        /// ・もう警告しない対象に含まれていない
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        private bool CanExecuteOveruseWarining(int appId)
        {
            return overuseWarningItems.ContainsKey(appId)
                && !(dataBanker["WarningNotAgain"] as List<int>).Contains(appId);
        }


        public void Initialize()
        {
            this.ShowInTaskbar = true;
        }
        

        public void WindowStateChanged(object sender, EventArgs e)
        {
            var window = sender as Views.MainWindow;
            if (window != null && window.WindowState != WindowState.Minimized)
            {
                this.WindowVisibility = Visibility.Collapsed;
                this.ShowInTaskbar = false;
            }
        }
        

        private WindowState windowState;
        public WindowState WindowState
        {
            get { return this.windowState; }
            set { this.SetProperty(ref this.windowState, value); }
        }

        private Visibility windowVisibility;
        public Visibility WindowVisibility
        {
            get { return this.windowVisibility; }
            set { this.SetProperty(ref this.windowVisibility, value); }
        }

        private bool showInTaskbar;
        public bool ShowInTaskbar
        {
            get { return this.showInTaskbar; }
            set { this.SetProperty(ref this.showInTaskbar, value); }
        }


        public Action ShowView { get; set; }
        public Action HideView { get; set; }
        public Func<bool> FocusView { get; set; }
        public Func<bool> ActivateView { get; set; }
        public Action ShowTodayReportDialog { get; set; }


        private ViewModelCommand _OpenWindowCommand;
        public ViewModelCommand OpenWindowCommand 
            => this._OpenWindowCommand ?? (this._OpenWindowCommand = new ViewModelCommand(OpenWindow));

        private ViewModelCommand _ExitCommand;
        public ViewModelCommand ExitCommand
            => this._ExitCommand ?? (this._ExitCommand = new ViewModelCommand(Exit));


        private void OpenWindow()
        {
            this.ShowView?.Invoke();
            this.WindowState = WindowState.Normal;
            this.WindowVisibility = Visibility.Visible;
            this.ShowInTaskbar = true;
            this.ActivateView?.Invoke();
            this.FocusView?.Invoke();
        }


        public void Closed()
        {
            //TodayReportの表示
            if (Properties.Settings.Default.checkBox_IsReportEnable)
            {
                this.ShowTodayReportDialog();
            }
        }

        private void Exit()
        {
            this.procMonitor.Stop();

            this.WindowVisibility = Visibility.Visible;
            this.HideView?.Invoke();

            Properties.Settings.Default.Save();

            //TodayReportの表示
            if (Properties.Settings.Default.checkBox_IsReportEnable)
            {
                this.ShowTodayReportDialog();
            }
            System.Windows.Application.Current.Shutdown();
        }
    }
}

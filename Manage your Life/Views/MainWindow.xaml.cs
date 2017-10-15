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
        
        /// <summary>
        /// データベースを操作
        /// </summary>
        DatabaseOperation dbOperator;
        
        
        /// <summary>
        /// 使いすぎ警告の対象Dictionary
        /// </summary>
        Dictionary<int, TimeSpan> overuseWarningItems;

        DataBanker dataBanker;

        #endregion

        
        public MainWindow()
        {
            this.DataContextChanged += (_, e) =>
            {
                var model = e.NewValue as ViewModels.MainWindowViewModel;
                if (model != null)
                {
                    model.ShowView = this.Show;
                    model.HideView = this.Hide;
                    model.FocusView = this.Focus;
                    model.ActivateView = this.Activate;
                    model.ShowOveruseWarningDialog = this.ShowOveruseWarining;
                    model.ShowTodayReportDialog = this.ShowTodayReportDialog;
                }
            };

            InitializeComponent();

            this.taskbarIcon.Icon = Properties.Resources.originalIconTray;
            this.Closed += (_, __) => this.taskbarIcon.Dispose();


            dbOperator = DatabaseOperation.Instance;
            overuseWarningItems = dbOperator.GetOveruseWarningCollection();
            dataBanker = DataBanker.Instance;
            dataBanker["WarningNotAgain"] = new List<int>();
            dataBanker["WarningCount"] = 0;            
        }

        
        private void ShowTodayReportDialog()
        {
            var window = new TodayReport();
            window.ShowDialog();
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
                AlertDialog window = new AlertDialog(processName, appId, warningTime);
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
    }      
}
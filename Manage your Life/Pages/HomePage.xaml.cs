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
using System.ComponentModel;
using De.TorstenMandelkow.MetroChart;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;


namespace Manage_your_Life
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        #region Field
        /// <summary>
        /// 一秒ごとにイベントを発生させるタイマー
        /// </summary>
        DispatcherTimer clockTimer;
        DispatcherTimer hatenaTimer;

        /// <summary>
        /// 15分ごとにイベントを発生させるタイマー
        /// </summary>
        DispatcherTimer updateUpTimeTimer;

        /// <summary>
        /// windowTitleからカテゴライズした今までのDictionary
        /// </summary>
        Dictionary<string, int> preCategorizedCountData;

        /// <summary>
        /// このアプリの終了時に「今日のまとめ」としてデータを使いたい
        /// MainWindowに渡すためにDataBanker使用
        /// </summary>
        DataBanker dataBanker;

        DatabaseOperation dbOperator;       
        ProcessInformation pInfo;
        
        string preWindowTitle = "";

        #endregion
        

        public HomePage()
        {
            InitializeComponent();

            pInfo = new ProcessInformation();
            preCategorizedCountData = new Dictionary<string, int>();
            dataBanker = DataBanker.Instance;

            dbOperator = DatabaseOperation.Instance;
            dbOperator.TimelineLog_Updated += new EventHandler(this.TimelineLog_Updated);

            this.chart_upTime.DataContext = new SystemUptimeViewModel();
            this.chart_Bar.DataContext = new UsageTimeViewModel(DateTime.Today, 5);

            //起動時刻保存
            dataBanker["StartUpTime"] = DateTime.Now;

            //タイマー登録
            clockTimer = new DispatcherTimer();
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.Tick += ClockDispatcherTimer_Tick;
            clockTimer.Start();
            
            updateUpTimeTimer = new DispatcherTimer();
            updateUpTimeTimer.Interval = new TimeSpan(0, 15, 0);
            updateUpTimeTimer.Tick += UpdateUpTimeDispatcherTimer_Tick;
            updateUpTimeTimer.Start();

            //カテゴライズを停止しない!
            if (!Properties.Settings.Default.checkBox_IsCategorizeStop)
            {
                int sec = Properties.Settings.Default.label_TimeSpan / 1000;
                int milliSec = Properties.Settings.Default.label_TimeSpan % 1000;

                hatenaTimer = new DispatcherTimer();
                hatenaTimer.Interval = new TimeSpan(0, 0, 0, sec, milliSec);
                hatenaTimer.Tick += HatenaDispatcherTimer_Tick;
                hatenaTimer.Start();
            }
        }



//----------------------------------------------------------タイマーイベント

        private void ClockDispatcherTimer_Tick(object sender, EventArgs e)
        {
            clockTimer.Stop();

            //システムの稼働時間を取得
            PerformanceCounter upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();

            //labelの値を更新
            this.label_upTime.Content = TimeSpan.FromSeconds((int)upTime.NextValue());

            clockTimer.Start();
        }



        private async void HatenaDispatcherTimer_Tick(object sender, EventArgs e)
        {
            hatenaTimer.Stop();

            //アクティブウィンドウが変化したか
            string windowTitle = pInfo.GetWindowTitle();
            if (windowTitle == preWindowTitle)
            {
                hatenaTimer.Start();
                return;
            }

            //labelの値を更新
            this.label_ForegroundWindow.Content = pInfo.GetWindowTitle();
            Utility.DoEvents();

            //Hatena系は時間が掛かるので非同期処理にした
            HatenaKeywordViewModel context = null;
            try
            {
                context = await Task.Run(() =>
                    GetHatenaKeywordViewModel(windowTitle, preCategorizedCountData));
            }
            catch (Exception ex)
            {
                MessageBox.Show("カテゴライズの過程で例外が発生しました。", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (context.HatenaKeyword.Count != 0)
            {
                this.chart_Hatena.DataContext = context;
            }

            //今回Chartに設定したカウントDictionaryがnullではないとき
            if (context.GetHatenaCategorizedCountData != null)
            {
                //今回の値を保持する
                preCategorizedCountData = context.GetHatenaCategorizedCountData;
                dataBanker["CategorizedCountData"] = preCategorizedCountData;
            }

            preWindowTitle = windowTitle;
            hatenaTimer.Start();
        }


        private void UpdateUpTimeDispatcherTimer_Tick(object sender, EventArgs e)
        {
            updateUpTimeTimer.Stop();

            //稼働時間Chartの値を更新させる
            this.chart_upTime.DataContext = new SystemUptimeViewModel();
            updateUpTimeTimer.Start();
        }



        /// <summary>
        /// HatenaKeywordViewModelのインスタンスを生成
        /// </summary>
        /// <param name="windowTitle"></param>
        /// <param name="preCategorizedCountData"></param>
        /// <returns></returns>
        private HatenaKeywordViewModel 
            GetHatenaKeywordViewModel(string windowTitle, Dictionary<string, int> preCategorizedCountData)
        {
            return new HatenaKeywordViewModel(windowTitle, preCategorizedCountData);
        }


//---------------------------------------------------------イベントハンドラ
        
        /// <summary>
        /// データベースのTimelineの使用時間が更新されたら、棒グラフを更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimelineLog_Updated(object sender, EventArgs e)
        {
            this.chart_Bar.DataContext = new UsageTimeViewModel(DateTime.Today, 5);
        }


    }
}

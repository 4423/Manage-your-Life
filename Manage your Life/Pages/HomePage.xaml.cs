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


namespace Manage_your_Life.Pages
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
        DispatcherTimer oneSecTimer;

        /// <summary>
        /// 15分ごとにイベントを発生させるタイマー
        /// </summary>
        DispatcherTimer tenMinTimer;

        ProcessInformation pInfo;

        List<ChartData> preHatenaKeywordCollections;

        string preWindowTitle = "";

        #endregion
        

        public HomePage()
        {
            InitializeComponent();

            pInfo = new ProcessInformation();
            preHatenaKeywordCollections = new List<ChartData>();

            this.DataContext = new HomePageViewModel();

            oneSecTimer = new DispatcherTimer();
            oneSecTimer.Interval = new TimeSpan(0, 0, 1);
            oneSecTimer.Tick += oneSecDispatcherTimer_Tick;
            oneSecTimer.Start();
            
            tenMinTimer = new DispatcherTimer();
            tenMinTimer.Interval = new TimeSpan(0, 15, 0);
            tenMinTimer.Tick += tenMinDispatcherTimer_Tick;
            tenMinTimer.Start();            
        }


//----------------------------------------------------------タイマーイベント

        private void oneSecDispatcherTimer_Tick(object sender, EventArgs e)
        {
            oneSecTimer.Stop();

            //システムの稼働時間を取得
            PerformanceCounter upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();

            //labelの値を更新
            this.label_upTime.Content = TimeSpan.FromSeconds((int)upTime.NextValue());


            //アクティブウィンドウが変化したか
            string windowTitle = pInfo.GetWindowTitle();
            if (windowTitle == preWindowTitle)
            {
                oneSecTimer.Start();
                return;
            }

            //labelの値を更新
            this.label_ForegroundWindow.Content = pInfo.GetWindowTitle();
            Utility.DoEvents();


            //TODO Hatena系は時間が掛かるので非同期処理にしたい
            var context = new HatenaKeywordViewModel(windowTitle, preHatenaKeywordCollections);
            this.chart_Hatena.DataContext = context;

            //今回のwindowTitleでカテゴライズされたものを追記
            if (context.GetHatenaKeywordData != null)
            {
                preHatenaKeywordCollections.AddRange(context.GetHatenaKeywordData);
            }
            
            //this.chart_Hatena.DataContext = GetInstanceHatena(windowTitle);
            Debug.WriteLine("ViewModel");

            preWindowTitle = windowTitle;
            oneSecTimer.Start();
        }


        private void tenMinDispatcherTimer_Tick(object sender, EventArgs e)
        {
            tenMinTimer.Stop();

            //稼働時間Chartの値を更新させる
            this.chart_upTime.DataContext = new HomePageViewModel();
            tenMinTimer.Start();
        }

        
    }
}

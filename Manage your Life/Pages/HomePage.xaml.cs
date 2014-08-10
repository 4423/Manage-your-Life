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
        /// <summary>
        /// 一秒ごとにイベントを発生させるタイマー
        /// </summary>
        DispatcherTimer oneSecTimer;

        /// <summary>
        /// 15分ごとにイベントを発生させるタイマー
        /// </summary>
        DispatcherTimer tenMinTimer;

        public HomePage()
        {
            InitializeComponent();

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
            //システムの稼働時間を取得
            PerformanceCounter upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();

            this.label_upTime.Content = TimeSpan.FromSeconds((int)upTime.NextValue());
        }


        private void tenMinDispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Chartの値を更新させる
            this.DataContext = new HomePageViewModel();
        }


        /// <summary>
        /// listBox1にアイテムを追加します
        /// </summary>
        /// <param name="addItems"></param>
        public void AddListBox(object addItems)
        {
            //listBox1.Items.Add(addItems);

        }


        
    }
}

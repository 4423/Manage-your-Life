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


namespace Manage_your_Life
{
    public class SystemUptimeViewModel : ViewModel
    {
        
        public ObservableCollection<ChartData> SystemUpTime { get; set; }


        //コンストラクタ
        public SystemUptimeViewModel()
        {
            LoadPalettes();
            Settings();

            SystemUpTime = new ObservableCollection<ChartData>();            
            SystemUpTime.Add(new ChartData() { Category = "", Number = GetSystemUpTimePercentage() });

            Series = new ObservableCollection<SeriesData>();         
            Series.Add(new SeriesData() { SeriesDisplayName = "SystemUpTime", Items = SystemUpTime });
        }



        /// <summary>
        /// パソコンの稼働時間を24時間に対してのパーセンテージで求める
        /// </summary>
        /// <returns></returns>
        private double GetSystemUpTimePercentage()
        {
            //パソコン稼働時間取得
            PerformanceCounter upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();

            double upTimeTotalMinutes = (TimeSpan.FromSeconds(upTime.NextValue())).TotalMinutes;
            //パーセンテージ
            double upTimePercentage = upTimeTotalMinutes / TimeSpan.FromHours(24).TotalMinutes;

            return upTimePercentage * 100;
        }


    }
     

}

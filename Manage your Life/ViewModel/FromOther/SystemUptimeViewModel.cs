using System;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace Manage_your_Life
{
    public class SystemUptimeViewModel : ViewModel
    {
        
        public ObservableCollection<ChartData> SystemUpTime { get; set; }


        //コンストラクタ
        public SystemUptimeViewModel()
        {
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

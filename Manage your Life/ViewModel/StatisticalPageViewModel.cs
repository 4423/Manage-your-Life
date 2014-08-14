using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using De.TorstenMandelkow.MetroChart;


namespace Manage_your_Life
{
    public class StatisticalPageViewModel : ViewModel
    {

        public ObservableCollection<string> ChartTypes { get; set; }
        public ObservableCollection<ChartData> Points { get; set; }  
      
        ApplicationDataClassesDataContext database;
        string basePath;
        string connStr;


        //コンストラクタ
        public StatisticalPageViewModel()
        {
            //データベース接続
            basePath = Directory.GetCurrentDirectory() + @"\ApplicationDatabase.mdf";
            connStr = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=""" + basePath + @""";Integrated Security=True";
            database = new ApplicationDataClassesDataContext(connStr);


            LoadPalettes();
            Settings();

            
            Series = new ObservableCollection<SeriesData>();
            
            Series.Add(new SeriesData() { SeriesDisplayName = "UsageTime", Items = TakeUsageTimeItems(5) });
        }



        /// <summary>
        /// データベースから使用時間の上位数項目を取得
        /// </summary>
        /// <param name="takeNumber">取得する項目数</param>
        /// <returns>使用時間とプロセス名のCollection</returns>
        private ObservableCollection<ChartData> TakeUsageTimeItems(int takeNumber)
        {
            Points = new ObservableCollection<ChartData>();



            //使用時間のうちトップtakeNumberを選択
            var q = (
                from p in database.DatabaseApplication
                select new
                {
                    Title = p.Title,
                    ProcName = p.DatabaseProcess.Name,
                    UsageTime = p.DatabaseDate.UsageTime,
                })
                .Take(takeNumber);


            //抽出された使用時間の合計を算出
            double sumOfUsageTime = 0;
            foreach (var r in q)
            {
                sumOfUsageTime += (TimeSpan.Parse(r.UsageTime)).TotalSeconds;
            }


            //グラフに表示する項目の追加
            foreach (var r in q)
            {
                //usageTimeから合計時間を秒で取得
                //http://dobon.net/vb/dotnet/system/timespan.html
                TimeSpan usageTime = TimeSpan.Parse(r.UsageTime);

                //パーセンテージにする
                double floorUsageTime = (usageTime.TotalSeconds / sumOfUsageTime) * 100;

                Points.Add(new ChartData()
                {
                    Category = r.ProcName,
                    //小数点2以下四捨五入
                    Number = Utility.ToRoundDown(floorUsageTime, 2),
                    TotalHours = Utility.ToRoundDown(usageTime.TotalHours, 2),
                    Days = usageTime.Days,
                    Hours = usageTime.Hours,
                    Minutes = usageTime.Minutes,
                    Seconds = usageTime.Seconds
                });
            }

            return Points;
        }

    }
}

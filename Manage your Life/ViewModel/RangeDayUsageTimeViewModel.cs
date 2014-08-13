using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using De.TorstenMandelkow.MetroChart;

namespace Manage_your_Life
{
    /// <summary>
    /// ある日に使用したアプリケーションのプロセス名と使用時間を得るViewModel
    /// </summary>
    public class RangeDayUsageTimeViewModel : ViewModel
    {

        public ObservableCollection<ChartData> UsageTime { get; set; }


        /// <summary></summary>
        /// <param name="oneDay">ある日</param>
        /// <param name="takeNumber">取得するアプリケーション数</param>
        public RangeDayUsageTimeViewModel(DateTime startDay, DateTime endDay, int takeNumber)
        {
            LoadPalettes();
            Settings();


            //データベース接続
            DatabaseOperation dbOperator = DatabaseOperation.Instance;
            var database = dbOperator.GetConnectionedDataContext;


            //ある期間に使用したプロセスと、その使用時間を得る
            var q = (
                from p in database.DatabaseApplication
                where startDay <= p.DatabaseTimeline.Today && p.DatabaseTimeline.Today.Value <= endDay
                select new
                {
                    Key = p.DatabaseProcess.Name,
                    Value = TimeSpan.Parse(p.DatabaseTimeline.UsageTime)
                });


            //データベースから抽出したデータについて、重複したプロセス名を単一にまとめる
            var dict = Utility.DictionaryOrganizingValue(q);


            //値に基づいて降順でソート + ループカウンタindexを使用
            //see: http://nanoappli.com/blog/archives/2012
            UsageTime = new ObservableCollection<ChartData>();
            foreach (var d in dict.OrderByDescending((x) => x.Value).
                                Select((value, index) => new { value, index }))
            {
                //取得数になったら抜ける
                if (d.index == takeNumber) break;

                UsageTime.Add(new ChartData()
                {
                    Category = d.value.Key,
                    Number = Utility.ToRoundDown(d.value.Value.TotalHours, 3)
                });
            }


            Series = new ObservableCollection<SeriesData>();
            Series.Add(new SeriesData() { SeriesDisplayName = "UsageTime", Items = UsageTime });
        }
        

    }
}

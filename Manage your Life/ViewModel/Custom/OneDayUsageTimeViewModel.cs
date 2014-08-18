using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life
{
    /// <summary>
    /// ある日に使用したアプリケーションのプロセス名と使用時間を得るViewModel
    /// </summary>
    public class OneDayUsageTimeViewModel : ViewModel
    {

        public ObservableCollection<ChartData> UsageTime { get; set; }


        /// <summary></summary>
        /// <param name="oneDay">ある日</param>
        /// <param name="takeNumber">取得するアプリケーション数</param>
        public OneDayUsageTimeViewModel(DateTime oneDay, int takeNumber)
        {

            //データベース接続
            DatabaseOperation dbOperator = DatabaseOperation.Instance;
            var database = dbOperator.GetConnectionedDataContext;

            //ある日に使用したプロセスと、その使用時間を得る
            var q = (
                from p in database.DatabaseApplication
                where p.DatabaseTimeline.Today == oneDay
                select new
                {
                    Name = p.DatabaseProcess.Name,
                    UsageTime = TimeSpan.Parse(p.DatabaseTimeline.UsageTime)
                });


            //データベースから抽出したデータについて、重複したプロセス名を単一にまとめる
            Dictionary<string, TimeSpan> dict = new Dictionary<string, TimeSpan>();
            foreach (var r in q)
            {
                //Dictionaryに既にKeyが含まれていなければ
                if (!dict.ContainsKey(r.Name))
                {
                    dict.Add(r.Name, r.UsageTime);
                }
                //含まれていればそのプロセスに対して使用時間を加算
                else
                {
                    dict[r.Name] += r.UsageTime;
                }
            }


            //値に基づいて降順でソート + ループカウンタindexを使用
            //see: http://nanoappli.com/blog/archives/2012
            UsageTime = new ObservableCollection<ChartData>();
            foreach (var d in dict.OrderByDescending((x) => x.Value).
                                Select((value, index) => new {value, index}))
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

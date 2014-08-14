using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Manage_your_Life
{
    /// <summary>
    /// 使用時間のカスタムViewModel
    /// </summary>
    public class CustomUsageTimeViewModel : ViewModel
    {

        public ObservableCollection<ChartData> ChartData { get; set; }
        private ApplicationDataClassesDataContext database;

        private string chartSubTitle = "";



        public CustomUsageTimeViewModel()
        {
            LoadPalettes();
            Settings();

            //データベース接続
            DatabaseOperation dbOperator = DatabaseOperation.Instance;
            database = dbOperator.GetConnectionedDataContext;
        }




        /// <summary>
        /// すべての日付
        /// </summary>
        /// <param name="isFavoritesOnly">お気に入り</param>
        /// <param name="order">順序</param>
        /// <param name="takeNumber">取得するアプリケーション数</param>
        public CustomUsageTimeViewModel
            (bool isFavoritesOnly, string order, int takeNumber)
            : this()
        {
            IQueryable q;

            //ある期間に使用したプロセスと、その使用時間を得る
            //Favoのみ
            if (isFavoritesOnly)
            {
                q = (
                    from p in database.DatabaseApplication
                    where p.Favorite == true
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = TimeSpan.Parse(p.DatabaseTimeline.UsageTime)
                    });
            }
            //すべて
            else
            {
                q = (
                    from p in database.DatabaseApplication
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = TimeSpan.Parse(p.DatabaseTimeline.UsageTime)
                    });
            }


            ChartData = new ObservableCollection<ChartData>();

            //データベースから抽出したデータについて、重複したプロセス名を単一にまとめる
            var dict = Utility.DictionaryOrganizingValue(q);

            //ソート + ループカウンタindexを使用してfor的な使い方
            //see: http://nanoappli.com/blog/archives/2012
            foreach (var d in Utility.SortingDictionary(order, dict).
                                Select((value, index) => new { value, index }))
            {
                //取得数になったら抜ける
                if (d.index == takeNumber) break;

                ChartData.Add(new ChartData()
                {
                    Category = d.value.Key,
                    Number = Utility.ToRoundDown(d.value.Value.TotalHours, 3)
                });
            }


            Series = new ObservableCollection<SeriesData>();
            Series.Add(new SeriesData() { SeriesDisplayName = "UsageTime", Items = ChartData });

            //ChartのSubtitle設定
            chartSubTitle = String.Format("Date: All date, Favorites {0}, Order: {1}, Take number: {2}",
                isFavoritesOnly ? "only" : "not only", order, takeNumber.ToString());
        }




        /// <summary>
        /// 期間指定
        /// </summary>
        /// <param name="startDay">ハジマリノ日</param>
        /// <param name="endDay">オワリノ日</param>
        /// <param name="isFavoritesOnly">お気に入り</param>
        /// <param name="order">順序</param>
        /// <param name="takeNumber">取得するアプリケーション数</param>
        public CustomUsageTimeViewModel
            (DateTime startDay, DateTime endDay, bool isFavoritesOnly, string order, int takeNumber)
            : this()
        {
            IQueryable q;

            //ある期間に使用したプロセスと、その使用時間を得る
            //Favoのみ
            if (isFavoritesOnly)
            {
                q = (
                    from p in database.DatabaseApplication
                    where startDay <= p.DatabaseTimeline.Today && p.DatabaseTimeline.Today.Value <= endDay
                    where p.Favorite == true
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = TimeSpan.Parse(p.DatabaseTimeline.UsageTime)
                    });
            }
            //すべて
            else
            {
                q = (
                    from p in database.DatabaseApplication
                    where startDay <= p.DatabaseTimeline.Today && p.DatabaseTimeline.Today.Value <= endDay
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = TimeSpan.Parse(p.DatabaseTimeline.UsageTime)
                    });
            }


            ChartData = new ObservableCollection<ChartData>();

            //データベースから抽出したデータについて、重複したプロセス名を単一にまとめる
            var dict = Utility.DictionaryOrganizingValue(q);

            //ソート + ループカウンタindexを使用してfor的な使い方
            //see: http://nanoappli.com/blog/archives/2012
            foreach (var d in Utility.SortingDictionary(order, dict).
                                Select((value, index) => new { value, index }))
            {
                //取得数になったら抜ける
                if (d.index == takeNumber) break;

                ChartData.Add(new ChartData()
                {
                    Category = d.value.Key,
                    Number = Utility.ToRoundDown(d.value.Value.TotalHours, 3)                    
                });
            }


            Series = new ObservableCollection<SeriesData>();
            Series.Add(new SeriesData() { SeriesDisplayName = "UsageTime", Items = ChartData });
            
            //ChartのSubtitle設定
            chartSubTitle = String.Format("Date: {0} -> {1}, Favorites {2}, Order: {3}, Take number: {4}",
                startDay.ToShortDateString(), endDay.ToShortDateString(),
                isFavoritesOnly ? "only" : "not only", order, takeNumber.ToString());
        }



        public override string ToolTipFormat
        {
            get
            {
                return "'{0}' has been used {1} hour ({3:P2})";
            }
        }



        public string ChartTitle
        {
            get
            {
                return "Application UsageTime";
            }
        }


        public string ChartSubTitle
        {
            get
            {
                return chartSubTitle;
            }
        }

        public string SeriesTitle { get { return "Usage time [hour]"; } }

    }
}

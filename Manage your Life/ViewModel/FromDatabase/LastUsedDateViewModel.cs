using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Manage_your_Life
{
    /// <summary>
    /// 最終使用日のカスタムViewModel
    /// </summary>
    public class LastUsedDateViewModel : ViewModel
    {

        private ApplicationDataClassesDataContext database;
        private string chartSubTitle = "";



        public LastUsedDateViewModel()
        {

            //データベース接続
            DatabaseOperation dbOperator = DatabaseOperation.Instance;
            database = dbOperator.GetConnectionedDataContext;
        }



        /// <summary>
        /// 期間指定
        /// </summary>
        /// <param name="startDay">ハジマリノ日</param>
        /// <param name="endDay">オワリノ日</param>
        /// <param name="isFavoritesOnly">お気に入り</param>
        /// <param name="order">順序</param>
        /// <param name="takeNumber">取得するアプリケーション数</param>
        public LastUsedDateViewModel
            (DateTime startDay, DateTime endDay, bool isFavoritesOnly, string order, int takeNumber)
            : this()
        {
            //ある期間に使用したプロセスと、その最終使用日を得る
            //Favo含むすべて
            var q = (
                    from p in database.DatabaseApplication
                    where startDay <= ((DateTime)p.DatabaseDate.LastDate).Date
                            && ((DateTime)p.DatabaseDate.LastDate).Date <= endDay
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = p.DatabaseDate.LastDate
                    });

            //Favoのみ
            if (isFavoritesOnly)
            {
                q = (
                    from p in database.DatabaseApplication
                    where startDay <= ((DateTime)p.DatabaseDate.LastDate).Date
                            && ((DateTime)p.DatabaseDate.LastDate).Date <= endDay
                    where p.Favorite == true
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = p.DatabaseDate.LastDate
                    });
            }


            //ソート
            switch (order)
            {
                case "Ascending":
                    q = q.OrderBy((x) => x.Value);
                    break;
                case "Descending":
                    q = q.OrderByDescending((x) => x.Value);
                    break;
            }


            ChartData = new ObservableCollection<ChartData>();

            //取得数分だけforeach
            foreach (var r in q.Take(takeNumber))
            {
                ChartData.Add(new ChartData()
                {
                    Category = r.Key,
                    Number = Utility.ToRoundDown((DateTime.Now - (DateTime)r.Value).TotalDays, 4)
                });
            }

            Series = new ObservableCollection<SeriesData>();
            Series.Add(new SeriesData() { SeriesDisplayName = "LastUsedDate", Items = ChartData });

            //ChartのSubtitle設定
            chartSubTitle = String.Format("Date: {0} -> {1}, Favorites {2}, Order: {3}, Take number: {4}",
                startDay.ToShortDateString(), endDay.ToShortDateString(),
                isFavoritesOnly ? "only" : "not only", order, takeNumber.ToString());
        }




        /// <summary>
        /// すべての日付
        /// </summary>
        /// <param name="isFavoritesOnly">お気に入り</param>
        /// <param name="order">順序</param>
        /// <param name="takeNumber">取得するアプリケーション数</param>
        public LastUsedDateViewModel
            (bool isFavoritesOnly, string order, int takeNumber)
            : this()
        {
            //プロセスとその使用時間を得る
            //Favo含むすべて
            var q = (
                    from p in database.DatabaseApplication
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = p.DatabaseDate.LastDate
                    });

            //Favoのみ
            if (isFavoritesOnly)
            {
                q = (
                    from p in database.DatabaseApplication
                    where p.Favorite == true
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = p.DatabaseDate.LastDate
                    });
            }


            //ソート
            switch (order)
            {
                case "Ascending":
                    q = q.OrderBy((x) => x.Value);
                    break;
                case "Descending":
                    q = q.OrderByDescending((x) => x.Value);
                    break;
            }


            ChartData = new ObservableCollection<ChartData>();

            foreach (dynamic r in q.Take(takeNumber))
            {
                ChartData.Add(new ChartData()
                {
                    Category = r.Key,
                    Number = Utility.ToRoundDown((DateTime.Now - (DateTime)r.Value).TotalDays, 4)
                });
            }

            Series = new ObservableCollection<SeriesData>();
            Series.Add(new SeriesData() { SeriesDisplayName = "LastUsedDate", Items = ChartData });
            
            //ChartのSubtitle設定
            chartSubTitle = String.Format("Date: All date, Favorites {0}, Order: {1}, Take number: {2}",
                isFavoritesOnly ? "only" : "not only", order, takeNumber.ToString());
        }




        public override string ToolTipFormat
        {
            get
            {
                return "'{0}' has been used for {1} days ago";
            }
        }


        public string ChartTitle
        {
            get
            {
                return "Application LastUsedDate";
            }
        }


        public string ChartSubTitle
        {
            get
            {
                return chartSubTitle;
            }
        }

        public string SeriesTitle
        {
            get
            {
                return "Time span [day]";
            }
        }

    }
}

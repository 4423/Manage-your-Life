using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Manage_your_Life
{

    /// <summary>
    /// 登録日のカスタムViewModel
    /// </summary>
    public class CustomRegistrationDateViewModel : ViewModel
    {

        public ObservableCollection<ChartData> ChartData { get; set; }
        private ApplicationDataClassesDataContext database;
        private string chartSubTitle = "";



        public CustomRegistrationDateViewModel()
        {
            LoadPalettes();
            Settings();

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
        public CustomRegistrationDateViewModel
            (DateTime startDay, DateTime endDay, bool isFavoritesOnly, string order, int takeNumber)
            : this()
        {
            //ある期間に登録したプロセスと、その登録日を得る
            //Favo含むすべて
            var q = (
                    from p in database.DatabaseApplication
                    where startDay <= ((DateTime)p.DatabaseDate.AddDate).Date
                            && ((DateTime)p.DatabaseDate.AddDate).Date <= endDay
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = p.DatabaseDate.AddDate
                    });

            //Favoのみ
            if (isFavoritesOnly)
            {
                q = (
                    from p in database.DatabaseApplication
                    where startDay <= ((DateTime)p.DatabaseDate.AddDate).Date
                            && ((DateTime)p.DatabaseDate.AddDate).Date <= endDay
                    where p.Favorite == true
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = p.DatabaseDate.AddDate
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

            int i = 0;
            foreach (var r in q)
            {
                //取得数になったら抜ける
                if (i++ == takeNumber) break;

                ChartData.Add(new ChartData()
                {
                    Category = r.Key,
                    Number = Utility.ToRoundDown((DateTime.Now - (DateTime)r.Value).TotalDays, 4)
                });
            }

            Series = new ObservableCollection<SeriesData>();
            Series.Add(new SeriesData() { SeriesDisplayName = "RegistrationDate", Items = ChartData });

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
        public CustomRegistrationDateViewModel
            (bool isFavoritesOnly, string order, int takeNumber)
            : this()
        {
            //プロセスとその登録日を得る
            //Favo含むすべて
            var q = (
                    from p in database.DatabaseApplication
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = p.DatabaseDate.AddDate
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
                        Value = p.DatabaseDate.AddDate
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

            int i = 0;
            foreach (dynamic r in q)
            {
                //取得数になったら抜ける
                if (i++ == takeNumber) break;

                ChartData.Add(new ChartData()
                {
                    Category = r.Key,
                    Number = Utility.ToRoundDown((DateTime.Now - (DateTime)r.Value).TotalDays, 4)
                });
            }

            Series = new ObservableCollection<SeriesData>();
            Series.Add(new SeriesData() { SeriesDisplayName = "RegistrationDate", Items = ChartData });

            //ChartのSubtitle設定
            chartSubTitle = String.Format("Date: All date, Favorites {0}, Order: {1}, Take number: {2}",
                isFavoritesOnly ? "only" : "not only", order, takeNumber.ToString());
        }



        public override string ToolTipFormat { get { return "'{0}' was registered on {1} day ago"; } }

        public string ChartTitle { get { return "Application RegistrationDate"; } }
        public string ChartSubTitle { get { return chartSubTitle; } }
        public string SeriesTitle { get { return "Time span [day]"; } }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Manage_your_Life
{
    /// <summary>
    /// 使用時間のカスタムViewModel
    /// </summary>
    public class TestViewModel : ViewModel
    {

        private ApplicationDataClassesDataContext database;


        public TestViewModel()
        {
            Items = new List<MyItem>();

            Items.Add(new MyItem()
            {
                Key = DateTime.Today,
                Value = 0
            });
        }

        public TestViewModel(int appId, DateTime day)
        {

            //データベース接続
            DatabaseOperation dbOperator = DatabaseOperation.Instance;
            database = dbOperator.GetConnectionedDataContext;

            var q = (
                    from p in database.DatabaseApplication
                    where p.DatabaseTimeline.Today == day
                    where p.Id == appId
                    select new
                    {
                        Key = p.DatabaseTimeline.Now,
                        Value = Utility.ToRoundDown((TimeSpan.Parse(p.DatabaseTimeline.UsageTime)).TotalMinutes, 3)
                    });


            Items = new List<MyItem>();

            //Items.Add(new MyItem()
            //{
            //    Key = DateTime.Today,
            //    Value = 0
            //});

            double stack = 0;
            foreach (var r in q)
            {
                stack += r.Value;

                Items.Add(new MyItem()
                    {
                        Key = (DateTime)r.Key,
                        Value = r.Value
                    });
            }

            //Items.Add(new MyItem()
            //{
            //    Key = (DateTime.Today.AddDays(1).AddMinutes(-1)),
            //    Value = stack
            //});
            //Series = new ObservableCollection<SeriesData>();
            //Series.Add(new SeriesData() { SeriesDisplayName = "ChartData", Items = ChartData });            
        }

        public List<MyItem> Items { get; set; }


        public class MyItem
        {
            public DateTime Key { get; set; }
            public double Value { get; set; }
        }



    }
}

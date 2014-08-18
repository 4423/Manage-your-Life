using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life
{
    //一つのChartに表示するデータ
    public class SeriesData
    {
        public string SeriesDisplayName { get; set; }

        public string SeriesDescription { get; set; }

        public ObservableCollection<ChartData> Items { get; set; }
    }
}

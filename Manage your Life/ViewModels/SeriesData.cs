using System.Collections.ObjectModel;

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

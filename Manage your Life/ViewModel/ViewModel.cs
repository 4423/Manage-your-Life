using System.Collections.ObjectModel;

namespace Manage_your_Life
{
    public class ViewModel
    {
        public ObservableCollection<ChartData> ChartData { get; set; }

        public ObservableCollection<SeriesData> Series { get; set; }

        public virtual string ToolTipFormat { get { return "{0}: {1} ({3:P2})"; } }
    }
}

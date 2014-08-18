using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;
using De.TorstenMandelkow.MetroChart;
using System.Collections.ObjectModel;
using System.IO;

namespace Manage_your_Life
{
    public class ViewModel
    {

        public ViewModel() { }


        public ObservableCollection<ChartData> ChartData
        {
            get;
            set;

        }

        public ObservableCollection<SeriesData> Series
        {
            get;
            set;
        }


        public virtual string ToolTipFormat
        {
            get
            {
                return "{0}: {1} ({3:P2})";
            }
        }

    }
}

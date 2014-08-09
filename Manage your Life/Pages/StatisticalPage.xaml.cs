using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO;
using System.Windows.Interop;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using FirstFloor.ModernUI.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Manage_your_Life.Pages
{
    /// <summary>
    /// Interaction logic for StatisticalPage.xaml
    /// </summary>
    public partial class StatisticalPage : UserControl
    {

        public StatisticalPage()
        {
            InitializeComponent();

            this.DataContext = new StatisticalPageViewModel();
        }
    }
}

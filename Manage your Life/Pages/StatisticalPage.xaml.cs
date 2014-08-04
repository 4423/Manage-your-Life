using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
//using System.Data.Objects;
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

        /// <summary>
        /// データベースを操作
        /// </summary>
        DatabaseOperation dbOperator;


        public ObservableCollection<CircleChart> points { get; private set; }


        public StatisticalPage()
        {
            InitializeComponent();


            dbOperator = new DatabaseOperation();

            this.DataContext = new StatisticalPageViewModel();

            
        }

        /// <summary>
        /// 円グラフを描画してみる
        /// <see cref="http://modernuicharts.codeplex.com/documentation#howto1"/>
        /// </summary>
        private void DrawChart()
        {
            points = new ObservableCollection<CircleChart>();

            var q = dbOperator.GetAllData();

            foreach (dynamic r in q)
            {
                //usageTimeから合計時間を秒で取得
                //http://dobon.net/vb/dotnet/system/timespan.html
                double totalSeconds = (TimeSpan.Parse(r.UsageTime)).TotalSeconds;

                //グラフに表示する項目の追加
                points.Add(new CircleChart
                {
                    Key = r.ProcName,
                    Value = totalSeconds
                });
            }
        }


        ////////////////



        /*
        /// <summary>
        /// 円グラフを描画してみる
        /// <see cref="http://www.c-sharpcorner.com/uploadfile/mahesh/pie-chart-in-wpf/"/>
        /// </summary>
        private void DrawChart()
        {
            points = new ObservableCollection<CircleChart>();

            var q = dbOperator.GetAllData();

            foreach (dynamic r in q)
            {
                //usageTimeから合計時間を秒で取得
                //http://dobon.net/vb/dotnet/system/timespan.html
                double totalSeconds = (TimeSpan.Parse(r.UsageTime)).TotalSeconds;

                //グラフに表示する項目の追加
                points.Add(new CircleChart
                {
                    Key = r.ProcName,
                    Value = totalSeconds
                });
            }

            //Pages.StatisticalPage pageStatistical = new Pages.StatisticalPage();
            //((PieSeries)pageStatistical.piChart1.Series[0]).ItemsSource = points;

            ((PieSeries)piChart1.Series[0]).ItemsSource = points;
        }
         * */
    }


    public class CircleChart
    {
        public string Key { get; set; }
        public double Value { get; set; }

    }

}

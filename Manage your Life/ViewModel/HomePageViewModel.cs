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
    public class HomePageViewModel : INotifyPropertyChanged
    {
        #region Field

        public List<double> FontSizes { get; set; }
        public List<double> DoughnutInnerRadiusRatios { get; set; }
        public Dictionary<string, ResourceDictionaryCollection> Palettes { get; set; }
        public List<string> SelectionBrushes { get; set; }

        public ObservableCollection<string> ChartTypes { get; set; }
        public ObservableCollection<ChartData> SystemUpTime { get; set; }
        public ObservableCollection<ChartData> UsageTime { get; set; }

        ApplicationDataClassesDataContext database;
        string basePath;
        string connStr;

        #endregion

        //コンストラクタ
        public HomePageViewModel()
        {
            LoadPalettes();
            Settings();

            SystemUpTime = new ObservableCollection<ChartData>();
            Series = new ObservableCollection<SeriesData>();

            SystemUpTime.Add(new ChartData() { Category = "", Number = GetSystemUpTimePercentage() });
                      
            Series.Add(new SeriesData() { SeriesDisplayName = "SystemUpTime", Items = SystemUpTime });
            Series.Add(new SeriesData() { SeriesDisplayName = "ApplicationUsageTime", Items = TakeUsageTimeItems(5) });
        }


        /// <summary>
        /// パソコンの稼働時間を24時間に対してのパーセンテージで求める
        /// </summary>
        /// <returns></returns>
        private double GetSystemUpTimePercentage()
        {
            //パソコン稼働時間取得
            PerformanceCounter upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();

            double upTimeTotalMinutes = (TimeSpan.FromSeconds(upTime.NextValue())).TotalMinutes;
            //パーセンテージ
            double upTimePercentage = upTimeTotalMinutes / TimeSpan.FromHours(24).TotalMinutes;

            return upTimePercentage * 100;
        }


        /// <summary>
        /// データベースから使用時間の上位数項目を取得
        /// </summary>
        /// <param name="takeNumber">取得する項目数</param>
        /// <returns>使用時間とプロセス名のCollection</returns>
        private ObservableCollection<ChartData> TakeUsageTimeItems(int takeNumber)
        {
            //データベース接続
            basePath = Directory.GetCurrentDirectory() + @"\ApplicationDatabase.mdf";
            connStr = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=""" + basePath + @""";Integrated Security=True";
            database = new ApplicationDataClassesDataContext(connStr);

            UsageTime = new ObservableCollection<ChartData>();
            

            //使用時間のうちトップtakeNumberを選択
            var q = (
                from p in database.DatabaseApplication
                orderby p.DatabaseDate.UsageTime descending
                select new
                {
                    Title = p.Title,
                    ProcName = p.DatabaseProcess.Name,
                    UsageTime = p.DatabaseDate.UsageTime,
                })
                .Take(takeNumber);


            //抽出された使用時間の合計を算出
            double sumOfUsageTime = 0;
            foreach (var r in q)
            {
                sumOfUsageTime += (TimeSpan.Parse(r.UsageTime)).TotalSeconds;
            }


            //グラフに表示する項目の追加
            foreach (var r in q)
            {
                //usageTimeから合計時間を秒で取得
                //http://dobon.net/vb/dotnet/system/timespan.html
                TimeSpan usageTime = TimeSpan.Parse(r.UsageTime);

                //パーセンテージにする
                double floorUsageTime = (usageTime.TotalSeconds / sumOfUsageTime) * 100;

                UsageTime.Add(new ChartData()
                {
                    Category = r.ProcName,
                    //小数点2以下四捨五入
                    Number = ToRoundDown(floorUsageTime, 2),
                    TotalHours = ToRoundDown(usageTime.TotalHours, 2),
                    Days = usageTime.Days,
                    Hours = usageTime.Hours,
                    Minutes = usageTime.Minutes,
                    Seconds = usageTime.Seconds
                });
            }

            return UsageTime;
        }


        /// <summary>
        /// 指定した精度の数値に切り捨てします。
        /// </summary>
        /// <see cref="http://jeanne.wankuma.com/tips/csharp/math/rounddown.html"/>
        /// <param name="dValue">丸め対象の倍精度浮動小数点数</param>
        /// <param name="iDigits">戻り値の有効桁数の精度</param>
        /// <returns>iDigits に等しい精度の数値に切り捨てられた数値</returns>
        public double ToRoundDown(double dValue, int iDigits)
        {
            double dCoef = System.Math.Pow(10, iDigits);

            return dValue > 0 ? System.Math.Floor(dValue * dCoef) / dCoef :
                                System.Math.Ceiling(dValue * dCoef) / dCoef;
        }



        #region Method

        /// <summary>
        /// Chartの設定
        /// </summary>
        private void Settings()
        {
            ChartTypes = new ObservableCollection<string>();
            ChartTypes.Add("All");
            ChartTypes.Add("Column");
            ChartTypes.Add("StackedColumn");
            ChartTypes.Add("Bar");
            ChartTypes.Add("StackedBar");
            ChartTypes.Add("Pie");
            ChartTypes.Add("Doughnut");
            ChartTypes.Add("Gauge");
            SelectedChartType = ChartTypes.FirstOrDefault();

            FontSizes = new List<double>();
            FontSizes.Add(9.0);
            FontSizes.Add(11.0);
            FontSizes.Add(13.0);
            FontSizes.Add(18.0);
            SelectedFontSize = 11.0;

            DoughnutInnerRadiusRatios = new List<double>();
            DoughnutInnerRadiusRatios.Add(0.90);
            DoughnutInnerRadiusRatios.Add(0.75);
            DoughnutInnerRadiusRatios.Add(0.5);
            DoughnutInnerRadiusRatios.Add(0.25);
            DoughnutInnerRadiusRatios.Add(0.1);
            SelectedDoughnutInnerRadiusRatio = 0.75;

            SelectionBrushes = new List<string>();
            SelectionBrushes.Add("Orange");
            SelectionBrushes.Add("Red");
            SelectionBrushes.Add("Yellow");
            SelectionBrushes.Add("Blue");
            SelectionBrushes.Add("[NoColor]");
            SelectedBrush = SelectionBrushes.FirstOrDefault();
        }


        /// <summary>
        /// ぱれっとの設定
        /// </summary>
        private void LoadPalettes()
        {
            Palettes = new Dictionary<string, ResourceDictionaryCollection>();
            Palettes.Add("Default", null);

            var resources = Application.Current.Resources.MergedDictionaries.ToList();
            foreach (var dict in resources)
            {
                foreach (var objkey in dict.Keys)
                {
                    if (dict[objkey] is ResourceDictionaryCollection)
                    {
                        Palettes.Add(objkey.ToString(), dict[objkey] as ResourceDictionaryCollection);
                    }
                }
            }

            SelectedPalette = Palettes.FirstOrDefault();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region Property

        private string selectedChartType = null;
        public string SelectedChartType
        {
            get
            {
                return selectedChartType;
            }
            set
            {
                selectedChartType = value;
                NotifyPropertyChanged("SelectedChartType");
            }
        }

        private object selectedPalette = null;
        public object SelectedPalette
        {
            get
            {
                return selectedPalette;
            }
            set
            {
                selectedPalette = value;
                NotifyPropertyChanged("SelectedPalette");
            }
        }

        private bool darkLayout = false;
        public bool DarkLayout
        {
            get
            {
                return darkLayout;
            }
            set
            {
                darkLayout = value;
                NotifyPropertyChanged("DarkLayout");
                NotifyPropertyChanged("Foreground");
                NotifyPropertyChanged("Background");
                NotifyPropertyChanged("MainBackground");
                NotifyPropertyChanged("MainForeground");
            }
        }

        public string Foreground
        {
            get
            {
                if (darkLayout)
                {
                    return "#FFEEEEEE";
                }
                return "#FF666666";
            }
        }
        public string MainForeground
        {
            get
            {
                if (darkLayout)
                {
                    return "#FFFFFFFF";
                }
                return "#FF666666";
            }
        }
        public string Background
        {
            get
            {
                if (darkLayout)
                {
                    return "#FF333333";
                }
                return "#FFF9F9F9";
            }
        }
        public string MainBackground
        {
            get
            {
                if (darkLayout)
                {
                    return "#FF000000";
                }
                return "#FFEFEFEF";
            }
        }


        private string selectedBrush = null;
        public string SelectedBrush
        {
            get
            {
                return selectedBrush;
            }
            set
            {
                selectedBrush = value;
                NotifyPropertyChanged("SelectedBrush");
            }
        }

        private double selectedDoughnutInnerRadiusRatio = 0.75;
        public double SelectedDoughnutInnerRadiusRatio
        {
            get
            {
                return selectedDoughnutInnerRadiusRatio;
            }
            set
            {
                selectedDoughnutInnerRadiusRatio = value;
                NotifyPropertyChanged("SelectedDoughnutInnerRadiusRatio");
                NotifyPropertyChanged("SelectedDoughnutInnerRadiusRatioString");
            }
        }

        public string SelectedDoughnutInnerRadiusRatioString
        {
            get
            {
                return String.Format("{0:P1}.", SelectedDoughnutInnerRadiusRatio);
            }
        }


        private bool isRowColumnSwitched = false;
        public bool IsRowColumnSwitched
        {
            get
            {
                return isRowColumnSwitched;
            }
            set
            {
                isRowColumnSwitched = value;
                NotifyPropertyChanged("IsRowColumnSwitched");
            }
        }

        private bool isLegendVisible = true;
        public bool IsLegendVisible
        {
            get
            {
                return isLegendVisible;
            }
            set
            {
                isLegendVisible = value;
                NotifyPropertyChanged("IsLegendVisible");
            }
        }

        private bool isTitleVisible = true;
        public bool IsTitleVisible
        {
            get
            {
                return isTitleVisible;
            }
            set
            {
                isTitleVisible = value;
                NotifyPropertyChanged("IsTitleVisible");
            }
        }

        private double fontSize = 11.0;
        public double SelectedFontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                NotifyPropertyChanged("SelectedFontSize");
                NotifyPropertyChanged("SelectedFontSizeString");
            }
        }

        public string SelectedFontSizeString
        {
            get
            {
                return SelectedFontSize.ToString() + "px";
            }
        }

        private object selectedItem = null;
        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        public ObservableCollection<SeriesData> Series
        {
            get;
            set;
        }


        public string ToolTipFormat
        {
            get
            {
                return "ProcessName: {0}, UsageSeconds: {1} ({3:P2})";
            }
        }

        #endregion
    }
     

}

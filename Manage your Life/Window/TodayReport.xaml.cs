using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CoreTweet;

namespace Manage_your_Life
{
    /// <summary>
    /// TodayReport.xaml の相互作用ロジック
    /// </summary>
    public partial class TodayReport : Window
    {
        DataBanker dataBanker;
        string tweetString = Properties.Settings.Default.TweetFormat;
           

        public TodayReport()
        {
            InitializeComponent();

            dataBanker = DataBanker.Instance;

            this.border.BorderBrush = new SolidColorBrush(FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor);
        }


        //画面に表示させる項目の設定
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //使用期間
            DateTime startDate = (DateTime)dataBanker["StartUpTime"];
            this.textBox_Period.Text = 
                String.Format("{0} -> {1}", startDate.ToString(), DateTime.Now.ToString());


            //システムの稼働時間
            PerformanceCounter upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();
            string strUpTime = (TimeSpan.FromSeconds((int)upTime.NextValue())).ToString(@"hh\:mm\:ss");
            this.textBox_SystemUpTime.Text = strUpTime;


            //警告ウィンドウ表示回数
            string warningCount = ((int)dataBanker["WarningCount"]).ToString();
            this.textBox_WarningCount.Text = warningCount;


            //アプリケーション使用時間Top5
            string usageTime = "";
            foreach (var d in GetTodayUsageTime(startDate))
            {
                usageTime += String.Format("{0} ({1})\n", d.Key, d.Value.ToString(@"hh\:mm\:ss"));                
            }
            this.textBox_Application.Text = usageTime.TrimEnd('\n');            

            
            //カテゴリTop5
            var CategorizedCountData =
                    (Dictionary<string, int>)dataBanker["CategorizedCountData"] ?? new Dictionary<string, int>();
            string categoriCount = "";
            foreach (var d in CategorizedCountData.OrderByDescending((x) => x.Value).Take(5))
            {
                categoriCount += String.Format("{0}\t: {1}\n", d.Key, d.Value);
            }
            this.textBox_Categories.Text = categoriCount.TrimEnd('\n');



            //何もデータがないとSplitで例外発生           
            tweetString = tweetString.Replace("<UPTIME>", strUpTime);
            dataBanker["TweetConfirm"] = tweetString.Replace("<WARNING_COUNT>", warningCount);
            try
            {
                dataBanker["TweetConfirm"] = ((string)dataBanker["TweetConfirm"]).Replace("<USAGE_TIME1>", usageTime.Split('\n')[0]);
                dataBanker["TweetConfirm"] = ((string)dataBanker["TweetConfirm"]).Replace("<USAGE_TIME2>", usageTime.Split('\n')[1]);
            }
            catch (IndexOutOfRangeException ex) 
            {
                dataBanker["TweetConfirm"] = ((string)dataBanker["TweetConfirm"]).Replace("<USAGE_TIME2>", "");
            }
            try
            {
                dataBanker["TweetConfirm"] = ((string)dataBanker["TweetConfirm"]).Replace("<CATEGORI_COUNT1>", categoriCount.Split('\n')[0]);
                dataBanker["TweetConfirm"] = ((string)dataBanker["TweetConfirm"]).Replace("<CATEGORI_COUNT2>", categoriCount.Split('\n')[1]);
            }
            catch (IndexOutOfRangeException ex) 
            {
                dataBanker["TweetConfirm"] = ((string)dataBanker["TweetConfirm"]).Replace("<CATEGORI_COUNT2>", "");
            }
        }



        /// <summary>
        /// TodayReportで表示する用の、アプリケーションとその使用時間を取得する
        /// </summary>
        /// <param name="startDate">PCの起動時刻</param>
        /// <returns>降順のアプリケーションとその使用時間のDictionary</returns>
        private Dictionary<string, TimeSpan> GetTodayUsageTime(DateTime startDate)
        {
            DatabaseOperation dbOp = DatabaseOperation.Instance;
            var database = dbOp.GetConnectionedDataContext;

            var q = (
                    from p in database.DatabaseApplication
                    where startDate <= p.DatabaseTimeline.Today 
                        && p.DatabaseTimeline.Today.Value <= DateTime.Now
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = TimeSpan.Parse(p.DatabaseTimeline.UsageTime)
                    });

            
            //データベースから抽出したデータについて、重複したプロセス名を単一にまとめる
            var dict = Utility.DictionaryOrganizingValue(q);

            //降順にソートして先頭から5番目までをDictionaryに追加
            var retDict = new Dictionary<string, TimeSpan>();
            foreach (var d in dict.OrderByDescending((x) => x.Value).Take(5))
            {
                retDict.Add(d.Key, d.Value);
            }

            return retDict;
        }



        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// ウィンドウをドラッグで動かせるようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            PostMessage(hwnd, 0xA1, (IntPtr)2, IntPtr.Zero);
        }





//----------------------------------------------------------------ボタンイベントハンドラ

        /// <summary>
        /// OKボタン押下で終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Tweetボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Tweet_Click(object sender, RoutedEventArgs e)
        {
            string filename = "tweet.png";
            if (Properties.Settings.Default.checkBox_IsTweetImage)
            {
                //画像の取得
                var bitmapEncoder = Utility.RenderingVisual(this);
                using (Stream stram = File.Create(filename))
                {
                    bitmapEncoder.Save(stram);
                }
            }

            TweetConfirm window = new TweetConfirm();
            this.Hide();
            window.Show();
            this.Close();
        }
    }
}

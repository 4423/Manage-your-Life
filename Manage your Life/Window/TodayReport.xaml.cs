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
        string tweetString = "#TodayReport\n";

        public TodayReport()
        {
            InitializeComponent();

            dataBanker = DataBanker.GetInstance();

            this.border.BorderBrush = new SolidColorBrush(FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //使用期間
            DateTime startDate = (DateTime)dataBanker["StartUpTime"];
            this.textBox_Period.Text = 
                String.Format("{0} -> {1}", startDate.ToString(), DateTime.Now.ToString());


            //システムの稼働時間
            PerformanceCounter upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();
            string upTimeString = (TimeSpan.FromSeconds((int)upTime.NextValue())).ToString(@"d\:hh\:mm\:ss");
            this.textBox_SystemUpTime.Text = upTimeString;
            tweetString += String.Format("SystemUpTime: {0}\n", upTimeString);


            //アプリケーション使用時間Top5
            string printApplicationUsageTime = "";
            foreach (var d in GetTodayUsageTime(startDate))
            {
                printApplicationUsageTime += String.Format("{0} ({1})\n", d.Key, d.Value.ToString(@"hh\:mm\:ss"));                
            }
            this.textBox_Application.Text = printApplicationUsageTime.TrimEnd('\n');            

            
            //null合体演算子
            var CategorizedCountData =
                (Dictionary<string, int>)dataBanker["CategorizedCountData"] ?? new Dictionary<string, int>();
            string printCategorizedCountData = "";

            //カテゴリTop5
            foreach (var d in CategorizedCountData.OrderByDescending((x) => x.Value).Take(5))
            {
                printCategorizedCountData += String.Format("{0}\t: {1}\n", d.Key, d.Value);
            }
            this.textBox_Categories.Text = printCategorizedCountData.TrimEnd('\n');


            //何もカテゴリデータがないとSplitで死ぬ
            try
            {
                tweetString += "\nApplicationUsageTime:\n";
                var tweetUsageTime = printApplicationUsageTime.Split('\n');
                tweetString += String.Format("{0}\n{1}\n", tweetUsageTime[0], tweetUsageTime[1]);
                tweetString += "\nCategoriesCount:\n";
                var tweetCategories = printCategorizedCountData.Split('\n');
                tweetString += String.Format("{0}\n{1}", tweetCategories[0], tweetCategories[1]);
            }
            catch (Exception ex) { }
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
            var tokens = Tokens.Create(
                "nV7WUMvQV0WNoXaL2jxb47ydC",
                "gAje4KL3JL9Y6Sfr2KnMNlrhxdX6Bf2xcgYMjnFyquxZ4z1aGw",
                "634603423-msefji4BeiSoRMXJW96YXIXBrWTiN6IjScPCIArp",
                "Sz1WGwB2nvMs9myblrOek7Qyc1mOFBs3SqAswRkIucMMJ"
                );
            string filename = "test.png";



            //画像の取得
            var bitmapEncoder = Utility.RenderingVisual(this);
            using (Stream stram = File.Create(filename))
            {
                bitmapEncoder.Save(stram);
            }

            //画像の投稿
            MediaUploadResult result = tokens.Media.Upload(media => new FileInfo(filename));
            //Status a =  tokens.Statuses.Update(status => "test", media_ids => new long[] { result.MediaId });



            if (tweetString.Length >= 140)
            {
                MessageBox.Show(String.Format("文字数がオーバーしました。 ({0})", tweetString.Length), "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                tweetString = "";
                return;
            }

            if (MessageBoxResult.OK == MessageBox.Show(tweetString, "Tweet内容の確認"))
            {
                try
                {
                    tokens.Statuses.Update(new { status = tweetString });
                    MessageBox.Show("Tweetしました。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    tweetString = "";
                }
            }            
        }
    }
}

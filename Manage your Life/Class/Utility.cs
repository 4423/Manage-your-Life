using Manage_your_Life.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Manage_your_Life
{
    static public class Utility
    {

        /// <summary>
        /// 指定した精度の数値に切り捨る
        /// </summary>
        /// <see cref="http://jeanne.wankuma.com/tips/csharp/math/rounddown.html"/>
        /// <param name="value">切り捨てる対象の数字</param>
        /// <param name="iDigits">切り捨てない小数点桁数</param>
        /// <returns>切り捨てられた数値</returns>
        public static double ToRoundDown(double value, int iDigits)
        {
            double coefficient = Math.Pow(10, iDigits);

            return value > 0 ? Math.Floor(value * coefficient) / coefficient : 
                Math.Ceiling(value * coefficient) / coefficient;
        }


        /// <summary>
        /// 使用間隔を計算
        /// 現在の時間から、最初にウィンドウがアクティブになったときまでの時間を計算
        /// </summary>
        /// <param name="firstActiveDate">最初にウィンドウがアクティブになったときのDateTime</param>
        /// <returns></returns>
        internal static TimeSpan GetInterval(DateTime firstActiveDate)
        {
            return DateTime.Now - firstActiveDate;
        }



        /// <summary>
        /// FormアプリケーションのDoEventsの代替
        /// </summary>
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);
            Dispatcher.PushFrame(frame);
        }
        public static object ExitFrames(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }


        /// <summary>
        /// aとbの中身を入れ替える。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a; 
            a = b;
            b = c;
        }



        /// <summary>
        /// データベースの中身が初期状態のままかどうか
        /// </summary>
        /// <returns>true: 初期状態　false: 何か入ってる</returns>
        public static bool IsDatabaseEmpty()
        {
            var dbOperator = DatabaseOperation.Instance;
            foreach (var nullTest in dbOperator.GetAllData())
            {
                return false;
            }

            return true;
        }



        /// <summary>
        /// 文字列がTimeSpanに変換出来るか
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean IsTimeSpanFormat(string value)
        {
            try
            {
                TimeSpan.Parse(value);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }



        /// <summary>
        /// 自身のスクリーンショットをレンダリングする
        /// </summary>
        /// <see cref="http://urx.nu/b0Bh"/>
        /// <param name="target">pngイメージ</param>
        public static PngBitmapEncoder RenderingVisual(Visual target)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);

            var renderTarget = new RenderTargetBitmap((Int32)bounds.Width,
                                (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(target);
                context.DrawRectangle(visualBrush, null, new Rect(new Point(), bounds.Size));
            }

            renderTarget.Render(visual);
            PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            return bitmapEncoder;
        }


        /// <summary>
        /// StringCollectionのクローンを作る
        /// 
        /// 愚直にthis.listBox.Items.Add(this.textBox_ItemAdd.Text)とかやると、
        /// 使用中のやつ変更するなと怒られるので、
        /// 参照渡しを回避するために遠回りな方法でクローンを作った
        /// </summary>
        /// <param name="preCategoryNGWord">クローン元のStringCollection</param>
        /// <returns>まっさらなStringCollection</returns>
        public static StringCollection CreateClone(StringCollection preCategoryNGWord)
        {
            string[] copyTmp = new string[preCategoryNGWord.Count];
            preCategoryNGWord.CopyTo(copyTmp, 0);

            StringCollection categoryNGWords = new StringCollection();
            categoryNGWords.AddRange(copyTmp);

            return categoryNGWords;
        }

//----------------------------------------------------------------Dictionary

        /// <summary>
        /// キーに対応する値をカウント
        /// Hatenaで使う
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, int> 
            KeysCount(IEnumerable<KeyValuePair<string, string>> enumerable)
        {
            Dictionary<string, int> count = new Dictionary<string, int>();

            var dictionary = new Dictionary<string, string>();

            foreach (var data in enumerable)
            {
                //カウンタDictionaryに既にキーが含まれていれば
                if (count.ContainsKey(data.Value))
                {
                    //キーに対応する値を+1
                    count[data.Value]++;
                }
                //キーが含まれていなければ、そのキーを追加
                else
                {
                    count.Add(data.Value, 1);
                }
            }

            return count;
        }



        /// <summary>
        /// 二つのDictionary<string,int>のValueについて、同じKey同士で加算する
        /// </summary>
        /// <param name="dict1">一つ目のDictionary</param>
        /// <param name="dict2">二つ目のDictionary</param>
        /// <returns>Valueについて加算されたDictionary</returns>
        public static Dictionary<string, int>
            MergeDictionaryValue(Dictionary<string, int> dict1, Dictionary<string, int> dict2)
        {
            var mergeDict = new Dictionary<string, int>(dict2);

            //一つ目のDictionaryのKeyについて二つ目で重複するKeyがあれば加算して作成
            foreach (var data1 in dict1)
            {
                foreach (var data2 in dict2)
                {
                    if (data1.Key == data2.Key)
                    {
                        mergeDict[data2.Key] += data1.Value;                        
                    }
                }
            }

            //一つ目にはないKeyが二つ目にあれば追加
            foreach (var data1 in dict1)
            {
                if (!mergeDict.ContainsKey(data1.Key))
                {
                    mergeDict.Add(data1.Key, data1.Value);
                }
            }

            return mergeDict;
        }



        /// <summary>
        /// 重複したKeyのもつValueを、一つのKeyとValueの合計なDictionaryに整理する
        /// </summary>
        /// <param name="q">整理の対象となるCollection。プロパティ名はKeyとValue</param>
        /// <returns>整理されたDictionary</returns>
        public static Dictionary<string, TimeSpan> DictionaryOrganizingValue(dynamic q)
        {
            Dictionary<string, TimeSpan> dict = new Dictionary<string, TimeSpan>();

            try
            {
                foreach (var r in q)
                {
                    //DictionaryにKeyが含まれていればValueを加算
                    if (dict.ContainsKey(r.Key))
                    {
                        dict[r.Key] += r.Value;
                    }
                    //含まれていなければ新規追加
                    else
                    {
                        dict.Add(r.Key, r.Value);
                    }
                }
            }
            catch (Exception ex) { }

            return dict;
        }

        

        /// <summary>
        /// orderの指定によってDictionaryを昇順または降順にソートする
        /// </summary>
        /// <param name="order">ソートの指定 Ascending / Descending</param>
        /// <param name="dict">ソートするDictionary</param>
        /// <returns>ソートされたDictionary</returns>
        public static IOrderedEnumerable<KeyValuePair<string, TimeSpan>>
            SortingDictionary(string order, Dictionary<string, TimeSpan> dict)
        {
            IOrderedEnumerable<KeyValuePair<string, TimeSpan>> sortedDict = null;

            switch (order)
            {
                case "Ascending":
                    sortedDict = dict.OrderBy((x) => x.Value);
                    break;

                case "Descending":
                    sortedDict = dict.OrderByDescending((x) => x.Value);
                    break;
            }

            return sortedDict;
        }

    }
}

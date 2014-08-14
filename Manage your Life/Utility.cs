using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Manage_your_Life
{
    static public class Utility
    {
        

        /// <summary>
        /// 指定した精度の数値に切り捨てします。
        /// </summary>
        /// <see cref="http://jeanne.wankuma.com/tips/csharp/math/rounddown.html"/>
        /// <param name="dValue">丸め対象の倍精度浮動小数点数</param>
        /// <param name="iDigits">戻り値の有効桁数の精度</param>
        /// <returns>iDigits に等しい精度の数値に切り捨てられた数値</returns>
        public static double ToRoundDown(double dValue, int iDigits)
        {
            double dCoef = System.Math.Pow(10, iDigits);

            return dValue > 0 ? System.Math.Floor(dValue * dCoef) / dCoef :
                                System.Math.Ceiling(dValue * dCoef) / dCoef;
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
        /// a と bの中身を入れ替える。
        /// </summary>
        /// <param name="a">オペランドa</param>
        /// <param name="b">オペランドb</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a; a = b; b = c;
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
                        //mergeDict.Add(data1.Key, data1.Value + data2.Value);
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

            foreach (var r in q)
            {
                //Dictionaryに既にKeyが含まれていなければ
                if (!dict.ContainsKey(r.Key))
                {
                    dict.Add(r.Key, r.Value);
                }
                //含まれていればそのKeyに対してValueを加算
                else
                {
                    dict[r.Key] += r.Value;
                }
            }

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

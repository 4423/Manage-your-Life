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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Manage_your_Life
{
    class HatenaKeywordViewModel : ViewModel
    {
        
        public ObservableCollection<ChartData> HatenaKeyword { get; set; }

        private Dictionary<string, int> hatenaCategorizedCountData;


        /// <summary>
        /// </summary>
        /// <param name="windowTitle">windowTitle: HatenaAPIに飛ばす文字列</param>
        /// <param name="preHatenaKeywordCollections">今までのHatenaでカテゴライズされたコレクション</param>
        public HatenaKeywordViewModel(string windowTitle, Dictionary<string, int> preHatenaCategorizedCountData)
        {

            HatenaKeyword = new ObservableCollection<ChartData>();
            Series = new ObservableCollection<SeriesData>();

            Debug.WriteLine("Hatena");
            Hatena hatena = new Hatena(0);
            Dictionary<string, string> hatenaCategorizedData = hatena.Categorizing(windowTitle);

            //カテゴライズさせて得られるものが無いとnullになる
            if (hatenaCategorizedData != null)
            {
                //重複したカテゴリーの数をカウント
                hatenaCategorizedCountData = Utility.KeysCount(hatenaCategorizedData);

                //いままでのカウント分と結合
                hatenaCategorizedCountData = 
                    Utility.MergeDictionaryValue(hatenaCategorizedCountData, preHatenaCategorizedCountData);

                foreach (var data in hatenaCategorizedCountData)
                {
                    HatenaKeyword.Add(new ChartData() { Category = data.Key, Number = data.Value });
                }

                Series.Add(new SeriesData() { SeriesDisplayName = "HatenaKeyword", Items = HatenaKeyword });    
            }
            
        }



        public Dictionary<string, int> GetHatenaCategorizedCountData
        {
            get { return hatenaCategorizedCountData; }
        }

    }
}

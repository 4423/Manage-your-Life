using Manage_your_Life.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Manage_your_Life
{
    class HatenaKeywordViewModel : ViewModel
    {
        
        public ObservableCollection<ChartData> HatenaKeyword { get; set; }

        public Dictionary<string, int> CategorizedCountData { get; set; }


        /// <summary>
        /// HatenaChartのViewModelのコンストラクタ
        /// </summary>
        /// <param name="windowTitle">windowTitle: HatenaAPIに飛ばす文字列</param>
        /// <param name="preHatenaKeywordCollections">今までのHatenaでカテゴライズされたコレクション</param>
        public HatenaKeywordViewModel(string windowTitle, Dictionary<string, int> preCategorizedCountData)
        {
            HatenaKeyword = new ObservableCollection<ChartData>();
            Series = new ObservableCollection<SeriesData>();

            Hatena hatena = new Hatena(0);
            Dictionary<string, string> hatenaCategorizedData = hatena.Categorizing(windowTitle);

            //カテゴライズさせて得られるものが無いとnullになる
            if (hatenaCategorizedData != null)
            {
                //重複したカテゴリーの数をカウント
                CategorizedCountData = Utils.KeysCount(hatenaCategorizedData);

                //いままでのカウント分と結合
                CategorizedCountData = 
                    Utils.MergeDictionaryValue(CategorizedCountData, preCategorizedCountData);

                foreach (var data in CategorizedCountData)
                {
                    HatenaKeyword.Add(new ChartData() { Category = data.Key, Number = data.Value });
                }

                Series.Add(new SeriesData() { SeriesDisplayName = "HatenaKeyword", Items = HatenaKeyword });    
            }            
        }
    }
}

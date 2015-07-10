using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life.Models
{

    /// <summary>
    /// Window間でデータをやり取りするシングルトンなクラス
    /// </summary>
    public sealed class DataBanker
    {
        private static readonly DataBanker instance = new DataBanker();
        private IDictionary holder = new Hashtable();


        private DataBanker() { }

        
        public static DataBanker Instance
        {
            get
            {
                return instance;
            }
        }


        /// <summary>
        /// キーに対応する保持しているデータを返す
        /// </summary>
        /// <param name="key">欲しいオブジェクトのキー</param>
        /// <returns>キーに対応するオブジェクト</returns>
        public object this[object key]
        {
            get
            {
                return holder[key];
            }
            set
            {
                //キーが重複する場合は元のデータを削除してから登録
                if (holder.Contains(key)) holder.Remove(key);
                holder[key] = value;
            }
        }


        /// <summary>
        /// キーの情報を返す
        /// </summary>
        public ICollection Keys
        {
            get
            {
                return holder.Keys;
            }
        }  


        /// <summary>  
        /// キーに対応するデータを削除する
        /// </summary>  
        /// <param name="key">削除したいオブジェクトのキー</param>  
        public void Remove(string key)
        {
            holder.Remove(key);
        }  
    }
}

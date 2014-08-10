using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life
{

    /// <summary>
    /// Window間でデータをやり取りするシングルトンなクラス
    /// </summary>
    /// <see cref="http://architect360.apricot-jp.com/300/post_2.html"/>
    public sealed class DataBanker
    {
        private static DataBanker _instance = new DataBanker();
        private IDictionary _holder = new Hashtable();


        private DataBanker() { }

        /// <summary>
        /// 自身のインスタンスを取得
        /// </summary>
        /// <returns></returns>
        public static DataBanker GetInstance()
        {
            return _instance;
        }


        /// <summary>
        /// キーを元にデータを取得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[object key]
        {
            get
            {
                return _holder[key];
            }
            set
            {
                if (_holder.Contains(key))
                {
                    _holder.Remove(key);
                }
                _holder[key] = value;
            }
        }


        /// <summary>  
        /// keyの情報を削除する  
        /// </summary>  
        /// <param name="key"></param>  
        public void Remove(string key)
        {
            _holder.Remove(key);
        }


        /// <summary>  
        /// すべての情報を削除する  
        /// </summary>  
        public void RemoveAll()
        {
            _holder.Clear();
        }


        /// <summary>
        /// キーの情報を返す
        /// </summary>
        public ICollection Keys
        {
            get
            {
                return _holder.Keys;
            }
        }  
  
    }
}

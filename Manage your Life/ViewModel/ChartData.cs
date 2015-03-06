using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life
{
    //Chatに表示するデータのクラス
    public class ChartData
    {
        public ChartData() { Number = 0; }

        //項目
        public string Category { get; set; }

        //項目の値
        public double Number { get; set; }
    }
}

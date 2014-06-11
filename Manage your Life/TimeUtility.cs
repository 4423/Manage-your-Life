using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Manage_your_Life
{
    class TimeUtility
    {
        /// <summary>
        /// 使用間隔を計算
        /// </summary>
        /// <param name="firstActiveDate"></param>
        /// <returns></returns>      
        internal TimeSpan GetInterval(DateTime firstActiveDate)
        {
            //現在の時間から、最初のアクティブまでの時間を計算
            return DateTime.Now - firstActiveDate;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Manage_your_Life
{
    class ProcessInformation
    {
        /// <seealso cref="http://mzs184.blogspot.jp/2009/03/c.html"/>
        /// <seealso cref="http://d.hatena.ne.jp/int128/20080110/1199975050"/>

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int length);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


        /// <summary>
        /// 現在アクティブなウィンドウのタイトルを取得
        /// </summary>
        /// <returns>
        /// ウィンドウタイトル
        /// </returns>
        internal string GetWindowTitle()
        {
            IntPtr hWnd = GetForegroundWindow();
            StringBuilder title = new StringBuilder(1048);
            GetWindowText(hWnd, title, 1024);

            return title.ToString();
        }


        /// <summary>
        /// アクティブなプロセスを取得
        /// </summary>
        /// <returns>
        /// Process プロセスID
        /// </returns>
        internal System.Diagnostics.Process GetActiveProcess()
        {
            IntPtr hWnd = GetForegroundWindow();

            int id;
            GetWindowThreadProcessId(hWnd, out id);
            Process procId = Process.GetProcessById(id);

            return procId;
        }
    }
}

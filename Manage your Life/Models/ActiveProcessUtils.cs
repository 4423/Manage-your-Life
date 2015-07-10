using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life.Models
{
    /// <summary>
    /// 最前面に表示されているプロセス情報をP/Invokeする。
    /// </summary>
    internal static class ActiveProcessUtils
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int length);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


        /// <summary>
        /// 最前面に表示されているWindowTitleを取得します。
        /// </summary>
        /// <returns></returns>
        public static string GetWindowTitle()
        {
            IntPtr hWnd = GetForegroundWindow();

            //ウィンドウタイトルを取得する
            StringBuilder title = new StringBuilder(1048);
            GetWindowText(hWnd, title, 1024);

            return title.ToString();
        }


        /// <summary>
        /// 最前面に表示されているProcessを取得します。
        /// </summary>
        /// <returns></returns>
        public static Process GetActiveProcess()
        {
            IntPtr hWnd = GetForegroundWindow();

            int procId;
            GetWindowThreadProcessId(hWnd, out procId);

            return Process.GetProcessById(procId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Manage_your_Life.Models
{
    /// <summary>
    /// アクティブウィンドウなプロセス取得クラス
    /// </summary>
    class ProcessInformation
    {
                
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
            //アクティブウィンドウを取得
            IntPtr hWnd = GetForegroundWindow();

            //ウィンドウタイトルを取得する
            StringBuilder title = new StringBuilder(1048);
            GetWindowText(hWnd, title, 1024);

            return title.ToString();
        }


        /// <summary>
        /// アクティブなプロセスを取得
        /// </summary>
        /// <returns>
        /// Process
        /// </returns>
        internal Process GetActiveProcess()
        {
            //アクティブウィンドウを取得
            IntPtr hWnd = GetForegroundWindow();

            //アクティブウィンドウのハンドルからプロセスIDを取得
            int procId;
            GetWindowThreadProcessId(hWnd, out procId);

            //プロセスIDからプロセスを取得
            return Process.GetProcessById(procId);
        }
    }
}

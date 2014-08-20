using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Manage_your_Life
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {

        //スタートアップ時
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }


        //UIスレッド外での例外
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("致命的な例外が発生しました。\nプログラムを終了します。", "エラー",
                   MessageBoxButton.OK, MessageBoxImage.Error);

            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                WriteErrorLog(ex, "UnhandledException");
            }

            this.Shutdown();
        }

        //UIスレッド内での例外
        private void Application_DispatcherUnhandledException(object sender, 
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("致命的な例外が発生しました。\nプログラムを終了します。", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);

            WriteErrorLog(e.Exception, "ThreadException");

            e.Handled = true;
            this.Shutdown();
        }


        private void WriteErrorLog(Exception ex, string title)
        {
            using (System.IO.StreamWriter stream = new System.IO.StreamWriter("error.txt", true))
            {
                stream.WriteLine("[" + title + "]");
                stream.WriteLine("[message]\r\n" + ex.Message);
                stream.WriteLine("[source]\r\n" + ex.Source);
                stream.WriteLine("[stacktrace]\r\n" + ex.StackTrace);
                stream.WriteLine("\n");
            }
        }
    }
}

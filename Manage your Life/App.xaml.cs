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
            this.Shutdown();
        }

        //UIスレッド内での例外
        private void Application_DispatcherUnhandledException(object sender, 
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("致命的な例外が発生しました。\nプログラムを終了します。", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            this.Shutdown();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace Manage_your_Life
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private bool hasHandledException = false;

        //スタートアップ時
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;           
        }


        //UIスレッド外での例外
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (hasHandledException) return;
            else hasHandledException = true;

            Exception ex = e.ExceptionObject as Exception;
            WriteErrorLog(ex, "UnhandledException");

            var result = TaskDialogShow(ex, true);
            if (result == TaskDialogResult.Ok) Restart();//再起動
            else this.Exit(); //終了
        }
        

        //UIスレッド内での例外
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (hasHandledException) return;
            else hasHandledException = true;

            WriteErrorLog(e.Exception, "ThreadException");
            e.Handled = true;

            var result = TaskDialogShow(e.Exception, true);
            if (result == TaskDialogResult.Ok) Restart();//再起動
            else this.Exit(); //終了
        }


        private void Restart()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Exit();
        }

        private void Exit()
        {
            Environment.Exit(1);
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }


        private void WriteErrorLog(Exception ex, string title)
        {
            if (ex == null) return;
            using (StreamWriter stream = new StreamWriter("errorlog", true))
            {
                stream.WriteLine("[" + title + "]");
                stream.WriteLine("[message]\r\n" + ex.Message);
                stream.WriteLine("[source]\r\n" + ex.Source);
                stream.WriteLine("[stacktrace]\r\n" + ex.StackTrace);
                stream.WriteLine("\n");
            }
        }


        //タスクダイアログ
        private TaskDialogResult TaskDialogShow(Exception ex, bool isUnhandledException)
        {
            var dialog = new TaskDialog();
            dialog.Caption = "Manage your Life";
            dialog.InstructionText = "エラーが発生しました";
            dialog.Text = "Manage your Lifeを終了します。";
            dialog.DetailsCollapsedLabel = "エラー情報";
            dialog.DetailsExpandedLabel = "エラー情報を非表示";
            dialog.DetailsExpandedText = ex.GetType().ToString() + "\n" + ex.Message;
            dialog.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandContent;
            dialog.DetailsExpanded = false;
            dialog.Icon = TaskDialogStandardIcon.Error;
            dialog.Opened += dialog_Opened;

            /*
            var link2 = new TaskDialogCommandLink("link2", "プログラムを終了します");
            link2.Default = true;
            link2.Click += (sender, e) => dialog.Close(TaskDialogResult.Cancel);
            dialog.Controls.Add(link2);

            var link1 = new TaskDialogCommandLink("link1", "プログラムを再起動します");
            link1.Click += (sender, e) => dialog.Close(TaskDialogResult.Ok);
            dialog.Controls.Add(link1);

            var link3 = new TaskDialogCommandLink("link3", "無視します");
            link3.Click += (sender, e) => dialog.Close(TaskDialogResult.None);
            dialog.Controls.Add(link3);
            */

            var shutButton = new TaskDialogButton();
            shutButton.Text = "終了";
            shutButton.Default = true;
            shutButton.Click += (sender, e) => dialog.Close(TaskDialogResult.Cancel);
            dialog.Controls.Add(shutButton);

            var rebootButton = new TaskDialogButton();
            rebootButton.Text = "再起動";
            rebootButton.Click += (sender, e) => dialog.Close(TaskDialogResult.Ok);
            dialog.Controls.Add(rebootButton);

            if (!isUnhandledException)
            {
                var continueButton = new TaskDialogButton();
                continueButton.Text = "続行";
                continueButton.Click += (sender, e) => dialog.Close(TaskDialogResult.None);
                dialog.Controls.Add(continueButton);
            }

            return dialog.Show();
        }

        void dialog_Opened(object sender, EventArgs e)
        {
            TaskDialog dialog = sender as TaskDialog;
            dialog.Icon = TaskDialogStandardIcon.Error;            
        }
    }
}

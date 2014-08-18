using FirstFloor.ModernUI.Presentation;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections;
using System.Collections.Generic;

namespace Manage_your_Life
{
    /// <summary>
    /// Splash.xaml の相互作用ロジック
    /// </summary>
    public partial class Splash : Window
    {
        DispatcherTimer timer;
        DatabaseOperation dbOperator;

        public Splash()
        {
            InitializeComponent();

            AppearanceManager.Current.AccentColor = Properties.Settings.Default.ThemeColor;
            this.border.Background = 
                new SolidColorBrush(FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += new EventHandler(DispatcherTimer_Tick);
            timer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Tick -= new EventHandler(DispatcherTimer_Tick);


            //データベース接続
            label_working.Content = "Connecting to database...";
            Utility.DoEvents();

            try
            {
                dbOperator = DatabaseOperation.Instance;
                new OneDayUsageTimeViewModel(DateTime.Today, 5);
            }
            catch (Exception ex)
            {
                label_working.Content = "Test connection failed...";
                Utility.DoEvents(); Thread.Sleep(1000);
                try
                {
                    label_working.Content = "Reconnecting...";
                    Utility.DoEvents();
                    new OneDayUsageTimeViewModel(DateTime.Today, 5);
                    goto EX;
                }
                catch (Exception exx)
                {
                    label_working.Content = "Error! Please retry.";
                    Utility.DoEvents(); Thread.Sleep(1000); Environment.Exit(1);
                }
            }
        EX:
            //MainWindow作成
            label_working.Content = "Initializing window...";
            Utility.DoEvents();

            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;

            this.Hide();
            main.Show();
            this.Close();
        }
    }
}

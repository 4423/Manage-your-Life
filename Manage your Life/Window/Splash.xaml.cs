using FirstFloor.ModernUI.Presentation;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Manage_your_Life
{
    /// <summary>
    /// Splash.xaml の相互作用ロジック
    /// </summary>
    public partial class Splash : Window
    {
        DispatcherTimer timer;

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

            label_working.Content = "Connecting to database...";
            Utility.DoEvents();

            DatabaseOperation dbOp = DatabaseOperation.Instance;
            new StatisticalPageViewModel();

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

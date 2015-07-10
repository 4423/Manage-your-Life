using FirstFloor.ModernUI.Presentation;
using Manage_your_Life.Models;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Manage_your_Life.Views
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
            this.border.Background = new SolidColorBrush(AppearanceManager.Current.AccentColor);

            //TODO スプラッシュウィンドウを表示させてから処理を始めたいけど、タイミングが上手く合わないのでtimer使ってる…
            //AsyncEntry();
            
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += new EventHandler(DispatcherTimer_Tick);
            timer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Tick -= new EventHandler(DispatcherTimer_Tick);

            Entry();
        }
        

        private async void AsyncEntry()
        {
            await Task.Run(() => {
                Thread.Sleep(2000);
                Entry();
            });
        }

        private void Entry()
        {
            //DB接続テスト
            UIAction(() => label_working.Content = "Connecting to database...");
            RetryHelper.Retry(
                () => { 
                    var db = DatabaseOperation.Instance;
                    new UsageTimeViewModel(DateTime.Today, 5);
                },
                ex => { ExceptionDispatchInfo.Capture(ex).Throw(); },
                ex => ex is SqlException,
                5
            );
            
            //MainWindow初期化
            UIAction(() => label_working.Content = "Initializing window...");
            UIAction(() =>
            {
                var main = new Views.MainWindow();
                App.Current.MainWindow = main;

                this.Hide();
                main.Show();
                this.Close();
            });
        }

        private void UIAction(Action onAction)
        {
            Application.Current.Dispatcher.BeginInvoke(onAction);
            Utils.DoEvents();
        }
    }
}

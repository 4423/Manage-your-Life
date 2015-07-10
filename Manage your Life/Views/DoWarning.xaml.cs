using FirstFloor.ModernUI.Presentation;
using Manage_your_Life.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Manage_your_Life.Views
{
    /// <summary>
    /// DoWarning.xaml の相互作用ロジック
    /// </summary>
    public partial class DoWarning : Window
    {
        private int appId = -1;

        public DoWarning(string procName, int appId, TimeSpan warningTime)
        {
            InitializeComponent();

            this.appId = appId;

            this.border.Background = new SolidColorBrush(AppearanceManager.Current.AccentColor);
            this.textBlock_warning.Text = 
                String.Format("'{0}'の使用時間が{1}分を超えています",procName, (int)warningTime.TotalMinutes);
            this.checkBox_NotAgain.Content =
                String.Format("これ以上'{0}'に対する警告を表示しない", procName);
        }


        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            //これ以上表示しない
            if (checkBox_NotAgain.IsChecked == true)
            {
                //IDをセット
                DataBanker banker = DataBanker.Instance;
                var noWarningId = (List<int>)banker["WarningNotAgain"] ?? new List<int>();
                noWarningId.Add(appId);
                banker["WarningNotAgain"] = noWarningId;
            }

            this.Close();
        }

        
    }
}

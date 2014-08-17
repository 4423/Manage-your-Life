using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Presentation;

namespace Manage_your_Life
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
        }


        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            //これ以上表示しない
            if (checkBox_NotAgain.IsChecked == true)
            {
                //IDをセット
                DataBanker banker = DataBanker.GetInstance();
                var noWarningId = (List<int>)banker["WarningNotAgain"] ?? new List<int>();
                noWarningId.Add(appId);
                banker["WarningNotAgain"] = noWarningId;
            }

            this.Close();
        }

        
    }
}

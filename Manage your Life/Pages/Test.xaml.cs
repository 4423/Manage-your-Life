using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Manage_your_Life
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : UserControl
    {
        public Test()
        {
            InitializeComponent();
                        
            this.chart_Line.DataContext = new OneDayProcessStatus();

            DatabaseOperation dbOperator = DatabaseOperation.Instance;
            this.listBox_ProcName.DataContext = dbOperator.GetAllData();
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (this.calendar.SelectedDates.Count == 0)
            {
                MessageBox.Show("日付が正しく設定されていません。", "エラー", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(this.listBox_ProcName.SelectedItems.Count == 0)
            {
                MessageBox.Show("プロセスが正しく設定されていません。", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //値の取得
            dynamic selectedItem = this.listBox_ProcName.SelectedItem;
            int appId = selectedItem.Id;
            DateTime selectedDay = this.calendar.SelectedDate.Value;

            //Chart描画
            this.chart_Line.DataContext = new OneDayProcessStatus(appId, selectedDay);
            this.chart_Line.Title = String.Format("Usage status of '{0}' ({1})", 
                    selectedItem.ProcName, selectedDay.ToShortDateString());
        }
    }



    
}

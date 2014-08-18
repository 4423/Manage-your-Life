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

            DatabaseOperation dbOperator = DatabaseOperation.Instance;
            this.chart_Line.DataContext = new TestViewModel();
            
            //this.piChart1.DataContext = new TestViewModel();
            //this.piChart1.DataContext = test();

            this.listBox_ProcName.DataContext = dbOperator.GetAllData();
        }

        private void test()
        {
            //データベース接続
            DatabaseOperation dbOperator = DatabaseOperation.Instance;
            var database = dbOperator.GetConnectionedDataContext;

            var q = (
                    from p in database.DatabaseTimeline
                    where p.Today == DateTime.Today
                    select new
                    {
                        Key = p.Now.ToString(),
                        Value = Utility.ToRoundDown((TimeSpan.Parse(p.UsageTime)).TotalMinutes, 3)
                    });

            foreach (var r in q)
            {
                
            }
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
            this.chart_Line.DataContext = new TestViewModel(appId, selectedDay);
            this.chart_Line.Title = String.Format("Usage status of '{0}' ({1})", 
                    selectedItem.ProcName, selectedDay.ToShortDateString());
        }
    }



    
}

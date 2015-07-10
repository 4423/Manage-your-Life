using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using De.TorstenMandelkow.MetroChart;
using Manage_your_Life.Models;

namespace Manage_your_Life.Views.Pages
{
    /// <summary>
    /// Interaction logic for StatisticalPage.xaml
    /// </summary>
    public partial class StatisticalPage : UserControl
    {

        public StatisticalPage()
        {
            InitializeComponent();
            this.chart_Custom.DataContext = new DefaultViewModel();
        }
        



        /// <summary>
        /// Chartの設定項目が正しく設定されているかどうか
        /// </summary>
        /// <returns>true: 正しい, false: 正しくない</returns>
        private bool isCorrectSettings()
        {
            //Calendarが正しく設定されているかどうか
            bool isCalendarCorrectSettings =
                (
                    this.calendar.SelectedDates.Count != 0
                    || this.checkBox_isAllDate.IsChecked == true
                );
            if (!isCalendarCorrectSettings)
            {
                ShowMessageBoxError("カレンダーが正しく設定されていません。");
                return false;
            }


            //ComboBoxが正しく設定(選択)されているかどうか
            bool isComboBoxCorrectSettings =
                (
                    this.comboBox_Item.SelectedIndex != -1
                    && this.comboBox_Order.SelectedIndex != -1
                );
            if (!isComboBoxCorrectSettings)
            {
                ShowMessageBoxError("TargetまたはOrderが正しく設定されていません。");
                return false;
            }


            //Take numberが正しく設定(1位上 && 数字変換可)されているかどうか
            try
            {
                int takeNumber = Convert.ToInt32(this.textBox_takeNumber.Text);
                if (takeNumber < 1) throw new FormatException();
            }
            catch (Exception ex)
            {
                ShowMessageBoxError("Take numberの値が正しく設定されていません。");
                return false;
            }

            return true;
        }


        /// <summary>
        /// MessageBoxのラッパー
        /// </summary>
        /// <param name="message">メッセージ</param>
        private static void ShowMessageBoxError(string message)
        {
            MessageBox.Show(message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }


//-------------------------------------------------------------イベントハンドラ

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!isCorrectSettings()) return;
            
            #region 設定項目の取得
            bool isAllDate = this.checkBox_isAllDate.IsChecked == true;
            bool isFavoritesOnly = this.checkBox_isFavoritesOnly.IsChecked == true;

            DateTime startDay = new DateTime();
            DateTime endDay = new DateTime();

            if (!isAllDate)
            {
                startDay = calendar.SelectedDates.First();
                endDay = calendar.SelectedDates.Last();
            }

            string selectedTarget = this.comboBox_Item.Text;
            string selectedOrder = this.comboBox_Order.Text;

            int takeNumber = Convert.ToInt32(this.textBox_takeNumber.Text);
            #endregion


            //期間の大小関係が不適切な場合は入れ替え
            if (endDay < startDay) Utils.Swap(ref startDay, ref endDay);

            switch (selectedTarget)
            {
                //使用時間について
                case "UsageTime":
                    //すべての日付
                    if (isAllDate)
                    {
                        this.chart_Custom.DataContext =
                            new UsageTimeViewModel(isFavoritesOnly, selectedOrder, takeNumber);
                    }
                    //期間指定
                    else
                    {
                        this.chart_Custom.DataContext =
                            new UsageTimeViewModel(startDay, endDay, isFavoritesOnly, selectedOrder, takeNumber);
                    }
                    break;

                //最終使用日について
                case "LastUsedDate":
                    //すべての日付
                    if (isAllDate)
                    {
                        this.chart_Custom.DataContext =
                            new LastUsedDateViewModel(isFavoritesOnly, selectedOrder, takeNumber);
                    }
                    //期間指定
                    else
                    {
                        this.chart_Custom.DataContext =
                            new LastUsedDateViewModel(startDay, endDay, isFavoritesOnly, selectedOrder, takeNumber);
                    }
                    break;

                //登録日について
                case "RegistrationDate":
                    //すべての日付
                    if (isAllDate)
                    {
                        this.chart_Custom.DataContext =
                            new RegistrationDateViewModel(isFavoritesOnly, selectedOrder, takeNumber);
                    }
                    //期間指定
                    else
                    {
                        this.chart_Custom.DataContext =
                            new RegistrationDateViewModel(startDay, endDay, isFavoritesOnly, selectedOrder, takeNumber);
                    }
                    break;
            }
            
        }//end of Method
    
    }
}

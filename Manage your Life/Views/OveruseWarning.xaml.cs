using FirstFloor.ModernUI.Presentation;
using Manage_your_Life.Models;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Manage_your_Life.Views
{
    /// <summary>
    /// OveruseWarning.xaml の相互作用ロジック
    /// </summary>
    public partial class OveruseWarning : Window
    {
        DatabaseOperation dbOperator;

        public OveruseWarning()
        {
            InitializeComponent();

            dbOperator = DatabaseOperation.Instance;
            
            var themeColor = new SolidColorBrush(AppearanceManager.Current.AccentColor);
            this.border.BorderBrush = themeColor;
            this.border_ListBox.BorderBrush = themeColor;
            this.border_Confirm.BorderBrush = themeColor;
        }


        bool IsWindowLoaded = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.listBox_WariningTarget.DataContext = dbOperator.GetAllData();
            IsWindowLoaded = true;
        }

        
        private void listBox_WariningTarget_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic selectedItem = this.listBox_WariningTarget.SelectedItem;
            if (selectedItem == null) return;

            this.label_ConfirmApp.Content = "警告対象のアプリケーション名： " + selectedItem.ProcName;
        }

        
        int warningTimeNumber;
        private void textBox_WarningTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsWindowLoaded) return;
            if (!Int32.TryParse(this.textBox_WarningTime.Text, out warningTimeNumber)) return;

            this.label_Confirm.Content = 
                String.Format("使用時間の限度：　　　　　　    {0} 分", warningTimeNumber.ToString());
        }



//--------------------------------------------------------ボタン
        
        private void button_Ok_Click(object sender, RoutedEventArgs e)
        {            
            dynamic selectedItem = this.listBox_WariningTarget.SelectedItem;

            //チェック
            bool check = false;
            if (selectedItem == null) check = true;
            if (!Int32.TryParse(this.textBox_WarningTime.Text, out warningTimeNumber)) check = true;
            if (warningTimeNumber < 0) check = true;
            if (check)
            {
                MessageBox.Show("設定を見直してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dynamic message = null;
            if (this.textBox_WarningTime.Text == "0")
            {
                message = String.Format("'{0}'の警告を消去しますか？",
                                    ((dynamic)this.listBox_WariningTarget.SelectedItem).ProcName);
            }
            else
            {
                message = String.Format("'{0}'の使用時間が'{1}'分になった時に警告を表示しますか？",
                                    ((dynamic)this.listBox_WariningTarget.SelectedItem).ProcName,
                                    this.textBox_WarningTime.Text);
            }

            if (MessageBoxResult.Yes == MessageBox.Show(message, "確認", MessageBoxButton.YesNo,MessageBoxImage.Question))
            {
                int appId = selectedItem.Id;

                //DBに警告時間を登録
                dbOperator.SetAlert(appId, TimeSpan.FromMinutes(warningTimeNumber));

                MessageBox.Show("登録しました。\nここで設定した情報は再起動後に反映されます。", "情報", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


//----------------------------------------------------

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// ウィンドウをドラッグで動かせるようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            PostMessage(hwnd, 0xA1, (IntPtr)2, IntPtr.Zero);
        }
    }
}

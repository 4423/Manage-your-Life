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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Manage_your_Life.Properties;
using System.Collections.Specialized;

namespace Manage_your_Life.Content
{
    /// <summary>
    /// Interaction logic for Behavior.xaml
    /// </summary>
    public partial class Behavior : UserControl
    {        
        bool isUserControlLoaded = false;


        public Behavior()
        {
            InitializeComponent();
        }



        //コントロールにユーザー設定を反映
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.button_SettingCautionApp.Visibility = System.Windows.Visibility.Visible;

            if (Utility.IsDatabaseEmpty() || !Settings.Default.checkBox_IsOveruseWarining)
            {
                this.button_SettingCautionApp.Visibility = System.Windows.Visibility.Hidden;
            }

            checkBox_IsOver.IsChecked = Settings.Default.checkBox_IsTweetOver;
            checkBox_IsImage.IsChecked = Settings.Default.checkBox_IsTweetImage;

            checkBox_IsCategorizeStop.IsChecked = Settings.Default.checkBox_IsCategorizeStop;
            slider_TimeSpan.Value = Settings.Default.label_TimeSpan;
            textBlock_TimeSpan.Text = Settings.Default.label_TimeSpan.ToString();
            checkBox_IsOveruseWarining.IsChecked = Settings.Default.checkBox_IsOveruseWarining;

            checkBox_IsBalloonEnable.IsChecked = Settings.Default.checkBox_IsBalloonEnable;
            checkBox_IsReportEnable.IsChecked = Settings.Default.checkBox_IsReportEnable;

            this.border_ListBox.BorderBrush = 
                new SolidColorBrush(FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor);

            isUserControlLoaded = true;
        }
        

        
        //カテゴライズを停止する
        private void checkBox_IsCategorizeStop_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.checkBox_IsCategorizeStop = true;
        }
        private void checkBox_IsCategorizeStop_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.checkBox_IsCategorizeStop = false;
        }

        //カテゴリーの基本取得間隔 (ミリ秒)
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Settingsから値を設定する前に読み込まれると、設定画面が規定値になってしまうため
            if (isUserControlLoaded)
            {
                Settings.Default.label_TimeSpan = (int)this.slider_TimeSpan.Value;
                Settings.Default.Save();
            }
        }



        //使用時間のバルーン通知を有効にする
        private void checkBox_IsBalloonEnable_Checked(object sender, RoutedEventArgs e)
        {
            if (isUserControlLoaded)
            {
                Settings.Default.checkBox_IsBalloonEnable = true;
                Settings.Default.Save();
            }
        }
        private void checkBox_IsBalloonEnable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isUserControlLoaded)
            {
                Settings.Default.checkBox_IsBalloonEnable = false;
                Settings.Default.Save();
            }
        }

        //終了時にToday Reportの表示をする
        private void checkBox_IsReportEnable_Checked(object sender, RoutedEventArgs e)
        {
            if (isUserControlLoaded)
            {
                Settings.Default.checkBox_IsReportEnable = true;
            }
        }
        private void checkBox_IsReportEnable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isUserControlLoaded)
            {
                Settings.Default.checkBox_IsReportEnable = false;
            }
        }

        //使いすぎ警告を有効にする
        private void checkBox_IsOveruseWarining_Checked(object sender, RoutedEventArgs e)
        {
            if (isUserControlLoaded)
            {
                Settings.Default.checkBox_IsOveruseWarining = true;
                this.button_SettingCautionApp.Visibility = System.Windows.Visibility.Visible;
                Settings.Default.Save();
            }
        }

        private void checkBox_IsOveruseWarining_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isUserControlLoaded)
            {
                Settings.Default.checkBox_IsOveruseWarining = false;
                this.button_SettingCautionApp.Visibility = System.Windows.Visibility.Hidden;
                Settings.Default.Save();
            }
        }



        //文字数オーバー時は省略して投稿
        private void checkBox_IsOver_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.checkBox_IsTweetOver = true;
        }
        private void checkBox_IsOver_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.checkBox_IsTweetOver = false;
            checkBox_IsImage.IsChecked = false;
        }

        //TweetにTodayReportのスクリーンショットを含める
        private void checkBox_IsImage_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.checkBox_IsTweetImage = true;
            checkBox_IsOver.IsChecked = true;
        }
        private void checkBox_IsImage_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.checkBox_IsTweetImage = false;
        }


//--------------------------------------------------------ボタン
        //カテゴリーに反映させない単語
        private void button_SettingNGWords_Click(object sender, RoutedEventArgs e)
        {
            this.textBlock_Item.Text = "カテゴリーに反映させない単語";
            this.grid_ListBox.Visibility = System.Windows.Visibility.Visible;

            this.listBox.ItemsSource = Settings.Default.categoryNGWord;
        }
        

        //使いすぎ警告
        private void button_SettingCautionApp_Click(object sender, RoutedEventArgs e)
        {
            OveruseWarning window = new OveruseWarning();
            window.ShowDialog();
        }


//--------------------------------------------------------リストボックス

        private void button_ItemAdd_Click(object sender, RoutedEventArgs e)
        {
            //空の文字は処理しない
            if (String.IsNullOrWhiteSpace(textBox_ItemAdd.Text)) return;

            //クローン作成
            var categoryNGWords = Utility.CreateClone(Settings.Default.categoryNGWord);
            
            //追加
            categoryNGWords.Add(this.textBox_ItemAdd.Text);
            this.listBox.ItemsSource = categoryNGWords;

            this.textBox_ItemAdd.Text = "";
            //クローン保存
            Settings.Default.categoryNGWord = categoryNGWords;
            Settings.Default.Save();            
        }

        private void button_ItemDel_Click(object sender, RoutedEventArgs e)
        {
            //選択項目がない場合は処理をしない
            if (listBox.SelectedItems.Count == 0) return;

            //クローン作成
            var categoryNGWords = Utility.CreateClone(Settings.Default.categoryNGWord);

            //削除
            categoryNGWords.RemoveAt(this.listBox.SelectedIndex);
            this.listBox.ItemsSource = categoryNGWords;

            //クローン保存
            Settings.Default.categoryNGWord = categoryNGWords;
            Settings.Default.Save();
        }

        private void button_ItemSave_Click(object sender, RoutedEventArgs e)
        {
            StringCollection items = new StringCollection();
            foreach (var item in this.listBox.Items)
            {
                items.Add(item.ToString());
            }

            //保存
            Settings.Default.categoryNGWord = items;
            Settings.Default.Save();

            this.grid_ListBox.Visibility = System.Windows.Visibility.Hidden;
        }



//--------------------------------------------------設定終了

        //設定画面から直接×で終了された場合は反映させない
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            isUserControlLoaded = false;
            Settings.Default.Save();
        }

        

    }
}

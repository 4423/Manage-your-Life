﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
//using System.Data.Objects;
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
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using System.Windows.Interop;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using FirstFloor.ModernUI.Windows.Controls;


namespace Manage_your_Life.Pages
{
    /// <summary>
    /// Interaction logic for ApplicationPage.xaml
    /// </summary>
    public partial class ApplicationPage : UserControl
    {

        /// <summary>
        /// データベースを操作
        /// </summary>
        DatabaseOperation dbOperator;
        

        public ApplicationPage()
        {
            InitializeComponent();
            
            dbOperator = new DatabaseOperation();

            //EventHandlerの追加
            dbOperator.UsageTime_Updated += new EventHandler(this.UsageTime_Updated);
            dbOperator.NewRecord_Registered += new EventHandler(this.NewRecord_Registered);

            SetDataGrid();
        }



        /// <summary>
        /// DataGridを再読み込みする
        /// </summary>
        private void SetDataGrid()
        {
            dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = dbOperator.GetAllData();
        }



                



        //-----------------------------------------------------------------イベントハンドラ

        /// <summary>
        /// DBに新規レコードが追加された時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewRecord_Registered(object sender, System.EventArgs e)
        {
            SetDataGrid();
        }


        /// <summary>
        /// DBの使用時間を更新した時
        /// dataGridを更新しつつ選択行は保持したい
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UsageTime_Updated(object sender, System.EventArgs e)
        {
            //選択された行のデータを取得
            int selectedIndex = dataGrid1.SelectedIndex;

            SetDataGrid();

            //何も選択されてなければreturn
            if (selectedIndex == -1) return;
            dataGrid1.SelectedIndex = selectedIndex;
        }


        /// <summary>
        /// DataGridの選択行が変更された時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                #region 実行ファイルパスからアイコンを表示したい
                //選択された行のデータを取得
                int selectedIndex = dataGrid1.SelectedIndex;
                var row = dataGrid1.Items[selectedIndex];

                //上のItemsより生成するObjectのプロパティの中からパスの値を取り出す
                string procPath = row.GetType().GetProperty("ProcPath").GetValue(row).ToString();

                //パスからアイコン生成
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(procPath);

                //see: http://bit.ly/1i44IJo Icon を ImageSource に変換する 
                iconImage.Source = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                #endregion


                //listBoxに現在選択中の行のデータを表示
                listBoxApp.ItemsSource = null;
                listBoxApp.Items.Clear();
                listBoxApp.ItemsSource = BindingListBox(row);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        


        private void editButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ref_Click(object sender, RoutedEventArgs e)
        {
            //SetDataGrid();
        }


        //Datagridの列非表示
        //see: http://ameblo.jp/shirokoma55/entry-11561024241.html
        //dataGrid1.ColumnFromDisplayIndex(0).Visibility = Visibility.Collapsed;

        //Listviewへのバインディングで参考になりそう
        //see: http://gushwell.ldblog.jp/archives/52333865.html





        //---------------------------------------------------------------ListBoxのバインディングとか

        public ObservableCollection<AppListBoxBindingData> ListData { get; set; }


        /// <summary>
        /// ListBoxのItemsSourceに流すデータを生成
        /// </summary>
        /// <param name="row">gridView選択行のデータ</param>
        /// <returns>選択行のobjectをコレクションにしたもの?</returns>
        /// <see cref="http://ufcpp.net/study/csharp/misc_dynamic.html"/>
        private ObservableCollection<AppListBoxBindingData> BindingListBox(dynamic row)
        {
            //コレクションに変更を加えると通知してくれる
            ListData = new ObservableCollection<AppListBoxBindingData>();

            //項目の追加
            //dynamicとかいう謎技術を使用
            ListData.Add(new AppListBoxBindingData
            {
                Title = "タイトル",
                Text = row.Title
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "プロセス名",
                Text = row.ProcName
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "場所",
                Text = row.ProcPath
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "使用時間",
                Text = row.UsageTime.ToString()
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "追加日時",
                Text = row.AddDate.ToString()
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "最終更新日時",
                Text = row.LastDate.ToString()
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "メモ",
                Text = row.Memo
            });

            return ListData;
        }





    }



    /// <summary>
    /// AppListBoxへのバインディング用クラス
    /// <see cref="http://gushwell.ldblog.jp/archives/52306210.html"/>
    /// </summary>
    public class AppListBoxBindingData
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }


}

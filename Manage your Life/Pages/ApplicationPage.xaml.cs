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
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using System.Windows.Interop;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Presentation;


namespace Manage_your_Life
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
        Brush themeColor;
        

        public ApplicationPage()
        {
            InitializeComponent();

            themeColor = new SolidColorBrush(AppearanceManager.Current.AccentColor);

            dbOperator = DatabaseOperation.Instance;

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



        /// <summary>
        /// 選択された行のデータを取得
        /// </summary>
        /// <param name="selectedIndex">選択された行のインデックス</param>
        /// <returns>選択された行のデータ</returns>
        private object GetSelectedItems(int selectedIndex)
        {
            return dataGrid1.Items[selectedIndex];
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
                var row = GetSelectedItems(dataGrid1.SelectedIndex);

                //上のItemsより生成するObjectのプロパティの中からパスの値を取り出す
                string procPath = row.GetType().GetProperty("ProcPath").GetValue(row).ToString();

                //パスからアイコン生成
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(procPath);

                //Icon を ImageSource に変換する 
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


        

        /// <summary>
        /// DataGridの編集ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            //選択行のデータを取得
            dynamic selectedItems = GetSelectedItems(dataGrid1.SelectedIndex);

            //データを格納
            DataBanker context = DataBanker.Instance;
            context["Id"] = selectedItems.Id;
            context["Favorite"] = selectedItems.Favorite;
            context["Title"] = selectedItems.Title;
            context["UsageTime"] = selectedItems.UsageTime;
            context["ProcName"] = selectedItems.ProcName;
            context["ProcPath"] = selectedItems.ProcPath;
            context["AddDate"] = selectedItems.AddDate;
            context["LastDate"] = selectedItems.LastDate;
            context["Memo"] = selectedItems.Memo;

            
            DataGridRowEdit window = new DataGridRowEdit();
            //保存ボタンで終了したらDataGrid再読み込み
            if (window.ShowDialog() == true)
            {
                SetDataGrid();
            }
        }


        //---------------------------------------------------------------ListBoxのバインディングとか

        public ObservableCollection<AppListBoxBindingData> ListData { get; set; }


        /// <summary>
        /// ListBoxのItemsSourceに流すデータを生成
        /// </summary>
        /// <param name="row">gridView選択行のデータ</param>
        /// <returns>選択行のobjectをコレクションにしたもの?</returns>
        private ObservableCollection<AppListBoxBindingData> BindingListBox(dynamic row)
        {
            //コレクションに変更を加えると通知してくれる
            ListData = new ObservableCollection<AppListBoxBindingData>();

            themeColor = new SolidColorBrush(AppearanceManager.Current.AccentColor);

            //項目の追加
            //dynamicとかいう動的解決を使用
            ListData.Add(new AppListBoxBindingData
            {
                Title = "タイトル",
                Text = row.Title,
                Color = themeColor
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "お気に入り",
                Text = row.Favorite.ToString(),
                Color = themeColor
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "プロセス名",
                Text = row.ProcName,
                Color = themeColor
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "場所",
                Text = row.ProcPath,
                Color = themeColor
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "使用時間",
                Text = row.UsageTime.ToString(),
                Color = themeColor
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "追加日時",
                Text = row.AddDate.ToString(),
                Color = themeColor
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "最終更新日時",
                Text = row.LastDate.ToString(),
                Color = themeColor
            });
            ListData.Add(new AppListBoxBindingData
            {
                Title = "メモ",
                Text = row.Memo,
                Color = themeColor
            });

            return ListData;
        }





    }



    /// <summary>
    /// AppListBoxへのバインディング用クラス
    /// </summary>
    public class AppListBoxBindingData
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public Brush Color { get; set; }
    }


}


using Manage_your_Life.Models;
using System.Windows;

namespace Manage_your_Life.Views
{
    /// <summary>
    /// GetTwitterPin.xaml の相互作用ロジック
    /// </summary>
    public partial class GetTwitterPin
    {
        public GetTwitterPin()
        {
            InitializeComponent();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (this.textBox_PIN.Text == "")
            {
                MessageBox.Show("PINが正しく入力されていません。", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataBanker banker = DataBanker.Instance;
            banker["PIN"] = this.textBox_PIN.Text;
            this.Close();
        }
    }
}

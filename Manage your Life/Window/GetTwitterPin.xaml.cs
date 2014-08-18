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

namespace Manage_your_Life
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
            DataBanker banker = DataBanker.GetInstance();
            banker["PIN"] = this.textBox_PIN.Text;
            this.Close();
        }
    }
}

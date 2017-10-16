using FirstFloor.ModernUI.Presentation;
using Manage_your_Life.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Manage_your_Life.Views
{
    /// <summary>
    /// AlertDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AlertDialog : Window
    {
        public AlertDialog()
        {
            InitializeComponent();
            this.closeButton.Click += (_,__) => this.Close();
        }
    }
}

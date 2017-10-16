using FirstFloor.ModernUI.Windows.Controls;
using Manage_your_Life.Models;
using Manage_your_Life.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Manage_your_Life.Views
{

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            this.DataContextChanged += (_, e) =>
            {
                var model = e.NewValue as ViewModels.MainWindowViewModel;
                if (model != null)
                {
                    model.ShowView = this.Show;
                    model.HideView = this.Hide;
                    model.FocusView = this.Focus;
                    model.ActivateView = this.Activate;
                    model.ShowTodayReportDialog = this.ShowTodayReportDialog;
                }
            };

            InitializeComponent();

            this.taskbarIcon.Icon = Properties.Resources.originalIconTray;
            this.Closed += (_, __) => this.taskbarIcon.Dispose();
        }

        
        private void ShowTodayReportDialog()
        {
            var window = new TodayReport();
            window.ShowDialog();
        }        
    }      
}
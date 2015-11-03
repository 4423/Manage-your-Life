using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using System.Windows.Forms;
using System.Windows;


namespace Manage_your_Life.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        private NotifyIcon notifyIcon;


        public void Initialize()
        {
            //コンテキストメニュー追加            
            ToolStripMenuItem exitItem = new ToolStripMenuItem() { Text = "終了(&E)" };
            ToolStripMenuItem openItem = new ToolStripMenuItem() { Text = "開く(&O)" };
            exitItem.Click += new EventHandler(NotifyIconExitClicked);
            openItem.Click += new EventHandler(NotifyIconOpenClicked);

            ContextMenuStrip menuStrip = new ContextMenuStrip();
            menuStrip.Items.Add(openItem);
            menuStrip.Items.Add(new ToolStripSeparator());
            menuStrip.Items.Add(exitItem);

            //バルーン通知の設定
            notifyIcon = new NotifyIcon()
            {
                Text = "Manage your Life",
                Icon = Properties.Resources.originalIconTray,
                Visible = true,
                BalloonTipIcon = ToolTipIcon.Info,
                ContextMenuStrip = menuStrip
            };
            notifyIcon.MouseDoubleClick += new MouseEventHandler(NotifyIconDoubleClicked);
        }



        #region WindowVisibility 変更通知プロパティ
        private Visibility _WindowVisibility;

        public Visibility WindowVisibility
        {
            get { return this._WindowVisibility; }
            set { this.SetProperty(ref this._WindowVisibility, value); }
        }
        #endregion
        

        #region CloseCommand 変更通知プロパティ
        private ViewModelCommand _CloseCommand;

        public ViewModelCommand CloseCommand
        {
            get { return this._CloseCommand ?? new ViewModelCommand(Close); }
        }
        #endregion
        

        #region NotifyIconCoomand 変更通知プロパティ
        private ViewModelCommand _NotifyIconCoomand;

        public ViewModelCommand NotifyIconCoomand
        {
            get { return this._NotifyIconCoomand ?? new ViewModelCommand(ShowNotifyIcon); }
        }
        #endregion



        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.notifyIcon.Visible = false;
                this.notifyIcon.Dispose();
            }
        }


        private void Close()
        {
            this.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }



        private void ShowNotifyIcon()
        {
            throw new NotImplementedException();
        }



        private void NotifyIconDoubleClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //WindowOpen();
            }
        }

        private void NotifyIconOpenClicked(object sender, EventArgs e)
        {
            //WindowOpen();
        }

        private void NotifyIconExitClicked(object sender, EventArgs e)
        {
            this.WindowVisibility = Visibility.Visible;
            //WindowClosingProcess();

            System.Windows.Application.Current.Shutdown();
        }

        
       

    }
}

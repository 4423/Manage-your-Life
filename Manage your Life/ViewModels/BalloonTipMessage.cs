using Hardcodet.Wpf.TaskbarNotification;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Manage_your_Life.ViewModels
{
    public class BalloonTipMessage : InteractionMessage
    {
        public BalloonTipMessage() : base() { }
        
        public BalloonTipMessage(string MessageKey) : base(MessageKey) { }
                
        public BalloonTipMessage(string Title, string Message, BalloonIcon Icon, string MessageKey) : base(MessageKey)
        {
            this.Title = Title;
            this.Message = Message;
            this.Icon = Icon;
        }

        protected override Freezable CreateInstanceCore() => new BalloonTipMessage(MessageKey);


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(BalloonTipMessage), new PropertyMetadata(null));
        
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(BalloonTipMessage), new PropertyMetadata(null));
                
        public BalloonIcon Icon
        {
            get { return (BalloonIcon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(BalloonIcon), typeof(BalloonTipMessage), new PropertyMetadata(BalloonIcon.None));
    }
}

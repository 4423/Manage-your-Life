using Hardcodet.Wpf.TaskbarNotification;
using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life.ViewModels
{
    public class BalloonTipMessageAction : InteractionMessageAction<TaskbarIcon>
    {
        protected override void InvokeAction(InteractionMessage message)
        {
            var msg = message as BalloonTipMessage;

            if (msg != null)
            {
                this.AssociatedObject.ShowBalloonTip(msg.Title, msg.Message, msg.Icon);
            }
        }
    }
}

using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Manage_your_Life.ViewModels
{
    public class AlertDialogViewModel : BindableBase
    {
        public AlertDialogViewModel() { }

        public AlertDialogViewModel(string processName, TimeSpan usageTime)
        {
            this.ProcessName = processName;
            this.UsageTime = usageTime;
        }

        private string processName;
        public string ProcessName
        {
            get { return this.processName; }
            set { this.SetProperty(ref this.processName, value); }
        }

        private TimeSpan usageTime;
        public TimeSpan UsageTime
        {
            get { return this.usageTime; }
            set { this.SetProperty(ref this.usageTime, value); }
        }

        private bool isNotDialogShowAgain;
        public bool IsNotDialogShowAgain
        {
            get { return this.isNotDialogShowAgain; }
            set { this.SetProperty(ref this.isNotDialogShowAgain, value); }
        }

        public Brush AccentColor
            => new SolidColorBrush(AppearanceManager.Current.AccentColor);        
    }
}

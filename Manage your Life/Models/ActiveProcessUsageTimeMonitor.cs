using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life.Models
{
    public delegate void UsageTimeOfProcess(Process process, TimeSpan timeSpan);

    public class ActiveProcessUsageTimeMonitor
    {
        public event UsageTimeOfProcess OnActiveProcessChanged;
        
        private ActiveProcessMonitor procMonitor;
        private DateTime previousDateTime;


        public ActiveProcessUsageTimeMonitor()
        {
            this.previousDateTime = DateTime.Now;

            this.procMonitor = ActiveProcessMonitor.Instance;
            this.procMonitor.OnActiveProcessChanged += (_, process) =>
            {
                this.OnActiveProcessChanged?.Invoke(process, DateTime.Now - this.previousDateTime);
                this.previousDateTime = DateTime.Now;
            };
            this.procMonitor.Start();
        }
    }
}

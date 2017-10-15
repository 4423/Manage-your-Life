using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Manage_your_Life.Models
{
    public delegate void ActiveProcessChanged(Process activeProcess, Process previousProcess);

    public class ActiveProcessMonitor
    {
        private DispatcherTimer timer;
        private Process previousProc;

        public event ActiveProcessChanged OnActiveProcessChanged;

        private static readonly ActiveProcessMonitor instance = new ActiveProcessMonitor();
        public static ActiveProcessMonitor Instance => instance;


        private ActiveProcessMonitor()
        {
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(1);
            this.timer.Tick += Timer_Tick;
        }


        public void Start()
        {
            this.timer.Start();
        }


        public void Stop()
        {
            this.timer.Stop();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            Process activeProc = ActiveProcessUtils.GetActiveProcess();
            if (this.previousProc == null)
            {
                this.previousProc = activeProc;
            }

            if (IsProcessChanged(activeProc))
            {
                this.OnActiveProcessChanged?.Invoke(activeProc, this.previousProc);
                this.previousProc = activeProc;
            }

            timer.Start();
        }


        private bool IsProcessChanged(Process activeProc)
        {
            return this.previousProc.ProcessName != activeProc.ProcessName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Manage_your_Life
{
    public delegate void ActiveProcessChanged(Process activeProcess);

    public class ActiveProcessMonitor
    {
        private DispatcherTimer timer;
        private Process previousProc;

        public event ActiveProcessChanged OnActiveProcessChanged;



        public ActiveProcessMonitor()
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
                this.previousProc = activeProc;

                if (this.OnActiveProcessChanged != null)
                {
                    this.OnActiveProcessChanged(activeProc);
                }
            }

            timer.Start();
        }


        private bool IsProcessChanged(Process activeProc)
        {
            return this.previousProc.ProcessName != activeProc.ProcessName;
        }
    }
}

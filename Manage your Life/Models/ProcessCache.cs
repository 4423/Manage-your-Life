using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life.Models
{
    /// <summary>
    /// <see cref="Process.MainModule.FileName"/>と<see cref="Process.ProcessName"/>をキャッシュします。
    /// </summary>
    public class ProcessCache
    {
        public string FileName { get; private set; }
        public string ProcessName { get; private set; }
        public Process Process { get; private set; }

        public void Register(Process proc)
        {
            this.FileName = proc.MainModule.FileName;
            this.ProcessName = proc.ProcessName;
            this.Process = proc;
        }
    }
}

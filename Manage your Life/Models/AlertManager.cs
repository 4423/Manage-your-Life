using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life.Models
{
    public class AlertManager
    {
        private ActiveProcessMonitor procMonitor;
        private DatabaseOperation db;
        private Dictionary<int, TimeSpan> overuseWarningItems;
        private List<string> unalertProcFileNameList = new List<string>();


        public int AlertCount { get; private set; } = 0;        
        public event UsageTimeOfProcess Alert;


        private static readonly AlertManager instance = new AlertManager();
        public static AlertManager Instance => instance;

        private AlertManager()
        {
            this.db = DatabaseOperation.Instance;
            this.overuseWarningItems = db.GetOveruseWarningCollection();

            this.procMonitor = ActiveProcessMonitor.Instance;
            this.procMonitor.OnActiveProcessChanged += (proc, _) =>
            {
                int appId = this.db.GetMatchedId(proc.MainModule.FileName);

                // 警告対象に現在のAppIDが含まれていない | 警告しない対象に含まれている
                if (!overuseWarningItems.ContainsKey(appId) || unalertProcFileNameList.Contains(proc.MainModule.FileName))
                {
                    return;
                }

                var warningTime = overuseWarningItems[appId];
                var todayUsageTime = db.GetTodayPrpcessUsageTime(appId);
                if (warningTime <= todayUsageTime)
                {
                    // 時間超過
                    this.Alert?.Invoke(proc, warningTime);
                    this.AlertCount++;
                }
            };
        }
        
        public void UnSubscribe(Process proc)
            => unalertProcFileNameList.Add(proc.MainModule.FileName);
    }
}

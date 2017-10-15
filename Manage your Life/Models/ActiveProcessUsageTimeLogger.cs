using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life.Models
{
    public class ActiveProcessUsageTimeLogger
    {
        public event UsageTimeOfProcess Logged;

        private ActiveProcessUsageTimeMonitor monitor;
        private DatabaseOperation db;

        private static readonly ActiveProcessUsageTimeLogger instance = new ActiveProcessUsageTimeLogger();
        public static ActiveProcessUsageTimeLogger Instance => instance;

        private ActiveProcessUsageTimeLogger()
        {
            this.db = DatabaseOperation.Instance;

            this.monitor = new ActiveProcessUsageTimeMonitor();
            this.monitor.OnActiveProcessChanged += (proc, timespan) =>
            {
                //DBに存在していなければ新規にデータ挿入
                if (!this.db.IsExist(proc.MainModule.FileName))
                {
                    this.db.Register(proc);
                }

                // DBから使用時間を取得し、今回の使用時間を加算してDB更新
                int appId = this.db.GetMatchedId(proc.MainModule.FileName);
                this.db.UpdateUsageTime(appId, timespan);

                // 更新を通知
                this.Logged?.Invoke(proc, timespan);
            };
        }
    }
}

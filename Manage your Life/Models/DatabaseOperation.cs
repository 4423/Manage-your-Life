using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using System.Runtime.ExceptionServices;

namespace Manage_your_Life.Models
{
    /// <summary>
    /// データベースを操作するクラス
    /// </summary>
    public sealed class DatabaseOperation
    {
        private static readonly DatabaseOperation instance = new DatabaseOperation();

        private ApplicationDataClassesDataContext database;
        private string basePath;
        private string connStr;

        private string timeSpanToStringFormat = @"d\.hh\:mm\:ss";

        /// <summary>
        /// TimelineDBに変更を加えた(追記)ときに発生
        /// </summary>
        public event EventHandler TimelineLog_Updated;

        /// <summary>
        /// レコードを新規追加したときに発生
        /// </summary>
        public event EventHandler NewRecord_Registered;

        /// <summary>
        /// 使用時間を更新したときに発生
        /// </summary>
        public event EventHandler UsageTime_Updated;
                
        /// <summary>
        /// レコードが削除されたときに発生
        /// </summary>
        public event EventHandler Record_Deleted;



        //シングルトン 
        private DatabaseOperation()
        {
            //接続文字列の生成
            basePath = Directory.GetCurrentDirectory() + @"\ApplicationDatabase.mdf";
            connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;Connection Timeout=30;AttachDbFilename=""" + basePath + @""";";

            //データベース接続
            RetryHelper.Retry(() => {
                database = new ApplicationDataClassesDataContext(connStr);
            },
            ex => ExceptionDispatchInfo.Capture(ex).Throw(),
            ex => ex is SqlException);
        }


        ~DatabaseOperation()
        {
            database.Dispose();
        }

        

        public static DatabaseOperation Instance
            => instance;


        public ApplicationDataClassesDataContext GetConnectionedDataContext
            => database;



//---------------------------------------------------------登録/更新

        /// <summary>
        /// 新規にデータベースへプロセスを登録
        /// </summary>
        /// <param name="proc">登録するプロセス</param>
        /// <returns>登録したID</returns>
        internal int Register(Process proc)
        {
            var app = new DatabaseApplication()
            {
                Title = proc.MainWindowTitle,
                Favorite = false
            };
            database.DatabaseApplication.InsertOnSubmit(app);
            RetryHelper.Retry(() =>
            {
                database.SubmitChanges();
            },
            ex => ExceptionDispatchInfo.Capture(ex).Throw(),
            ex => ex is SqlException);
            
            #region 取得したIDを元にProcessDBに登録
            var pro = new DatabaseProcess()
            {
                AppId = app.Id,
                Name = proc.ProcessName,
                Path = proc.MainModule.FileName
            };
            database.DatabaseProcess.InsertOnSubmit(pro);
            #endregion

            #region 取得したIDを元にDateDBに登録
            var date = new DatabaseDate()
            {
                AppId = app.Id,
                AddDate = DateTime.Now,
                LastDate = DateTime.Now,
                UsageTime = "0.00:00:00",
                AlertTime = "0.00:00:00"
            };            
            database.DatabaseDate.InsertOnSubmit(date);
            #endregion

            RetryHelper.Retry(() =>
            {
                database.SubmitChanges();
            },
            ex => ExceptionDispatchInfo.Capture(ex).Throw(),
            ex => ex is SqlException);

            //レコード新規登録のイベント発生
            NewRecord_Registered?.Invoke(this, EventArgs.Empty);

            return app.Id;
        }


        /// <summary>
        /// 使いすぎ警告の使用時間を登録(更新)
        /// </summary>
        /// <param name="appId">警告の対象となるID</param>
        internal void SetAlert(int appId, TimeSpan alertTime)
        {
            var q = database.DatabaseDate.Single(p => p.AppId == appId);
            RetryHelper.Retry(() =>
            {
                q.AlertTime = alertTime.ToString(timeSpanToStringFormat);
                database.SubmitChanges();
            }, 
            ex => ExceptionDispatchInfo.Capture(ex).Throw(),
            ex => ex is SqlException);
        }


        /// <summary>
        /// IDに一致するレコードを削除
        /// </summary>
        /// <param name="appId">削除するレコードのID</param>
        internal void Delete(int appId)
        {
            var date = database.DatabaseDate.Single(x => x.AppId == appId);
            database.DatabaseDate.DeleteOnSubmit(date);
            
            var proc = database.DatabaseProcess.Single(p => p.AppId == appId);
            database.DatabaseProcess.DeleteOnSubmit(proc);
            
            var timeline = database.DatabaseTimeline.Where(t => t.AppId == appId);
            database.DatabaseTimeline.DeleteAllOnSubmit(timeline);

            var app = database.DatabaseApplication.Single(x => x.Id == appId);
            database.DatabaseApplication.DeleteOnSubmit(app);

            RetryHelper.Retry(() =>
            {
                database.SubmitChanges();
                Record_Deleted?.Invoke(this, EventArgs.Empty);
            }, 
            ex => ExceptionDispatchInfo.Capture(ex).Throw(),
            ex => ex is SqlException);
        }


        /// <summary>
        /// DBへの使用時間の更新を行う
        /// </summary>
        /// <param name="appId">使用時間の更新を行う対象のID</param>
        /// <param name="activeInterval">DBに加算する時間</param>
        /// <returns>DBと現在を合わせた使用時間</returns>
        internal TimeSpan UpdateUsageTime(int appId, TimeSpan activeInterval)
        {
            TimeSpan usageSum = new TimeSpan();

            //クエリ発行
            var q =
                from p in database.DatabaseDate
                where p.AppId == appId
                select p;

            //怪しい情報がTimelineに記録されてしまう
            bool isUpdated = false;
            RetryHelper.Retry(() =>
            {
                foreach (var p in q)
                {
                    //DB内の使用時間を取得しTimeSpanにキャスト
                    usageSum = TimeSpan.Parse(p.UsageTime);
                    //今回の測定の時間を加算
                    usageSum += activeInterval;
                    //DBに上書き
                    p.UsageTime = usageSum.ToString(timeSpanToStringFormat);

                    //最終使用日の更新
                    p.LastDate = DateTime.Now;

                    isUpdated = true;
                }
            },
            ex => { ExceptionDispatchInfo.Capture(ex).Throw(); },
            ex => ex is SqlException);

            //何らかのデータが変更された時のみ
            if (isUpdated)
            {
                //DBの更新
                database.SubmitChanges();
                //UsageTime更新のイベント発生
                if (UsageTime_Updated != null)
                {
                    UsageTime_Updated(this, EventArgs.Empty);
                }

                //Timelineの方にも新規レコードとしてロギング         
                AddingTimeline(appId, activeInterval);
            }

            return usageSum;
        }


        /// <summary>
        /// 時間軸としてのTimelineにロギングする
        /// </summary>
        /// <param name="database"></param>
        private void AddingTimeline(int appId, TimeSpan activeInterval)
        {
            DatabaseTimeline log = new DatabaseTimeline();

            log.AppId = appId;
            log.Today = DateTime.Today;
            log.Now = DateTime.Now;
            log.UsageTime = activeInterval.ToString();

            RetryHelper.Retry(() =>
            {
                database.DatabaseTimeline.InsertOnSubmit(log);
                database.SubmitChanges();
            },
            ex => ExceptionDispatchInfo.Capture(ex).Throw(),
            ex => ex is SqlException);

            //Timeline更新のイベント発生
            TimelineLog_Updated?.Invoke(this, EventArgs.Empty);
        }
        


//--------------------------------------------------------取得

        /// <summary>
        /// Databases内の全てのデータを取得
        /// </summary>
        /// <returns>全てのレコード</returns>
        internal IQueryable GetAllData()
            => database.DatabaseApplication.Select(p => new
            {
                Id = p.Id,
                Favorite = p.Favorite,
                Title = p.Title,
                ProcName = p.DatabaseProcess.Name,
                ProcPath = p.DatabaseProcess.Path,
                UsageTime = p.DatabaseDate.UsageTime,
                AlertTime = p.DatabaseDate.AlertTime,
                AddDate = p.DatabaseDate.AddDate,
                LastDate = p.DatabaseDate.LastDate,
                Memo = p.Memo
            });


        /// <summary>
        /// 使いすぎ警告の対象に含まれているプロセスのIDと警告時間を取得
        /// </summary>
        /// <returns>IDと警告時間のDictionary</returns>
        internal Dictionary<int, TimeSpan> GetOveruseWarningCollection()
            => database.DatabaseApplication
                .Where(p => p.DatabaseDate.AlertTime != "0.00:00:00")
                .ToDictionary(p => p.Id, p => TimeSpan.Parse(p.DatabaseDate.AlertTime));


        /// <summary>
        /// 指定されたIDの今日の使用時間を取得
        /// </summary>
        /// <param name="appId">使用時間を知りたいID</param>
        /// <returns>IDに対応する今日の使用時間</returns>
        internal TimeSpan GetTodayPrpcessUsageTime(int appId)
        {
            var totalSecStr = database.DatabaseApplication
                .Where(a => a.Id == appId)
                .Where(a => a.DatabaseTimeline.Today == DateTime.Today)
                .Select(a => a.DatabaseTimeline.UsageTime)
                .ToList()
                .Sum(s => TimeSpan.Parse(s).TotalSeconds);
            return TimeSpan.FromSeconds(totalSecStr);
        }


        /// <summary>
        /// プロセスのPathが既にデータベース内に存在するか
        /// </summary>
        /// <param name="proc">比較対象のProcess</param>
        /// <returns>存在する：true　存在しない:false</returns>
        internal bool IsExist(string procPath)
            => database.DatabaseProcess.Any(p => p.Path == procPath);

        internal bool IsExist(Process process)
            => IsExist(process.MainModule.FileName);

        /// <summary>
        /// DatabaseProcessの中から同じファイルパスを探索し
        /// 対応するDatabaseApplicationのAppIDを取得
        /// </summary>
        /// <param name="previousProcess">検索対象の(前回の)Process</param>
        /// <returns>プロセス名に対応するAppID</returns>
        internal int GetMatchedId(string procPath)
            => database.DatabaseProcess.Single(p => p.Path == procPath).AppId;

        internal int GetMatchedId(Process process)
            => GetMatchedId(process.MainModule.FileName);
    }
}

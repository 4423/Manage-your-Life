﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;

namespace Manage_your_Life
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



        //シングルトン 
        private DatabaseOperation()
        {
            //接続文字列の生成
            basePath = Directory.GetCurrentDirectory() + @"\ApplicationDatabase.mdf";
            connStr = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=""" + basePath + @""";Integrated Security=True";

            //データベース接続
            try
            {
                database = new ApplicationDataClassesDataContext(connStr);
            }
            catch (SqlException ex) { throw; }
        }


        ~DatabaseOperation()
        {
            database.Dispose();
        }

        

        public static DatabaseOperation Instance
        {
            get
            {
                return instance;
            }
        }


        public ApplicationDataClassesDataContext GetConnectionedDataContext
        {
            get { return database; }
        }



//---------------------------------------------------------登録/更新

        /// <summary>
        /// 新規にデータベースへプロセスを登録
        /// </summary>
        /// <param name="proc">登録するプロセス</param>
        /// <returns>登録したID</returns>
        internal int Register(Process proc)
        {
            DatabaseApplication app = new DatabaseApplication();
            app.Title = proc.MainWindowTitle;
            app.Favorite = false;
            database.DatabaseApplication.InsertOnSubmit(app);
            try
            {
                database.SubmitChanges();
            }
            catch (SqlException ex) { throw; }

            //SubmitChanges()すると挿入したIDが取得できるようになる
            //see: http://bluestick.jp/tech/index.php/archives/50
            var id = app.Id;

            #region 取得したIDを元にProcessDBに登録
            DatabaseProcess pro = new DatabaseProcess();
            pro.AppId = id;
            pro.Name = proc.ProcessName;
            pro.Path = proc.MainModule.FileName;
            database.DatabaseProcess.InsertOnSubmit(pro);
            #endregion

            #region 取得したIDを元にDateDBに登録
            DatabaseDate date = new DatabaseDate();
            date.AppId = id;
            date.AddDate = date.LastDate = DateTime.Now;
            date.UsageTime = "0.00:00:00";
            date.AlertTime = "0.00:00:00";
            database.DatabaseDate.InsertOnSubmit(date);
            #endregion

            try
            {
                database.SubmitChanges();
            }
            catch (SqlException ex) { throw; }

            //レコード新規登録のイベント発生
            if (NewRecord_Registered != null)
            {
                NewRecord_Registered(this, EventArgs.Empty);
            }

            return id;
        }


        /// <summary>
        /// 使いすぎ警告の使用時間を登録(更新)
        /// </summary>
        /// <param name="appId">警告の対象となるID</param>
        internal void SetAlert(int appId, TimeSpan alertTime)
        {
            var q = (
                from p in database.DatabaseDate
                where p.AppId == appId
                select p).First();

            try
            {
                q.AlertTime = alertTime.ToString(timeSpanToStringFormat);
                database.SubmitChanges();
            }
            catch (SqlException ex) { throw; }
        }


        /// <summary>
        /// IDに一致するレコードを削除
        /// </summary>
        /// <param name="appId">削除するレコードのID</param>
        internal void Delete(int appId)
        {
            var dateQuery = (
                from p in database.DatabaseDate
                where p.AppId == appId
                select p).First();

            database.DatabaseDate.DeleteOnSubmit(dateQuery);

            
            var procQuery = (
                from p in database.DatabaseProcess
                where p.AppId == appId
                select p).First();

            database.DatabaseProcess.DeleteOnSubmit(procQuery);


            var timelineQuery = (
                from p in database.DatabaseTimeline
                where p.AppId == appId
                select p);

            foreach (var r in timelineQuery)
            {
                database.DatabaseTimeline.DeleteOnSubmit(r);
            }


            var appQuery = (
                from p in database.DatabaseApplication
                where p.Id == appId
                select p).First();

            database.DatabaseApplication.DeleteOnSubmit(appQuery);

            try
            {
                database.SubmitChanges();
            }
            catch (SqlException ex) { throw; }
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
            bool isLooped = false;
            try
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

                    isLooped = true;
                }
            }
            catch (SqlException ex) { throw; }

            //何らかのデータが変更された時のみ
            if (isLooped)
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

            try
            {
                database.DatabaseTimeline.InsertOnSubmit(log);
                database.SubmitChanges();
            }
            catch (SqlException ex) { throw; }

            //Timeline更新のイベント発生
            if (TimelineLog_Updated != null)
            {
                TimelineLog_Updated(this, EventArgs.Empty);
            }
        }
        


//--------------------------------------------------------取得

        /// <summary>
        /// Databases内の全てのデータを取得
        /// </summary>
        /// <returns>全てのレコード</returns>
        internal IQueryable GetAllData(){
            var r =
                from p in database.DatabaseApplication
                select new { 
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
                };

            return r;
        }


        /// <summary>
        /// 使いすぎ警告の対象に含まれているプロセスのIDと警告時間を取得
        /// </summary>
        /// <returns>IDと警告時間のDictionary</returns>
        internal Dictionary<int, TimeSpan> GetOveruseWarningCollection()
        {            
            var q =
                from p in database.DatabaseApplication
                where p.DatabaseDate.AlertTime != "0.00:00:00"
                select new
                {
                    Id = p.Id,
                    AlertTime = p.DatabaseDate.AlertTime
                };

            var dict = new Dictionary<int, TimeSpan>();

            try
            {
                foreach (var r in q)
                {
                    dict.Add(r.Id, TimeSpan.Parse(r.AlertTime));
                }
            }
            catch (SqlException ex) { throw; }

            return dict;
        }


        /// <summary>
        /// 指定されたIDの今日の使用時間を取得
        /// </summary>
        /// <param name="appId">使用時間を知りたいID</param>
        /// <returns>IDに対応する今日の使用時間</returns>
        internal TimeSpan GetTodayPrpcessUsageTime(int appId)
        {
            var q = (
                    from p in database.DatabaseApplication
                    where appId == p.Id
                    where DateTime.Today == p.DatabaseTimeline.Today                    
                    select new
                    {
                        Key = p.DatabaseProcess.Name,
                        Value = TimeSpan.Parse(p.DatabaseTimeline.UsageTime)
                    });
                        
            var dict = new KeyValuePair<string, TimeSpan>();

            //断片的な使用時間をまとめる
            try
            {
                dict = Utility.DictionaryOrganizingValue(q).Single();
            }
            //シーケンスに要素が含まれていない場合
            catch (Exception ex)
            {
                return TimeSpan.FromSeconds(0);
            }

            return dict.Value;
        }


        /// <summary>
        /// プロセスのPathが既にデータベース内に存在するか
        /// </summary>
        /// <param name="proc">比較対象のProcess</param>
        /// <returns>存在する：true　存在しない:false</returns>
        /// <see cref="http://bit.ly/1jAFEn3"/>
        internal bool IsExist(string procPath)
        {
            return database.DatabaseProcess.Any(p => p.Path.Contains(procPath));
        }
        

        /// <summary>
        /// DatabaseProcessの中から同じファイルパスを探索し
        /// 対応するDatabaseApplicationのAppIDを取得
        /// </summary>
        /// <param name="previousProcess">検索対象の(前回の)Process</param>
        /// <returns>プロセス名に対応するAppID</returns>
        internal int GetCorrespondingAppId(string procPath)
        {
            int appId = -1;

            //クエリ発行            
            var q =
                from p in database.DatabaseProcess
                where p.Path == procPath
                select p;

            try
            {
                foreach (var r in q)
                {
                    appId = r.AppId;
                }
            }
            catch (SqlException ex) { throw; }

            return appId;
        }

    }
}
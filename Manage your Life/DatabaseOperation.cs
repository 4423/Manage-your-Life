using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Manage_your_Life
{
    /// <summary>
    /// データベースを操作するクラス
    /// </summary>
    class DatabaseOperation
    {

        string basePath;
        string connStr;

        ApplicationDataClassesDataContext database;


        public DatabaseOperation()
        {
            //接続文字列の生成
            basePath = Directory.GetCurrentDirectory() + @"\ApplicationDatabase.mdf";
            connStr = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=""" + basePath + @""";Integrated Security=True";

            //データベース接続
            database = new ApplicationDataClassesDataContext(connStr);
        }

        ~DatabaseOperation()
        {
            database.Dispose();
        }

        //TODO プロセスが途中終了した時のために例外処理を実装する


        /// <summary>
        /// 新規にデータベースへプロセスを登録
        /// </summary>
        /// <param name="proc">登録するプロセス</param>
        internal void Register(Process proc)
        {
            DatabaseApplication app = new DatabaseApplication();
            app.Title = proc.MainWindowTitle;
            database.DatabaseApplication.InsertOnSubmit(app);
            database.SubmitChanges();

            //SubmitChanges()すると挿入したIDが取得できるようになる
            //see: http://bluestick.jp/tech/index.php/archives/50
            var id = app.Id;

            //取得したIDを元に(そうでもない)Process/Dateに登録
            DatabaseProcess pro = new DatabaseProcess();
            pro.AppId = id;
            pro.Name = proc.ProcessName;
            pro.Path = proc.MainModule.FileName;
            database.DatabaseProcess.InsertOnSubmit(pro);

            DatabaseDate date = new DatabaseDate();
            date.AppId = id;
            date.AddDate = date.LastDate = DateTime.Now;
            date.UsageTime = "0.00:00:00";
            database.DatabaseDate.InsertOnSubmit(date);

            database.SubmitChanges();
        }


        /// <summary>
        /// Pathが既にデータベース内に存在するか
        /// </summary>
        /// <param name="proc">比較対象のProcess</param>
        /// <returns>存在する：true　存在しない:false</returns>
        /// <see cref="http://bit.ly/1jAFEn3"/>
        internal bool IsExist(Process proc)
        {
            //TODO ここでは実行ファイルのパスで判別しているが…要検討
            return database.DatabaseProcess.Any(
                p => p.Path.Contains(proc.MainModule.FileName));
        }


        /// <summary>
        /// DatabaseProcessの中から同じファイルパスを探索し
        /// 対応するDatabaseApplicationのAppIDを取得
        /// </summary>
        /// <param name="previousProcess">検索対象の(前回の)Process</param>
        /// <returns>プロセス名に対応するAppID</returns>
        internal int GetCorrespondingAppId(Process previousProcess)
        {
            int appId = -1;   
                        
            //クエリ発行
            var q =
                from p in database.DatabaseProcess
                where p.Path == previousProcess.MainModule.FileName
                select p;

            //TODO データ複数ある場合は…?
            foreach (var p in q)
            {
                appId = p.AppId;
                break;
            }

            return appId;
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

            foreach (var p in q)
            {
                //DB内の使用時間を取得しTimeSpanにキャスト
                usageSum = TimeSpan.Parse(p.UsageTime);
                //今回の測定の時間を加算
                usageSum += activeInterval;
                //DBに上書き
                p.UsageTime = usageSum.ToString();

                break;
            }

            //DBの更新
            database.SubmitChanges();

            return usageSum;
        }


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
                    AddDate = p.DatabaseDate.AddDate, 
                    LastDate = p.DatabaseDate.LastDate,
                    Memo = p.Memo 
                };

            return r;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using theori.Charting;
using theori.Graphics;

namespace theori.Database
{
    public class ChartDatabase
    {
        private const int DEFAULT_VERSION = 1;

        public readonly string FilePath;

        public string ChartsDirectory { get; private set; }

        protected virtual int Version => DEFAULT_VERSION;

        private SQLiteConnection m_connection;
        private FileSystemWatcher m_watcher;

        private readonly HashSet<string> m_setFiles = new HashSet<string>();

        private readonly Dictionary<long, ChartSetInfo> m_chartSets = new Dictionary<long, ChartSetInfo>();
        private readonly Dictionary<long, ChartInfo> m_charts = new Dictionary<long, ChartInfo>();

        private readonly Dictionary<string, ChartSetInfo> m_chartSetsByFilePath = new Dictionary<string, ChartSetInfo>();

        public ChartDatabase(string filePath)
        {
            FilePath = filePath;
            m_connection = new SQLiteConnection($"Data Source={ filePath }");
        }

        public void OpenLocal(string chartsDirectory)
        {
            ChartsDirectory = chartsDirectory;
            m_connection.Open();

            bool rebuild;
            try
            {
                int vGot = -1;
                using (var reader = ExecReader("SELECT version FROM `Database`"))
                {
                    reader.Read();
                    vGot = reader.GetInt32(0);
                }

                rebuild = vGot != Version;
            }
            catch (SQLiteException e)
            {
                Logger.Log(e.Message);
                rebuild = true;
            }

            if (rebuild)
                Initialize();
            else LoadData();
        }

        private int Exec(string commandText)
        {
            using (var command = new SQLiteCommand(commandText, m_connection))
                return command.ExecuteNonQuery();
        }

        private object ExecScalar(string commandText)
        {
            using (var command = new SQLiteCommand(commandText, m_connection))
                return command.ExecuteScalar();
        }

        private SQLiteDataReader ExecReader(string commandText)
        {
            using (var command = new SQLiteCommand(commandText, m_connection))
                return command.ExecuteReader();
        }

        private int Exec(string commandText, params object[] values)
        {
            using (var command = new SQLiteCommand(commandText, m_connection))
            {
                for (int i = 0; i < values.Length; i++)
                {
                    var param = command.CreateParameter();
                    param.Value = values[i];

                    command.Parameters.Add(param);
                }
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Empties the database and reconstructs the tables.
        /// </summary>
        public void Initialize()
        {
            Exec($"DROP TABLE IF EXISTS Database");
            Exec($"CREATE TABLE Database ( version INTEGER )");
            Exec($"INSERT INTO Database ( rowid, version ) VALUES ( 1, { Version } )");

            InitializeTables();
        }

        /// <summary>
        /// This should drop all tables and recreate them.
        /// </summary>
        protected virtual void InitializeTables()
        {
            Exec($"DROP TABLE IF EXISTS Sets");
            Exec($"DROP TABLE IF EXISTS Charts");
            Exec($"DROP TABLE IF EXISTS Scores");

            // lwt = last write time, and should likely be epoch time (the usual 1970 one)
            //  rather than C#'s (which starts from the year 0001) if we plan for other applications
            //  reading in the database themselves.
            // If we don't care, then it's fine that it's C#'s epoch instead.
            // C# gives easy access to DateTime ticks, which are 100 nanoseconds each and count
            //  from 1/1/0001.
            Exec($@"CREATE TABLE Sets (
                id INTEGER PRIMARY KEY,
                lwt INTEGER NOT NULL,
                uploadID INTEGER,
                filePath TEXT NOT NULL,
                fileName TEXT NOT NULL
            )");

            Exec($@"CREATE TABLE Charts (
                id INTEGER PRIMARY KEY,
                setId INTEGER NOT NULL,
                lwt INTEGER NOT NULL,
                fileName TEXT NOT NULL,
                songTitle TEXT NOT NULL COLLATE NOCASE,
                songArtist TEXT NOT NULL COLLATE NOCASE,
                songFileName TEXT NOT NULL,
                songVolume INTEGER NOT NULL,
                charter TEXT NOT NULL COLLATE NOCASE,
                jacketFileName TEXT,
                jacketArtist TEXT,
                backgroundFileName TEXT,
                backgroundArtist TEXT,
                diffLevel REAL NOT NULL,
                diffIndex INTEGER,
                diffName TEXT NOT NULL,
                diffNameShort TEXT,
                diffColor INTEGER,
                chartDuration REAL NOT NULL,
                tags TEXT NOT NULL COLLATE NOCASE,
                FOREIGN KEY(setId) REFERENCES Sets(id)
            )");

            Exec($@"CREATE TABLE Scores (
                id INTEGER PRIMARY KEY,
                chartId INTEGER NOT NULL,
                score INTEGER NOT NULL,
                FOREIGN KEY(chartId) REFERENCES Charts(id)
            )");
        }

        public void WatchForFileSystemChanges()
        {
            if (m_watcher != null) return;

            var watcher = new FileSystemWatcher(ChartsDirectory);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

            watcher.Filter = "*.theori-set|*.theori";

            watcher.Changed += Watcher_Changed;
            watcher.Deleted += Watcher_Changed;
            watcher.Created += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;

            m_watcher = watcher;
            watcher.EnableRaisingEvents = true;
        }

        public void StopWatchingForFileSystemChanges()
        {
            if (m_watcher == null) return;

            m_watcher.Dispose();
            m_watcher = null;
        }

        /// <summary>
        /// Looks for every set this database is responsible for watching and sets up
        ///  listeners on them for added/removed sets.
        /// </summary>
        public void Scan()
        {
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
        }

        public void AddSetFileRelative(string relPath)
        {
            if (PathL.IsFullPath(relPath))
                throw new ArgumentException($"{ nameof(AddSetFileRelative) } expects a relative path.");

            bool isUpdate = !m_setFiles.Add(relPath);

            string setDir = Directory.GetParent(relPath).FullName;
            string setFile = Path.GetFileName(relPath);

            Debug.Assert(Path.Combine(setDir, setFile) == relPath);

            var setSerializer = new ChartSetSerializer();
            var setInfo = setSerializer.LoadFromFile(ChartsDirectory, setDir, setFile);

            AddSetInfoToDatabase(relPath, setInfo, isUpdate);
        }

        public void AddSetFile(string fullPath)
        {
            if (!PathL.IsFullPath(fullPath))
                throw new ArgumentException($"{ nameof(AddSetFile) } expects a full path and will convert it to a relative path.");

            string relPath;
            try
            {
                relPath = PathL.RelativePath(fullPath, ChartsDirectory);
            }
            catch (ArgumentException e)
            {
                Logger.Log(e);
                return;
            }

            AddSetFileRelative(relPath);
        }

        public void AddSet(ChartSetInfo setInfo)
        {
            string relPath = Path.Combine(setInfo.FilePath, setInfo.FileName);
            bool isUpdate = !m_setFiles.Add(relPath);

            AddSetInfoToDatabase(relPath, setInfo, isUpdate);
        }

        private void AddSetInfoToDatabase(string relPath, ChartSetInfo setInfo, bool isUpdate)
        {
            Debug.Assert(Path.Combine(setInfo.FilePath, setInfo.FileName) == relPath);

            m_chartSetsByFilePath[relPath] = setInfo;
            // check that we need to update an entry or create a new one i guess

            if (isUpdate)
            {
            }
            else
            {
                if (setInfo.ID != 0) Logger.Log($"Adding a set info with non-zero primary key already set. This will be overwritten with the new key.");
                int setResult = Exec("INSERT INTO Sets (lwt,uploadID,filePath,fileName) VALUES (?,?,?,?)",
                    setInfo.LastWriteTime,
                    setInfo.OnlineID,
                    setInfo.FilePath,
                    setInfo.FileName);
                if (setResult == 0)
                {
                    Logger.Log($"Failed to insert chart set { setInfo.FilePath }\\{ setInfo.FileName }");
                    return;
                }

                
                setInfo.ID = m_connection.LastInsertRowId;
                foreach (var chart in setInfo.Charts)
                {
                    int chartResult = Exec("INSERT INTO Charts (setId,lwt,fileName,songTitle,songArtist,songFileName,songVolume,charter,jacketFileName,jacketArtist,backgroundFileName,backgroundArtist,diffLevel,diffIndex,diffName,diffNameShort,diffColor,chartDuration,tags) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)",
                        chart.SetID,
                        chart.LastWriteTime,
                        chart.FileName,
                        chart.SongTitle,
                        chart.SongArtist,
                        chart.SongFileName,
                        chart.SongVolume,
                        chart.Charter,
                        chart.JacketFileName,
                        chart.JacketArtist,
                        chart.BackgroundFileName,
                        chart.BackgroundArtist,
                        chart.DifficultyLevel,
                        chart.DifficultyIndex,
                        chart.DifficultyName,
                        chart.DifficultyNameShort,
                        chart.DifficultyColor == null ? (int?)null : Color.Vector3ToHex(chart.DifficultyColor.Value),
                        (double)chart.ChartDuration,
                        chart.Tags);
                }
            }
        }

        public void LoadData()
        {
            using (var reader = ExecReader("SELECT id,lwt,uploadID,filePath,fileName FROM Sets"))
            {
                while (reader.Read())
                {
                    var set = new ChartSetInfo()
                    {
                        ID = reader.GetInt64(0),
                        LastWriteTime = reader.GetInt64(1),
                        OnlineID = reader.GetValue(2) is DBNull ? (long?)null : reader.GetInt64(2),
                        FilePath = reader.GetString(3),
                        FileName = reader.GetString(4),
                    };
                    m_chartSets[set.ID] = set;

                    Logger.Log($@"DB Loaded Chart Set { set.ID }:
    lwt={ set.LastWriteTime },
    uploadID={ set.OnlineID  },
    filePath={ set.FilePath },
    fileName={ set.FileName }");
                }
            }

            using (var reader = ExecReader("SELECT id,setId,lwt,fileName,songTitle,songArtist,songFileName,songVolume,charter,jacketFileName,jacketArtist,backgroundFileName,backgroundArtist,diffLevel,diffIndex,diffName,diffNameShort,diffColor,chartDuration,tags FROM Charts"))
            {
                while (reader.Read())
                {
                    var set = m_chartSets[reader.GetInt64(1)];
                    var chart = new ChartInfo();
                    chart.ID = reader.GetInt64(0);
                    chart.LastWriteTime = reader.GetInt64(2);
                    chart.FileName = reader.GetString(3);
                    chart.SongTitle = reader.GetString(4);
                    chart.SongArtist = reader.GetString(5);
                    chart.SongFileName = reader.GetString(6);
                    chart.SongVolume = reader.GetInt32(7);
                    chart.Charter = reader.GetString(8);
                    chart.JacketFileName = reader.GetString(9);
                    chart.JacketArtist = reader.GetString(10);
                    chart.BackgroundFileName = reader.GetString(11);
                    chart.BackgroundArtist = reader.GetString(12);
                    chart.DifficultyLevel = reader.GetDouble(13);
                    chart.DifficultyIndex = reader.GetValue(14) is DBNull ? (int?)null : reader.GetInt32(14);
                    chart.DifficultyName = reader.GetString(15);
                    chart.DifficultyNameShort = reader.GetString(16);
                    chart.DifficultyColor = reader.GetValue(17) is DBNull ? (Vector3?)null : Color.HexToVector3(reader.GetInt32(17));
                    chart.ChartDuration = reader.GetDouble(18);
                    chart.Tags = reader.GetString(19);

                    set.Charts.Add(chart);
                    chart.Set = set;

                    Logger.Log($@"DB Loaded Chart { chart.ID } in Set { chart.SetID }:
    lwt={ chart.LastWriteTime },
    fileName={ chart.FileName },
    songTitle={ chart.SongTitle },
    songArtist={ chart.SongArtist },
    songFileName={ chart.SongFileName },
    songVolume={ chart.SongVolume },
    charter={ chart.Charter },
    jacketFileName={ chart.JacketFileName },
    jacketArtist={ chart.JacketArtist },
    backgroundFileName={ chart.BackgroundFileName },
    backgroundArtist={ chart.BackgroundArtist },
    diffLevel={ chart.DifficultyLevel },
    diffIndex={ chart.DifficultyIndex },
    diffName={ chart.DifficultyName },
    diffNameShort={ chart.DifficultyNameShort },
    diffColor={ chart.DifficultyColor },
    chartDuration={ chart.ChartDuration },
    tags={ chart.Tags }");
                }
            }
        }

        public void SaveData()
        {
        }

        public void Update()
        {
        }

        public void Close()
        {
            m_connection.Close();
        }
    }
}

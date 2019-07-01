using System;
using System.Collections.Generic;
using System.Data.SQLite;
using theori.Charting;

namespace theori.Database
{
    public class ChartDatabase
    {
        private const int DEFAULT_VERSION = 1;

        public readonly string FilePath;

        public string ChartsDirectory { get; private set; }

        protected virtual int Version => DEFAULT_VERSION;

        private SQLiteConnection m_connection;

        private readonly Dictionary<long, ChartSetInfo> m_chartSets = new Dictionary<long, ChartSetInfo>();
        private readonly Dictionary<long, ChartInfo> m_charts = new Dictionary<long, ChartInfo>();

        private readonly Dictionary<string, ChartSetInfo> m_chartSetsByFilePath = new Dictionary<string, ChartSetInfo>();

        public ChartDatabase(string filePath)
        {
            FilePath = filePath;
            m_connection = new SQLiteConnection($"Data Source={ filePath }");
        }

        public void Open(string chartsDirectory)
        {
            ChartsDirectory = chartsDirectory;
            m_connection.Open();

            bool rebuild;
            try
            {
                int vGot = -1;
                using (var reader = ExecReader(""))
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
            var command = new SQLiteCommand(commandText, m_connection);
            return command.ExecuteNonQuery();
        }

        private SQLiteDataReader ExecReader(string commandText)
        {
            var command = new SQLiteCommand(commandText, m_connection);
            return command.ExecuteReader();
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

            Exec($@"CREATE TABLE Sets (
                id INTEGER PRIMARY KEY,
                uploadID INTEGER,
                filePath TEXT NOT NULL,
                songTitle TEXT NOT NULL,
                songArtist TEXT NOT NULL,
                songFileName TEXT NOT NULL
            )");

            Exec($@"
            CREATE TABLE Charts (
                id INTEGER PRIMARY KEY,
                fileName TEXT NOT NULL,
                lwt INTEGER NOT NULL,
                charter TEXT NOT NULL,
                jacketFileName TEXT,
                jacketArtist TEXT,
                backgroundFileName TEXT,
                backgroundArtist TEXT,
                diffLevel REAL NOT NULL,
                diffIndex INTEGER,
                diffName TEXT NOT NULL,
                diffNameShort TEXT,
                diffColor INTEGER,
                chartDuration INTEGER NOT NULL,
                FOREIGN KEY(setId) REFERENCES Sets(id)
            )");
            
            Exec($@"CREATE TABLE Scores (
                id INTEGER PRIMARY KEY,
                score INTEGER NOT NULL,
                FOREIGN KEY(chartId) REFERENCES Charts(id)
            )");
        }

        public void LoadData()
        {
        }

        public void SaveData()
        {
        }
    }
}

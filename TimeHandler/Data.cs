using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;

namespace TimeHandler
{
    class Data : IDisposable
    {
        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1).ToUniversalTime().Date;
        private SQLiteConnection _connection;
        public Data()
        {
            this._connection = GetConnection();
        }

        public static List<TimeEntry> GetThisWeeksEntries()
        {
            return GetWeekEntries().Select(e => new TimeEntry
            {
                what = (TimeEntryEvent)e.What,
                when = EPOCH.AddMilliseconds(e.When).ToLocalTime(),
            })
            .ToList();
        }

        public static Week GetThisWeek()
        {
            return GetThisWeeksEntries().Aggregate(new Dictionary<DateTime, List<TimeEntry>>(),
                (acc, entry) =>
                {
                    if (acc.ContainsKey(entry.when.Date))
                    {
                        acc[entry.when.Date].Add(entry);
                    }
                    else
                    {
                        acc[entry.when.Date] = new List<TimeEntry> { entry };
                    }
                    return acc;
                })
                .Aggregate(new Week(), (acc, day) =>
                {
                    var minutes = 0;
                    DateTime? lastDate = null;
                    foreach (var entry in day.Value)
                    {
                        switch (entry.what)
                        {
                            case TimeEntryEvent.StartOfDay:
                            case TimeEntryEvent.EndOfBreak:
                                lastDate = entry.when;
                                break;
                            case TimeEntryEvent.EndOfDay:
                            case TimeEntryEvent.StartOfBreak:
                                if (lastDate.HasValue)
                                {
                                    minutes += (int)(entry.when - lastDate.Value).TotalMinutes;
                                    lastDate = null;
                                }
                                break;
                        }
                    }
                    if (day.Key.DayOfYear == DateTime.Now.DayOfYear && lastDate.HasValue)
                    {
                        minutes += (int)(DateTime.Now - lastDate.Value).TotalMinutes;
                    }
                    acc.Add(day.Key.DayOfWeek, minutes);
                    return acc;
                });
        }

        private static IQueryable<DbTimeEntry> GetWeekEntries(int weeksInPast = 0)
        {
            var monday = DateTime.Now.Date;
            while (monday.DayOfWeek != DayOfWeek.Monday)
            {
                monday = monday.AddDays(-1);
            }
            for (var i = 0; i < weeksInPast; i++)
            {
                monday = monday.AddDays(-7);
            }
            var utc = monday.ToUniversalTime();
            var minTimestamp = (monday - EPOCH).TotalMilliseconds;
            var maxTimeStamp = (monday.AddDays(7) - EPOCH).TotalMilliseconds;
            var entries = new List<DbTimeEntry>();
            using (var data = new Data())
            {
                using (var cmd = data.GenerateCommand("SELECT \"what\", \"when\" FROM \"entries\" WHERE \"when\" > @min AND \"when\" < @max"))
                {
                    cmd.Parameters.AddWithValue("@min", minTimestamp);
                    cmd.Parameters.AddWithValue("@max", maxTimeStamp);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var what = reader.GetInt32(0);
                            var when = (double)reader.GetValue(1);
                            entries.Add(new DbTimeEntry
                            {
                                What = what,
                                When = when,
                            });
                        }
                    }
                }
            }
            return entries.AsQueryable();
        }

        public static void AddStartOfDay()
        {
            var now = DateTime.UtcNow;
            using (var data = new Data())
            {
                if (data.HasStartOfDay())
                {
                    return;
                }
                data._AddEntry(new TimeEntry
                {
                    what = TimeEntryEvent.StartOfDay,
                    when = now
                });
            }
        }

        public bool HasStartOfDay()
        {
            var now = DateTime.UtcNow;
            var timeMin = (now.Date - EPOCH).TotalMilliseconds;
            var timeMax = (now.Date.AddDays(1) - EPOCH).TotalMilliseconds;
            using (var cmd = this.GenerateCommand("SELECT count(*) FROM \"entries\" WHERE \"when\" > @min AND \"when\" < @max AND \"what\" = @what"))
            {
                cmd.Parameters.AddWithValue("@min", timeMin);
                cmd.Parameters.AddWithValue("@max", timeMax);
                cmd.Parameters.AddWithValue("@what", (int)TimeEntryEvent.StartOfDay);
                var ct = Convert.ToInt32(cmd.ExecuteScalar());
                return ct > 0;
            }
        }

        public static void AddEntry(TimeEntry entry)
        {
            using (var data = new Data())
            {
                data._AddEntry(entry);
                
            }
        }

        private void _AddEntry(TimeEntry entry)
        {
            using (var cmd = this.GenrateInsertCommand())
            {
                var when = (entry.when - EPOCH).TotalMilliseconds;
                ExecuteInsert(cmd, (int)entry.what, when);
            }
        }
        
        public static void AddEntries(IEnumerable<TimeEntry> entries)
        {
            using (var data = new Data())
            {
                data._AddEntries(entries);
            }
        }

        private void _AddEntries(IEnumerable<TimeEntry> entries)
        {
            using (var cmd = this.GenrateInsertCommand()) {
                foreach (var entry in entries)
                {
                    ExecuteInsert(cmd, (int)entry.what, (entry.when - EPOCH).TotalMilliseconds);
                    cmd.Parameters.Clear();
                }
            }
        }

        private static void ExecuteInsert(SQLiteCommand cmd, int what, double when)
        {
            cmd.Parameters.AddWithValue("@what", what);
            cmd.Parameters.AddWithValue("@when", when);
            cmd.ExecuteNonQuery();
        }

        private SQLiteCommand GenrateInsertCommand()
        {
            return this.GenerateCommand("INSERT INTO \"entries\" (\"what\", \"when\") VALUES (@what, @when);");
        }

        private SQLiteCommand GenerateCommand(string text)
        {
            var cmd = this._connection.CreateCommand();
            cmd.CommandText = text;
            return cmd;
        }
        private static bool ValidatedTables = false;
        private static SQLiteConnection GetConnection()
        {
            var path = $"{TrayContext.APP_DATA_PATH }\\data.sqlite";
            if (!ValidatedTables && !System.IO.File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
            }
            var ret = new SQLiteConnection($"Data Source={path};Version=3;").OpenAndReturn();
            if (!ValidatedTables) {
                using (var cmd = ret.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if ((string)reader.GetValue(0) == "entries")
                            {
                                ValidatedTables = true;
                            }
                        }
                    }
                }
                if (!ValidatedTables)
                {
                    using (var cmd = ret.CreateCommand())
                    {
                        cmd.CommandText = "CREATE TABLE \"entries\" (\"what\" INTEGER NOT NULL, \"when\" REAL NOT NULL)";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return ret;
        }
        private class DbTimeEntry
        {
            public int Id
            {
                get; set;
            }
            public int What
            {
                get; set;
            }
            public double When
            {
                get; set;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    this._connection.Close();
                    this._connection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Data() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }


    class TimeEntry
    {
        public TimeEntryEvent what;
        public DateTime when;

    }

    public enum TimeEntryEvent
    {
        StartOfDay,
        EndOfDay,
        StartOfBreak,
        EndOfBreak,
    }
}

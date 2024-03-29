﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Text;

namespace TimeHandler
{
    public struct ReportDay
    {
        public DateTime Date;
        public int WorkTime;
        public int PTOTime;
        public List<JiraStory> Stories;

        public string ToCsv()
        {
            double work = this.WorkTime / 60.0;
            double pto = this.PTOTime / 60.0;
            var storyKeys = this.Stories?.Select(story => story.Key).ToList() ?? new List<string>();
            return $"{this.Date:ddd M/d/yyyy},{work:F2},{pto:F2},{string.Join("|", storyKeys)}";
        }
    }
    public class Report
    {
        public DateTime Monday;
        public List<ReportDay> Days;
        
        public string Headers()
        {
            return "Date,Worked,PTO,Stories";
        }
        public string FileName()
        {
            return $"TimeHandlerReport-{this.Monday:yyyy-MM-dd}";
        }
        public string Totals()
        {
            var totalWork = 0.0;
            var totalPTO = 0.0;
            foreach (var day in this.Days)
            {
                totalWork += day.WorkTime / 60.0;
                totalPTO += day.PTOTime / 60.0;
            }
            return $"Total,{totalWork:F2},{totalPTO:F2}";
        }
    }
    class Data : IDisposable
    {
        private SQLiteConnection _connection;
        public static event EventHandler DataChanged;
        public Data()
        {
            this._connection = GetConnection();
        }

        public static List<TimeEntry> GetThisWeeksEntries()
        {
            return GetWeekEntries()
                .ToList();
        }

        public static Week GetThisWeek()
        {
            return GetThisWeeksEntries()
                .Aggregate(new Dictionary<DateTime, List<TimeEntry>>(),
                (acc, entry) =>
                {
                    if (acc.ContainsKey(entry.When.Date))
                    {
                        acc[entry.When.Date].Add(entry);
                        acc[entry.When.Date].Sort((lhs, rhs) => lhs.When.CompareTo(rhs.When));
                    }
                    else
                    {
                        acc[entry.When.Date] = new List<TimeEntry> { entry };
                    }
                    return acc;
                })
                .Aggregate(new Week(), (acc, day) =>
                {
                    var minutes = 0;
                    DateTime? lastDate = null;
                    foreach (var entry in day.Value)
                    {
                        switch (entry.What)
                        {
                            case TimeEntryEvent.StartOfDay:
                            case TimeEntryEvent.EndOfBreak:
                                lastDate = entry.When;
                                break;
                            case TimeEntryEvent.EndOfDay:
                            case TimeEntryEvent.StartOfBreak:
                                if (lastDate.HasValue)
                                {
                                    minutes += (int)(entry.When - lastDate.Value).TotalMinutes;
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

        public static List<ReportDay> GetReportDays(int weeksInPast)
        {
            return GetWeekEntries(weeksInPast)
                .Aggregate(new Dictionary<DateTime, List<TimeEntry>>(), AggregateTimeEntries)
                .Aggregate(new List<ReportDay>(), AggregateReportDays);
        }

        private static Dictionary<DateTime, List<TimeEntry>> AggregateTimeEntries(Dictionary<DateTime, List<TimeEntry>> acc, TimeEntry entry)
        {
            if (acc.ContainsKey(entry.When.Date))
            {
                acc[entry.When.Date].Add(entry);
                acc[entry.When.Date].Sort((lhs, rhs) => lhs.When.CompareTo(rhs.When));
            }
            else
            {
                acc[entry.When.Date] = new List<TimeEntry> { entry };
            }
            return acc;
        }

        private static List<ReportDay> AggregateReportDays(List<ReportDay> acc, KeyValuePair<DateTime, List<TimeEntry>> day)
        {
            var work = 0;
            var pto = 0;
            DateTime? lastDate = null;
            foreach (var entry in day.Value)
            {
                switch (entry.What)
                {
                    case TimeEntryEvent.StartOfDay:
                    case TimeEntryEvent.EndOfBreak:
                        lastDate = entry.When;
                        break;
                    case TimeEntryEvent.EndOfDay:
                    case TimeEntryEvent.StartOfBreak:
                        if (lastDate.HasValue)
                        {
                            var minutes = (int)(entry.When - lastDate.Value).TotalMinutes;
                            if (entry.IsPto)
                            {
                                pto += minutes;
                            }
                            else
                            {
                                work += minutes;
                            }
                            lastDate = null;
                        }
                        break;
                }
            }
            acc.Add(new ReportDay
            {
                Date = day.Key,
                PTOTime = pto,
                WorkTime = work,
                Stories = new List<JiraStory>()
            });
            return acc;
        }

        public static void BulkUpsertPto(PtoWeekEntrys entries)
        {
            using (var data = new Data())
            {
                foreach (var entry in entries)
                {
                    _UpsertPto(data, entry);
                }
            }
        }

        private static void _UpsertPto(Data data, PtoWeekEntry entry)
        {
            if (entry.Start != null)
            {
                _UpsertPtoPart(data, entry.Start);
            }
            if (entry.End != null)
            {
                _UpsertPtoPart(data, entry.End);
            }
        }

        private static void _UpsertPtoPart(Data data, TimeEntry entry)
        {
            if (entry == null)
            {
                return;
            }
            if (entry.Id < 0)
            {
                data._AddEntry(entry);
            }
            else
            {
                data._UpdateEntry(entry);
            }
        }

        private void _UpdateEntry(TimeEntry entry)
        {
            using (var cmd = this.GenerateCommand("UPDATE \"entries\" SET \"when\" = @when, \"what\" = @what, \"is_pto\" = @pto WHERE \"id\" = @id"))
            {
                cmd.Parameters.AddWithValue("@when", entry.When.ToUniversalTime());
                cmd.Parameters.AddWithValue("@what", (int)entry.What);
                cmd.Parameters.AddWithValue("@pto", entry.IsPto);
                cmd.Parameters.AddWithValue("@id", entry.Id);
                cmd.ExecuteNonQuery();
            }
            FireChangeEvent();
        }

        public static void AddWeek(Week week, bool isPto = false)
        {
            var entries = new List<TimeEntry>();
            var monday = Monday().ToUniversalTime();
            MaybeAddEntry(week.Monday, monday, entries, isPto);
            MaybeAddEntry(week.Tuesday, monday.AddDays(1), entries, isPto);
            MaybeAddEntry(week.Wednesday, monday.AddDays(2), entries, isPto);
            MaybeAddEntry(week.Thursday, monday.AddDays(3), entries, isPto);
            MaybeAddEntry(week.Friday, monday.AddDays(4), entries, isPto);
            MaybeAddEntry(week.Saturday, monday.AddDays(5), entries, isPto);
            MaybeAddEntry(week.Sunday, monday.AddDays(6), entries, isPto);
            AddEntries(entries);
        }

        private static void MaybeAddEntry(int minutes, DateTime day, List<TimeEntry> list, bool isPto = false)
        {
            if (minutes < 1)
            {
                return;
            }
            list.Add(new TimeEntry
            {
                What = TimeEntryEvent.StartOfDay,
                When = day,
                IsPto = isPto
            });
            list.Add(new TimeEntry
            {
                What = TimeEntryEvent.EndOfDay,
                When = day.AddMinutes(minutes),
                IsPto = isPto
            });
        }

        private static IQueryable<TimeEntry> GetWeekEntries(int weeksInPast = 0)
        {
            var monday = Monday(weeksInPast);
            var end = monday.AddDays(7);
            return GetEntriesFor(monday, end);
        }

        public static IQueryable<TimeEntry> GetEntriesFor(DateTime start, DateTime end)
        {
            var entries = new List<TimeEntry>();
            using (var data = new Data())
            {
                using (var cmd = data.GenerateCommand("SELECT id, what, \"when\", is_pto FROM \"entries\" WHERE \"when\" > @min AND \"when\" < @max ORDER BY \"when\""))
                {
                    cmd.Parameters.AddWithValue("@min", start);
                    cmd.Parameters.AddWithValue("@max", end);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var what = reader.GetInt32(1);
                            var when = reader.GetDateTime(2);
                            var isPto = reader.GetBoolean(3);
                            entries.Add(new TimeEntry
                            {
                                Id = id,
                                What = (TimeEntryEvent)what,
                                When = when,
                                IsPto = isPto
                            });
                        }
                    }
                }
            }
            return entries.AsQueryable();
        }

        private static DateTime Monday(int weeksInPast = 0)
        {
            var start = DateTime.Now.Date.AddDays(-7 * weeksInPast);
            return Monday(start);
        }

        public static DateTime Monday(DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(-1);
            }
            return date;
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
                    What = TimeEntryEvent.StartOfDay,
                    When = now
                });
            }
        }

        public static void AddEndOfDay(DateTime? when)
        {
            using (var data = new Data())
            {
                if (data.HasEndOfDay())
                {
                    return;
                }
                if (!when.HasValue)
                {
                    when = DateTime.UtcNow;
                }
                data._AddEntry(new TimeEntry
                {
                    What = TimeEntryEvent.EndOfDay,
                    When = when.Value
                });
            }
        }

        public bool HasStartOfDay()
        {
            var now = DateTime.UtcNow.Date;
            using (var cmd = this.GenerateCommand("SELECT count(*) FROM \"entries\" WHERE \"when\" > @min AND \"when\" < @max AND \"what\" = @what"))
            {
                cmd.Parameters.AddWithValue("@min", now);
                cmd.Parameters.AddWithValue("@max", now.AddDays(1));
                cmd.Parameters.AddWithValue("@what", (int)TimeEntryEvent.StartOfDay);
                var ct = Convert.ToInt32(cmd.ExecuteScalar());
                return ct > 0;
            }
        }

        public bool HasEndOfDay()
        {
            var now = DateTime.UtcNow.Date;
            using (var cmd = this.GenerateCommand("SELECT count(*) FROM \"entries\" WHERE \"when\" > @min AND \"when\" < @max AND \"what\" = @what"))
            {
                cmd.Parameters.AddWithValue("@min", now);
                cmd.Parameters.AddWithValue("@max", now.AddDays(1));
                cmd.Parameters.AddWithValue("@what", (int)TimeEntryEvent.EndOfDay);
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
            FireChangeEvent();
        }

        private void _AddEntry(TimeEntry entry)
        {
            using (var cmd = this.GenrateInsertCommand())
            {
                ExecuteInsert(cmd, (int)entry.What, entry.When.ToUniversalTime(), entry.IsPto);
            }
        }
        
        public static void AddEntries(IEnumerable<TimeEntry> entries)
        {
            using (var data = new Data())
            {
                data._AddEntries(entries);
            }
            FireChangeEvent();
        }

        private void _AddEntries(IEnumerable<TimeEntry> entries)
        {
            using (var cmd = this.GenrateInsertCommand()) {
                foreach (var entry in entries)
                {
                    ExecuteInsert(cmd, (int)entry.What, entry.When.ToUniversalTime(), entry.IsPto);
                    cmd.Parameters.Clear();
                }
            }
        }

        public static List<TimeEntry> GetDayEntries(DateTime date)
        {
            var midnight = date.ToUniversalTime().Date;
            var tomorrow = midnight.AddDays(1);
            return GetEntriesFor(midnight, tomorrow).ToList();
        }

        public static DateTime GetDateFor(DayOfWeek dow, int weeksInPast = 0)
        {
            var start = DateTime.Now.Date.AddDays(-weeksInPast);
            if (start.DayOfWeek == dow)
            {
                return start;
            }
            int increment = -1;
            if (dow > start.DayOfWeek || dow == DayOfWeek.Sunday)
            {
                increment = 1;
            }
            while (start.DayOfWeek != dow)
            {
                start = start.AddDays(increment);
            }
            return start;
        }

        public static DateTime GetDateFor(DayOfWeek dow, DateTime start)
        {
            if (start.DayOfWeek == dow)
            {
                return start;
            }
            int increment = -1;
            if (dow > start.DayOfWeek || dow == DayOfWeek.Sunday)
            {
                increment = 1;
            }
            while (start.DayOfWeek != dow)
            {
                start = start.AddDays(increment);
            }
            return start;
        }

        public static void UpdateEntry(int id, TimeEntryEvent what, DateTime when)
        {
            using (var data = new Data())
            {
                using (var cmd = data.GenerateCommand("UPDATE \"entries\" SET \"what\" = @what, \"when\" = @when WHERE id = @id"))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@what", (int)what);
                    cmd.Parameters.AddWithValue("@when", when);
                    cmd.ExecuteNonQuery();
                }
            }
            FireChangeEvent();
        }

        public static void RemoveEntry(int id)
        {
            using (var data = new Data())
            {
                using (var cmd = data.GenerateCommand("DELETE FROM \"entries\" WHERE id = @id"))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var ct = cmd.ExecuteNonQuery();
                    Logger.Log($"Deleted {ct} rows");
                }
            }
        }

        private static void ExecuteInsert(SQLiteCommand cmd, int what, DateTime when, bool isPto = false)
        {
            cmd.Parameters.AddWithValue("@what", what);
            cmd.Parameters.AddWithValue("@when", when);
            cmd.Parameters.AddWithValue("@pto", isPto);
            cmd.ExecuteNonQuery();
        }

        public static void InsertLog(DateTime when, string level, string message)
        {
            using (var data = new Data())
            {
                using (var cmd = data.GenerateCommand("INSERT INTO \"logs\" (\"when\", \"level\", \"message\") VALUES (@when, @level, @message)"))
                {
                    cmd.Parameters.AddWithValue("@when", when);
                    cmd.Parameters.AddWithValue("@level", level);
                    cmd.Parameters.AddWithValue("@message", message);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static Report GenerateReport(int weeksInPast)
        {
            var entries = GetReportDays(weeksInPast);
            var monday = Monday(weeksInPast);
            var end = monday.AddDays(7);
            var jc = new JiraClient();
            var jiraIssues = jc.GetIssues(monday, end).Result;
            foreach (var issue in jiraIssues)
            {
                foreach (var day in entries)
                {
                    if (issue.ShouldIncludeIn(day.Date))
                    {
                        day.Stories.Add(issue);
                    }
                }
            }
            entries.Sort((lhs, rhs) => lhs.Date.CompareTo(rhs.Date));
            if (entries.Count > 0)
            {
                return new Report
                {
                    Monday = entries[0].Date,
                    Days = entries
                };
            } else
            {
                return new Report
                {
                    Monday = DateTime.MinValue,
                    Days = entries
                };
            }
        }

        private SQLiteCommand GenrateInsertCommand()
        {
            return this.GenerateCommand("INSERT INTO \"entries\" (\"what\", \"when\", \"is_pto\") VALUES (@what, @when, @pto);");
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
            var entriesExists = ValidateEntries(ref ret);
            if (!entriesExists) {
                using (var cmd2 = ret.CreateCommand())
                {
                    cmd2.CommandText = "CREATE TABLE \"entries\" (\"id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, \"what\" INTEGER NOT NULL, \"when\" TIMESTAMP NOT NULL, \"is_pto\" BIT NOT NULL DEFAULT 0)";
                    cmd2.ExecuteNonQuery();
                }
                entriesExists = ValidateEntries(ref ret);
            }
            var logsExists = ValidateLogs(ref ret);
            if (!logsExists)
            {
                using (var cmd = ret.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE \"logs\" (\"id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, \"when\" TIMESTAMP NOT NULL, \"level\" TEXT NOT NULL, \"message\" TEXT NOT NULL)";
                    cmd.ExecuteNonQuery();
                }
                logsExists = ValidateEntries(ref ret);
            }
            ValidatedTables = entriesExists && logsExists;
            return ret;
        }

        public static void CleanLogs()
        {
            using (var data = new Data())
            {
                using (var cmd = data.GenerateCommand("DELETE FROM \"logs\" WHERE \"when\" < @limit"))
                {
                    cmd.Parameters.AddWithValue("@limit", DateTime.Now.Date - TimeSpan.FromDays(7));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static bool ValidateEntries(ref SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name = 'entries'";
                var ct = Convert.ToInt32(cmd.ExecuteScalar());
                if (ct > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ValidateLogs(ref SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name = 'logs'";
                var ct = Convert.ToInt32(cmd.ExecuteScalar());
                if (ct > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static void FireChangeEvent()
        {
            if (Data.DataChanged != null)
            {
                Data.DataChanged.Invoke(null, null);
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


    public class TimeEntry
    {
        public int Id;
        public TimeEntryEvent What;
        public DateTime When;
        public bool IsPto;
        public TimeEntry()
        {
            Id = -1;
        }
    }

    public enum TimeEntryEvent
    {
        StartOfDay,
        EndOfDay,
        StartOfBreak,
        EndOfBreak,
    }

    public static class TimeEntryEventExtensions
    {
        public static string ToString(this TimeEntryEvent entry)
        {
            switch (entry)
            {
                case TimeEntryEvent.StartOfDay:
                    return "Start Day";
                case TimeEntryEvent.StartOfBreak:
                    return "Start Break";
                case TimeEntryEvent.EndOfBreak:
                    return "End Break";
                case TimeEntryEvent.EndOfDay:
                    return "End Day";
                default:
                    throw new Exception("Unknown TimeEntryEvent");
            }
        }

        public static TimeEntryEvent Next(this TimeEntryEvent entry)
        {
            switch (entry)
            {
                case TimeEntryEvent.StartOfDay:
                    return TimeEntryEvent.EndOfDay;
                case TimeEntryEvent.StartOfBreak:
                    return TimeEntryEvent.EndOfBreak;
                case TimeEntryEvent.EndOfBreak:
                    return TimeEntryEvent.EndOfDay;
                default:
                    return TimeEntryEvent.StartOfDay;
            }
        }
    }
}

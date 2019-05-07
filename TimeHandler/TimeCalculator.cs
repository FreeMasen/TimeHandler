using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace TimeHandler
{
    class TimeCalculator
    {
        private FileSystemWatcher Watcher;
        public event EventHandler<EventArgs> RunCalculation;
        public event EventHandler<WeekUpdatedEventArgs> CalculationComplete;
        public TimeCalculator()
        {
            this.Watcher = new FileSystemWatcher(TrayContext.APP_DATA_PATH);
            this.Watcher.Changed += this.FileChanged;
            this.StartCalculation();
            this.RunCalculation += this.OnRequestStartCalculation;
            this.AddStartOfDay();
        }

        public void Push30()
        {
            var now = DateTime.Now;
            var start = now.AddMinutes(30);
            this.AddEntries(new List<TimeEntry>() {
                new TimeEntry {when = start, what = TimeEntryEvent.StartOfBreak },
                new TimeEntry {when = now, what = TimeEntryEvent.EndOfBreak },
            });
        }

        public void AddEntries(IEnumerable<TimeEntry> entries)
        {
            Data.AddEntries(entries);
        }

        public void AddEntry(TimeEntry entry)
        {
            Data.AddEntry(entry);
        }

        public void AddStartOfDay()
        {
            Data.AddStartOfDay();
        }


        public void FileChanged(object sender, FileSystemEventArgs args)
        {
            switch (args.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    this.StartCalculation();
                break;
            }
        }

        public void FireCalculation()
        {
            if (this.RunCalculation != null)
            {
                this.RunCalculation.Invoke(new { }, new EventArgs());
            }
        }

        public void OnRequestStartCalculation(object sender, EventArgs args)
        {
            this.StartCalculation();
        }

        private void StartCalculation()
        {
            var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\calendar_info";
            var events = Data.GetThisWeeksEntries();
                
            this.Calculate(events);
            
        }

        private void Calculate(IEnumerable<TimeEntry> info)
        {
            var week = Data.GetThisWeek();
            if (this.CalculationComplete != null)
            {
                this.CalculationComplete.Invoke(
                    new { }, 
                    new WeekUpdatedEventArgs() { week = week }
                );
            }
        }
    }

    

    class WeekEntrys
    {
        public List<TimeEntry> Monday = new List<TimeEntry>();
        public List<TimeEntry> Tuesday = new List<TimeEntry>();
        public List<TimeEntry> Wednesday = new List<TimeEntry>();
        public List<TimeEntry> Thursday = new List<TimeEntry>();
        public List<TimeEntry> Friday = new List<TimeEntry>();
        public List<TimeEntry> Saturday = new List<TimeEntry>();
        public List<TimeEntry> Sunday = new List<TimeEntry>();
    }

    struct Week
    {
        public int Monday;
        public int Tuesday;
        public int Wednesday;
        public int Thursday;
        public int Friday;
        public int Saturday;
        public int Sunday;

        public Week(int m = 0, int t = 0, int w = 0,
                    int r = 0, int f = 0, int s = 0, 
                    int u = 0)
        {
            this.Monday = m;
            this.Tuesday = t;
            this.Wednesday = w;
            this.Thursday = r;
            this.Friday = f;
            this.Saturday = s;
            this.Sunday = u;
        }
        public int Total()
        {
            return this.Monday 
                    + this.Tuesday
                    + this.Wednesday
                    + this.Thursday
                    + this.Friday
                    + this.Saturday
                    + this.Sunday;
        }

        public int Expectation()
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 8 * 60;
                case DayOfWeek.Tuesday:
                    return 8 * 2 *60;
                case DayOfWeek.Wednesday:
                    return 8 * 3 * 60;
                case DayOfWeek.Thursday:
                    return 8 * 4 * 60;
                default:
                    return 40 * 60;
            }
        }

        public void Add(DayOfWeek day, int value)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    this.Monday += value;
                    break;
                case DayOfWeek.Tuesday:
                    this.Tuesday += value;
                    break;
                case DayOfWeek.Wednesday:
                    this.Wednesday += value;
                    break;
                case DayOfWeek.Thursday:
                    this.Thursday += value;
                    break;
                case DayOfWeek.Friday:
                    this.Friday += value;
                    break;
                case DayOfWeek.Saturday:
                    this.Saturday += value;
                    break;
                case DayOfWeek.Sunday:
                    this.Sunday += value;
                    break;
            }
        }
        public bool Equals(Week other)
        {
            return this.Monday == other.Monday
                && this.Tuesday == other.Tuesday
                && this.Wednesday == other.Wednesday
                && this.Thursday == other.Thursday
                && this.Friday == other.Friday
                && this.Saturday == other.Saturday
                && this.Sunday == other.Sunday;
        }
    }
}

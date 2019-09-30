using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace TimeHandler
{
    class TimeCalculator
    {
        public event EventHandler<EventArgs> RunCalculation;
        public event EventHandler<WeekUpdatedEventArgs> CalculationComplete;
        public TimeCalculator()
        {
            this.StartCalculation();
            this.RunCalculation += this.OnRequestStartCalculation;
            this.AddStartOfDay();
        }

        public void Push30()
        {
            var now = DateTime.Now;
            var start = now.AddMinutes(30);
            this.AddEntries(new List<TimeEntry>() {
                new TimeEntry {When = start, What = TimeEntryEvent.StartOfBreak },
                new TimeEntry {When = now, What = TimeEntryEvent.EndOfBreak },
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
            this.Calculate();
        }

        private void Calculate()
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

        public PtoWeekEntrys GetWeekPto(DateTime date)
        {
            var monday = Data.Monday(date);
            var entries = Data.GetEntriesFor(monday, monday.AddDays(7)).Where(entry => entry.IsPto);
            return entries.Aggregate(new PtoWeekEntrys(), PtoAggregator);
        }

        private PtoWeekEntrys PtoAggregator(PtoWeekEntrys acc, TimeEntry entry)
        {
            switch (entry.When.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    TimeOfDayHandler(acc.Monday, entry);
                    break;
                case DayOfWeek.Tuesday:
                    TimeOfDayHandler(acc.Tuesday, entry);
                    break;
                case DayOfWeek.Wednesday:
                    TimeOfDayHandler(acc.Wednesday, entry);
                    break;
                case DayOfWeek.Thursday:
                    TimeOfDayHandler(acc.Thursday, entry);
                    break;
                case DayOfWeek.Friday:
                    TimeOfDayHandler(acc.Friday, entry);
                    break;
                case DayOfWeek.Saturday:
                    TimeOfDayHandler(acc.Saturday, entry);
                    break;
                case DayOfWeek.Sunday:
                    TimeOfDayHandler(acc.Sunday, entry);
                    break;

            }
            return acc;
        }
        private void TimeOfDayHandler(PtoWeekEntry day, TimeEntry time)
        {
            switch (time.What)
            {
                case TimeEntryEvent.StartOfDay:
                    day.Start = time;
                    break;
                case TimeEntryEvent.EndOfDay:
                    day.End = time;
                    break;
            }
        }
    }

    

    public class PtoWeekEntrys : IEnumerable<PtoWeekEntry>
    {
        public PtoWeekEntry Monday = new PtoWeekEntry(DayOfWeek.Monday);
        public PtoWeekEntry Tuesday = new PtoWeekEntry(DayOfWeek.Tuesday);
        public PtoWeekEntry Wednesday = new PtoWeekEntry(DayOfWeek.Wednesday);
        public PtoWeekEntry Thursday = new PtoWeekEntry(DayOfWeek.Thursday);
        public PtoWeekEntry Friday = new PtoWeekEntry(DayOfWeek.Friday);
        public PtoWeekEntry Saturday = new PtoWeekEntry(DayOfWeek.Saturday);
        public PtoWeekEntry Sunday = new PtoWeekEntry(DayOfWeek.Sunday);

        IEnumerator<PtoWeekEntry> IEnumerable<PtoWeekEntry>.GetEnumerator()
        {
            return new PtoWeekEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PtoWeekEnumerator(this);
        }
    }

    public class PtoWeekEnumerator : IEnumerator<PtoWeekEntry>
    {
        private int currentIdx = -1;
        private readonly PtoWeekEntrys Inner;
        public PtoWeekEnumerator(PtoWeekEntrys inner)
        {
            this.Inner = inner;
        }

        PtoWeekEntry IEnumerator<PtoWeekEntry>.Current => this.GetCurrent();

        object IEnumerator.Current => this.GetCurrent();

        private PtoWeekEntry GetCurrent()
        {
            switch (this.currentIdx)
            {
                case 0:
                    return this.Inner.Monday;
                case 1:
                    return this.Inner.Tuesday;
                case 2:
                    return this.Inner.Wednesday;
                case 3:
                    return this.Inner.Thursday;
                case 4:
                    return this.Inner.Friday;
                case 5:
                    return this.Inner.Saturday;
                case 6:
                    return this.Inner.Sunday;
                default:
                    return null;
            }
        }

        bool IEnumerator.MoveNext()
        {
            this.currentIdx += 1;
            if (this.currentIdx > 6)
            {
                return false;
            }
            return true;
        }

        void IEnumerator.Reset()
        {
            this.currentIdx = -1;
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
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PtoWeekEnumerator() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

    public class PtoWeekEntry
    {
        public TimeEntry Start;
        public TimeEntry End;
        public DayOfWeek Dow;
        public PtoWeekEntry(DayOfWeek dow)
        {
            this.Dow = dow;
        }
        public void SetNewEnd(double minutes)
        {
            if (this.Start == null)
            {
                return;
            }
            this.End.When = this.Start.When.AddMinutes(minutes);
        }

        public Time CalculateDiff()
        {
            if (Start == null)
            {
                return new Time(0, 0);
            }
            if (End == null)
            {
                return new Time(8, 0);
            }
            var total = (End.When - Start.When).TotalMinutes;
            var hours = (int)Math.Floor(total / 60);
            var minutes = (int)(total - (double)hours * 60.0);
            return new Time(hours, minutes);
        }

        public double CalculateSimpleDiff()
        {
            if (Start == null)
            {
                return 0.0;
            }
            if (End == null)
            {
                return 8.0 * 60.0;
            }
            return (End.When - Start.When).TotalMinutes;
        }
    }

    public struct Time
    {
        public int Hours;
        public int Minutes;
        public Time(int hours, int minutes)
        {
            this.Hours = hours;
            this.Minutes = minutes;
        }
    }

    public struct Week: IEnumerable<int>
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
            var ret = 0;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return this.Monday;
                case DayOfWeek.Tuesday:
                    return this.Monday 
                        + this.Tuesday;
                case DayOfWeek.Wednesday:
                    return this.Monday
                        + this.Tuesday
                        + this.Wednesday;
                case DayOfWeek.Thursday:
                    return this.Monday
                        + this.Tuesday
                        + this.Wednesday
                        + this.Thursday;
                case DayOfWeek.Friday:
                    return this.Monday
                        + this.Tuesday
                        + this.Wednesday
                        + this.Thursday
                        + this.Friday;
                case DayOfWeek.Saturday:
                    return this.Monday
                        + this.Tuesday
                        + this.Wednesday
                        + this.Thursday
                        + this.Friday
                        + this.Saturday;
                default:
                    return this.Monday
                        + this.Tuesday
                        + this.Wednesday
                        + this.Thursday
                        + this.Friday
                        + this.Saturday
                        + this.Sunday;
            }
            
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

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            return new WeekEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new WeekEnumerator(this);
        }
    }

    public class WeekEnumerator : IEnumerator<int>
    {
        private readonly Week Inner;
        private int currentIdx = -1;
        public WeekEnumerator(Week inner)
        {
            this.Inner = inner;
        }

        int IEnumerator<int>.Current => this.GetCurrent();

        object IEnumerator.Current => this.GetCurrent();
        private int GetCurrent()
        {
            switch (this.currentIdx)
            {
                case 0:
                    return this.Inner.Monday;
                case 1:
                    return this.Inner.Tuesday;
                case 2:
                    return this.Inner.Wednesday;
                case 3:
                    return this.Inner.Thursday;
                case 4:
                    return this.Inner.Friday;
                case 5:
                    return this.Inner.Saturday;
                case 6:
                    return this.Inner.Sunday;
                default:
                    return 0;
            }
        }
        bool IEnumerator.MoveNext()
        {
            this.currentIdx += 1;
            if (this.currentIdx > 6)
            {
                return false;
            }
            return true;
        }

        void IEnumerator.Reset()
        {
            this.currentIdx = -1;
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
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~WeekEnumerator() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

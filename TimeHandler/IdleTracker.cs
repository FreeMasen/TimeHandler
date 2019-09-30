using System;
using System.Runtime.InteropServices;
using System.Timers;

namespace TimeHandler
{
    enum SleepState
    {
        None,
        Break,
        EndOfDay,
    }
    class IdleTracker
    {
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();
        private readonly Timer IdleCheckTimer;
        public event EventHandler<int> IdleLimitReached;
        public event EventHandler<SleepState> WokeUp;
        public event EventHandler<int> BreakLimitReached;

        public uint limit;
        public uint breakLimit;
        private SleepState SleepLevel = SleepState.None;
        private double lastMs = 0;

        public IdleTracker(int limitMinutes, int breakLimit = 5)
        {
            this.limit = (uint)TimeSpan.FromMinutes(limitMinutes).TotalMilliseconds;
            this.breakLimit = (uint)TimeSpan.FromMinutes(breakLimit).TotalMilliseconds;
            this.IdleCheckTimer = new Timer
            {
                Interval = TimeSpan.FromMinutes(1).TotalMilliseconds,
                Enabled = true,
                AutoReset = true
            };
            this.IdleCheckTimer.Elapsed += this.TimerElapsed;
            this.IdleCheckTimer.Start();
            this.TimerElapsed(null, null);
        }

        public uint GetCurrentIdle()
        {
            var ticks = new LASTINPUTINFO();
            ticks.cbSize = (uint)Marshal.SizeOf(ticks);
            if (!IdleTracker.GetLastInputInfo(ref ticks))
            {
                throw new Exception(GetLastError().ToString());
            }

            return ticks.dwTime;
        }

        public void TimerElapsed(object sender, ElapsedEventArgs args)
        {
            var idle = this.GetCurrentIdle();
            var ms = ((uint)Environment.TickCount) - idle;
            var minutes = TimeSpan.FromMilliseconds(ms).TotalMinutes;
            if (this.HandleWake(ms))
            {
                return;
            }
            switch (this.SleepLevel)
            {
                case SleepState.None:
                    this.HandleBreakStart(ms, minutes);
                    break;
                case SleepState.Break:
                    this.HandleEodStart(ms, minutes);
                    break;
            }
            this.lastMs = ms;
        }

        public void HandleBreakStart(uint ms, double minutes)
        {
            if (this.SleepLevel != SleepState.None)
            {
                return;
            }
            if (ms < this.breakLimit)
            {
                return;
            }
            this.SleepLevel = SleepState.Break;
            if (this.BreakLimitReached != null)
            {
                this.BreakLimitReached.Invoke(null, (int)minutes);
            }
        }
        private void HandleEodStart(uint ms, double minutes)
        {
            if (this.SleepLevel != SleepState.Break)
            {
                return;
            }
            if (ms < this.limit)
            {
                return;
            }
            this.SleepLevel = SleepState.EndOfDay;
            if (this.IdleLimitReached != null)
            {
                this.IdleLimitReached.Invoke(null, (int)minutes);
            }
        }
        private bool HandleWake(uint ms)
        {
            if (this.lastMs < ms)
            {
                return false;
            }
            if (this.WokeUp != null && this.SleepLevel != SleepState.None)
            {
                this.WokeUp.Invoke(null, this.SleepLevel);
            }
            this.SleepLevel = SleepState.None;
            return true;
        }
    }

    internal struct LASTINPUTINFO
    {
        public uint cbSize;

        public uint dwTime;
    }
}

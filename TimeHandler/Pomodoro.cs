using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeHandler
{
    class Pomodoro
    {
        public event EventHandler<string> Switch;
        private int counter = 0;
        private Timer t;
        public Pomodoro()
        {
            this.t = new Timer(
                _ => { },
                null,
                Timeout.Infinite,
                Timeout.Infinite
            );
        }

        private void StartNewBreakTimer(int minutes)
        {
            Logger.Log("StartNewBreakTimer");
            t = new Timer(
                _ => this.FireBreakEnd(), 
                null, 
                TimeSpan.FromMinutes(minutes),
                Timeout.InfiniteTimeSpan
            );
        }

        private void StartNewWorkTimer()
        {
            Logger.Log("StartNewWorkTimer");
            t = new Timer(
                _ => this.FireWorkEnd(),
                null,
                TimeSpan.FromMinutes(25),
                Timeout.InfiniteTimeSpan
            );
        }

        private void FireBreakEnd()
        {
            Logger.Log("FireBreakEnd");
            this.StartNewWorkTimer();
            this.PerformSwitch("Back to work!");
        }

        private void FireWorkEnd()
        {
            Logger.Log("FireWorkEnd");
            this.counter += 1;
            var breakLength = 5;
            if (this.counter >= 4)
            {
                breakLength = 15;
            }
            this.StartNewBreakTimer(breakLength);
            this.PerformSwitch($"Breaktime! ({breakLength} min)");
        }

        private void PerformSwitch(string message)
        {
            Logger.Log($"PerformSwitch: {message}");
            if (this.Switch != null)
            {
                Logger.Log("Firing Event");
                this.Switch.Invoke(null, message);
            }
        }

        public void Stop()
        {
            if (this.t != null)
            {
                this.t.Change(Timeout.Infinite, Timeout.Infinite);
            }
            this.PerformSwitch("Pomodoro Stopped");
        }

        public void Start()
        {
            this.counter = 0;
            this.StartNewWorkTimer();
            this.PerformSwitch("Starting Pomodoro");
        }

    }
}

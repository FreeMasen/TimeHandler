using System;
using System.Windows.Forms;
using System.Threading;

namespace TimeHandler
{
    class TrayContext : ApplicationContext
    {
        private WorkedTimeForm _iconForm;
        private readonly NotifyIcon _icon;
        private Week week;
        public event EventHandler<EventArgs> TimerExpired;
        private readonly TimeCalculator _calc;
        private ContextMenuStrip contextMenuStrip1;
        private readonly ToolStripMenuItem exitToolStripMenuItem;
        private readonly ToolStripMenuItem refreshToolStripMenuItem;
        private readonly ToolStripMenuItem goHomeToolStripMenuItem;
        private readonly Watcher watcher;

        private System.Threading.Timer _timer;
        private DateTime lastNotified = new DateTime(0);
        private TrayContext()
        {
            this.contextMenuStrip1 = new ContextMenuStrip();
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            this.refreshToolStripMenuItem = new ToolStripMenuItem();
            this.goHomeToolStripMenuItem = new ToolStripMenuItem();

            this.contextMenuStrip1.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
                this.goHomeToolStripMenuItem,
                this.refreshToolStripMenuItem,
                this.exitToolStripMenuItem,
            });
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(211, 56);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);

            //
            // goHomeToolStripMenuItem
            //
            this.goHomeToolStripMenuItem.Name = "goHomeToolStripMenuItem";
            this.goHomeToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.goHomeToolStripMenuItem.Text = "Go Home";
            this.goHomeToolStripMenuItem.Click += new System.EventHandler((o, a) => 
                    Data.AddEntry(new TimeEntry { what = TimeEntryEvent.EndOfDay, when = DateTime.UtcNow }));

            this._calc = new TimeCalculator();
            //
            // refreshToolStripMenuItem
            //
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new EventHandler(this._calc.OnRequestStartCalculation);

            this.contextMenuStrip1.ResumeLayout(false);
            this._icon = new NotifyIcon()
            {
                ContextMenuStrip = contextMenuStrip1,
                Icon = TimeHandler.Properties.Resources.TrayIcon,
                Text = "Time Handler",
                Visible = true,
            };
            this._icon.MouseClick += this.OnClick;
            this._calc.CalculationComplete += this.OnWeekUpdated;
            this.TimerExpired += this._calc.OnRequestStartCalculation;
            this._timer = new System.Threading.Timer(this.TimerCB, null, 0, Timeout.Infinite);
            this.watcher = new Watcher();
        }

        private void TimerCB(object state)
        {
            if(this.TimerExpired != null)
            {
                this.TimerExpired.Invoke(new { }, new EventArgs());
            }
            this._timer = new System.Threading.Timer(TimerCB, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            this._timer.Change(TimeSpan.FromMinutes(5.0), Timeout.InfiniteTimeSpan);
            CheckToday();
            
        }

        private void CheckToday()
        {
            var now = DateTime.Now;
            var diff = this.week.Expectation() - this.week.Total();
            if (diff < 0 && (now - this.lastNotified).TotalHours > 1)
            {
                this.lastNotified = now;
                this._icon.BalloonTipText = $"Head Home? ({((float)diff / 60.0).ToString("F2")}) hours";
                this._icon.ShowBalloonTip(5000);
            }
        }

        private void OnWeekUpdated(object sender, WeekUpdatedEventArgs args)
        {
            this.week = args.week;
            if (this._iconForm != null && !this._iconForm.IsDisposed && !this._iconForm.Disposing && !this._iconForm.IsDisposed)
            {
                this._iconForm.Refresh(args.week);
            }
        }

        private void OnClick(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                if (this._iconForm != null && !this._iconForm.Disposing)
                {
                    this._iconForm.Dispose();
                    this._iconForm = null;
                }
                this._iconForm = new WorkedTimeForm(this.week);
                this._iconForm.Show();
                this._iconForm.BringToFront();
                this._iconForm.Activate();
                this._iconForm.Focus();
            } else
            {
                this.contextMenuStrip1.Show();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._timer.Dispose();
            this.ExitThread();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                EnsureAppDataFilesExist();
                var context = new TrayContext();
                Application.Run(context);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Time Handler Error", MessageBoxButtons.OK);
            }
        }
        public static string APP_DATA_PATH = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "calendarInfo");
        static void EnsureAppDataFilesExist()
        {
            if (!System.IO.Directory.Exists(APP_DATA_PATH))
            {
                System.IO.Directory.CreateDirectory(APP_DATA_PATH);
            }
        }
    }

    class WeekUpdatedEventArgs : EventArgs
    {
        public Week week;
    }
}

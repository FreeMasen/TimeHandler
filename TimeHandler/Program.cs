using System;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

namespace TimeHandler
{
    class TrayContext : ApplicationContext
    {
        private WorkedTimeForm _iconForm;

        private readonly NotifyIcon _icon;
        private AdjustDay AdjustDayForm;
        private Week week;
        public event EventHandler<EventArgs> TimerExpired;
        private readonly TimeCalculator _calc;
        private ContextMenuStrip contextMenuStrip1;
        private readonly ToolStripMenuItem exitToolStripMenuItem;
        private readonly ToolStripMenuItem refreshToolStripMenuItem;
        private readonly ToolStripMenuItem goHomeToolStripMenuItem;
        private readonly ToolStripMenuItem push15MenuItem;
        private readonly ToolStripMenuItem push30MenuItem;
        private readonly ToolStripMenuItem push60MenuItem;
        private readonly ToolStripMenuItem adjustPtoMenuItem;
        private readonly ToolStripMenuItem pomodoroMenuItem;
        private readonly ToolStripMenuItem reportMenuItem;
        private readonly ToolStripMenuItem configMenuItem;
        private readonly Pomodoro pomodoro;
        private bool PomodoroRunning = false;
        private Toast toast;

        private TimeSpan TimerTick = TimeSpan.FromMinutes(5.0);

        private System.Threading.Timer _timer;
        private DateTime lastNotified = new DateTime(0);
        private UserConfig Config;

        private TrayContext()
        {
            this.contextMenuStrip1 = new ContextMenuStrip();
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            this.refreshToolStripMenuItem = new ToolStripMenuItem();
            this.goHomeToolStripMenuItem = new ToolStripMenuItem();
            this.push15MenuItem = new ToolStripMenuItem();
            this.push30MenuItem = new ToolStripMenuItem();
            this.push60MenuItem = new ToolStripMenuItem();
            this.adjustPtoMenuItem = new ToolStripMenuItem();
            this.pomodoroMenuItem = new ToolStripMenuItem();
            this.reportMenuItem = new ToolStripMenuItem();
            this.configMenuItem = new ToolStripMenuItem();
            this.Config = Data.GetConfig();

            this.contextMenuStrip1.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            var sep = new ToolStripMenuItem();
            this.contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
                reportMenuItem,
                this.pomodoroMenuItem,
                this.push15MenuItem,
                this.push30MenuItem,
                this.push60MenuItem,
                this.adjustPtoMenuItem,
                this.goHomeToolStripMenuItem,
                this.refreshToolStripMenuItem,
                this.configMenuItem,
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
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);

            //
            // goHomeToolStripMenuItem
            //
            this.goHomeToolStripMenuItem.Name = "goHomeToolStripMenuItem";
            this.goHomeToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.goHomeToolStripMenuItem.Text = "Go Home";
            this.goHomeToolStripMenuItem.Click += new System.EventHandler((o, a) => {
                Data.AddEntry(new TimeEntry { What = TimeEntryEvent.EndOfDay, When = DateTime.UtcNow });
            });

            this._calc = new TimeCalculator();
            //
            // refreshToolStripMenuItem
            //
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new EventHandler(this._calc.OnRequestStartCalculation);

            this.push15MenuItem.Name = "push15MenuItem";
            this.push15MenuItem.Size = new System.Drawing.Size(210, 24);
            this.push15MenuItem.Text = "Push 15";
            this.push15MenuItem.Click += new System.EventHandler((o, a) =>
                    Data.AddBreak(15)
            );

            this.push30MenuItem.Name = "push30MenuItem";
            this.push30MenuItem.Size = new System.Drawing.Size(210, 24);
            this.push30MenuItem.Text = "Push 30";
            this.push30MenuItem.Click += new System.EventHandler((o, a) =>
                    Data.AddBreak(30)
            );

            this.push60MenuItem.Name = "push60MenuItem";
            this.push60MenuItem.Size = new System.Drawing.Size(210, 24);
            this.push60MenuItem.Text = "Push Hour";
            this.push60MenuItem.Click += new System.EventHandler((o, a) =>
                    Data.AddBreak(60)
            );

            this.adjustPtoMenuItem.Name = "adjustPtoMenuItem";
            this.adjustPtoMenuItem.Size = new System.Drawing.Size(210, 24);
            this.adjustPtoMenuItem.Text = "Adjust PTO";
            this.adjustPtoMenuItem.Click += new EventHandler((o, a) => {
                Logger.Log("Opening AdjustPto");
                var frm = new AdjustPto(week =>
                {
                    Logger.Log($"Closing AdjustPto ${week}");
                    if (week != null)
                    {
                        Data.BulkUpsertPto(week);
                    }
                })
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                frm.Show();
            });

            this.pomodoroMenuItem.Name = "pomodoroMenuItem";
            this.pomodoroMenuItem.Size = new System.Drawing.Size(210, 24);
            this.pomodoroMenuItem.Text = "Start Pomodoro";
            this.pomodoroMenuItem.Click += this.TogglePomodoro;

            this.reportMenuItem.Name = "reportMenuItem";
            this.reportMenuItem.Size = new System.Drawing.Size(210, 24);
            this.reportMenuItem.Text = "Generate Report";
            this.reportMenuItem.Click += this.GenerateReport;

            this.configMenuItem.Name = "configMenuItem";
            this.configMenuItem.Size = new System.Drawing.Size(210, 24);
            this.configMenuItem.Text = "Configuration";
            this.configMenuItem.Click += this.OpenConfig;

            this.contextMenuStrip1.ResumeLayout(false);
            this._icon = new NotifyIcon()
            {
                ContextMenuStrip = contextMenuStrip1,
                Icon = Properties.Resources.TrayIcon,
                Text = "Time Handler",
                Visible = true,
            };
            this._icon.MouseClick += this.OnClick;
            this._calc.CalculationComplete += this.OnWeekUpdated;
            this.TimerExpired += this._calc.OnRequestStartCalculation;
            this._timer = new System.Threading.Timer(this.TimerCB, null, 0, Timeout.Infinite);
            
            this.pomodoro = new Pomodoro();
            this.pomodoro.Switch += (s, m) => this.ShowMessage(m);
            Data.DataChanged += this._calc.OnRequestStartCalculation;
        }

        private void TimerCB(object state)
        {
            if(this.TimerExpired != null)
            {
                this.TimerExpired.Invoke(new { }, new EventArgs());
            }
            if (this._timer == null)
            {
                this._timer = new System.Threading.Timer(
                    TimerCB
                );
            }
            this._timer.Change(
                this.TimerTick, 
                Timeout.InfiniteTimeSpan
            );
            CheckToday();
        }

        private void CheckToday()
        {
            var now = DateTime.Now;
            var diff = this.week.Expectation() - this.week.Total();
            if (diff < 0 && (now - this.lastNotified).TotalHours > 1)
            {
                this.lastNotified = now;
                this.ShowMessage($"Head Home? ({((float)diff / 60.0).ToString("F2")}) hours");
            }
        }

        private void ShowMessage(string message)
        {
            Logger.Log($"ShowMessage: {message}");
            this._icon.BalloonTipText = message;
            this._icon.BalloonTipTitle = "Time Handler";
            this._icon.BalloonTipIcon = ToolTipIcon.Info;
            this._icon.ShowBalloonTip(5000);
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
                this._iconForm.OnAdjustmentRequested += this.AdjustmentRequested;
            } else
            {
                this.contextMenuStrip1.Show();
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._timer.Dispose();
            this._icon.Visible = false;
            this.ExitThread();
        }

        private void AdjustmentRequested(object sender, DayOfWeek day)
        {
            var d = Data.GetDateFor(day);
            this.AdjustDayForm = new AdjustDay(d)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            this.AdjustDayForm.Show();
        }

        private void TogglePomodoro(object s, EventArgs e)
        {
            string text;
            if (this.PomodoroRunning)
            {
                this.pomodoro.Stop();
                text = "Start Pomodoro";
                this.PomodoroRunning = false;
            }
            else
            {
                this.pomodoro.Start();
                text = "Stop Pomodoro";
                this.PomodoroRunning = true;
            }
            this.pomodoroMenuItem.Text = text;
        }

        private void GenerateReport(object s, EventArgs e)
        {
            Data.GenerateReport(1);
        }

        private void OpenConfig(object sender, EventArgs e)
        {
            var frm = new ConfigEditor(this.Config, (updated) =>
            {
                if (updated != null)
                {
                    this.Config = updated;
                    
                }
            });
            frm.Show();
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
                Logger.Log("Starting");
                var context = new TrayContext();
                Application.Run(context);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                MessageBox.Show(ex.Message, "Time Handler Error", MessageBoxButtons.OK);
            }
        }
        public static string APP_DATA_PATH = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "calendarInfo");
        public static string MY_DOCUMENTS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeHandler
{
    public partial class Toast : Form
    {
        private Action<TimeEntry[]> _complete;
        private DateTime BreakStart;
        private double Minutes = 0.0;
        
        public Toast(DateTime start, Action<TimeEntry[]>  complete)
        {
            this.BreakStart = start;
            this._complete = complete;
            InitializeComponent();
            var screen = Screen.PrimaryScreen;

            this.StartPosition = FormStartPosition.Manual;

            this.Left = screen.Bounds.Right - this.Width - 100;
            this.Top = screen.Bounds.Bottom - this.Height - 35;
            this.lblMinutes.Text = $"";
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            FireAnswered(true);
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            FireAnswered(false);
        }

        private void FireAnswered(bool value)
        {
            this.Hide();
            TimeEntry[] args = null;
            if (value)
            {
                args = new TimeEntry[] {
                    new TimeEntry
                    {
                        IsPto = false,
                        What = TimeEntryEvent.StartOfBreak,
                        When = this.BreakStart,
                    },
                    new TimeEntry
                    {
                        IsPto = false,
                        What = TimeEntryEvent.EndOfBreak,
                        When = this.BreakStart.Add(TimeSpan.FromMinutes(this.Minutes)),
                    },
                };
            }
            this._complete?.Invoke(args);
        }
        public void SetEnd(DateTime end)
        {
            this.Minutes = (end - this.BreakStart).TotalMinutes;
            this.lblMinutes.Text = $"You were away for\n{Math.Abs(this.Minutes):F0} Minutes.";
        }

        
    }
}

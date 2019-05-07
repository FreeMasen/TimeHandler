using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading.Channels;

namespace TimeHandler
{
    class WorkedTimeForm : Form
    {
        private System.ComponentModel.IContainer components;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label lblSunday;
        private Label lblSaturday;
        private Label lblFriday;
        private Label lblThursday;
        private Label lblWednesday;
        private Label lblTuesday;
        private Label lblMonday;
        private Label lblTotal;
        private Label label9;
        private Week week;
        

        public event EventHandler<WeekUpdatedEventArgs> RequestUpdate;
        static private int WORKWEEK_MINUTES = 40 * 60;
        private Timer _timer;
        public WorkedTimeForm(Week week)
        {
            this.week = week;
            this.InitializeComponent();
            this.updateLabels();
            this.StartPosition = FormStartPosition.Manual;
            this.Left = Cursor.Position.X - (this.Width / 2);
            this.Top = Cursor.Position.Y - (this.Height + 25);
            this.RequestUpdate += this.UpdateRequested;
            this.LostFocus += this.On_LostFocus;
            this.GotFocus += this.On_GotFocus;
            this._timer = new Timer();
            this._timer.Interval = 15 * 1000;
            this._timer.Tick += (sender, args) => this.On_LostFocus(sender, args);
            this._timer.Start();
            
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkedTimeForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblSunday = new System.Windows.Forms.Label();
            this.lblSaturday = new System.Windows.Forms.Label();
            this.lblFriday = new System.Windows.Forms.Label();
            this.lblThursday = new System.Windows.Forms.Label();
            this.lblWednesday = new System.Windows.Forms.Label();
            this.lblTuesday = new System.Windows.Forms.Label();
            this.lblMonday = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.GhostWhite;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "M";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.GhostWhite;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "T";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.GhostWhite;
            this.label3.Location = new System.Drawing.Point(12, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "W";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.GhostWhite;
            this.label4.Location = new System.Drawing.Point(12, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "R";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.GhostWhite;
            this.label5.Location = new System.Drawing.Point(12, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "F";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.GhostWhite;
            this.label6.Location = new System.Drawing.Point(12, 145);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "S";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.GhostWhite;
            this.label7.Location = new System.Drawing.Point(12, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 17);
            this.label7.TabIndex = 6;
            this.label7.Text = "U";
            // 
            // lblSunday
            // 
            this.lblSunday.AutoSize = true;
            this.lblSunday.ForeColor = System.Drawing.Color.GhostWhite;
            this.lblSunday.Location = new System.Drawing.Point(45, 172);
            this.lblSunday.Name = "lblSunday";
            this.lblSunday.Size = new System.Drawing.Size(0, 17);
            this.lblSunday.TabIndex = 13;
            // 
            // lblSaturday
            // 
            this.lblSaturday.AutoSize = true;
            this.lblSaturday.ForeColor = System.Drawing.Color.GhostWhite;
            this.lblSaturday.Location = new System.Drawing.Point(45, 145);
            this.lblSaturday.Name = "lblSaturday";
            this.lblSaturday.Size = new System.Drawing.Size(0, 17);
            this.lblSaturday.TabIndex = 12;
            // 
            // lblFriday
            // 
            this.lblFriday.AutoSize = true;
            this.lblFriday.ForeColor = System.Drawing.Color.GhostWhite;
            this.lblFriday.Location = new System.Drawing.Point(45, 118);
            this.lblFriday.Name = "lblFriday";
            this.lblFriday.Size = new System.Drawing.Size(0, 17);
            this.lblFriday.TabIndex = 11;
            // 
            // lblThursday
            // 
            this.lblThursday.AutoSize = true;
            this.lblThursday.ForeColor = System.Drawing.Color.GhostWhite;
            this.lblThursday.Location = new System.Drawing.Point(45, 91);
            this.lblThursday.Name = "lblThursday";
            this.lblThursday.Size = new System.Drawing.Size(0, 17);
            this.lblThursday.TabIndex = 10;
            // 
            // lblWednesday
            // 
            this.lblWednesday.AutoSize = true;
            this.lblWednesday.ForeColor = System.Drawing.Color.GhostWhite;
            this.lblWednesday.Location = new System.Drawing.Point(45, 64);
            this.lblWednesday.Name = "lblWednesday";
            this.lblWednesday.Size = new System.Drawing.Size(0, 17);
            this.lblWednesday.TabIndex = 9;
            // 
            // lblTuesday
            // 
            this.lblTuesday.AutoSize = true;
            this.lblTuesday.ForeColor = System.Drawing.Color.GhostWhite;
            this.lblTuesday.Location = new System.Drawing.Point(45, 37);
            this.lblTuesday.Name = "lblTuesday";
            this.lblTuesday.Size = new System.Drawing.Size(0, 17);
            this.lblTuesday.TabIndex = 8;
            // 
            // lblMonday
            // 
            this.lblMonday.AutoSize = true;
            this.lblMonday.ForeColor = System.Drawing.Color.GhostWhite;
            this.lblMonday.Location = new System.Drawing.Point(45, 10);
            this.lblMonday.Name = "lblMonday";
            this.lblMonday.Size = new System.Drawing.Size(0, 17);
            this.lblMonday.TabIndex = 7;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.GhostWhite;
            this.lblTotal.Location = new System.Drawing.Point(45, 199);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(0, 17);
            this.lblTotal.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.GhostWhite;
            this.label9.Location = new System.Drawing.Point(12, 199);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 17);
            this.label9.TabIndex = 14;
            this.label9.Text = "R";
            // 
            // WorkedTimeForm
            // 
            this.BackColor = System.Drawing.Color.DarkSlateGray;
            this.ClientSize = new System.Drawing.Size(148, 226);
            this.ControlBox = false;
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblSunday);
            this.Controls.Add(this.lblSaturday);
            this.Controls.Add(this.lblFriday);
            this.Controls.Add(this.lblThursday);
            this.Controls.Add(this.lblWednesday);
            this.Controls.Add(this.lblTuesday);
            this.Controls.Add(this.lblMonday);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WorkedTimeForm";
            this.ShowInTaskbar = false;
            
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void On_GotFocus(object sender, EventArgs args)
        {
            Console.WriteLine("Got Focus");
            this._timer.Stop();
        }


        private void On_LostFocus(object sender, EventArgs args)
        {
            Console.WriteLine("Lost Focus");
            this.Close();
            this.Dispose();
        }
        private void UpdateRequested(object sender, WeekUpdatedEventArgs args)
        {
            this.week = args.week;
            this.updateLabelsAsync();
        }

        public void Refresh(Week week)
        {
            if (this.week.Equals(week))
            {
                return;
            }
            this.RequestUpdate.Invoke(new { }, new WeekUpdatedEventArgs { week = week });
        }

        public delegate void InvokeDelegate();
        private void updateLabels()
        {
            try
            {
                this.lblMonday.Text = this.FormatEntry(this.week.Monday);
                this.lblTuesday.Text = this.FormatEntry(this.week.Tuesday);
                this.lblWednesday.Text = this.FormatEntry(this.week.Wednesday);
                this.lblThursday.Text = this.FormatEntry(this.week.Thursday);
                this.lblFriday.Text = this.FormatEntry(this.week.Friday);
                this.lblSaturday.Text = this.FormatEntry(this.week.Saturday);
                this.lblSunday.Text = this.FormatEntry(this.week.Sunday);
                this.lblTotal.Text = this.FormatEntry(
                    $"{this.FormatMinutes(WORKWEEK_MINUTES - this.week.Total())} ({this.RemainingToday()})"
                );
            } catch { }
        }

        private string RemainingToday()
        {
            return FormatMinutes(this.week.Expectation() - this.week.Total());
        }

        private void updateLabelsAsync()
        {
            if (this.Handle == null || this.Handle == IntPtr.Zero)
            {
                return;
            }
            try
            {
                this.lblMonday.BeginInvoke(new InvokeDelegate(()
                        => this.lblMonday.Text = this.FormatMinutes(this.week.Monday)));
                this.lblTuesday.BeginInvoke(new InvokeDelegate(()
                    => this.lblTuesday.Text = this.FormatMinutes(this.week.Tuesday)));
                this.lblWednesday.BeginInvoke(new InvokeDelegate(()
                    => this.lblWednesday.Text = this.FormatMinutes(this.week.Wednesday)));
                this.lblThursday.BeginInvoke(new InvokeDelegate(()
                    => this.lblThursday.Text = this.FormatMinutes(this.week.Thursday)));
                this.lblFriday.BeginInvoke(new InvokeDelegate(()
                    => this.lblFriday.Text = this.FormatMinutes(this.week.Friday)));
                this.lblSaturday.BeginInvoke(new InvokeDelegate(()
                    => this.lblSaturday.Text = this.FormatMinutes(this.week.Saturday)));
                this.lblSunday.BeginInvoke(new InvokeDelegate(()
                    => this.lblSunday.Text = this.FormatMinutes(this.week.Sunday)));
                this.lblTotal.BeginInvoke(new InvokeDelegate(()
                    => this.lblTotal.Text = this.FormatEntry(
                        $"{this.FormatMinutes(WORKWEEK_MINUTES - this.week.Total())} ({this.RemainingToday()})"
                    )));
            } catch { }
        }

        private string FormatEntry(string hours)
        {
            return $"{hours}";
        }

        private string FormatEntry(int hours)
        {
            return FormatEntry(FormatMinutes(hours));
        }

        private string FormatMinutes(int minutes)
        {
            var hours = minutes / 60;
            var remainder = minutes % 60;
            return $"{hours.ToString()}:{remainder.ToString("D2")}";
        }

        
    }
}

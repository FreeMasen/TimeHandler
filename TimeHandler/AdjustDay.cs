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
    public partial class AdjustDay : Form
    {
        private List<TimeEntry> Entries;
        private TimeEntryEvent[] EventOptions = new List<TimeEntryEvent>
            {
                    TimeEntryEvent.StartOfDay,
                    TimeEntryEvent.StartOfBreak,
                    TimeEntryEvent.EndOfBreak,
                    TimeEntryEvent.EndOfDay,
            }.ToArray();
        public AdjustDay(DateTime selectedDate)
        {
            InitializeComponent();
            this.dtpSelectedDate.Value = selectedDate;
            this.UpdateEntries();
        }

        public void Setup()
        {
            var calculatedHeight = (this.Entries.Count() * 35) + this.dtpSelectedDate.Bottom + 15 + 35;
            this.SuspendLayout();
            var ids = new List<int>();
            foreach (Control control in this.Controls.OfType<GroupBox>().ToArray())
            {
                    control.Dispose();
            }
            for (var i = 0; i < this.Entries.Count(); i++)
            {
                this.Controls.Add(
                    this.NewEntryGroupBox(i, this.Entries[i])
                );
            }
            this.Height = (this.Entries.Count() * 35) + 25 + 100;
            this.ClientSize = new Size(315, calculatedHeight);
            this.btnAdd.Location = new Point(this.btnAdd.Location.X, calculatedHeight - 30);
            this.btnClose.Location = new Point(this.btnClose.Location.X, calculatedHeight - 30);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private GroupBox NewEntryGroupBox(int idx, TimeEntry entry)
        {
            var box = new GroupBox
            {
                Height = 35,
                Width = 245,
                Tag = entry.Id,
            };
            var name = new ComboBox()
            {
                Location = new Point(5, 10),
                Width = 100
            };
            foreach (var e in EventOptions)
            {
                name.Items.Add((object)e);
            }
            name.SelectedItem = entry.What;
            var time = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = entry.When,
                Location = new Point(10 + name.Width, 10),
                Tag = entry.Id,
                Width = 100,
            };
            name.SelectedValueChanged += (o, e) => this.TimeValueChange(entry, entry.What, time.Value);
            time.ValueChanged += (o, e) => this.TimeValueChange(entry, entry.What, time.Value);
            //Data.UpdateEntry(entry.Id, entry.What, new DateTime(time.Value.Ticks, DateTimeKind.Local));
            var remove = new Button()
            {
                Width = 20,
                Height = 20,
                Text = "X",
                Location = new Point(10 + name.Width + time.Width + 5, 10),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            remove.Click += (o, e) => {
                Data.RemoveEntry(entry.Id);
                this.UpdateEntries();
            };
            box.Controls.Add(name);
            box.Controls.Add(time);
            box.Controls.Add(remove);
            box.Location = new Point(35, (idx * 35) + this.dtpSelectedDate.Bottom + 15);
            return box;
        }

        private void TimeValueChange(TimeEntry original, TimeEntryEvent updated_event, DateTime updated_time)
        {
            Data.UpdateEntry(original.Id, updated_event, new DateTime(updated_time.Ticks, DateTimeKind.Local));
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            TimeEntryEvent What;
            DateTime When;
            if (this.Entries.Count() > 0)
            {
                var last = this.Entries.Last();
                When = last.When.AddMinutes(1);
                What = last.What.Next();
            }
            else
            {

                When = DateTime.Now;
                What = TimeEntryEvent.StartOfDay;
            }
            Data.AddEntry(new TimeEntry
            {
                What = What,
                When = When,
            });

            this.UpdateEntries();
        }

        private void UpdateEntries()
        {
            this.Entries = Data.GetDayEntries(this.dtpSelectedDate.Value);
            Setup();
        }

        private void dtpSelectedDate_ValueChanged(object sender, EventArgs e)
        {
            this.UpdateEntries();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

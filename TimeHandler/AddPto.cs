using System;
using System.Linq;
using System.Windows.Forms;

namespace TimeHandler
{
    public partial class AdjustPto : Form
    {
        private Action<PtoWeekEntrys> _complete;
        private PtoWeekEntrys Entries;
        public AdjustPto(Action<PtoWeekEntrys> complete)
        {
            this._complete = complete;
            InitializeComponent();
            this.dtpSelectedDate.Value = Data.Monday(DateTime.Now).Date;
            SetCurrentPto();
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.UpdateEntries();
            this._complete(this.Entries);
            this.Close();
        }

        private void UpdateEntries()
        {
            var week = CaptureTotals();
            foreach (var (raw, entry) in week.Zip(this.Entries, Tuple.Create))
            {
                UpdateEntry(raw, entry);
            }
        }

        private void UpdateEntry(int raw, PtoWeekEntry entry)
        {
            var diff = entry.CalculateSimpleDiff();
            if (entry.Start == null && raw > 0)
            {
                var when = Data.GetDateFor(entry.Dow, this.dtpSelectedDate.Value);
                entry.Start = new TimeEntry()
                {
                    Id = -1,
                    What = TimeEntryEvent.StartOfDay,
                    When = when,
                    IsPto = true,
                };
            }
            if (entry.End == null && raw > 0)
            {
                var when = Data.GetDateFor(entry.Dow, this.dtpSelectedDate.Value).AddMinutes(diff);
                entry.End = new TimeEntry()
                {
                    Id = -1,
                    What = TimeEntryEvent.EndOfDay,
                    When = when,
                    IsPto = true,
                };
            }
            if (raw != diff)
            {
                entry.SetNewEnd(raw);
            }
        }

        private Week CaptureTotals()
        {
            var ret = new Week
            {
                Monday = ((int)udMHours.Value * 60) + (int)udMMin.Value,
                Tuesday = ((int)udTHours.Value * 60) + (int)udTMin.Value,
                Wednesday = ((int)udWHours.Value * 60) + (int)udWMin.Value,
                Thursday = ((int)udRHours.Value * 60) + (int)udRMin.Value,
                Friday = ((int)udFHours.Value * 60) + (int)udFMin.Value,
                Saturday = ((int)udSHours.Value * 60) + (int)udSMin.Value,
                Sunday = ((int)udUHours.Value * 60) + (int)udUMin.Value
            };
            return ret;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            this._complete(null);
            this.Close();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            UpdateDate(true);
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            UpdateDate(false);
        }

        private void UpdateDate(bool forward)
        {
            if (forward)
            {
                this.Forward();
            }
            else
            {
                this.Backward();
            }
        }
        private void Forward()
        {
            var newDate = this.dtpSelectedDate.Value.AddDays(7);
            if (newDate > Data.Monday(DateTime.Now))
            {
                return;
            }
            this.dtpSelectedDate.Value = newDate;
            SetCurrentPto();
        }
        private void Backward()
        {
            this.dtpSelectedDate.Value = this.dtpSelectedDate.Value.AddDays(-7);
            SetCurrentPto();
        }
        private void SetCurrentPto()
        {
            var calc = new TimeCalculator();
            this.Entries = calc.GetWeekPto(this.dtpSelectedDate.Value);
            UpdateControls();
        }

        private void UpdateControls()
        {
            var mon = this.Entries.Monday.CalculateDiff();
            udMHours.Value = mon.Hours;
            udMMin.Value = mon.Minutes;
            var tue = this.Entries.Tuesday.CalculateDiff();
            udTHours.Value = tue.Hours;
            udTMin.Value = tue.Minutes;
            var wed = this.Entries.Wednesday.CalculateDiff();
            udWHours.Value = wed.Hours;
            udWMin.Value = wed.Minutes;
            var thurs = this.Entries.Thursday.CalculateDiff();
            udRHours.Value = thurs.Hours;
            udRMin.Value = thurs.Minutes;
            var fri = this.Entries.Friday.CalculateDiff();
            udFHours.Value = fri.Hours;
            udFMin.Value = fri.Minutes;
            var sat = this.Entries.Saturday.CalculateDiff();
            udSHours.Value = sat.Hours;
            udSMin.Value = sat.Minutes;
            var sun = this.Entries.Sunday.CalculateDiff();
            udUHours.Value = sun.Hours;
            udUMin.Value = sun.Minutes;
            this.Refresh();
        }
        
    }
}

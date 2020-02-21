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
    public partial class ConfigEditor : Form
    {
        private string ReportStoragePath;
        private string JiraUserName;
        private string JiraToken;
        private int PomWork;
        private int PomLongBrk;
        private int PomShortBrk;
        private Action<UserConfig> _cb;

        public ConfigEditor(UserConfig config, Action<UserConfig> cb)
        {
            this._cb = cb;
            this.ReportStoragePath = config.ReportConfig.StoragePath;
            this.JiraUserName = config.JiraConfig.JiraUserName;
            this.JiraToken = config.JiraConfig.JiraAuthToken;
            this.PomWork = config.PomorodoConfig.WorkLength;
            this.PomShortBrk = config.PomorodoConfig.ShortBreakLength;
            this.PomLongBrk = config.PomorodoConfig.LongBreakLength;
            InitializeComponent();
            FillForm();
            Refresh();
        }

        private void FillForm()
        {
            FillPath();
            FillJiraValues();
            FillPomValues();
        }

        private void FillPath()
        {
            if (this.ReportStoragePath.Length > 50)
            {
                lblReportPath.Text = this.ReportStoragePath.Substring(this.ReportStoragePath.Length - 50);
            }
            else
            {
                lblReportPath.Text = this.ReportStoragePath;
            }
        }

        private void FillJiraValues()
        {
            this.txtJiraUser.Text = this.JiraUserName ?? "";
            this.txtJiraToken.Text = this.JiraToken ?? "";
        }

        private void FillPomValues()
        {
            this.numPomWork.Value = this.PomWork;
            this.numPomSBrk.Value = this.PomShortBrk;
            this.numPomLBrk.Value = this.PomLongBrk;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this._cb != null)
            {
                this._cb(
                    new UserConfig(
                        this.ReportStoragePath,
                        this.JiraUserName,
                        this.JiraToken,
                        this.PomWork,
                        this.PomShortBrk,
                        this.PomLongBrk
                    )
                );
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this._cb != null)
            {
                this._cb(null);
            }
            this.Close();
        }

        private void btnBrowseReport_Click(object sender, EventArgs e)
        {
            this.reportFolderSelection.Reset();
            this.reportFolderSelection.RootFolder = Environment.SpecialFolder.UserProfile;
            this.reportFolderSelection.SelectedPath = this.ReportStoragePath;
            if (this.reportFolderSelection.ShowDialog() == DialogResult.OK)
            {
                this.ReportStoragePath = this.reportFolderSelection.SelectedPath;
                this.FillForm();
            }
        }

        private void numPomLBrk_ValueChanged(object sender, EventArgs e)
        {
            this.PomLongBrk = (int)this.numPomLBrk.Value;
        }

        private void numPomSBrk_ValueChanged(object sender, EventArgs e)
        {
            this.PomShortBrk = (int)this.numPomSBrk.Value;
        }

        private void numPomWork_ValueChanged(object sender, EventArgs e)
        {
            this.PomWork = (int)this.numPomWork.Value;
        }

        private void txtJiraToken_TextChanged(object sender, EventArgs e)
        {
            this.JiraToken = this.txtJiraToken.Text;
        }

        private void txtJiraUser_TextChanged(object sender, EventArgs e)
        {
            this.JiraUserName = this.txtJiraUser.Text;
        }
    }
}

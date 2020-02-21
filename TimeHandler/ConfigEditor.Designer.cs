namespace TimeHandler
{
    partial class ConfigEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.reportFolderSelection = new System.Windows.Forms.FolderBrowserDialog();
            this.lblReportPath = new System.Windows.Forms.Label();
            this.btnBrowseReport = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblJiraToken = new System.Windows.Forms.Label();
            this.lblJiraUser = new System.Windows.Forms.Label();
            this.txtJiraToken = new System.Windows.Forms.TextBox();
            this.txtJiraUser = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numPomWork = new System.Windows.Forms.NumericUpDown();
            this.numPomSBrk = new System.Windows.Forms.NumericUpDown();
            this.numPomLBrk = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPomWork)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPomSBrk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPomLBrk)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(448, 293);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(139, 46);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(12, 293);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(139, 46);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBrowseReport);
            this.groupBox1.Controls.Add(this.lblReportPath);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(575, 59);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Report Configuration";
            // 
            // reportFolderSelection
            // 
            this.reportFolderSelection.RootFolder = System.Environment.SpecialFolder.Recent;
            // 
            // lblReportPath
            // 
            this.lblReportPath.AutoSize = true;
            this.lblReportPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReportPath.Location = new System.Drawing.Point(6, 16);
            this.lblReportPath.Name = "lblReportPath";
            this.lblReportPath.Size = new System.Drawing.Size(156, 24);
            this.lblReportPath.TabIndex = 3;
            this.lblReportPath.Text = "No Path Selected";
            // 
            // btnBrowseReport
            // 
            this.btnBrowseReport.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnBrowseReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseReport.ForeColor = System.Drawing.Color.White;
            this.btnBrowseReport.Location = new System.Drawing.Point(488, 10);
            this.btnBrowseReport.Name = "btnBrowseReport";
            this.btnBrowseReport.Size = new System.Drawing.Size(81, 37);
            this.btnBrowseReport.TabIndex = 1;
            this.btnBrowseReport.Text = "browse";
            this.btnBrowseReport.UseVisualStyleBackColor = false;
            this.btnBrowseReport.Click += new System.EventHandler(this.btnBrowseReport_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtJiraUser);
            this.groupBox2.Controls.Add(this.txtJiraToken);
            this.groupBox2.Controls.Add(this.lblJiraToken);
            this.groupBox2.Controls.Add(this.lblJiraUser);
            this.groupBox2.Location = new System.Drawing.Point(12, 77);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(575, 100);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Jira Configuration";
            // 
            // lblJiraToken
            // 
            this.lblJiraToken.AutoSize = true;
            this.lblJiraToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblJiraToken.Location = new System.Drawing.Point(52, 51);
            this.lblJiraToken.Name = "lblJiraToken";
            this.lblJiraToken.Size = new System.Drawing.Size(64, 24);
            this.lblJiraToken.TabIndex = 4;
            this.lblJiraToken.Text = "Token";
            // 
            // lblJiraUser
            // 
            this.lblJiraUser.AutoSize = true;
            this.lblJiraUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblJiraUser.Location = new System.Drawing.Point(19, 16);
            this.lblJiraUser.Name = "lblJiraUser";
            this.lblJiraUser.Size = new System.Drawing.Size(97, 24);
            this.lblJiraUser.TabIndex = 5;
            this.lblJiraUser.Text = "Username";
            // 
            // txtJiraToken
            // 
            this.txtJiraToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtJiraToken.Location = new System.Drawing.Point(122, 48);
            this.txtJiraToken.Name = "txtJiraToken";
            this.txtJiraToken.Size = new System.Drawing.Size(175, 29);
            this.txtJiraToken.TabIndex = 3;
            this.txtJiraToken.UseSystemPasswordChar = true;
            this.txtJiraToken.TextChanged += new System.EventHandler(this.txtJiraToken_TextChanged);
            // 
            // txtJiraUser
            // 
            this.txtJiraUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtJiraUser.Location = new System.Drawing.Point(122, 13);
            this.txtJiraUser.Name = "txtJiraUser";
            this.txtJiraUser.Size = new System.Drawing.Size(175, 29);
            this.txtJiraUser.TabIndex = 2;
            this.txtJiraUser.TextChanged += new System.EventHandler(this.txtJiraUser_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numPomLBrk);
            this.groupBox3.Controls.Add(this.numPomSBrk);
            this.groupBox3.Controls.Add(this.numPomWork);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(12, 183);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(575, 100);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Pomodoro Configuration";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Work Time";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Short Break";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(191, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 24);
            this.label3.TabIndex = 2;
            this.label3.Text = "Long Break";
            // 
            // numPomWork
            // 
            this.numPomWork.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPomWork.Location = new System.Drawing.Point(122, 16);
            this.numPomWork.Name = "numPomWork";
            this.numPomWork.Size = new System.Drawing.Size(56, 29);
            this.numPomWork.TabIndex = 4;
            this.numPomWork.ValueChanged += new System.EventHandler(this.numPomWork_ValueChanged);
            // 
            // numPomSBrk
            // 
            this.numPomSBrk.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPomSBrk.Location = new System.Drawing.Point(122, 51);
            this.numPomSBrk.Name = "numPomSBrk";
            this.numPomSBrk.Size = new System.Drawing.Size(56, 29);
            this.numPomSBrk.TabIndex = 6;
            this.numPomSBrk.ValueChanged += new System.EventHandler(this.numPomSBrk_ValueChanged);
            // 
            // numPomLBrk
            // 
            this.numPomLBrk.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPomLBrk.Location = new System.Drawing.Point(303, 14);
            this.numPomLBrk.Name = "numPomLBrk";
            this.numPomLBrk.Size = new System.Drawing.Size(56, 29);
            this.numPomLBrk.TabIndex = 5;
            this.numPomLBrk.ValueChanged += new System.EventHandler(this.numPomLBrk_ValueChanged);
            // 
            // ConfigEditor
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(599, 351);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ConfigEditor";
            this.Text = "ConfigEditor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPomWork)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPomSBrk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPomLBrk)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnBrowseReport;
        private System.Windows.Forms.Label lblReportPath;
        private System.Windows.Forms.FolderBrowserDialog reportFolderSelection;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtJiraUser;
        private System.Windows.Forms.TextBox txtJiraToken;
        private System.Windows.Forms.Label lblJiraToken;
        private System.Windows.Forms.Label lblJiraUser;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numPomLBrk;
        private System.Windows.Forms.NumericUpDown numPomSBrk;
        private System.Windows.Forms.NumericUpDown numPomWork;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
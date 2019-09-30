namespace TimeHandler
{
    partial class Toast
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
            this.lblMinutes = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMinutes
            // 
            this.lblMinutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMinutes.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblMinutes.Location = new System.Drawing.Point(1, 3);
            this.lblMinutes.Name = "lblMinutes";
            this.lblMinutes.Size = new System.Drawing.Size(248, 50);
            this.lblMinutes.TabIndex = 0;
            this.lblMinutes.Text = "You were away for 5 minutes.";
            this.lblMinutes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(-3, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(252, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Add Break?";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnYes
            // 
            this.btnYes.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnYes.FlatAppearance.BorderSize = 0;
            this.btnYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnYes.Location = new System.Drawing.Point(127, 83);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(109, 32);
            this.btnYes.TabIndex = 3;
            this.btnYes.Text = "Yes";
            this.btnYes.UseVisualStyleBackColor = false;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnNo.FlatAppearance.BorderSize = 0;
            this.btnNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNo.Location = new System.Drawing.Point(12, 83);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(109, 32);
            this.btnNo.TabIndex = 4;
            this.btnNo.Text = "No";
            this.btnNo.UseVisualStyleBackColor = false;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // Toast
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(248, 127);
            this.ControlBox = false;
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblMinutes);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Toast";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Break?";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblMinutes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
    }
}
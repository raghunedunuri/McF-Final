namespace McKeany
{
    partial class PhysicalComm
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
            this.grpCategories = new System.Windows.Forms.GroupBox();
            this.treeGroups = new System.Windows.Forms.TreeView();
            this.DateFilters = new System.Windows.Forms.GroupBox();
            this.dtPickerEndtime = new System.Windows.Forms.DateTimePicker();
            this.dtPickerStartTime = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbRange = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.chkCommodity = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbRollUp = new System.Windows.Forms.ComboBox();
            this.cmdField = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ChkMatrixFormat = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ChkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.grpCategories.SuspendLayout();
            this.DateFilters.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCategories
            // 
            this.grpCategories.Controls.Add(this.treeGroups);
            this.grpCategories.Location = new System.Drawing.Point(20, 35);
            this.grpCategories.Name = "grpCategories";
            this.grpCategories.Size = new System.Drawing.Size(642, 245);
            this.grpCategories.TabIndex = 23;
            this.grpCategories.TabStop = false;
            this.grpCategories.Text = "Commodities";
            // 
            // treeGroups
            // 
            this.treeGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGroups.Location = new System.Drawing.Point(3, 18);
            this.treeGroups.Name = "treeGroups";
            this.treeGroups.Size = new System.Drawing.Size(636, 224);
            this.treeGroups.TabIndex = 0;
            // 
            // DateFilters
            // 
            this.DateFilters.Controls.Add(this.dtPickerEndtime);
            this.DateFilters.Controls.Add(this.dtPickerStartTime);
            this.DateFilters.Controls.Add(this.label4);
            this.DateFilters.Controls.Add(this.label5);
            this.DateFilters.Controls.Add(this.cmbRange);
            this.DateFilters.Controls.Add(this.label7);
            this.DateFilters.Controls.Add(this.label6);
            this.DateFilters.Location = new System.Drawing.Point(29, 292);
            this.DateFilters.Name = "DateFilters";
            this.DateFilters.Size = new System.Drawing.Size(629, 148);
            this.DateFilters.TabIndex = 25;
            this.DateFilters.TabStop = false;
            this.DateFilters.Text = "Date Filters";
            // 
            // dtPickerEndtime
            // 
            this.dtPickerEndtime.Location = new System.Drawing.Point(423, 38);
            this.dtPickerEndtime.Name = "dtPickerEndtime";
            this.dtPickerEndtime.Size = new System.Drawing.Size(200, 22);
            this.dtPickerEndtime.TabIndex = 20;
            // 
            // dtPickerStartTime
            // 
            this.dtPickerStartTime.Location = new System.Drawing.Point(167, 38);
            this.dtPickerStartTime.Name = "dtPickerStartTime";
            this.dtPickerStartTime.Size = new System.Drawing.Size(177, 22);
            this.dtPickerStartTime.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Date Range:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(91, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "Start Date";
            // 
            // cmbRange
            // 
            this.cmbRange.FormattingEnabled = true;
            this.cmbRange.Location = new System.Drawing.Point(154, 86);
            this.cmbRange.Name = "cmbRange";
            this.cmbRange.Size = new System.Drawing.Size(464, 24);
            this.cmbRange.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 92);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 17);
            this.label7.TabIndex = 15;
            this.label7.Text = "Or Select Range";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(353, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 17);
            this.label6.TabIndex = 14;
            this.label6.Text = "End Date";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(465, 670);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 39);
            this.btnCancel.TabIndex = 27;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(119, 671);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(73, 39);
            this.btnRun.TabIndex = 26;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // chkCommodity
            // 
            this.chkCommodity.AutoSize = true;
            this.chkCommodity.Enabled = false;
            this.chkCommodity.Location = new System.Drawing.Point(501, 12);
            this.chkCommodity.Name = "chkCommodity";
            this.chkCommodity.Size = new System.Drawing.Size(88, 21);
            this.chkCommodity.TabIndex = 1;
            this.chkCommodity.Text = "Select All";
            this.chkCommodity.UseVisualStyleBackColor = true;
            this.chkCommodity.CheckedChanged += new System.EventHandler(this.chkCommodity_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbRollUp);
            this.groupBox1.Controls.Add(this.cmdField);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(22, 454);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(639, 141);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Group by Period";
            // 
            // cmbRollUp
            // 
            this.cmbRollUp.FormattingEnabled = true;
            this.cmbRollUp.Location = new System.Drawing.Point(161, 82);
            this.cmbRollUp.Name = "cmbRollUp";
            this.cmbRollUp.Size = new System.Drawing.Size(215, 24);
            this.cmbRollUp.TabIndex = 2;
            this.cmbRollUp.SelectedIndexChanged += new System.EventHandler(this.cmbRollUp_SelectedIndexChanged);
            // 
            // cmdField
            // 
            this.cmdField.FormattingEnabled = true;
            this.cmdField.Location = new System.Drawing.Point(161, 34);
            this.cmdField.Name = "cmdField";
            this.cmdField.Size = new System.Drawing.Size(215, 24);
            this.cmdField.TabIndex = 4;
            this.cmdField.SelectedIndexChanged += new System.EventHandler(this.cmdField_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Period Frequency";
            // 
            // ChkMatrixFormat
            // 
            this.ChkMatrixFormat.AutoSize = true;
            this.ChkMatrixFormat.Location = new System.Drawing.Point(35, 25);
            this.ChkMatrixFormat.Name = "ChkMatrixFormat";
            this.ChkMatrixFormat.Size = new System.Drawing.Size(168, 21);
            this.ChkMatrixFormat.TabIndex = 1;
            this.ChkMatrixFormat.Text = "Seasonal Comparison";
            this.ChkMatrixFormat.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.ChkMatrixFormat);
            this.groupBox4.Location = new System.Drawing.Point(25, 607);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(632, 55);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Display Options";
            // 
            // ChkAutoUpdate
            // 
            this.ChkAutoUpdate.AutoSize = true;
            this.ChkAutoUpdate.Location = new System.Drawing.Point(162, 8);
            this.ChkAutoUpdate.Name = "ChkAutoUpdate";
            this.ChkAutoUpdate.Size = new System.Drawing.Size(143, 21);
            this.ChkAutoUpdate.TabIndex = 32;
            this.ChkAutoUpdate.Text = "Auto Update Data";
            this.ChkAutoUpdate.UseVisualStyleBackColor = true;
            this.ChkAutoUpdate.CheckedChanged += new System.EventHandler(this.ChkAutoUpdate_CheckedChanged);
            // 
            // PhysicalComm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 715);
            this.Controls.Add(this.ChkAutoUpdate);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkCommodity);
            this.Controls.Add(this.grpCategories);
            this.Controls.Add(this.DateFilters);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRun);
            this.Name = "PhysicalComm";
            this.Text = "Physical Commodities";
            this.grpCategories.ResumeLayout(false);
            this.DateFilters.ResumeLayout(false);
            this.DateFilters.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpCategories;
        private System.Windows.Forms.TreeView treeGroups;
        private System.Windows.Forms.GroupBox DateFilters;
        private System.Windows.Forms.DateTimePicker dtPickerEndtime;
        private System.Windows.Forms.DateTimePicker dtPickerStartTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbRange;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.CheckBox chkCommodity;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmdField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRollUp;
        private System.Windows.Forms.CheckBox ChkMatrixFormat;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox ChkAutoUpdate;
    }
}
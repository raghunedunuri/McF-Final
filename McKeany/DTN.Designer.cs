namespace McKeany
{
    partial class DTN
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
            this.grpFileds = new System.Windows.Forms.GroupBox();
            this.treeFields = new System.Windows.Forms.TreeView();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbRange = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.DateFilters = new System.Windows.Forms.GroupBox();
            this.dtPickerEndtime = new System.Windows.Forms.DateTimePicker();
            this.dtPickerStartTime = new System.Windows.Forms.DateTimePicker();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCategories = new System.Windows.Forms.CheckBox();
            this.chkFeilds = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbFiscal = new System.Windows.Forms.ComboBox();
            this.cmbRollUp = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdField = new System.Windows.Forms.ComboBox();
            this.ChkMatrixFormat = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ChkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.grpCategories.SuspendLayout();
            this.grpFileds.SuspendLayout();
            this.DateFilters.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCategories
            // 
            this.grpCategories.Controls.Add(this.treeGroups);
            this.grpCategories.Location = new System.Drawing.Point(34, 33);
            this.grpCategories.Name = "grpCategories";
            this.grpCategories.Size = new System.Drawing.Size(662, 173);
            this.grpCategories.TabIndex = 8;
            this.grpCategories.TabStop = false;
            this.grpCategories.Text = "Categories";
            // 
            // treeGroups
            // 
            this.treeGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGroups.Location = new System.Drawing.Point(3, 18);
            this.treeGroups.Name = "treeGroups";
            this.treeGroups.Size = new System.Drawing.Size(656, 152);
            this.treeGroups.TabIndex = 0;
            // 
            // grpFileds
            // 
            this.grpFileds.Controls.Add(this.treeFields);
            this.grpFileds.Location = new System.Drawing.Point(34, 234);
            this.grpFileds.Name = "grpFileds";
            this.grpFileds.Size = new System.Drawing.Size(666, 168);
            this.grpFileds.TabIndex = 9;
            this.grpFileds.TabStop = false;
            this.grpFileds.Text = "Fields";
            // 
            // treeFields
            // 
            this.treeFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeFields.Location = new System.Drawing.Point(3, 18);
            this.treeFields.Name = "treeFields";
            this.treeFields.Size = new System.Drawing.Size(660, 147);
            this.treeFields.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(353, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 17);
            this.label6.TabIndex = 14;
            this.label6.Text = "End Date";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 17);
            this.label7.TabIndex = 15;
            this.label7.Text = "Or Select Range";
            // 
            // cmbRange
            // 
            this.cmbRange.FormattingEnabled = true;
            this.cmbRange.Location = new System.Drawing.Point(154, 76);
            this.cmbRange.Name = "cmbRange";
            this.cmbRange.Size = new System.Drawing.Size(464, 24);
            this.cmbRange.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(91, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "Start Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Date Range:";
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
            this.DateFilters.Location = new System.Drawing.Point(34, 413);
            this.DateFilters.Name = "DateFilters";
            this.DateFilters.Size = new System.Drawing.Size(662, 118);
            this.DateFilters.TabIndex = 19;
            this.DateFilters.TabStop = false;
            this.DateFilters.Text = "Date Filters";
            // 
            // dtPickerEndtime
            // 
            this.dtPickerEndtime.Location = new System.Drawing.Point(423, 28);
            this.dtPickerEndtime.Name = "dtPickerEndtime";
            this.dtPickerEndtime.Size = new System.Drawing.Size(200, 22);
            this.dtPickerEndtime.TabIndex = 20;
            // 
            // dtPickerStartTime
            // 
            this.dtPickerStartTime.Location = new System.Drawing.Point(167, 28);
            this.dtPickerStartTime.Name = "dtPickerStartTime";
            this.dtPickerStartTime.Size = new System.Drawing.Size(177, 22);
            this.dtPickerStartTime.TabIndex = 19;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(161, 743);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(99, 33);
            this.btnRun.TabIndex = 21;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(507, 740);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(99, 33);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkCategories
            // 
            this.chkCategories.AutoSize = true;
            this.chkCategories.Enabled = false;
            this.chkCategories.Location = new System.Drawing.Point(522, 12);
            this.chkCategories.Name = "chkCategories";
            this.chkCategories.Size = new System.Drawing.Size(88, 21);
            this.chkCategories.TabIndex = 23;
            this.chkCategories.Text = "Select All";
            this.chkCategories.UseVisualStyleBackColor = true;
            this.chkCategories.CheckedChanged += new System.EventHandler(this.chkCategories_CheckedChanged);
            // 
            // chkFeilds
            // 
            this.chkFeilds.AutoSize = true;
            this.chkFeilds.Enabled = false;
            this.chkFeilds.Location = new System.Drawing.Point(522, 212);
            this.chkFeilds.Name = "chkFeilds";
            this.chkFeilds.Size = new System.Drawing.Size(88, 21);
            this.chkFeilds.TabIndex = 24;
            this.chkFeilds.Text = "Select All";
            this.chkFeilds.UseVisualStyleBackColor = true;
            this.chkFeilds.CheckedChanged += new System.EventHandler(this.chkFeilds_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbFiscal);
            this.groupBox1.Controls.Add(this.cmbRollUp);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmdField);
            this.groupBox1.Location = new System.Drawing.Point(34, 539);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(661, 138);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Group by Period";
            // 
            // cmbFiscal
            // 
            this.cmbFiscal.FormattingEnabled = true;
            this.cmbFiscal.Location = new System.Drawing.Point(450, 33);
            this.cmbFiscal.Name = "cmbFiscal";
            this.cmbFiscal.Size = new System.Drawing.Size(121, 24);
            this.cmbFiscal.TabIndex = 6;
            // 
            // cmbRollUp
            // 
            this.cmbRollUp.FormattingEnabled = true;
            this.cmbRollUp.Location = new System.Drawing.Point(167, 79);
            this.cmbRollUp.Name = "cmbRollUp";
            this.cmbRollUp.Size = new System.Drawing.Size(223, 24);
            this.cmbRollUp.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Period Frequency";
            // 
            // cmdField
            // 
            this.cmdField.FormattingEnabled = true;
            this.cmdField.Location = new System.Drawing.Point(167, 34);
            this.cmdField.Name = "cmdField";
            this.cmdField.Size = new System.Drawing.Size(223, 24);
            this.cmdField.TabIndex = 4;
            this.cmdField.SelectedIndexChanged += new System.EventHandler(this.cmdField_SelectedIndexChanged);
            // 
            // ChkMatrixFormat
            // 
            this.ChkMatrixFormat.AutoSize = true;
            this.ChkMatrixFormat.Location = new System.Drawing.Point(20, 25);
            this.ChkMatrixFormat.Name = "ChkMatrixFormat";
            this.ChkMatrixFormat.Size = new System.Drawing.Size(168, 21);
            this.ChkMatrixFormat.TabIndex = 1;
            this.ChkMatrixFormat.Text = "Seasonal Comparison";
            this.ChkMatrixFormat.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ChkMatrixFormat);
            this.groupBox3.Location = new System.Drawing.Point(36, 683);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(663, 52);
            this.groupBox3.TabIndex = 32;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Display Options";
            // 
            // ChkAutoUpdate
            // 
            this.ChkAutoUpdate.AutoSize = true;
            this.ChkAutoUpdate.Location = new System.Drawing.Point(149, 12);
            this.ChkAutoUpdate.Name = "ChkAutoUpdate";
            this.ChkAutoUpdate.Size = new System.Drawing.Size(143, 21);
            this.ChkAutoUpdate.TabIndex = 33;
            this.ChkAutoUpdate.Text = "Auto Update Data";
            this.ChkAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // DTN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 787);
            this.Controls.Add(this.ChkAutoUpdate);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkFeilds);
            this.Controls.Add(this.chkCategories);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.DateFilters);
            this.Controls.Add(this.grpFileds);
            this.Controls.Add(this.grpCategories);
            this.Name = "DTN";
            this.Text = "Monthly Contracts";
            this.grpCategories.ResumeLayout(false);
            this.grpFileds.ResumeLayout(false);
            this.DateFilters.ResumeLayout(false);
            this.DateFilters.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox grpCategories;
        private System.Windows.Forms.GroupBox grpFileds;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbRange;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox DateFilters;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TreeView treeGroups;
        private System.Windows.Forms.TreeView treeFields;
        private System.Windows.Forms.DateTimePicker dtPickerEndtime;
        private System.Windows.Forms.DateTimePicker dtPickerStartTime;
        private System.Windows.Forms.CheckBox chkCategories;
        private System.Windows.Forms.CheckBox chkFeilds;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmdField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRollUp;
        private System.Windows.Forms.CheckBox ChkMatrixFormat;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmbFiscal;
        private System.Windows.Forms.CheckBox ChkAutoUpdate;
    }
}
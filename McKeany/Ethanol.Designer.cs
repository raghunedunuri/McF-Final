namespace McKeany
{
    partial class Ethanol
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ChkYearFormat = new System.Windows.Forms.CheckBox();
            this.ChkRollUP = new System.Windows.Forms.CheckBox();
            this.treeGroups = new System.Windows.Forms.TreeView();
            this.treeFields = new System.Windows.Forms.TreeView();
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
            this.grpFileds = new System.Windows.Forms.GroupBox();
            this.chkCondition = new System.Windows.Forms.CheckBox();
            this.chkCommodity = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbFiscal = new System.Windows.Forms.ComboBox();
            this.cmbRollUp = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdField = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ChkMatrixFormat = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ChkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.grpCategories.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.DateFilters.SuspendLayout();
            this.grpFileds.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCategories
            // 
            this.grpCategories.Controls.Add(this.groupBox1);
            this.grpCategories.Controls.Add(this.treeGroups);
            this.grpCategories.Location = new System.Drawing.Point(20, 23);
            this.grpCategories.Name = "grpCategories";
            this.grpCategories.Size = new System.Drawing.Size(642, 170);
            this.grpCategories.TabIndex = 23;
            this.grpCategories.TabStop = false;
            this.grpCategories.Text = "Commodities";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ChkYearFormat);
            this.groupBox1.Controls.Add(this.ChkRollUP);
            this.groupBox1.Location = new System.Drawing.Point(2, 599);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(639, 10);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Group by Period";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 303);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Type";
            // 
            // ChkYearFormat
            // 
            this.ChkYearFormat.AutoSize = true;
            this.ChkYearFormat.Location = new System.Drawing.Point(17, 21);
            this.ChkYearFormat.Name = "ChkYearFormat";
            this.ChkYearFormat.Size = new System.Drawing.Size(108, 21);
            this.ChkYearFormat.TabIndex = 1;
            this.ChkYearFormat.Text = "Year Format";
            this.ChkYearFormat.UseVisualStyleBackColor = true;
            // 
            // ChkRollUP
            // 
            this.ChkRollUP.AutoSize = true;
            this.ChkRollUP.Location = new System.Drawing.Point(305, 266);
            this.ChkRollUP.Name = "ChkRollUP";
            this.ChkRollUP.Size = new System.Drawing.Size(152, 21);
            this.ChkRollUP.TabIndex = 0;
            this.ChkRollUP.Text = "Seasonal Summary";
            this.ChkRollUP.UseVisualStyleBackColor = true;
            // 
            // treeGroups
            // 
            this.treeGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGroups.Location = new System.Drawing.Point(3, 18);
            this.treeGroups.Name = "treeGroups";
            this.treeGroups.Size = new System.Drawing.Size(636, 149);
            this.treeGroups.TabIndex = 0;
            // 
            // treeFields
            // 
            this.treeFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeFields.Location = new System.Drawing.Point(3, 18);
            this.treeFields.Name = "treeFields";
            this.treeFields.Size = new System.Drawing.Size(633, 104);
            this.treeFields.TabIndex = 1;
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
            this.DateFilters.Location = new System.Drawing.Point(29, 355);
            this.DateFilters.Name = "DateFilters";
            this.DateFilters.Size = new System.Drawing.Size(629, 141);
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
            this.btnCancel.Location = new System.Drawing.Point(465, 712);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 39);
            this.btnCancel.TabIndex = 27;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(119, 715);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(73, 39);
            this.btnRun.TabIndex = 26;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // grpFileds
            // 
            this.grpFileds.Controls.Add(this.treeFields);
            this.grpFileds.Location = new System.Drawing.Point(23, 223);
            this.grpFileds.Name = "grpFileds";
            this.grpFileds.Size = new System.Drawing.Size(639, 125);
            this.grpFileds.TabIndex = 24;
            this.grpFileds.TabStop = false;
            this.grpFileds.Text = "Conditions";
            // 
            // chkCondition
            // 
            this.chkCondition.AutoSize = true;
            this.chkCondition.Location = new System.Drawing.Point(501, 200);
            this.chkCondition.Name = "chkCondition";
            this.chkCondition.Size = new System.Drawing.Size(88, 21);
            this.chkCondition.TabIndex = 28;
            this.chkCondition.Text = "Select All";
            this.chkCondition.UseVisualStyleBackColor = true;
            this.chkCondition.CheckedChanged += new System.EventHandler(this.chkCondition_CheckedChanged);
            // 
            // chkCommodity
            // 
            this.chkCommodity.AutoSize = true;
            this.chkCommodity.Location = new System.Drawing.Point(503, 4);
            this.chkCommodity.Name = "chkCommodity";
            this.chkCommodity.Size = new System.Drawing.Size(88, 21);
            this.chkCommodity.TabIndex = 1;
            this.chkCommodity.Text = "Select All";
            this.chkCommodity.UseVisualStyleBackColor = true;
            this.chkCommodity.CheckedChanged += new System.EventHandler(this.chkCommodity_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbFiscal);
            this.groupBox2.Controls.Add(this.cmbRollUp);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmdField);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(28, 503);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(633, 134);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Group by Period";
            // 
            // cmbFiscal
            // 
            this.cmbFiscal.FormattingEnabled = true;
            this.cmbFiscal.Location = new System.Drawing.Point(475, 39);
            this.cmbFiscal.Name = "cmbFiscal";
            this.cmbFiscal.Size = new System.Drawing.Size(121, 24);
            this.cmbFiscal.TabIndex = 6;
            // 
            // cmbRollUp
            // 
            this.cmbRollUp.FormattingEnabled = true;
            this.cmbRollUp.Location = new System.Drawing.Point(195, 89);
            this.cmbRollUp.Name = "cmbRollUp";
            this.cmbRollUp.Size = new System.Drawing.Size(215, 24);
            this.cmbRollUp.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Period Frequency";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // cmdField
            // 
            this.cmdField.FormattingEnabled = true;
            this.cmdField.Location = new System.Drawing.Point(195, 36);
            this.cmdField.Name = "cmdField";
            this.cmdField.Size = new System.Drawing.Size(215, 24);
            this.cmdField.TabIndex = 4;
            this.cmdField.SelectedIndexChanged += new System.EventHandler(this.cmdField_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Type";
            // 
            // ChkMatrixFormat
            // 
            this.ChkMatrixFormat.AutoSize = true;
            this.ChkMatrixFormat.Location = new System.Drawing.Point(19, 21);
            this.ChkMatrixFormat.Name = "ChkMatrixFormat";
            this.ChkMatrixFormat.Size = new System.Drawing.Size(168, 21);
            this.ChkMatrixFormat.TabIndex = 1;
            this.ChkMatrixFormat.Text = "Seasonal Comparison";
            this.ChkMatrixFormat.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.ChkMatrixFormat);
            this.groupBox4.Location = new System.Drawing.Point(26, 649);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(632, 52);
            this.groupBox4.TabIndex = 33;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Display Options";
            // 
            // ChkAutoUpdate
            // 
            this.ChkAutoUpdate.AutoSize = true;
            this.ChkAutoUpdate.Location = new System.Drawing.Point(137, 4);
            this.ChkAutoUpdate.Name = "ChkAutoUpdate";
            this.ChkAutoUpdate.Size = new System.Drawing.Size(143, 21);
            this.ChkAutoUpdate.TabIndex = 32;
            this.ChkAutoUpdate.Text = "Auto Update Data";
            this.ChkAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // Ethanol
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 762);
            this.Controls.Add(this.ChkAutoUpdate);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.chkCommodity);
            this.Controls.Add(this.chkCondition);
            this.Controls.Add(this.grpCategories);
            this.Controls.Add(this.DateFilters);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.grpFileds);
            this.Name = "Ethanol";
            this.Text = "Ethanol";
            this.grpCategories.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.DateFilters.ResumeLayout(false);
            this.DateFilters.PerformLayout();
            this.grpFileds.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpCategories;
        private System.Windows.Forms.TreeView treeGroups;
        private System.Windows.Forms.TreeView treeFields;
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
        private System.Windows.Forms.GroupBox grpFileds;
        private System.Windows.Forms.CheckBox chkCondition;
        private System.Windows.Forms.CheckBox chkCommodity;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRollUp;
        private System.Windows.Forms.CheckBox ChkYearFormat;
        private System.Windows.Forms.CheckBox ChkRollUP;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmdField;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ChkMatrixFormat;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cmbFiscal;
        private System.Windows.Forms.CheckBox ChkAutoUpdate;
    }
}
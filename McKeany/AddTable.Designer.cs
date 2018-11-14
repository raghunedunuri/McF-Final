namespace McKeany
{
    partial class AddTable
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtTable = new System.Windows.Forms.TextBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.Da = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(91, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Table Name:";
            // 
            // txtTable
            // 
            this.txtTable.Location = new System.Drawing.Point(186, 90);
            this.txtTable.Name = "txtTable";
            this.txtTable.Size = new System.Drawing.Size(312, 22);
            this.txtTable.TabIndex = 1;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(114, 153);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(95, 35);
            this.btnCreate.TabIndex = 3;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(327, 153);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 35);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtDataSource
            // 
            this.txtDataSource.Location = new System.Drawing.Point(186, 46);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(312, 22);
            this.txtDataSource.TabIndex = 5;
            // 
            // Da
            // 
            this.Da.AutoSize = true;
            this.Da.Location = new System.Drawing.Point(91, 49);
            this.Da.Name = "Da";
            this.Da.Size = new System.Drawing.Size(87, 17);
            this.Da.TabIndex = 6;
            this.Da.Text = "Data Source";
            // 
            // AddTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 241);
            this.Controls.Add(this.Da);
            this.Controls.Add(this.txtDataSource);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.txtTable);
            this.Controls.Add(this.label1);
            this.Name = "AddTable";
            this.Text = "AddTable";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTable;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtDataSource;
        private System.Windows.Forms.Label Da;
    }
}
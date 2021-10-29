
namespace EInvoice.DesktopUI
{
    partial class DocumentSearchForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.issuanceDateFrom = new System.Windows.Forms.DateTimePicker();
            this.issunaceDateTo = new System.Windows.Forms.DateTimePicker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.submissionDateTo = new System.Windows.Forms.DateTimePicker();
            this.submissionDateFrom = new System.Windows.Forms.DateTimePicker();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbCustomers = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInvoiceNumber = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.issunaceDateTo);
            this.groupBox1.Controls.Add(this.issuanceDateFrom);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(233, 81);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Issuance Date";
            // 
            // issuanceDateFrom
            // 
            this.issuanceDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.issuanceDateFrom.Location = new System.Drawing.Point(6, 21);
            this.issuanceDateFrom.Name = "issuanceDateFrom";
            this.issuanceDateFrom.Size = new System.Drawing.Size(200, 22);
            this.issuanceDateFrom.TabIndex = 0;
            // 
            // issunaceDateTo
            // 
            this.issunaceDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.issunaceDateTo.Location = new System.Drawing.Point(6, 49);
            this.issunaceDateTo.Name = "issunaceDateTo";
            this.issunaceDateTo.Size = new System.Drawing.Size(200, 22);
            this.issunaceDateTo.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.submissionDateTo);
            this.groupBox2.Controls.Add(this.submissionDateFrom);
            this.groupBox2.Location = new System.Drawing.Point(298, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(225, 81);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Submission Date";
            // 
            // submissionDateTo
            // 
            this.submissionDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.submissionDateTo.Location = new System.Drawing.Point(6, 49);
            this.submissionDateTo.Name = "submissionDateTo";
            this.submissionDateTo.Size = new System.Drawing.Size(200, 22);
            this.submissionDateTo.TabIndex = 1;
            // 
            // submissionDateFrom
            // 
            this.submissionDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.submissionDateFrom.Location = new System.Drawing.Point(6, 21);
            this.submissionDateFrom.Name = "submissionDateFrom";
            this.submissionDateFrom.Size = new System.Drawing.Size(200, 22);
            this.submissionDateFrom.TabIndex = 0;
            this.submissionDateFrom.Value = new System.DateTime(2021, 9, 23, 10, 48, 0, 0);
            // 
            // cmbStatus
            // 
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Items.AddRange(new object[] {
            "",
            "Valid",
            "Invalid",
            "Submitted"});
            this.cmbStatus.Location = new System.Drawing.Point(122, 156);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(186, 23);
            this.cmbStatus.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Status:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Customer:";
            // 
            // cmbCustomers
            // 
            this.cmbCustomers.FormattingEnabled = true;
            this.cmbCustomers.Items.AddRange(new object[] {
            "",
            "Valid",
            "Invalid",
            "Submitted"});
            this.cmbCustomers.Location = new System.Drawing.Point(122, 127);
            this.cmbCustomers.Name = "cmbCustomers";
            this.cmbCustomers.Size = new System.Drawing.Size(401, 23);
            this.cmbCustomers.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Invoice Number:";
            // 
            // txtInvoiceNumber
            // 
            this.txtInvoiceNumber.Location = new System.Drawing.Point(122, 99);
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.Size = new System.Drawing.Size(204, 22);
            this.txtInvoiceNumber.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(430, 197);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 32);
            this.button1.TabIndex = 8;
            this.button1.Text = "Search...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DocumentSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 241);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtInvoiceNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbCustomers);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DocumentSearchForm";
            this.Text = "Serach Documents";
            this.Load += new System.EventHandler(this.DocumentSearchForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker issunaceDateTo;
        private System.Windows.Forms.DateTimePicker issuanceDateFrom;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DateTimePicker submissionDateTo;
        private System.Windows.Forms.DateTimePicker submissionDateFrom;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbCustomers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInvoiceNumber;
        private System.Windows.Forms.Button button1;
    }
}
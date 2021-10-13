using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EInvoice.DesktopUI.ViewModel;
using EInvoice.DesktopUI.Controllers;
namespace EInvoice.DesktopUI
{
    public partial class SettingsForm : Form
    {
        private Settings settings;
        public SettingsForm(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;
            txtServer.DataBindings.Add("Text", this.settings, "Server");
            txtDB.DataBindings.Add("Text", this.settings, "Database");
            txtUserName.DataBindings.Add("Text", this.settings, "UserName");
            txtPassword.DataBindings.Add("Text", this.settings, "Password");
            var b = txtInvoiceMaxAmount.DataBindings.Add("Text", this.settings, "MaximumInvoiceTotalAmountWithoutNationalId");
            b.Format += (o, e) =>
            {
                if (e.DesiredType == typeof(string))
                {
                    e.Value = ((double)e.Value).ToString("0.0#####");
                }
            };
            txtSubmissionInHours.DataBindings.Add("Text", this.settings, "InvoiceSubmissionInHours");
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                AppSettingsController.Settings = settings;
                Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtInvoiceMaxAmount_Validating(object sender, CancelEventArgs e)
        {
            if(!decimal.TryParse(txtInvoiceMaxAmount.Text??"0",out decimal val))
            {
                e.Cancel = true;
                MessageBox.Show("Invalid Invoice Max Value!");
            }
        }

        private void txtSubmissionInHours_Validating(object sender, CancelEventArgs e)
        {
            if (!int.TryParse(txtSubmissionInHours.Text ?? "0", out int val))
            {
                e.Cancel = true;
                MessageBox.Show("Invalid Submission Hours.");
            }
        }
    }
}

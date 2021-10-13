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

namespace EInvoice.DesktopUI
{
    public partial class SelectReportForm : Form
    {
        private SelectReportViewModel _model;
        private Controllers.NavigatorController _controller;
        public SelectReportForm(SelectReportViewModel model,Controllers.NavigatorController controller)
        {
            InitializeComponent();
            _model = model;
            _controller = controller;
            cmbReports.DataSource = _model.AvailableReports;
            cmbReports.DisplayMember = "Name";
            cmbReports.ValueMember = "Name";
            btnOk.DataBindings.Add("Enabled", _model, "EnableOkButton");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _controller.SelectReport(_model.SelectReport,_model.Issuer,_model.APIEnvironment);
        }

        private void cmbReports_SelectedIndexChanged(object sender, EventArgs e)
        {
            _model.SelectReport = _model.AvailableReports[cmbReports.SelectedIndex];
        }
    }
}

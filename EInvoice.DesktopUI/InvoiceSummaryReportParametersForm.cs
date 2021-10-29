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
    public partial class InvoiceSummaryReportParametersForm : Form
    {
        private InvoiceSummaryReportParametersViewModel _model;
        private Controllers.NavigatorController _controller;
        public InvoiceSummaryReportParametersForm(InvoiceSummaryReportParametersViewModel model,Controllers.NavigatorController controller)
        {
            InitializeComponent();
            _model = model;
            _controller = controller;
            lblValidationMessage.DataBindings.Add("Text", _model, "ValidationResult");
            cmbCustomers.DataSource = _model.Receivers;
            cmbCustomers.DisplayMember = "Name";
            cmbCustomers.ValueMember = "Name";
            dateFrom_dateTimePicker.DataBindings.Add("Value", _model, "IssueDateFrom");
            dateTo_datepicker.DataBindings.Add("Value", _model, "IssueDateTo");
            _model.PropertyChanged += _model_PropertyChanged;
        }

        private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ValidationResult")
            {
                lblValidationMessage.Text = _model.ValidationResult;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
           var data =  _controller.ShowInvoiceSummaryReport(_model);
            InvoiceSummaryReportForm reportForm = new InvoiceSummaryReportForm(data);
            reportForm.ShowDialog(this);
        }

        private void cmbCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            _model.SelectedReceiver = _model.Receivers[cmbCustomers.SelectedIndex];
        }
    }
}

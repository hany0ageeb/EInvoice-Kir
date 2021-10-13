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
    public partial class DocumentSearchForm : Form
    {
        private DocumentSearchViewModel _model;
        private Controllers.NavigatorController _navigatorController;
        public DocumentSearchForm(DocumentSearchViewModel model, Controllers.NavigatorController navigatorController)
        {
            InitializeComponent();
            _model = model;
            _navigatorController = navigatorController;
            txtInvoiceNumber.DataBindings.Add("Text", _model, "InvoiceNumber",false,DataSourceUpdateMode.OnPropertyChanged);
            issuanceDateFrom.DataBindings.Add("Value", _model, "IssuanceDateFrom");
            issunaceDateTo.DataBindings.Add("Value", _model, "IssuanceDateTo");
            submissionDateFrom.DataBindings.Add("Value", _model, "SubmissionDateFrom");
            submissionDateTo.DataBindings.Add("Value", _model, "SubmissionDateTo");
            var binding = new BindingSource();
            binding.DataSource = _model.Receivers;
            cmbCustomers.DataSource = binding;
            cmbCustomers.DisplayMember = "Name";
            cmbCustomers.ValueMember = "InternalId";
        }

        private void DocumentSearchForm_Load(object sender, EventArgs e)
        {
            Text = Text + "..."+_model.Issuer.Name + " / " + _model.APIEnvironment.Name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _model.SelectedStatus = cmbStatus.SelectedItem as string;
                _model.SelectedReceiver = _model.Receivers[cmbCustomers.SelectedIndex];
                _model.IssuanceDateFrom = new DateTime(_model.IssuanceDateFrom.Year, _model.IssuanceDateFrom.Month, _model.IssuanceDateFrom.Day,0,0,0);
                _model.IssuanceDateTo = new DateTime(_model.IssuanceDateTo.Year, _model.IssuanceDateTo.Month, _model.IssuanceDateTo.Day, 23, 59, 59);
                _model.SubmissionDateFrom = new DateTime(_model.SubmissionDateFrom.Year, _model.SubmissionDateFrom.Month, _model.SubmissionDateFrom.Day, 0, 0, 0);
                _model.SubmissionDateTo = new DateTime(_model.SubmissionDateTo.Year, _model.SubmissionDateTo.Month, _model.SubmissionDateTo.Day, 23, 59, 59);
                var searchResult = _navigatorController.SearchDocuments(_model);
                DocumentSearchResultForm form = new DocumentSearchResultForm(searchResult, _navigatorController);
                form.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

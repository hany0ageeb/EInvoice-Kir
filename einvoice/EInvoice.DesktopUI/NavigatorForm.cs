using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EInvoice.DesktopUI.Controllers;
using EInvoice.DesktopUI.ViewModel;

namespace EInvoice.DesktopUI
{
    public partial class NavigatorForm : Form
    {
        private readonly NavigatorController _controller;
        private NavigatorViewModel NavigatorViewModel;
        public NavigatorForm(NavigatorController controller,NavigatorViewModel model)
        {
            _controller = controller;
            NavigatorViewModel = model;
            InitializeComponent();
            lstOptions.DataSource = model.Options;
            lstOptions.Focus();
            lstOptions.SetSelected(0, true);
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOption();
            }
            catch(Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
            }
        }
        private void SelectOption()
        {
           
                switch (lstOptions.SelectedItem as string)
                {
                    case "Submit Documents":
                        Cursor = Cursors.WaitCursor;
                        _controller.ProcessCrashData(NavigatorViewModel.Issuer, NavigatorViewModel.APIEnvironment);
                        var model = _controller.SubmitDocuments(NavigatorViewModel.Issuer, NavigatorViewModel.APIEnvironment);
                        SubmitDocumentsForm submitDocumentsForm = new SubmitDocumentsForm(model, _controller);
                        Cursor = Cursors.Default;
                        submitDocumentsForm.ShowDialog(this);
                        break;
                    case "Documents":
                        DocumentSearchViewModel documentSearchFormmodel = _controller.SearchDocuments(NavigatorViewModel.Issuer, NavigatorViewModel.APIEnvironment);
                        DocumentSearchForm documentSearchForm = new DocumentSearchForm(documentSearchFormmodel, _controller);
                        documentSearchForm.ShowDialog(this);
                        break;
                    case "Reports":
                        SelectReportViewModel selectReportViewModel = _controller.SelectReport(NavigatorViewModel.Issuer, NavigatorViewModel.APIEnvironment);
                        SelectReportForm selectReportForm = new SelectReportForm(selectReportViewModel, _controller);
                        selectReportForm.ShowDialog(this);
                        break;
                }
           
        }
        private void NavigatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(this, "Are You Sure You Want To Exit?", "Caution", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (result == DialogResult.OK)
                {
                    e.Cancel = false;
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}

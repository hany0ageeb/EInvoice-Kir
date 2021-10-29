using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EInvoice.DesktopUI
{
    public partial class InvoiceSummaryReportForm : Form
    {
        public InvoiceSummaryReportForm(IList<Model.InvoiceSummaryView> invoicessummary)
        {
            InitializeComponent();
            
            InvoiceSummaryViewBindingSource.DataSource = invoicessummary;
        }

        private void InvoiceSummaryReportForm_Load(object sender, EventArgs e)
        {
            AutoScaleMode = AutoScaleMode.Font;
            reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.PageWidth;
            //reportViewer1.SetPageSettings(new System.Drawing.Printing.PageSettings() { Landscape = false });
            reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            reportViewer1.ShowRefreshButton = true;
            reportViewer1.ShowExportButton = true;
            reportViewer1.ShowFindControls = true;
           
            reportViewer1.RefreshReport();
        }

        private void invoiceLineViewModelBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void InvoiceSummaryReportForm_SizeChanged(object sender, EventArgs e)
        {
            
        }
    }
}

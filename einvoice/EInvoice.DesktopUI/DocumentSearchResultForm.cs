using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EInvoice.Model;
using EInvoice.DesktopUI.ViewModel;

namespace EInvoice.DesktopUI
{
    public partial class DocumentSearchResultForm : Form
    {
        private DocumentSerachResultViewModel _model;
        private readonly Controllers.NavigatorController navigatorController;
        public DocumentSearchResultForm(DocumentSerachResultViewModel model,Controllers.NavigatorController controller)
        {
            navigatorController = controller;
            this._model = model;
            InitializeComponent();
            dataGridView1.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    ReadOnly = true,
                    DataPropertyName = "InternalId",
                    HeaderText = "Id"
                }
             );
            dataGridView1.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    ReadOnly = true,
                    DataPropertyName = "InvoiceNumber",
                    HeaderText = "Invoice Number"
                }
            );
            dataGridView1.Columns.Add(
               new DataGridViewTextBoxColumn()
               {
                   ReadOnly = true,
                   DataPropertyName = "DateTimeIssued",
                   HeaderText = "Issue Date"
               }
           );
            dataGridView1.Columns.Add(
               new DataGridViewTextBoxColumn()
               {
                   ReadOnly = true,
                   DataPropertyName = "ReceiverName",
                   HeaderText = "Customer Name"
               }
           );
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn()
            {
                ReadOnly = true,
                DataPropertyName = "Total",
                HeaderText = "Total Value",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader,
            };
            column.DefaultCellStyle.Format = "##,##.00";
            dataGridView1.Columns.Add(
              column
           );
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                ReadOnly = true,
                DataPropertyName = "UUID",
                HeaderText = "UUID"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                ReadOnly = true,
                DataPropertyName = "DateTimeReceived",
                HeaderText = "Sumission Date"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                ReadOnly = true,
                DataPropertyName = "Status",
                HeaderText = "State"
            });
            dataGridView1.DataSource =_model.Lines;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView1.MultiSelect = false;
            var summary = new
            {
                Count = _model.Lines.Count,
                Total = _model.Lines.Sum(l => l.Total)
            };
            txtInvoiceCount.DataBindings.Add("Text", summary, "Count");
            var b = txtTotalAmount.DataBindings.Add("Text", summary, "Total");
            b.Format += (o,e) => 
            {
                if (e.DesiredType == typeof(string))
                {
                    e.Value = ((decimal)e.Value).ToString("##,##.00");
                }
            };
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_model.Lines[dataGridView1.CurrentRow.Index].Status == "Valid")
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = _model.Issuer.Name+"_"+_model.Lines[dataGridView1.CurrentRow.Index].InvoiceNumber +"_"+_model.Environment.Name+ ".pdf";
                if (!System.IO.Directory.Exists(Environment.CurrentDirectory + "\\PDF"))
                    System.IO.Directory.CreateDirectory(Environment.CurrentDirectory + "\\PDF");
                saveFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\PDF";
                saveFileDialog.Filter = "Pdf Files | *.pdf";
                saveFileDialog.DefaultExt = "pdf";
                var result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;
                    navigatorController.DownloadPdfFile(saveFileDialog.FileName, _model.Lines[dataGridView1.CurrentRow.Index].UUID,_model.Environment,_model.Issuer);
                    Cursor = Cursors.Default;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DocumentSearchResultForm_Load(object sender, EventArgs e)
        {
            Text = Text +"..."+_model.Issuer.Name + " / " + _model.Environment.Name;
        }
    }
}

using EInvoice.DesktopUI.ViewModel;
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
    public partial class SubmitDocumentsForm : Form
    {
        private SubmitDocumentFormViewModel _model;
        private Controllers.NavigatorController _controller;
        public SubmitDocumentsForm(SubmitDocumentFormViewModel model,Controllers.NavigatorController controller)
        {
            InitializeComponent();
            _model = model;
            _controller = controller;
            _model.Submits.ListChanged += Submits_ListChanged;
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = _model.Submits;
            dataGridView1.DataSource = bindingSource;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowDrop = false;
            //dataGridView1.ReadOnly = true;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() 
            {
                DataPropertyName = "InternalId",
                HeaderText = "Id",
                Name = "InternalId",
                ReadOnly = true
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "ProformaInvoiceNumber",
                HeaderText = "رقم الفاتورة",
                Name = "ProformaInvoiceNumber",
                ReadOnly = true
            });
            var col = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "IssueDate",
                HeaderText = "تاريخ الاصدار",
                Name = "IssueDate",
                ReadOnly = false
            };
            col.DefaultCellStyle.Format = "dd/MM/yyyy";
            dataGridView1.Columns.Add(col);
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "ReceiverName",
                HeaderText = "العميل",
                Name = "ReceiverName",
                ReadOnly = true
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "TotalAmount",
                HeaderText = "إجمالى القيمة",
                Name = "TotalAmount",
                ReadOnly = true
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "LocalValidationResult",
                HeaderText = "Validation Result",
                Name = "LocalValidationResult",
                ReadOnly = true
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "SubmissionUUID",
                HeaderText = "Submission UUID",
                Name = "SubmissionUUID",
                ReadOnly = true
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "UUID",
                HeaderText = "UUID",
                Name = "UUID",
                ReadOnly = true
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "StatusOnPortal",
                HeaderText = "Status On Portal",
                Name = "StatusOnPortal",
                ReadOnly = true
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "StatusErrorOnPortal",
                HeaderText = "Error On Portal",
                Name = "StatusErrorOnPortal",
                ReadOnly = true
            });
            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                DataPropertyName = "Submit",
                HeaderText = "Submit",
                Name = "Submit",
                ReadOnly = false
            });
            dataGridView1.Columns.Add(new DataGridViewButtonColumn()
            {
                DataPropertyName = "Document",
                HeaderText = "Document",
                Name = "Document",
                Visible = false
            });
            dataGridView1.Columns.Add(new DataGridViewButtonColumn()
            {
                DataPropertyName = "DocumentButtonText",
                HeaderText = "Invoice Details",
                Name = "DocumentButtonText",
                Visible = true
            });
            dataGridView1.Columns["TotalAmount"].DefaultCellStyle.Format = "##,##.00";
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() 
            { 
                DataPropertyName = "PortalValidationResult",
                Visible = false
            });
            var Summary = new 
            { 
                Count = (from sub in model.Submits where sub.Submit select sub).Count(),
                Total = (from s in model.Submits where s.Submit select s.TotalAmount).Sum().ToString("##,##.00")
            };
            txtInvoiceCount.DataBindings.Add("Text", Summary, "Count");
            txtTotalAmount.DataBindings.Add("Text", Summary , "Total");
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
            submissionProgress.DataBindings.Add("Visible", _model, "ProgressBarVisible",true,DataSourceUpdateMode.OnPropertyChanged);
            submissionProgress.DataBindings.Add("Minimum", _model, "ProgressBarMin");
            submissionProgress.DataBindings.Add("Maximum", _model, "ProgressBarMax");
            submissionProgress.DataBindings.Add("Value", _model, "ProgressBarValue");
            txtMessage.DataBindings.Add("Text", _model, "MessageBoardText");
            btnSubmit.DataBindings.Add("Enabled", _model, "SubmitButtonEnabled");
            _model.PropertyChanged += _model_PropertyChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
        }

        private void Submits_ListChanged(object sender, ListChangedEventArgs e)
        {
            if(e.ListChangedType == ListChangedType.ItemChanged)
            {
                if (!string.IsNullOrEmpty(_model.Submits[e.NewIndex].StatusOnPortal) && _model.Submits[e.NewIndex].StatusOnPortal == "Valid" || _model.Submits[e.NewIndex].StatusOnPortal == "Submitted")
                {
                    dataGridView1.Rows[e.NewIndex].DefaultCellStyle.BackColor = Color.White;
                }
                else
                {

                }
                var Summary = new
                {
                    Count = (from sub in _model.Submits where sub.Submit select sub).Count(),
                    Total = (from s in _model.Submits where s.Submit select s.TotalAmount).Sum().ToString("##,##.00")
                };
                txtInvoiceCount.Text = Summary.Count.ToString();
                txtTotalAmount.Text = Summary.Total;
            }
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 11)
            {
               
            }
        }

        private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            submissionProgress.Visible = _model.ProgressBarVisible;
            submissionProgress.Minimum = _model.ProgressBarMin;
            submissionProgress.Maximum = _model.ProgressBarMax;
            submissionProgress.Value = _model.ProgressBarValue;
            btnSubmit.Enabled = _model.SubmitButtonEnabled;
        }
        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < _model.Submits.Count; i++)
            {
                if (_model.Submits[i].LocalValidationResult.ValidationState == Validation.ValidationState.Invalid)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 13)
            {
                if (e.RowIndex >= 0)
                {
                    _controller.ViewDocumentDetails(_model.Submits[e.RowIndex].Document);
                }
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
         {
            txtMessage.Clear();
            var itm = (SubmitDocumentViewModel)dataGridView1.CurrentRow.DataBoundItem;
            if (string.IsNullOrEmpty(itm.StatusOnPortal) || itm.StatusOnPortal != "InValid") 
            {
                var result = ((SubmitDocumentViewModel)dataGridView1.CurrentRow.DataBoundItem).LocalValidationResult;
                if (result.ValidationState == Validation.ValidationState.Invalid)
                {
                    foreach (var err in result.Errors)
                        txtMessage.AppendText(err.Message+"\n");
                }
            }
            else
            {
                var result = _model.Submits[dataGridView1.CurrentRow.Index].PortalValidationResult;
                foreach(var step in result.validationSteps)
                {
                    if(step.status == "Invalid")
                    {
                        txtMessage.AppendText(step.name+"\n"+step.error.Message+"\n");
                    }
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                btnSubmit.Enabled = false;
                IList<Model.DocumentRejected> rejects =  _controller.SubmitDocument(_model);
                if (rejects.Count > 0)
                {
                    var dresult = MessageBox.Show(this, $"{rejects.Count} Invoice(s) has been Rejected From Portal.\nDo You Want To Generate Report for the rejected Documents?","Rejected Invoices",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                    if (dresult == DialogResult.Yes)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        saveFileDialog.FileName = "REJECTED_DOCUMENTS_" + DateTime.Now.ToShortDateString() + ".html";
                        saveFileDialog.Filter = "html Files | *.html";
                        saveFileDialog.DefaultExt = "html";
                        var result = saveFileDialog.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            _controller.GenerateRejectedDocumentReport(rejects, saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch(DllNotFoundException ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Eror", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
           
            }
        }
        private void SubmitDocumentsForm_Load(object sender, EventArgs e)
        {
            Text = Text + "( " + _model?.Issuer?.Name + " / " + _model?.APIEnvironment?.Name + " )";
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 1)
            {
                foreach (var itm in _model.Submits)
                {
                    if (!itm.Submit)
                        itm.Submit = true;
                }
            }
            else
            {
                for (int index = 0; index < dataGridView1.SelectedRows.Count; index++)
                {
                    var itm = _model.Submits[dataGridView1.SelectedRows[index].Index];
                    if (!itm.Submit)
                        itm.Submit = true;
                }
            }

        }
        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 1)
            {
                foreach (var itm in _model.Submits)
                {
                    if (itm.Submit)
                        itm.Submit = false;
                }
            }
            else
            {
                for(int index = 0; index < dataGridView1.SelectedRows.Count; index++)
                {
                    var itm = _model.Submits[dataGridView1.SelectedRows[index].Index];
                    if (itm.Submit)
                        itm.Submit = false;
                }
            }
            
        }
    }
}

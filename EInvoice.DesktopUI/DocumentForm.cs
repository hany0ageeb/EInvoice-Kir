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
    public partial class DocumentForm : Form
    {
        private ViewModel.DocumentViewModel _model;
        public DocumentForm(ViewModel.DocumentViewModel model)
        {
            InitializeComponent();
            _model = model;
            InitializeDocument();
        }
        private void InitializeDocument()
        {
            txtReceiverName.DataBindings.Add("Text", _model.Document.Receiver, "Name");
            txtReceiverName.DataBindings.Add("Enabled", _model, "IsEditable");
            txtPrformaInvoiceNumber.DataBindings.Add("Text", _model.Document, "ProformaInvoiceNumber");
            txtPrformaInvoiceNumber.DataBindings.Add("Enabled", _model, "IsEditable");
            Binding b = new Binding("Value", _model.Document, "DateTimeIssued");
            IssueDatePicker.DataBindings.Add(b);
            IssueDatePicker.DataBindings.Add("Enabled", _model, "IsEditable");
            b = new Binding("Text", _model.Document, "NetAmount");
            b.Format += (object o,ConvertEventArgs e) => 
            { 
                if (e.DesiredType == typeof(string))
                {
                    e.Value = ((double)e.Value).ToString("##,##.00");
                }
            };
            txtNetAmount.DataBindings.Add(b);
            txtNetAmount.DataBindings.Add("Enabled", _model, "IsEditable");
            txtTotalDiscountAmount.DataBindings.Add("Text", _model.Document, "TotalDiscountAmount");
            txtTotalDiscountAmount.DataBindings.Add("Enabled", _model, "IsEditable");
            b = new Binding("Text", _model.Document, "TotalAmount");
            b.Format += (o, e) =>
            {
                if (e.DesiredType == typeof(string))
                {
                    e.Value = ((double)e.Value).ToString("##,##.00");
                }
            };
            //txtTotalAmount.DataBindings.Add("Text", _model.Document, "TotalAmount");
            txtTotalAmount.DataBindings.Add(b);
            txtTotalAmount.DataBindings.Add("Enabled", _model, "IsEditable");
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowDrop = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "InternalCode",
                HeaderText = "الكود الداخلى",
                Name = "InternalCode",
                ReadOnly = true
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Description",
                HeaderText = "الوصف",
                Name = "Description",
                ReadOnly = true
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "ItemCode",
                HeaderText = "الكود",
                Name = "ItemCode",
                ReadOnly = true
            });
            var c = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Quantity",
                HeaderText = "الكمية",
                Name = "Quantity",
                ReadOnly = true
            };
            c.DefaultCellStyle.Format = "##,##.00";
            dataGridView1.Columns.Add(c);
            c = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "UnitPrice",
                HeaderText = "سعر الوحدة",
                Name = "UnitPrice",
                ReadOnly = true
            };
            c.DefaultCellStyle.Format = "##,##.00";
            dataGridView1.Columns.Add(c);
            c = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "DiscountAmount",
                HeaderText = "الخصم",
                Name = "DiscountAmount",
                ReadOnly = true
            };
            c.DefaultCellStyle.Format = "##,##.00";
            dataGridView1.Columns.Add(c);
            c = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "SalesTotal",
                HeaderText = "Sales Total",
                Name = "SalesTotal",
                ReadOnly = true
            };
            c.DefaultCellStyle.Format = "##,##.00";
            dataGridView1.Columns.Add(c);
            c = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "NetTotal",
                Name = "NetTotal",
                ReadOnly = true,
                HeaderText = "Net Total",
            };
            c.DefaultCellStyle.Format = "##,##.00";
            dataGridView1.Columns.Add(c);
            c = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "ValueAddedTax",
                HeaderText = "ضريبة القيمة المضافة",
                Name = "ValueAddedTax",
                ReadOnly = true
            };
            c.DefaultCellStyle.Format = "##,##.00";
            dataGridView1.Columns.Add(c);
            c = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "TableTaxAmount",
                HeaderText = "ضريبة الجدول",
                Name = "TableTaxAmount",
                ReadOnly = true
            };
            c.DefaultCellStyle.Format = "##,##.00";
            dataGridView1.Columns.Add(c);
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "WithHoldingTax",
                HeaderText = "الخصم تحت حساب الضريبة",
                Name = "WithHoldingTax",
                ReadOnly = true
            });
            c = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Total",
                HeaderText = "الاجمالى",
                Name = "Total",
                ReadOnly = true
            };
            c.DefaultCellStyle.Format = "##,##.00";
            dataGridView1.Columns.Add(c);
            List<ViewModel.InvoiceLineViewModel> lines = new List<ViewModel.InvoiceLineViewModel>();
            foreach (Model.InvoiceLine invoiceLine in _model.Document.InvoiceLines)
            {
                lines.Add(new ViewModel.InvoiceLineViewModel()
                {
                    Description = invoiceLine.Description,
                    InternalCode = invoiceLine.InternalCode,
                    ItemCode = invoiceLine.ItemCode,
                    Quantity = Convert.ToDecimal(invoiceLine.Quantity),
                    UnitPrice = Convert.ToDecimal(invoiceLine.UnitValue.AmountEGP),
                    SalesTotal = Convert.ToDecimal(invoiceLine.SalesTotal),
                    NetTotal = Convert.ToDecimal(invoiceLine.NetTotal),
                    DiscountAmount = Convert.ToDecimal(invoiceLine.Discount?.Amount),
                    ValueAddedTax = Convert.ToDecimal((from tt in invoiceLine.TaxableItems where tt.TaxType == "T1" select tt.Amount).Sum()),
                    WithHoldingTax = Convert.ToDecimal((from tt in invoiceLine.TaxableItems where tt.TaxType == "T4" select tt.Amount).Sum()),
                    TableTaxAmount = Convert.ToDecimal((from tt in invoiceLine.TaxableItems where tt.TaxType == "T3" select tt.Amount).Sum()),
                    Total = Convert.ToDecimal(invoiceLine.Total)
                }); 
            }
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = lines;
            dataGridView1.DataSource = bindingSource;
        }
        private void DocumentHeadergroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

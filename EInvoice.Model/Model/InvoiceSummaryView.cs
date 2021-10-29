using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.Model
{
    public class InvoiceSummaryView
    {
        public string ProformaInvoiceNumber { get; set; }
        public DateTime DateTimeIssued { get; set; }
        public string IssuerName { get; set; }
        public string ReceiverName { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal ValueAddedTax { get; set; }
        public decimal TableTax { get; set; }
        public decimal WithHoldingTax { get; set; }
        public decimal TotalAmount { get; set; }
        public string UUID { get; set; }
        public string Status { get; set; }
    }
}

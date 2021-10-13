using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.DesktopUI.ViewModel
{
    public class InvoiceLineViewModel
    {
        public string InternalCode { get; set; }
        public string Description { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SalesTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal NetTotal { get; set; } = 0;
        public decimal? ValueAddedTax { get; set; } = 0;
        public decimal? WithHoldingTax { get; set; } = 0;
        public decimal? TableTaxAmount { get; set; } = 0;
        public decimal Total { get; set; } = 0;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;

namespace EInvoice.DesktopUI.ViewModel
{
    public class DocumentSearchViewModel
    {
        public IList<string> Status { get; set; } = new List<string>() { "", "Valid", "Invalid", "Submitted", "Cancelled", "Rejected" };
        public IList<Receiver> Receivers { get; set; }
        public DateTime IssuanceDateFrom { get; set; }
        public DateTime IssuanceDateTo { get; set; }
        public DateTime SubmissionDateFrom { get; set; } 
        public DateTime SubmissionDateTo { get; set; }
        public string InvoiceNumber { get; set; }
        public Issuer Issuer { get; set; }
        public APIEnvironment APIEnvironment { get; set; }
        public Receiver SelectedReceiver { get; set; }
        public string SelectedStatus { get; set; } = "ALL";
    }
}

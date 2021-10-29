using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;

namespace EInvoice.DesktopUI.ViewModel
{
    public class DocumentSearchResultLineViewModel
    {
        public string InternalId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime DateTimeIssued { get; set; }
        public string ReceiverName { get; set; }
        public decimal Total { get; set; }
        public string UUID { get; set; }
        public DateTime? DateTimeReceived { get; set; }
        public string Status { get; set; }

    }
    public class DocumentSerachResultViewModel 
    {
        private int _progressBarValue = 0;
       
        public BindingList<DocumentSearchResultLineViewModel> Lines { get; set; } = new BindingList<DocumentSearchResultLineViewModel>();
        public DocumentSearchViewModel DocumentSearchViewModel { get; set; }
        public Issuer Issuer { get; set; }
        public APIEnvironment Environment { get; set; }
        public bool ProgressBarVisible { get; set; } = false;
        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; }
        public int ProgressBarValue 
        {
            get
            {
               
                {
                    return _progressBarValue;
                }
            }
            set
            {
               
                {
                    if (value != _progressBarValue)
                    {
                        _progressBarValue = value;
                       
                    }
                }
            }
        }

       
    }
}

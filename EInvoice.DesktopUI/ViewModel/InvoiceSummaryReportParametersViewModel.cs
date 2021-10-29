using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;

namespace EInvoice.DesktopUI.ViewModel
{
    public class InvoiceSummaryReportParametersViewModel : INotifyPropertyChanged
    {
        private string _validationResult;
        public DateTime IssueDateFrom { get; set; }
        public DateTime IssueDateTo { get; set; }
        public IList<Receiver> Receivers { get; set; }
        public Receiver SelectedReceiver { get; set; }
        public Issuer Issuer { get; set; }
        public APIEnvironment APIEnvironment { get; set; }
        public string ValidationResult 
        { 
            get=>_validationResult; 
            set 
            {
                if (_validationResult != value)
                {
                    _validationResult = value;
                    OnPropertyChanged("ValidationResult");
                }
            } 
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.DesktopUI.ViewModel;
namespace EInvoice.DesktopUI.ViewModel
{
    public class ModelValidationResult : INotifyPropertyChanged
    {
        private string _message;
        public ModelValidationState ValidationState { get; set; } = ModelValidationState.Valid;
        public string Message 
        { 
            get=>_message; 
            set 
            {
                if (value != _message)
                {
                    _message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }

        protected void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

using EInvoice.DesktopUI.Controllers;
using EInvoice.DesktopUI.ViewModel;
using EInvoice.Model;
using System.ComponentModel;

namespace EInvoice.DesktopUI.ViewModel
{
    public class LogInViewModel : INotifyPropertyChanged
    {
        private ModelValidationResult _validationResult = new ModelValidationResult() { Message = "" };
        public User User { get; set; } = new User() { Password="",Name="",Issuer = null};
        public bool IsLoggedIn { get; set; } = false;
        public ModelValidationResult ValidationResult
        {
            get => _validationResult;
            set
            {
                if (value != _validationResult)
                {
                    _validationResult = value;
                    NotifyPropertyChanged("ValidationResult");
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

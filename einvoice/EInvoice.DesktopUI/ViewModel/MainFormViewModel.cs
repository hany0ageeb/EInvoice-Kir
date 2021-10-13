using EInvoice.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.DesktopUI.ViewModel
{
    public class MainFormViewModel : INotifyPropertyChanged
    {
        private string _title = "";
        private User _currentUser = null;
        private APIEnvironment _currentAPI = null;
        public User CurrentUser 
        { 
            get=>_currentUser; 
            set 
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    NotifyPropertyChanged("CurrentUser");
                    Title = CurrentUser?.Issuer?.Name + " / " + CurrentAPI?.Name;
                }
            } 
        }
        public APIEnvironment CurrentAPI 
        { 
            get=>_currentAPI; 
            set 
            {
                if (_currentAPI != value)
                {
                    _currentAPI = value;
                    NotifyPropertyChanged("CurrentAPI");
                    Title = CurrentUser?.Issuer?.Name + " / " + CurrentAPI?.Name;
                }
            } 
        }
        public IList<APIEnvironment> APIEnvironments { get; set; } = new List<APIEnvironment>();
        public string Title 
        { 
            get=>_title; 
            set 
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            } 
        } 
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

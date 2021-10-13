using EInvoice.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EInvoice.DesktopUI.ViewModel
{
    public class SubmitDocumentViewModel : INotifyPropertyChanged
    {
        private string _uuid;
        private string _statusOnPortal;
        private string _statusErrorOnPortal;
        private string _submissionUUID;
        public string InternalId { get; set; }
        public string ProformaInvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string ReceiverName { get; set; }
        public decimal TotalAmount { get; set; }
        public ValidationResult LocalValidationResult { get; set; }
        public DocumentValidationResult PortalValidationResult { get; set; }
        private object lockthis = new object();

        public string SubmissionUUID 
        { 
            get=>_submissionUUID; 
            set 
            {
                lock (lockthis)
                {
                    if (value != _submissionUUID)
                    {
                        _submissionUUID = value;
                        NotifyPropertyChanged("SubmissionUUID");
                    }
                }
            } 
        }
        public string UUID 
        { 
            get=>_uuid; 
            set 
            {
                lock (lockthis)
                {
                    if (value != _uuid)
                    {
                        _uuid = value;
                        NotifyPropertyChanged("UUID");
                    }
                }
            } 
        }
        public string StatusOnPortal 
        { 
            get=>_statusOnPortal; 
            set 
            {
                lock (lockthis)
                {
                    if (value != _statusOnPortal)
                    {
                        _statusOnPortal = value;
                        NotifyPropertyChanged("StatusOnPortal");
                    }
                }
            } 
        }
        public string StatusErrorOnPortal
        {
            get => _statusErrorOnPortal;
            set
            {
                lock (lockthis)
                {
                    if (value != _statusErrorOnPortal)
                    {
                        _statusErrorOnPortal = value;
                        NotifyPropertyChanged("StatusErrorOnPortal");
                    }
                }
            }
        }
        public bool Submit { get; set; } = true;
        public Document Document { get; set; }
        public string DocumentButtonText { get; set; } = "Details";

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class SubmitDocumentFormViewModel : INotifyPropertyChanged
    {
        private bool _progressBarVisible = false;
        private int _progressBarMax;
        private int _progressBarMin;
        private int _progressBarValue;
        private bool _submitButtonEnabled;
        private object lockThis = new object();
        public bool SubmitButtonEnabled
        {
            get => _submitButtonEnabled;
            set
            {
                if (value != _submitButtonEnabled)
                {
                    _submitButtonEnabled = value;
                    NotifyPropertyChanged("SubmitButtonEnabled");
                }
            }
        }
        public bool ProgressBarVisible 
        { 
            get => _progressBarVisible; 
            set 
            {
                lock (lockThis)
                {
                    if (value != _progressBarVisible)
                    {
                        _progressBarVisible = value;
                        NotifyPropertyChanged("ProgressBarVisible");
                    }
                }
            } 
        } 
        public int ProgressBarMax 
        { 
            get=>_progressBarMax; 
            set 
            {
                lock (lockThis)
                {
                    if (value != _progressBarMax)
                    {
                        _progressBarMax = value;
                        NotifyPropertyChanged("ProgressBarMax");
                    }
                }
            } 
        }
        public int ProgressBarMin 
        { 
            get=>_progressBarMin; 
            set 
            {
                lock (lockThis)
                {
                    if (value != _progressBarMin)
                    {
                        _progressBarMin = value;
                        NotifyPropertyChanged("ProgressBarMin");
                    }
                }
            } 
        }
        public int ProgressBarValue 
        { 
            get=>_progressBarValue;

            set 
            {
                lock (lockThis)
                {
                    if (value != _progressBarValue)
                    {
                        _progressBarValue = value;
                        NotifyPropertyChanged("ProgressBarValue");
                    }
                }
            } 
        }
        public StringBuilder MessageBoardText { get; set; }
       
        public BindingList<SubmitDocumentViewModel> Submits { get; set; } = new BindingList<SubmitDocumentViewModel>();
        public event PropertyChangedEventHandler PropertyChanged;
        public APIEnvironment APIEnvironment { get; set; }
        public Issuer Issuer { get; set; }
        private void NotifyPropertyChanged([CallerMemberName] string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EInvoice.DesktopUI.Controllers;
using EInvoice.DesktopUI.ViewModel;
namespace EInvoice.DesktopUI
{

    public partial class LogInForm : Form
    {
        private readonly UserController userController;

        private LogInViewModel logInViewModel;

        public LogInForm(UserController controller, LogInViewModel logInViewModel)
        {
            InitializeComponent();
            userController = controller;
            this.logInViewModel = logInViewModel;
            lblValidationMessage.DataBindings.Add("Text", logInViewModel.ValidationResult, "Message");
            txtUserName.DataBindings.Add("Text", logInViewModel.User, "Name");
            txtPassword.DataBindings.Add("Text", logInViewModel.User, "Password");
            logInViewModel.PropertyChanged += LogInViewModel_PropertyChanged;
            logInViewModel.ValidationResult.PropertyChanged += ValidationResult_PropertyChanged;
        }

        private void ValidationResult_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {   
            lblValidationMessage.Text = logInViewModel.ValidationResult.Message;
            lblValidationMessage.Visible = true;
        }

        private void LogInViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        public void SetValidationMessage(ModelValidationState state,string msg)
        {
            logInViewModel.ValidationResult.ValidationState = state;
            logInViewModel.ValidationResult.Message = msg;
            if (state == ModelValidationState.Valid)
                lblValidationMessage.Visible = false;
            else
                lblValidationMessage.Visible = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            logInViewModel.ValidationResult.Message = "";
            logInViewModel.ValidationResult.ValidationState = ModelValidationState.Invalid;
            if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                SetValidationMessage(ModelValidationState.Invalid, "Invalid User Name/Password.");
                return;
            }
            userController.LogIn(logInViewModel);
            if (logInViewModel.ValidationResult.ValidationState != ModelValidationState.Invalid)
                Close();
        }
        private void LogInForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
                logInViewModel.IsLoggedIn = false;
        }

        private void LogInForm_Load(object sender, EventArgs e)
        {

        }
    }
}

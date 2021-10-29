using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EInvoice.DesktopUI.Controllers;
using EInvoice.Model;
using EInvoice.DesktopUI.ViewModel;
using EInvoice.DAL.DAO;

namespace EInvoice.DesktopUI
{
    public partial class MainForm : Form
    {

        private readonly HomeController _homeController;
        private MainFormViewModel _model;

        public MainForm(HomeController homeController,MainFormViewModel model)
        {
            InitializeComponent();
            _homeController = homeController;
            _model = model;
            _model.PropertyChanged += _model_PropertyChanged;
        }
        private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Text = _model.Title;
        }
        private void InitializeEnvironment()
        {
            if (_model.APIEnvironments != null)
            {
                cmbEnvironment.ComboBox.DataSource = _model.APIEnvironments;
                cmbEnvironment.ComboBox.DisplayMember = "Name";
                cmbEnvironment.ComboBox.ValueMember = "Id";
                _model.CurrentAPI = (from aPIEnvironment in _model.APIEnvironments where aPIEnvironment.Id == ((APIEnvironment)cmbEnvironment.SelectedItem).Id select aPIEnvironment).FirstOrDefault();
            }
        }
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(AppSettingsController.Settings);
            settingsForm.ShowDialog(this);
        }
        private void cmbEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            _model.CurrentAPI = (from aPIEnvironment in _model.APIEnvironments where aPIEnvironment.Id == ((APIEnvironment)cmbEnvironment.SelectedItem).Id select aPIEnvironment).FirstOrDefault();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private void LoadMainForm()
        {
            try
            {
                InitializeEnvironment();
                var logInModel = _homeController.LogIn(_model);
                LogInForm logInForm = new LogInForm(ControllerFactory.UserController, logInModel);
                logInForm.ShowDialog(this);
                if (!logInModel.IsLoggedIn)
                {
                    Application.Exit();
                }
                _model.CurrentUser = logInModel.User;
                //Create a task to update all submissions
                Task.Run(() =>
                {
                    DbConnection connection = null;
                    try
                    {
                        connection = ObjectFactory.CreateConnection(AppSettingsController.Settings);
                        ITaxableItemDao taxableItemDao = new TaxableItemDaoAdoImpl(connection);
                        IInvoiceLineDao invoiceLineDao = new InvoiceLineDaoAdoImpl(connection, taxableItemDao);
                        IReceiverDao receiverDao = new ReceiverDaoAdoImpl(connection);
                        IDocumentDao documentDao = new DocumentDaoAdoImpl(connection, invoiceLineDao, receiverDao);
                        IIssuerAPIAccessDetailsDao accessDetailsDao = new IssuerAPIAccessDetailsDaoAdoImpl(connection);

                        NavigatorController navigatorController = new NavigatorController(documentDao, accessDetailsDao, receiverDao);
                        do
                        {
                            navigatorController.UpdateDocumentStatus(_model.CurrentUser.Issuer, _model.CurrentAPI);
                            if (connection != null && connection.State != ConnectionState.Closed)
                                connection.Close();
                            Thread.Sleep(240000);
                        } while (true);
                    }
                    finally
                    {
                        if (connection != null && connection.State != ConnectionState.Closed)
                            connection.Close();
                    }
                });
                var navModel = _homeController.Navigator(_model);
                NavigatorForm navigatorForm = new NavigatorForm(ControllerFactory.NavigatorController, navModel);
                navigatorForm.Show(this);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                cmbEnvironment.Enabled = false;
                //Application.Exit();
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadMainForm();
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void cmbEnvironment_Click(object sender, EventArgs e)
        {

        }
    }
}

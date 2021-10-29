using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using EInvoice.DesktopUI.Controllers;
using EInvoice.DesktopUI.ViewModel;

namespace EInvoice.DesktopUI
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            HomeController homeController = ControllerFactory.HomeController;
            MainFormViewModel model = new MainFormViewModel() 
            { 
                APIEnvironments = null,
                CurrentAPI = null,
                CurrentUser = null,
                Title = ""
            };
            try
            {
                model = homeController.Index();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Application.Run(new MainForm(homeController, model));
        }
    }
}

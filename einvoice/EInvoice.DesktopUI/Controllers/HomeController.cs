using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using EInvoice.DAL.DAO;
using EInvoice.Model;
using EInvoice.DesktopUI.ViewModel;

namespace EInvoice.DesktopUI.Controllers
{
    public class HomeController
    {
        public HomeController()
        {
           
        }
        public MainFormViewModel Index()
        {
            IList<APIEnvironment> envs = null;
            MainFormViewModel model = new MainFormViewModel()
            {
                APIEnvironments = envs,
                CurrentAPI = null,
                CurrentUser = null
            };
            model.APIEnvironments = ObjectFactory.APIEnvironmentDao.Find();
            return model;
        }
        public LogInViewModel LogIn(MainFormViewModel model)
        {
            LogInViewModel logInViewModel = new LogInViewModel()
            {
                IsLoggedIn = false,
                ValidationResult = new ModelValidationResult()
                {
                    Message = "",
                    ValidationState = ModelValidationState.Valid
                }
            };
            return logInViewModel;
        }
        public NavigatorViewModel Navigator(MainFormViewModel model)
        {
            var navModel = new NavigatorViewModel()
            {
                Options = new List<string>() { "Submit Documents", "Documents", "Reports" },
                Issuer = model.CurrentUser.Issuer,
                APIEnvironment = model.CurrentAPI
            };
            model.PropertyChanged += (o,e) => 
            {
                navModel.APIEnvironment = ((MainFormViewModel)o).CurrentAPI;
            };
            return navModel;
        }
    }
}

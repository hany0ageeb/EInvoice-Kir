using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EInvoice.Model;
using EInvoice.DAL.DAO;
using EInvoice.DesktopUI.ViewModel;

namespace EInvoice.DesktopUI.Controllers
{
    public class UserController
    {
        private readonly IUserDao _userDao;

        public UserController(IUserDao userDao)
        {
            _userDao = userDao;
        }
        public bool LogIn(LogInViewModel model)
        {
            User user = _userDao.Find(model.User.Name, model.User.Password);
            if (user == null)
            {
                model.ValidationResult.ValidationState = ModelValidationState.Invalid;
                model.ValidationResult.Message = "Invalid User Name / Password.";
                model.IsLoggedIn = false;
                return false;
            }
            else
            {
                model.ValidationResult.ValidationState = ModelValidationState.Valid;
                model.ValidationResult.Message = "";
                model.User = user;
                model.IsLoggedIn = true;
                return true;
            }
        }
    }
}

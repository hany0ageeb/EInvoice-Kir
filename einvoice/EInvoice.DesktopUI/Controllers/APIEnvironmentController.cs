using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;
using EInvoice.DAL.DAO;

namespace EInvoice.DesktopUI.Controllers
{
    public class APIEnvironmentController
    {
        private readonly IAPIEnvironmentDao _APIEnvironmentDao;

        public APIEnvironmentController(IAPIEnvironmentDao APIEnvironmentDao)
        {
            _APIEnvironmentDao = APIEnvironmentDao;
        }
        public IList<APIEnvironment> Index()
        {
            return _APIEnvironmentDao.Find();
        }
    }
}

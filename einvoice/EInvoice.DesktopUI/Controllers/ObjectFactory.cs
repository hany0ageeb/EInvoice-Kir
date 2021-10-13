using System.Data.Common;
using System.Data.SqlClient;
using EInvoice.DAL.EInvoiceAPI;
using EInvoice.DAL.DAO;
using EInvoice.DesktopUI.ViewModel;
using EInvoice.Model;

namespace EInvoice.DesktopUI.Controllers
{
    public static class ObjectFactory
    {
        private static DbConnection _connection = null;
        private static IReceiverDao _receiverDao = null;
        private static IIssuerDao _issuerDao = null;
        private static IIssuerAPIAccessDetailsDao _issuerAPIAccessDetailsDao;
        private static IAPIEnvironmentDao _aPIEnvironmentDao = null;
        private static ITaxableItemDao _taxableItemDao = null;
        private static IInvoiceLineDao _invoiceLineDao = null;
        private static IDocumentDao _documentDao = null;
        private static IUserDao _userDao = null;
        private static ICountryCodeDao _countryCodeDao = null;
        private static IActivityCodeDao _activityCodeDao = null;
        private static ITaxTypeDao _taxTypeDao = null;
        private static IReportDefinitionDao _reportDefinitionDao = null;

        public static IEInvoiceAPIProxy CreateEInvoiceAPIProxy(APIEnvironment environment,string clientId, string clientSecret)
        {
            return new EInvoiceAPIRestSharpProxy(environment, clientId, clientSecret);
        }
        public static IReportDefinitionDao ReportDefinitionDao
        {
            get
            {
                if (_reportDefinitionDao == null)
                {
                    _reportDefinitionDao = new ReportDefinitionDao(Connection);
                }
                return _reportDefinitionDao;
            }
        }
        public static IUserDao UserDao
        {
            get
            {
                if (_userDao == null)
                {
                    _userDao = new UserDaoAdoImpl(Connection, IssuerDao);
                }
                return _userDao;
            }
        }
        public static DbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = CreateConnection(AppSettingsController.Settings);
                }
                return _connection;
            }
        }
        public static IReceiverDao ReceiverDao
        {
            get
            {
                if (_receiverDao == null)
                {
                    _receiverDao = new ReceiverDaoAdoImpl(Connection);
                }
                return _receiverDao;
            }
        }
        public static IIssuerDao IssuerDao
        {
            get
            {
                if (_issuerDao == null)
                {
                    _issuerDao = new IssuerDaoAdoImpl(Connection);
                }
                return _issuerDao;
            }
        }
        public static IIssuerAPIAccessDetailsDao IssuerAPIAccessDetailsDao
        {
            get
            {
                if (_issuerAPIAccessDetailsDao == null)
                {
                    _issuerAPIAccessDetailsDao = new IssuerAPIAccessDetailsDaoAdoImpl(Connection);
                }
                return _issuerAPIAccessDetailsDao;
            }
        }
        public static IAPIEnvironmentDao APIEnvironmentDao
        {
            get
            {
                if (_aPIEnvironmentDao == null)
                {
                    _aPIEnvironmentDao = new APIEnvironmentDaoAdoImpl(Connection);
                }
                return _aPIEnvironmentDao;
            }
        }
        public static ITaxableItemDao TaxableItemDao
        {
            get
            {
                if (_taxableItemDao == null)
                {
                    _taxableItemDao = new TaxableItemDaoAdoImpl(Connection);
                }
                return _taxableItemDao;
            }
        }
        public static IInvoiceLineDao InvoiceLineDao
        {
            get
            {
                if (_invoiceLineDao == null)
                {
                    _invoiceLineDao = new InvoiceLineDaoAdoImpl(Connection, TaxableItemDao);
                }
                return _invoiceLineDao;
            }
        }
        public static IDocumentDao DocumentDao
        {
            get
            {
                if (_documentDao == null)
                {
                    _documentDao = new DocumentDaoAdoImpl(Connection, InvoiceLineDao, ReceiverDao);
                }
                return _documentDao;
            }
        }
        public static ICountryCodeDao CountryCodeDao
        {
            get
            {
                if (_countryCodeDao == null)
                {
                    _countryCodeDao = new CountryCodeDaoAdoImpl(Connection);
                }
                return _countryCodeDao;
            }
        }
        public static IActivityCodeDao ActivityCodeDao
        {
            get
            {
                if (_activityCodeDao == null)
                {
                    _activityCodeDao = new ActivityCodeDaoAdoImpl(Connection);
                }
                return _activityCodeDao;
            }
        }
        public static ITaxTypeDao TaxTypeDao
        {
            get
            {
                if (_taxTypeDao == null)
                {
                    _taxTypeDao = new TaxTypeDaoAdoImpl(Connection);
                }
                return _taxTypeDao;
            }
        }
        public static DbConnection CreateConnection(Settings settings)
        {
            SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder();
            stringBuilder.InitialCatalog = settings.Database;
            stringBuilder.DataSource = settings.Server;
            stringBuilder.IntegratedSecurity = false;
            stringBuilder.UserID = settings.UserName;
            stringBuilder.Password = settings.Password;
            stringBuilder.MultipleActiveResultSets = true;
            if (settings.RemoteConnection)
                stringBuilder.NetworkLibrary = "DBMSSOCN";
            return new SqlConnection(stringBuilder.ToString());
        }
    }
}

namespace EInvoice.DesktopUI.Controllers
{
    public static class ControllerFactory
    {
        private static HomeController _homeController = null;
        private static UserController _userController = null;
        private static NavigatorController _navigatorController = null;

        public static HomeController HomeController
        {
            get
            {
                if (_homeController == null)
                {
                    _homeController = new HomeController();
                }
                return _homeController;
            }
        }
        public static UserController UserController
        {
            get
            {
                if (_userController == null)
                {
                    _userController = new UserController(ObjectFactory.UserDao);
                }
                return _userController;
            }
        }
        public static NavigatorController NavigatorController
        {
            get
            {
                if (_navigatorController == null)
                {
                    _navigatorController = new NavigatorController(ObjectFactory.DocumentDao,ObjectFactory.IssuerAPIAccessDetailsDao,ObjectFactory.ReceiverDao);
                }
                return _navigatorController;
            }
        }
    }
}

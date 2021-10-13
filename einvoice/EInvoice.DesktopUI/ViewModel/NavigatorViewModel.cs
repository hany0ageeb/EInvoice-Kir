using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;

namespace EInvoice.DesktopUI.ViewModel
{
    public class NavigatorViewModel
    {
        public IList<string> Options { get; set; } = new List<string>();
        public Issuer Issuer { get; set; }
        public APIEnvironment APIEnvironment { get; set; }
        public IssuerAPIAccessDetails APIAccessDetails { get; set; }
    }
}

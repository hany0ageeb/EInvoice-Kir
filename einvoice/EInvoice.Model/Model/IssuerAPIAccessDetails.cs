using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.Model
{
    public class IssuerAPIAccessDetails
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SecurityToken { get; set; }
    }
}

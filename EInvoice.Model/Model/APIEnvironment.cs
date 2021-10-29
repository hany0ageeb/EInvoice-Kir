using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.Model
{
    public class APIEnvironment
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public Uri LogInUri { get; set; }
        public Uri BaseUri { get; set; }
        public byte[] Version { get; set; }
    }
}

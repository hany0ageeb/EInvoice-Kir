using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;

namespace EInvoice.DesktopUI.ViewModel
{
    public class DocumentViewModel
    {
        public Document Document { get; set; }
        public bool IsEditable { get; set; } = false;
    }
}

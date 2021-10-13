using EInvoice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.DesktopUI.ViewModel
{
    public class SelectReportViewModel
    {
        public IList<ReportDefinition> AvailableReports { get; set; }
        public ReportDefinition SelectReport { get; set; } = null;
        public Issuer Issuer { get; set; }
        public APIEnvironment APIEnvironment { get; set; }
        public bool EnableOkButton { get; set; } = false;
    }
}

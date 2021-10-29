using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.Model
{
    public class DocumentSubmission
    {
        public string SubmissionUUID { get; set; }
        public string UUID { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.Now;
        public APIEnvironment APIEnvironment { get; set; }
        public Document Document { get; set; }
        public string Status { get; set; }
        public byte[] Version { get; set; }
    }
}

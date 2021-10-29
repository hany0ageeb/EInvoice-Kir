using System;
using System.Collections.Generic;

namespace EInvoice.Model
{
    public class QueryParameters
    {
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public IList<string> documentTypeNames { get; set; } = new List<string>() { "i" };
        public IList<string> statuses { get; set; } = new List<string>() { "valid" };
        public int receiverSenderType { get; set; } = 0;
        public string receiverSenderId { get; set; }
    }
}

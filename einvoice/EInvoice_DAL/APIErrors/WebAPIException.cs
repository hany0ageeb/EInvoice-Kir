using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.DAL.APIErrors
{ 
        [Serializable]
        public class WebAPIException : Exception
        {
            public WebAPIException() { }
            public WebAPIException(string message) : base(message) { }
            public WebAPIException(string message, Exception inner) : base(message, inner) { }
            protected WebAPIException(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
}

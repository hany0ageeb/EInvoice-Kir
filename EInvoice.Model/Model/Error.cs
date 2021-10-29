using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace EInvoice.Model
{
    public class Error
    {
        public Error() 
        {
            Code = "";
        }
        public Error(string code,string message,IList<Error> details) 
        {
            Code = code;
            Message = message;
            if (details != null)
            {
                foreach (Error err in details)
                {
                    Details.Add(err);
                }
            }
        }
        [Required]
        [JsonProperty("code")]
        public string Code { get; set; }
        [Required]
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("target")]
        public string Target { get; set; }
        [JsonProperty("details")]
        public IList<Error> Details { get; set; } = new List<Error>();
    }
    public class PortalError
    {
        [JsonProperty("error")]
        public Error Error { get; set; }
    }
}

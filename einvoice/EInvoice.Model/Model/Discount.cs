using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class Discount
    {
        [Range(0,100)]
        [JsonProperty("rate")]
        public double? Rate { get; set; }
        [JsonProperty("amount")]
        public double? Amount { get; set; }
    }
}

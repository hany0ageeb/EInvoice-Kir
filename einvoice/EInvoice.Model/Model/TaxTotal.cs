using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
namespace EInvoice.Model
{
    public class TaxTotal
    {
        [Required]
        [JsonProperty("taxType")]
        public string TaxType { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
    }
}

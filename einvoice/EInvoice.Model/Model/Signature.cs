using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class Signature
    {
        [Required]
        [JsonProperty("signatureType")]
        public string SignatureType { get; set; }
        [Required]
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}

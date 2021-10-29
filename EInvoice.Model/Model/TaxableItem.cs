using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class TaxableItem
    {
        [JsonIgnore]
        public int? Id { get; set; } = null;
        [Required]
        [JsonProperty("taxType")]
        public string TaxType { get; set; }
        [Required]
        [JsonProperty("subType")]
        public string SubType { get; set; }
        [JsonProperty("rate")]
        public double Rate { get; set; } = 0;
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [Range(0, 100)]
        [JsonIgnore]
        public int? InvoiceLineId { get; set; } = null;
        [JsonIgnore]
        public byte[] Version { get; set; }
    }
}

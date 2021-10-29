using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class Delivery
    {
        [StringLength(100)]
        [JsonProperty("approach")]
        public string Approach { get; set; }
        [StringLength(100)]
        [JsonProperty("packaging")]
        public string Packaging { get; set; }
        [RegularExpression("^(?:[1-9]\\d{3}-(?:(?:0[1-9]|1[0-2])-(?:0[1-9]|1\\d|2[0-8])|(?:0[13-9]|1[0-2])-(?:29|30)|(?:0[13578]|1[02])-31)|(?:[1-9]\\d(?:0[48]|[2468][048]|[13579][26])|(?:[2468][048]|[13579][26])00)-02-29)T(?:[01]\\d|2[0-3]):[0-5]\\d:[0-5]\\d(Z)?$|^$")]
        [JsonProperty("dateValidity")]
        //[JsonConverter(typeof(JsonNullToEmptyStringConverter))]
        public string DateValidity { get; set; }
        [StringLength(100)]
        [JsonProperty("exportPort")]
        public string ExportPort { get; set; }
        [StringLength(100)]
        [JsonProperty("countryOfOrigin")]
        public string CountryOfOrigin { get; set; }
        [JsonProperty("grossWeight")]
        public double? GrossWeight { get; set; }
        [JsonProperty("netWeight")]
        public double? NetWeight { get; set; }
        [StringLength(500)]
        [JsonProperty("terms")]
        public string Terms { get; set; }
    }
}

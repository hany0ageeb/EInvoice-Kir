using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class ReceiverAddress
    {
        [StringLength(2)]
        [JsonProperty("country")]
        public string Country { get; set; } = "";
        [StringLength(100)]
        [JsonProperty("governate")]
        public string Governate { get; set; }
        [StringLength(100)]
        [JsonProperty("regionCity")]
        public string RegionCity { get; set; }
        [StringLength(200)]
        [JsonProperty("street")]
        public string Street { get; set; }
        [StringLength(100)]
        [JsonProperty("buildingNumber")]
        public string BuildingNumber { get; set; }
        [StringLength(30)]
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        [StringLength(100)]
        [JsonProperty("floor")]
        public string Floor { get; set; }
        [StringLength(100)]
        [JsonProperty("room")]
        public string Room { get; set; }
        [StringLength(500)]
        [JsonProperty("landmark")]
        public string Landmark { get; set; }
        [StringLength(500)]
        [JsonProperty("additionalInformation")]
        public string AdditionalInformation { get; set; }
    }
}

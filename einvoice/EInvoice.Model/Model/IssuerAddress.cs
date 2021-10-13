using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class IssuerAddress
    {
        [JsonProperty("branchId")]
        [StringLength(50)]
        public string BranchId { get; set; }
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("country")]
        [StringLength(2,MinimumLength = 1)]
        public string Country { get; set; }
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("governate")]
        [StringLength(100,MinimumLength = 1)]
        public string Governate { get; set; }
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("regionCity")]
        [StringLength(100, MinimumLength = 1)]
        public string RegionCity { get; set; }
        [Required]
        [JsonProperty("street")]
        [StringLength(200,MinimumLength = 1 )]
        public string Street { get; set; }
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("buildingNumber")]
        [StringLength(100,MinimumLength =1)]
        public string BuildingNumber { get; set; }
        [JsonProperty("postalCode")]
        [StringLength(30)]
        public string PostalCode { get; set; }
        [JsonProperty("floor")]
        [StringLength(100)]
        public string Floor { get; set; }
        [JsonProperty("room")]
        [StringLength(100)]
        public string Room { get; set; }
        [JsonProperty("landMark")]
        [StringLength(500)]
        public string Landmark { get; set; }
        [JsonProperty("additionalInformation")]
        [StringLength(500)]
        public string AdditionalInformation { get; set; }
    }
}

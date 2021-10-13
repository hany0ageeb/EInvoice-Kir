using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class Issuer
    {
        private string _type = "B";
        [Required]
        [JsonProperty("address")]
        public IssuerAddress Address { get; set; }
        [Required]
        [JsonProperty("type",DefaultValueHandling = DefaultValueHandling.Include)]
        public string Type { get => _type; set { _type = value; } }
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("id")]
        [StringLength(30,MinimumLength =1)]
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(200,MinimumLength = 1)]
        [JsonProperty("name")]
        public string Name { get; set; }
        public static bool IsValidIssuerType(string type)
        {
            return type == "B";
        }
        [JsonIgnore]
        public byte[] Version { get; set; }
    }
}

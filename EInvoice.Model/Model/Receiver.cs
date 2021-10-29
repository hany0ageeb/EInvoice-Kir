using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class Receiver
    {
        [JsonIgnore]
        public int? InternalId { get; set; }
        [JsonProperty("address")]
        public ReceiverAddress Address { get; set; }
        [Required]
        [JsonProperty("type")]
        [EnumDataType(typeof(ReceiverType))]
        [JsonConverter(typeof(ReceiverTypeConverter))]
        public ReceiverType Type { get; set; }
        [JsonProperty("id")]
        [StringLength(30)]
        public string Id { get; set; }
        [JsonProperty("name")]
        [StringLength(200)]
        public string Name { get; set; }
        
        [JsonIgnore]
        public byte[] Version { get; set; }
    }
}

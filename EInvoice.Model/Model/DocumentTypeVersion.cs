using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EInvoice.Model
{
    public class DocumentTypeVersion
    {
        [JsonProperty("typeName")]
        public string TypeName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("versionNumber")]
        public string VersionNumber { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("activeFrom")]
        public DateTime ActiveFrom { get; set; }
        [JsonProperty("activeTo")]
        public DateTime? ActiveTo { get; set; }
        [JsonProperty("jsonSchema")]
        public string JsonSchema { get; set; }
        [JsonProperty("xmlSchema")]
        public string XmlSchema { get; set; }
    }
}

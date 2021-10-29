using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace EInvoice.Model
{
    public class TaxSubType
    {
        [Required]
        public string Code { get; set; }
        [JsonProperty("Desc_en")]
        public string EnglishDescription { get; set; }
        [JsonProperty("Desc_ar")]
        public string ArabicDescription { get; set; }
        public string TaxtypeReference { get; set; }
    }
}

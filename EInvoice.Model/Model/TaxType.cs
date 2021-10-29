using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class TaxType
    {
        private static readonly IList<string> taxableTypes = new List<string>() { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12" };
        private static readonly IList<string> nontaxableType = new List<string>() { "T13", "T14", "T15", "T16", "T17", "T18", "T19", "T20" };
        [Required]
        public string Code { get; set; }
        [JsonProperty("Desc_en")]
        public string EnglishDescription { get; set; }
        [JsonProperty("Desc_ar")]
        public string ArabicDescription { get; set; }
        public IList<TaxSubType> SubType { get; set; } = new List<TaxSubType>();

        public static bool IsTaxable(string taxT)
        {
            return taxableTypes.Contains(taxT);
        }
        public static bool IsNonTaxable(string taxT)
        {
            return nontaxableType.Contains(taxT);
        }
    }
}

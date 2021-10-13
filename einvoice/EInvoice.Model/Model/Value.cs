using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class Value
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(3,MinimumLength = 1)]
        [JsonProperty("currencySold")]
        public string CurrencySold { get; set; }
        [JsonProperty("amountSold")]
        public double? AmountSold { get; set; } = 0;
        [Range(0, 99999)]
        [JsonProperty("currencyExchangeRate")]
        public double? CurrencyExchangeRate { get; set; }
        [Range(typeof(decimal),"0", "999999999999")]
        [JsonProperty("amountEGP")]
        public double AmountEGP { get; set; }
    }
}

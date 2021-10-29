using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class ActivityType
    {
        [Required]
        public string Code { get; set; }
        [JsonProperty("Desc_en")]
        public string EnglishDescription { get; set; }
        [JsonProperty("Desc_ar")]
        public string ArabicDescription { get; set; }
    }
}

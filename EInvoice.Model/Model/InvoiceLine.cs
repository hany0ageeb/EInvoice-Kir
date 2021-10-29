using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class InvoiceLine
    {
        private int? _id = null;
        [JsonIgnore]
        public int? Id 
        { 
            get => _id; 
            set 
            {
                _id = value;
                foreach (TaxableItem taxableItem in TaxableItems)
                    taxableItem.InvoiceLineId = _id;
            } 
        }
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("description")]
        [StringLength(500,MinimumLength =1)]
        public string Description { get; set; }
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("itemType")]
        [StringLength(30,MinimumLength =1)]
        public string ItemType { get; set; }
        [Required]
        [JsonProperty("itemCode")]
        [StringLength(100,MinimumLength =1)]
        public string ItemCode { get; set; }
        [JsonProperty("internalCode")]
        public string InternalCode { get; set; }
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("unitType")]
        [StringLength(30,MinimumLength = 1)]
        public string UnitType { get; set; }
        [Range(0,double.MaxValue)]
        [JsonProperty("quantity")]
        public double Quantity { get; set; }
        [Required]
        [JsonProperty("unitValue")]
        public Value UnitValue { get; set; }
        [JsonProperty("salesTotal")]
        public double SalesTotal { get; set; }
        [JsonProperty("valueDifference")]
        public double ValueDifference { get; set; }
        [JsonProperty("totalTaxableFees")]
        public double TotalTaxableFees { get; set; }
        [JsonProperty("discount")]
        public Discount Discount { get; set; }
        [JsonProperty("netTotal")]
        public double NetTotal { get; set; }
        [JsonProperty("itemsDiscount")]
        public double ItemsDiscount { get; set; }
        [JsonProperty("taxableItems")]
        public IList<TaxableItem> TaxableItems { get; set; } = new List<TaxableItem>();
        [JsonProperty("total")]
        public double Total { get; set; }
        [JsonIgnore]
        public byte[] Version { get; set; }
        [JsonIgnore]
        public int? DocumentId { get; set; }
        public IList<TaxableItem> FindTaxableItems(string taxType)
        {
            return (from tt in TaxableItems where tt.TaxType == taxType select tt).ToList();
        }
        public double CalcualteTaxableFees()
        {
            return (from ti in TaxableItems where TaxType.IsTaxable(ti.TaxType) select ti.Amount).Sum();
        }
    }
}

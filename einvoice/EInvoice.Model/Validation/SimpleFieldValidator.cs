using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;
namespace EInvoice.Validation
{
    public class SimpleFieldValidator : IValidator<Document>
    {
        private int _invoiceSubmissionInHours;
        public SimpleFieldValidator(int invoiceSubmissionInHours)
        {
            _invoiceSubmissionInHours = invoiceSubmissionInHours;
        }
        public ValidationResult IsValid(Document document)
        {
           var result = new ValidationResult() 
            { 
                ValidationState = ValidationState.Valid 
            };
            if (DateTime.Now.AddHours( -1 * _invoiceSubmissionInHours) > document.DateTimeIssued)
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error() { Code = "", Message = "Invalid Issue Date." });
            }
            if (document.InvoiceLines.Count <= 0)
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error() { Code = "", Message = "Invalid Document #At least one invoice line.#", Target = "document/InvoiceLines" });
            }
            //Validation on document level.
            //1.Total Sales Amount = Sum of all invoice line sales total.
            if (document.TotalSalesAmount != Math.Round(document.InvoiceLines.Sum(line => line.SalesTotal),5))
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error() { Code="",Message="Invalid Total Sales Amount.",Target= "document/TotalSalesAmount" });
            }
            //2. Total item discount = sum of all invoiceline.discount.amount
            if (document.TotalDiscountAmount != Math.Round(document.InvoiceLines.Sum(line => line.Discount.Amount)??0,5))
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error() { Code = "", Message = "Invalid Total Items Discount Amount.\nInvoice Total Discount Amount not equal to the sum of all invoice lines discount amount", Target = "document/TotalDiscountAmount" });
            }
            //3. Net Amount  = sum of all lines net total
            if (document.NetAmount != Math.Round(document.InvoiceLines.Sum(line => line.NetTotal),5))
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error() { Code = "", Message = "Invalid Net Amount.", Target = "document/NetAmount" });
            }
            //4. totalItemsDiscountAmount = sum of all items discount amount elements
            if (document.TotalItemsDiscountAmount != Math.Round(document.InvoiceLines.Sum(line => line.ItemsDiscount),5))
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error() { Code = "", Message = "Invalid Total Items Discount Amount.", Target = "document/TotalItemsDiscountAmount" });
            }
            //5. TotalAmount = sum of all lines total - extra discount amount.
            var VAL = Math.Round(document.InvoiceLines.Sum(line => line.Total) - document.ExtraDiscountAmount, 5);
            if (Math.Round(document.TotalAmount,5) != Math.Round(document.InvoiceLines.Sum(line => line.Total) - document.ExtraDiscountAmount,5))
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error() { Code = "", Message = "Invalid Total Amount.\n Invoice Total Amount not equal to the sum of all invoice lines total." ,Target="document/TotalAmount"});
            }
            //7.8.
            foreach(TaxTotal taxTotal in document.TaxTotals)
            {
                double amount = 0;
                foreach(InvoiceLine line in document.InvoiceLines)
                {
                    var all = line.FindTaxableItems(taxTotal.TaxType);
                    amount += all.Sum(ti => ti.Amount);
                }
                if (Math.Round(taxTotal.Amount,5) != Math.Round(amount,5))
                {
                    result.ValidationState = ValidationState.Invalid;
                    result.Errors.Add(new Error() { Code="",Message=$"Invalid Tax Total.\n Tax Total Amount For {taxTotal.TaxType} is not equal to the sum of all invoice lines taxable items amount.",Target="document/TaxTotals/Amount"});
                }
            }
            //validation on invoiceline level.
            foreach(InvoiceLine line in document.InvoiceLines)
            {
                if (string.IsNullOrEmpty(line.ItemCode))
                {
                    result.ValidationState = ValidationState.Invalid;
                    result.Errors.Add(new Error() { Code = "", Message = $"Invalid GS1 Code. For Line {line.InternalCode}" });
                }
                //1.Invoice Line Sales Total is equal to multiplication of qyantity by amount.
                if (line.SalesTotal != Math.Round(line.Quantity * line.UnitValue.AmountEGP,5) && !line.InternalCode.Contains("VAT"))
                {
                    result.ValidationState = ValidationState.Invalid;
                    result.Errors.Add(new Error("",$"Invalid Sales Total. # Item Internal Code {line.InternalCode}",null));
                }
                //2.Discount Amount = Discount Rate * Invoice Line Sales Total. discount rate is not zero.
                if(line.Discount.Rate!=0 && line.Discount.Amount != Math.Round((line.Discount.Rate / 100) * line.SalesTotal??0,5))
                {
                    result.ValidationState = ValidationState.Invalid;
                    result.Errors.Add(new Error() { Code = "",Message=$"Invalid Discount Rate. Item Internal Code {line.InternalCode}",Target="Document/InvoiceLine/Discount/Rate"});
                }
                //3.Net Total = Sales Total - Discount Amount (net total equal sales total minus discount amount.)
                if (Math.Round(line.NetTotal,5) != Math.Round(line.SalesTotal - line.Discount?.Amount??0,5))
                {
                    result.ValidationState = ValidationState.Invalid;
                    result.Errors.Add(new Error() { Code="",Message=$"Invalid Net Total.For Item {line.InternalCode}",Target="Document/InvliceLine/NetTotal"});
                }
                //4.1 taxable item amount = taxable item rate * invoice line net total
                foreach(TaxableItem taxableItem in line.TaxableItems)
                {
                    if(taxableItem.Rate!=0 && line.NetTotal!=0 && taxableItem.Amount != Math.Round((taxableItem.Rate / 100) * line.NetTotal,5))
                    {
                        result.ValidationState = ValidationState.Invalid;
                        result.Errors.Add(new Error() { Code = "" ,Message = "Invalid Taxable Item Amount."});
                    }
                    //6.Table tax percentage T2
                    if(taxableItem.TaxType == "T2")
                    {
                        double t3Amount = (from tt in line.TaxableItems where tt.TaxType == "T3" select tt.Amount).Sum();
                        if(taxableItem.Amount != (line.NetTotal + line.TotalTaxableFees + line.ValueDifference + t3Amount) * (taxableItem.Rate / 100))
                        {
                            result.ValidationState = ValidationState.Invalid;
                            result.Errors.Add(new Error() { Code = "", Message = $"Invalid Table Tax (Percentage). At Line {line.InternalCode}" });
                        }
                    }
                    //7. Table Tax Fixed Amount T3 Rate should be equal to zero. 8.Stamping Tax
                    if((taxableItem.TaxType == "T3" || taxableItem.TaxType == "T6") && taxableItem.Rate != 0)
                    {
                        result.ValidationState = ValidationState.Invalid;
                        result.Errors.Add(new Error() { Code = "", Message = "Invalid TaxableItem Rate." });
                    }
                    //9.Value Added Service VAT T1 unclear for me.
                    if(taxableItem.TaxType == "T1")
                    {
                        if(line.NetTotal!=0 && taxableItem.Amount!= Math.Round((line.NetTotal+line.TotalTaxableFees+line.ValueDifference) * (taxableItem.Rate/100),5))
                        {
                            result.ValidationState = ValidationState.Invalid;
                            result.Errors.Add(new Error() { Code = "", Message = "Invalid Value Added Tax." });
                        }
                    }
                    //10.With holding Tax T4
                    if(taxableItem.TaxType == "T4" && taxableItem.Amount != Math.Round((taxableItem.Rate/100) * (line.NetTotal - line.ItemsDiscount),5))
                    {
                        result.ValidationState = ValidationState.Invalid;
                        result.Errors.Add(new Error() { Code = "", Message = "Invalid With Holding Tax." });
                    }
                }
                //11.Line Total = line net total + taxableitem.amount+total taxable fees + ta
                var T4Amount = (from tt in line.TaxableItems where tt.TaxType == "T4" select tt.Amount).Sum();
                var TaxableItemsAmount = (from tt in line.TaxableItems where tt.TaxType != "T4" select tt.Amount).Sum();
                if (Math.Round(line.Total,5) != Math.Round(line.NetTotal + line.TotalTaxableFees + TaxableItemsAmount - T4Amount,5))
                {
                    result.ValidationState = ValidationState.Invalid;
                    result.Errors.Add(new Error() { Code = "", Message = "Invalid Line Total." });
                }
            }
            return result;
        }
    }
}

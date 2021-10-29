using EInvoice.Model;
using System.Collections.Generic;
using System.Data.Common;
namespace EInvoice.DAL.DAO
{
    public interface ITaxableItemDao : IEntityDao<TaxableItem>
    {
        void SaveOrUpdate(TaxableItem taxableItem,DbTransaction transaction);
        IList<TaxableItem> FindByInvoiceLineId(int? invoiceLineId);
        void Insert(TaxableItem taxableItem, DbTransaction transaction);
    }
}

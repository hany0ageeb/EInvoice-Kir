using EInvoice.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
namespace EInvoice.DAL.DAO
{
    public interface IInvoiceLineDao : IEntityDao<InvoiceLine>
    {
        void Insert(InvoiceLine invoiceLine, DbTransaction transaction);
        void SaveOrUpdate(InvoiceLine invoiceLine,DbTransaction transaction);
        IList<InvoiceLine> FindByDocumentId(int? documentId);
    }
}

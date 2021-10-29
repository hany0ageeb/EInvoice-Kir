using System.Collections.Generic;

using EInvoice.Model;

namespace EInvoice.DAL.DAO
{
    public interface ITaxTypeDao : IEntityDao<TaxType>
    {
        IList<TaxType> Find();
        void AddRange(IList<TaxType> taxTypes);
    }
}

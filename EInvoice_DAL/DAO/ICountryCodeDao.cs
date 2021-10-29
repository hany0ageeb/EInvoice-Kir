using System.Collections.Generic;

using EInvoice.Model;

namespace EInvoice.DAL.DAO
{
    public interface ICountryCodeDao : IEntityDao<CountryCode>
    {
        IList<CountryCode> Find();
        void AddRange(IList<CountryCode> countryCodes);
    }
}

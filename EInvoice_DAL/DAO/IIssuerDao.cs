using EInvoice.Model;
using System;
using System.Collections.Generic;

namespace EInvoice.DAL.DAO
{
    public interface IIssuerDao : IEntityDao<Issuer>
    {
        Issuer Find(string id);
        IList<Issuer> Find();        
    }
}

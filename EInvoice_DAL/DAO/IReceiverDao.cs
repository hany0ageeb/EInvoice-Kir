using EInvoice.Model;
using System.Collections.Generic;

namespace EInvoice.DAL.DAO
{
    public interface IReceiverDao : IEntityDao<Receiver>
    {
        int? FindReceiverId(string name);
        IList<Receiver> Find();
        IList<Receiver> Find(Issuer issuer);
        
    }
}

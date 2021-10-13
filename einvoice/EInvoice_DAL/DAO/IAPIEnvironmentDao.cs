using System.Collections.Generic;
using EInvoice.Model;
namespace EInvoice.DAL.DAO
{
    public interface IAPIEnvironmentDao : IEntityDao<APIEnvironment>
    {
        IList<APIEnvironment> Find();
    }
}

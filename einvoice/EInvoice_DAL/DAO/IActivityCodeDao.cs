using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using EInvoice.Model;

namespace EInvoice.DAL.DAO
{
    public interface IActivityCodeDao : IEntityDao<ActivityType>
    {
        IList<ActivityType> Find();
        ActivityType Find(string code);
        void AddRange(IList<ActivityType> activities);
    }
}

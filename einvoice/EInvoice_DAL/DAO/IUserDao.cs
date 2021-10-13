using EInvoice.Model;

namespace EInvoice.DAL.DAO
{
    public interface IUserDao : IEntityDao<User>
    {
        User Find(string userName, string password);
    }
}

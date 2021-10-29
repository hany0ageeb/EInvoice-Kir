using System;
namespace EInvoice.DAL.DAO
{
    public interface IEntityDao<T>
    {
        void Insert(T entity);
    }
}

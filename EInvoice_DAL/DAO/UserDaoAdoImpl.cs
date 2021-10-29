using EInvoice.Model;
using System;
using System.Data.Common;

namespace EInvoice.DAL.DAO
{
    public class UserDaoAdoImpl : IUserDao
    {
        private readonly DbConnection _connection;
        private readonly IIssuerDao _issuerDao;
        public UserDaoAdoImpl(DbConnection conn,IIssuerDao issuerDao)
        {
            _connection = conn;
            _issuerDao = issuerDao;
        }
        public void Insert(User user)
        {
            throw new NotImplementedException();
        }
        public User Find(string userName,string password)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetUserByName_Password]", System.Data.CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@Name", parameterValue: userName));
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@Password",parameterValue:password));
            selectCommand.Connection = _connection;
            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = selectCommand.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                User user = new User
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    Version = reader.GetFieldValue<byte[]>(reader.GetOrdinal("VerCol")),
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Issuer = _issuerDao.Find(reader.GetString(reader.GetOrdinal("TaxPayerId")))
                };
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}

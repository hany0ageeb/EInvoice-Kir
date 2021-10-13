using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using EInvoice.Model;
namespace EInvoice.DAL.DAO
{
    public class APIEnvironmentDaoAdoImpl : IAPIEnvironmentDao
    {
        private readonly DbConnection _connection;
        public APIEnvironmentDaoAdoImpl(DbConnection connection)
        {
            _connection = connection;
        }
        public IList<APIEnvironment> Find()
        {
            IList<APIEnvironment> envs = new List<APIEnvironment>();
            DbCommand command = _connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[GetAllAPIEnvironments]";
            command.Connection = _connection;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                APIEnvironment environment = new APIEnvironment()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    BaseUri = new Uri(reader.GetString(reader.GetOrdinal("BaseUri"))),
                    LogInUri = new Uri(reader.GetString(reader.GetOrdinal("LogInUri"))),
                    Version = reader.GetFieldValue<byte[]>(reader.GetOrdinal("VerCol"))
                    
                };
                envs.Add(environment);
            }
            reader.Close();
            _connection.Close();
            return envs;
        }

        public void Insert(APIEnvironment entity)
        {
            throw new NotImplementedException();
        }
    }
}

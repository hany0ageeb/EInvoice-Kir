using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;
using System.Data;
using System.Data.Common;

namespace EInvoice.DAL.DAO
{
    public interface IIssuerAPIAccessDetailsDao : IEntityDao<IssuerAPIAccessDetails>
    {
        IssuerAPIAccessDetails Find(APIEnvironment env,Issuer issuer);
    }
    public class IssuerAPIAccessDetailsDaoAdoImpl : IIssuerAPIAccessDetailsDao
    {
        private readonly DbConnection _connection;
        public IssuerAPIAccessDetailsDaoAdoImpl(DbConnection connection)
        {
            _connection = connection;
        }
        public void Insert(IssuerAPIAccessDetails entity)
        {
            throw new NotImplementedException();
        }
        public IssuerAPIAccessDetails Find(APIEnvironment env, Issuer issuer)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetAccessDetailsByIssuerId_APIId]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@TaxPayerId", parameterValue: issuer?.Id??""));
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@APIEnvId", parameterValue: env?.Id, dbType: DbType.Int32));
            selectCommand.Connection = _connection;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = selectCommand.ExecuteReader();
            IssuerAPIAccessDetails result = null;
            if (reader.Read())
            {
                Dictionary<string, string> propertyColumnMappings = new Dictionary<string, string>()
                {
                    { "ClientId","ClientId" },
                    {"ClientSecret","ClientSecret" },
                    {"SecurityToken","SecurityToken" }
                };
                result = reader.ReadIssuerAPIAccessDetails(propertyColumnMappings);
            }
            reader.Close();
            _connection.Close();
            return result;
        }
    }
}

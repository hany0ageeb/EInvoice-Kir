using EInvoice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace EInvoice.DAL.DAO
{
    public interface IReportDefinitionDao : IEntityDao<ReportDefinition>
    {
        IList<ReportDefinition> Find();
    }
    public class ReportDefinitionDao : IReportDefinitionDao
    {
        private DbConnection _connection;

        public ReportDefinitionDao(DbConnection connection)
        {
            _connection = connection;
        }
        public void Insert(ReportDefinition entity)
        {
            throw new NotImplementedException();
        }
        public IList<ReportDefinition> Find()
        {
            DbCommand selectCommand = _connection.CreateCommand();
            selectCommand.CommandText = "SELECT [Id],[Name],[Description] from dbo.Report";
            selectCommand.CommandType = CommandType.Text;
            selectCommand.Connection = _connection;
            IList<ReportDefinition> reports = new List<ReportDefinition>();
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
                DbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    ReportDefinition report = new ReportDefinition()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"] as string,
                        Description = reader["Description"] as string
                    };
                    reports.Add(report);
                }
                return reports;
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();
            }
        }
    }
}

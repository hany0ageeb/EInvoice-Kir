using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;

using EInvoice.Model;

namespace EInvoice.DAL.DAO
{
    public class ActivityCodeDaoAdoImpl : IActivityCodeDao
    {
        private DbConnection _connection;
        public ActivityCodeDaoAdoImpl(DbConnection conn)
        {
            _connection = conn;
        }
        public ActivityType Find(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                DbCommand selectCommand = _connection.CreateCommand();
                selectCommand.CommandText = "select [Code],[EnglishDescription],[ArabicDescription] from [dbo].[ActivityCode] where [Code] = '"+code+"'";
                DbDataAdapter adapter = DbProviderFactories.GetFactory(_connection).CreateDataAdapter();
                adapter.SelectCommand = selectCommand;
                DataTable table = new DataTable("ActivityCode");
                adapter.Fill(table);
                if (table.Rows.Count == 1)
                {
                    var data = table.AsEnumerable();
                    foreach(var itm in data)
                    {
                        return new ActivityType()
                        {
                            Code = itm.Field<string>("Code"),
                            ArabicDescription = itm.Field<string>("ArabicDescription"),
                            EnglishDescription = itm.Field<string>("EnglishDescription")
                        };
                    }
                }
                return null;

            }
            else
            {
                return null;
            }
        }
        public void AddRange(IList<ActivityType> activities)
        {
            DbCommand selectcommand = _connection.CreateCommand("[dbo].[GetAllActivityCodes]", CommandType.StoredProcedure);
            DbCommand insertCommand = _connection.CreateCommand("[dbo].[InsertActivityCode]", CommandType.StoredProcedure);
            //to do
            DbParameter para = insertCommand.CreateParameter();
            para.ParameterName = "@Code";
            para.DbType = DbType.String;
            para.SourceColumn = "Code";
            insertCommand.Parameters.Add(para);
            para = insertCommand.CreateParameter();
            para.ParameterName = "@EnglishDescription";
            para.DbType = DbType.String;
            para.SourceColumn = "EnglishDescription";
            insertCommand.Parameters.Add(para);
            para = insertCommand.CreateParameter();
            para.ParameterName = "@ArabicDescription";
            para.DbType = DbType.String;
            para.SourceColumn = "ArabicDescription";
            insertCommand.Parameters.Add(para);
            DbDataAdapter adapter = DbProviderFactories.GetFactory(_connection).CreateDataAdapter();
            adapter.SelectCommand = selectcommand;
            adapter.InsertCommand = insertCommand;
            DataTable table = new DataTable("ActivityCode");
            table.Columns.Add(new DataColumn("Code", typeof(string)));
            table.Columns.Add(new DataColumn("EnglishDescription", typeof(string)));
            table.Columns.Add(new DataColumn("ArabicDescription", typeof(string)));
            adapter.Fill(table);
            if(table.PrimaryKey==null || table.PrimaryKey.Length == 0)
            {
                table.PrimaryKey = new DataColumn[1] { table.Columns["Code"]};
            }
            foreach(ActivityType activityType in activities)
            {
                DataRow row = table.Rows.Find(activityType.Code);
                if (row == null)
                {
                    var newRow = table.NewRow();
                    newRow["Code"] = activityType.Code;
                    newRow["EnglishDescription"] = activityType.EnglishDescription;
                    newRow["ArabicDescription"] = activityType.ArabicDescription;
                    table.Rows.Add(newRow);
                }
                else
                {
                    //to do
                }
            }
            adapter.Update(table);
        }
        public void Insert(ActivityType entity)
        {
            DbCommand insertCommand = _connection.CreateCommand("[dbo].[InsertActivityCode]", CommandType.StoredProcedure);
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@Code", parameterValue: entity.Code));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@EnglishDescription", parameterValue: entity.EnglishDescription));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@ArabicDescription", parameterValue: entity.ArabicDescription));
            insertCommand.Connection = _connection;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            insertCommand.ExecuteNonQuery();
            _connection.Close();
        }
        public IList<ActivityType> Find()
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetAllActivityCodes]", CommandType.StoredProcedure);
            selectCommand.Connection = _connection;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = selectCommand.ExecuteReader();
            IList<ActivityType> result = new List<ActivityType>();
            Dictionary<string, string> mapping = new Dictionary<string, string>()
            {
                {"Code","Code" },
                {"EnglishDescription","EnglishDescription" },
                {"ArabicDescription","ArabicDescription" }
            };
            while (reader.Read())
            {
                result.Add(reader.ReadActivityType(mapping));
            }
            reader.Close();
            _connection.Close();
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using EInvoice.Model;

namespace EInvoice.DAL.DAO
{
    public class CountryCodeDaoAdoImpl : ICountryCodeDao
    {
        private readonly DbConnection _connection;
        public CountryCodeDaoAdoImpl(DbConnection conn)
        {
            _connection = conn;
        }
        public void AddRange(IList<CountryCode> countryCodes)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetAllCountryCodes]", CommandType.StoredProcedure);
            DbCommand insertCommand = _connection.CreateCommand("[dbo].[InsertCountryCode]", CommandType.StoredProcedure);
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@Code", dbType: DbType.String));
            insertCommand.Parameters["@Code"].SourceColumn = "Code";
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@ArabicDescription", dbType: DbType.String));
            insertCommand.Parameters["@ArabicDescription"].SourceColumn = "ArabicDescription";
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@EnglishDescription", dbType: DbType.String));
            insertCommand.Parameters["@EnglishDescription"].SourceColumn = "EnglishDescription";
            DbDataAdapter adapter = DbProviderFactories.GetFactory(_connection).CreateDataAdapter();
            adapter.SelectCommand = selectCommand;
            adapter.InsertCommand = insertCommand;
            DataTable table = new DataTable("CountryCode");
            adapter.Fill(table);
            if (table.PrimaryKey == null || table.PrimaryKey.Length == 0)
            {
                table.PrimaryKey = new DataColumn[1] { table.Columns["Code"] };
            }
            
            foreach(var countryCode in countryCodes)
            {
                DataRow row = table.Rows.Find(countryCode.Code);
                if (row == null)
                {
                    DataRow newRow = table.NewRow();
                    newRow.SetField<string>("Code", countryCode.Code);
                    newRow.SetField<string>("ArabicDescription", countryCode.ArabicDescription);
                    newRow.SetField<string>("EnglishDescription", countryCode.EnglishDescription);
                    table.Rows.Add(newRow);
                   
                }
                else
                {

                }
            }
            adapter.Update(table);
        }
        public void Insert(CountryCode entity)
        {
            throw new NotImplementedException();
        }
        public IList<CountryCode> Find()
        {
            
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetAllCountryCodes]", CommandType.StoredProcedure);
            selectCommand.Connection = _connection;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = selectCommand.ExecuteReader();
            IList<CountryCode> result = new List<CountryCode>();
            Dictionary<string, string> mapping = new Dictionary<string, string>()
            {
                {"Code","Code" },
                {"EnglishDescription","EnglishDescription" },
                {"ArabicDescription","ArabicDescription" }
            };
            while (reader.Read())
            {
                result.Add(reader.ReadCountryCode(mapping));
            }
            reader.Close();
            _connection.Close();
            return result;
        }
    }
}

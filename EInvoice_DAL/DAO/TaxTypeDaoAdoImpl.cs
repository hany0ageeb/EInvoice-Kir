using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;

using EInvoice.Model;

namespace EInvoice.DAL.DAO
{
    public class TaxTypeDaoAdoImpl : ITaxTypeDao
    {
        private readonly DbConnection _connection;
        public TaxTypeDaoAdoImpl(DbConnection conn)
        {
            _connection = conn;
        }
        public void Insert(TaxType entity)
        {
            throw new NotImplementedException();
        }
        public IList<TaxType> Find()
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetAllTaxTypes]", CommandType.StoredProcedure);
            selectCommand.Connection = _connection;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = selectCommand.ExecuteReader();
            IList<TaxType> result = new List<TaxType>();
            IList<TaxSubType> taxSubTypes = new List<TaxSubType>();
            Dictionary<string, string> mapping = new Dictionary<string, string>()
            {
                {"Code","TaxTypeCode" },
                {"EnglishDescription","TaxTypeEngDesc" },
                {"ArabicDescription","TaxTypeAraDesc" }
            };
            while (reader.Read())
            {
                result.Add(reader.ReadTaxType(mapping));
            }
            if (reader.NextResult())
            {
                mapping = new Dictionary<string, string>()
                {
                    {"Code","SubtypeCode" },
                    {"EnglishDescription","SubtypeEngDesc" },
                    {"ArabicDescription","SubtypeAraDesc" }
                };
                while (reader.Read())
                {
                    var subtype = reader.ReadTaxSubType(mapping);
                    (from tt in result where tt.Code == (reader["TaxTypeCode"] as string) select tt).First().SubType.Add(subtype);
                }
            }
            reader.Close();
            _connection.Close();
            return result;
        }
        public void AddRange(IList<TaxType> taxTypes)
        {

            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetAllTaxTypes]", CommandType.StoredProcedure);
            DbDataAdapter adapter = DbProviderFactories.GetFactory(_connection).CreateDataAdapter();
            adapter.SelectCommand = selectCommand;
            DbCommand insertCommand = _connection.CreateCommand();
            insertCommand.CommandText = "[dbo].[InsertTaxType]";
            insertCommand.CommandType = CommandType.StoredProcedure;

            DbParameter para = insertCommand.CreateParameter();
            para.DbType = DbType.String;
            para.ParameterName = "@Code";
            para.SourceColumn = "TaxTypeCode";
            insertCommand.Parameters.Add(para);

            para = insertCommand.CreateParameter();
            para.DbType = DbType.String;
            para.ParameterName = "@ArabicDescription";
            para.SourceColumn = "TaxTypeAraDesc";
            insertCommand.Parameters.Add(para);

            para = insertCommand.CreateParameter();
            para.DbType = DbType.String;
            para.ParameterName = "@EnglishDesciption";
            para.SourceColumn = "TaxTypeEngDesc";
            insertCommand.Parameters.Add(para);

            para = insertCommand.CreateParameter();
            para.DbType = DbType.String;
            para.ParameterName = "@SubTypeCode";
            para.SourceColumn = "SubtypeCode";
            insertCommand.Parameters.Add(para);

            para = insertCommand.CreateParameter();
            para.DbType = DbType.String;
            para.ParameterName = "@SubTypeEnglishDescription";
            para.SourceColumn = "SubtypeEngDesc";
            insertCommand.Parameters.Add(para);

            para = insertCommand.CreateParameter();
            para.DbType = DbType.String;
            para.ParameterName = "@SubTypeArabicDescription";
            para.SourceColumn = "SubtypeAraDesc";
            insertCommand.Parameters.Add(para);

            adapter.InsertCommand = insertCommand;
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            if (dataSet.Tables[0].PrimaryKey == null || dataSet.Tables[0].PrimaryKey.Length == 0)
            {
                dataSet.Tables[0].PrimaryKey = new DataColumn[1] { dataSet.Tables[0].Columns["TaxTypeCode"] };
            }
            foreach (TaxType taxType in taxTypes)
            {
                DataRow row = dataSet.Tables[0].Rows.Find(taxType.Code);
                if (row == null)
                {
                    DataRow row1 = dataSet.Tables[0].NewRow();
                    row1.SetField<string>("TaxTypeCode", taxType.Code);
                    row1.SetField<string>("TaxTypeAraDesc", taxType.ArabicDescription);
                    row1.SetField<string>("TaxTypeEngDesc", taxType.EnglishDescription);
                    dataSet.Tables[0].Rows.Add(row1);
                    foreach(TaxSubType taxSubType in taxType.SubType)
                    {
                        DataRow row2 = dataSet.Tables[1].NewRow();
                        row2.SetField<string>("SubtypeCode", taxSubType.Code);
                        row2.SetField<string>("SubtypeAraDesc", taxSubType.ArabicDescription);
                        row2.SetField<string>("SubtypeEngDesc", taxSubType.EnglishDescription);
                        row2.SetField<string>("TaxTypeCode", taxType.Code);
                        dataSet.Tables[1].Rows.Add(row2);
                    }
                }
            }
            Console.WriteLine();
            adapter.Update(dataSet);
        }
    }
}

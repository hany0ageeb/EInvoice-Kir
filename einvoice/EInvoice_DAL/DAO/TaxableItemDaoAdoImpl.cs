using System.Data.Common;
using System.Data;
using EInvoice.Model;
using System.Collections.Generic;

namespace EInvoice.DAL.DAO
{
    public class TaxableItemDaoAdoImpl : ITaxableItemDao
    {
        private readonly DbConnection _connection;
        public TaxableItemDaoAdoImpl(DbConnection conn)
        {
            _connection = conn;
        }

        public IList<TaxableItem> FindByInvoiceLineId(int? invoiceLineId)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetTaxableItemsByInvoiceLineId]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@InvoiceLineId", parameterValue: invoiceLineId, dbType: DbType.Int32));
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            IList<TaxableItem> taxableItems = new List<TaxableItem>();
            DbDataReader reader = selectCommand.ExecuteReader();
            Dictionary<string, string> propertyColumnMappings = new Dictionary<string, string>()
            {
                { "TaxableItem.Amount","Amount" },
                { "TaxableItem.Id","Id" },
                { "TaxableItem.InvoiceLineId","InvoiceLineId" },
                { "TaxableItem.Rate","Rate" },
                { "TaxableItem.SubType","SubType"},
                { "TaxableItem.TaxType","TaxType"},
                { "TaxableItem.Version","VerCol"}
            };
            while (reader.Read())
            {
                taxableItems.Add(reader.ReadTaxableItem(propertyColumnMappings));
            }
            reader.Close();
            return taxableItems;
        }

        public void Insert(TaxableItem taxableItem,DbTransaction transaction)
        {
            DbCommand insertCommand = CreateInsertCommand(taxableItem);
            insertCommand.Transaction = transaction;
            insertCommand.ExecuteNonQuery();
            taxableItem.Id = insertCommand.Parameters["@Id"].Value as int?;
            taxableItem.Version = insertCommand.Parameters["@VerCol"].Value as byte[];
        }

        public void Insert(TaxableItem entity)
        {
            throw new System.NotImplementedException();
        }

        public void SaveOrUpdate(TaxableItem taxableItem,DbTransaction transaction)
        {
            DbCommand command = _connection.CreateCommand("[dbo].[SaveOrUpdateTaxableItem]", CommandType.StoredProcedure);
            command.Transaction = transaction;
            command.Parameters.Add(command.CreateParameter(parameterName: "@OldId",parameterValue:taxableItem.Id,dbType:DbType.Int32));
            command.Parameters.Add(command.CreateParameter(parameterName: "@TaxType", parameterValue: taxableItem.TaxType, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@SubType", parameterValue: taxableItem.SubType, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Amount", parameterValue: taxableItem.Amount, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Rate", parameterValue: taxableItem.Rate, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@InvoiceLineId", parameterValue: taxableItem.InvoiceLineId, dbType: DbType.Int32));
            command.Parameters.Add(command.CreateParameter(parameterName: "@OldVerCol", parameterValue: taxableItem.Version, dbType: DbType.Binary));
            command.Parameters["@OldVerCol"].Size = 8;
            command.Parameters.Add(command.CreateParameter(parameterName: "@NewVerCol", direction:ParameterDirection.Output, dbType: DbType.Binary));
            command.Parameters["@NewVerCol"].Size = 8;
            command.Parameters.Add(command.CreateParameter(parameterName: "@NewId", direction: ParameterDirection.Output, dbType: DbType.Int32));
            command.Connection = _connection;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            if (command.ExecuteNonQuery() > 0)
            {
                taxableItem.Id = command.Parameters["@NewId"].Value as int?;
                taxableItem.Version = command.Parameters["@NewVerCol"].Value as byte[];
            }
        }

        private DbCommand CreateInsertCommand(TaxableItem taxableItem)
        {
            DbCommand insertCommand = _connection.CreateCommand("[dbo].[InsertTaxableItem]", CommandType.StoredProcedure);
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName:"@Amount",parameterValue:taxableItem.Amount,dbType:DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@InvoiceLineId", parameterValue: taxableItem.InvoiceLineId, dbType: DbType.Int32));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@Rate", parameterValue: taxableItem.Rate, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@SubType", parameterValue: taxableItem.SubType, dbType: DbType.String));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@TaxType", parameterValue: taxableItem.TaxType, dbType: DbType.String));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@Id", dbType: DbType.Int32,direction:ParameterDirection.Output));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@VerCol", dbType: DbType.Binary, direction: ParameterDirection.Output));
            insertCommand.Parameters["@VerCol"].Size = 8;
            return insertCommand;
        }
    }
}

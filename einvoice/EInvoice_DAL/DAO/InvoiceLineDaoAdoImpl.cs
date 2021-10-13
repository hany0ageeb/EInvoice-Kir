using System.Data.Common;
using System.Data;
using EInvoice.Model;
using System.Collections.Generic;

namespace EInvoice.DAL.DAO
{
    public class InvoiceLineDaoAdoImpl : IInvoiceLineDao
    {
        private readonly DbConnection _connection;
        private readonly ITaxableItemDao _taxableItemDao;
        public InvoiceLineDaoAdoImpl(DbConnection connection,ITaxableItemDao taxableItemDao)
        {
            _connection = connection;
            _taxableItemDao = taxableItemDao;
        }

        public IList<InvoiceLine> FindByDocumentId(int? documentId)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetInvoiceLinesByDocumentId]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@documentId", parameterValue: documentId, dbType: DbType.Int32));
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = selectCommand.ExecuteReader();
            Dictionary<string, string> propertyColumnMappings = new Dictionary<string, string>()
            {
                { "Value.AmountEGP","AmountEGP" },
                { "Value.AmountSold","AmountSold" },
                { "Value.CurrencyExchangeRate","CurrencyExchangeRate" },
                { "Value.CurrencySold","CurrencySold"},
                { "InvoiceLine.Description","Description"},
                { "Discount.Amount","DiscountAmount"},
                { "Discount.Rate","DiscountRate"},
                { "InvoiceLine.DocumentId","DocumentId"},
                { "InvoiceLine.Id","Id"},
                { "InvoiceLine.InternalCode","InternalCode"},
                { "InvoiceLine.ItemCode","ItemCode"},
                { "InvoiceLine.ItemsDiscount","ItemsDiscount"},
                { "InvoiceLien.ItemType","ItemType" },
                { "InvoiceLine.NetTotal","NetTotal" },
                { "InvoiceLine.Quantity","Quantity" },
                { "InvoiceLine.SalesTotal","SalesTotal" },
                { "InvoiceLine.Total","Total" },
                { "InvoiceLine.UnitType","UnitType" },
                { "InvoiceLine.ValueDifference","ValueDifference" },
                { "InvoiceLine.Version","VerCol" },
                { "InvoiceLine.TotalTaxableFees","TotalTaxableFees"}
            };
            IList<InvoiceLine> lines = new List<InvoiceLine>();
            while (reader.Read())
            {
                InvoiceLine line = reader.ReadInvoiceLine(propertyColumnMappings);
                line.TaxableItems = _taxableItemDao.FindByInvoiceLineId(line.Id);
                lines.Add(line);
            }
            reader.Close();
            return lines;
        }

        public void Insert(InvoiceLine invoiceLine,DbTransaction transaction)
        {
            DbCommand insertCommand = CreateInsertCommand(invoiceLine);
            insertCommand.Connection = _connection;
            insertCommand.Transaction = transaction;
            insertCommand.ExecuteNonQuery();
            invoiceLine.Id = insertCommand.Parameters["@LineId"].Value as int?;
            invoiceLine.Version = insertCommand.Parameters["@VerCol"].Value as byte[];
            foreach (TaxableItem taxableItem in invoiceLine.TaxableItems)
            {
                _taxableItemDao.Insert(taxableItem, transaction);
            }
        }

        public void Insert(InvoiceLine entity)
        {
            throw new System.NotImplementedException();
        }

        public void SaveOrUpdate(InvoiceLine invoiceLine,DbTransaction transaction)
        {
            DbCommand command = _connection.CreateCommand("[dbo].[SaveOrUpdateInvoiceLine]", CommandType.StoredProcedure);
            command.Transaction = transaction;
            command.Parameters.Add(command.CreateParameter(parameterName: "@OldId",parameterValue:invoiceLine.Id,dbType:DbType.Int32));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Description", parameterValue: invoiceLine.Description, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@ItemType", parameterValue: invoiceLine.ItemType, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@ItemCode", parameterValue: invoiceLine.ItemCode, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@UnitType", parameterValue: invoiceLine.UnitType, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Quantity", parameterValue: invoiceLine.Quantity, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@SalesTotal", parameterValue: invoiceLine.SalesTotal, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@CurrencySold", parameterValue: invoiceLine.UnitValue.CurrencySold, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@AmountEGP", parameterValue: invoiceLine.UnitValue.AmountEGP, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@AmountSold", parameterValue: invoiceLine.UnitValue.AmountEGP, dbType: DbType.Decimal));

            command.Parameters.Add(command.CreateParameter(parameterName: "@CurrencyExchangeRate", parameterValue: invoiceLine.UnitValue.CurrencyExchangeRate, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@InternalCode", parameterValue: invoiceLine.InternalCode, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@ItemsDiscount", parameterValue: invoiceLine.ItemsDiscount, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@NetTotal", parameterValue: invoiceLine.NetTotal, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@TotalTaxableFees", parameterValue: invoiceLine.TotalTaxableFees, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@ValueDifference", parameterValue: invoiceLine.ValueDifference, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Total", parameterValue: invoiceLine.Total, dbType: DbType.Decimal));

            command.Parameters.Add(command.CreateParameter(parameterName: "@DiscountRate", parameterValue: invoiceLine.Discount.Rate, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@DiscountAmount", parameterValue: invoiceLine.Discount.Amount, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@DocumentId", parameterValue: invoiceLine.DocumentId, dbType: DbType.Int32));
            command.Parameters.Add(command.CreateParameter(parameterName: "@OldverCol", parameterValue: invoiceLine.Version, dbType: DbType.Binary));

            command.Parameters.Add(command.CreateParameter(parameterName: "@NewId",direction:ParameterDirection.Output, dbType: DbType.Int32));
            command.Parameters.Add(command.CreateParameter(parameterName: "@NewVerCol", direction: ParameterDirection.Output, dbType: DbType.Binary));
            command.Parameters["@OldverCol"].Size = 8;
            command.Parameters["@NewVerCol"].Size = 8;

            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            if (command.ExecuteNonQuery() > 0)
            {
                invoiceLine.Id = command.Parameters["@NewId"].Value as int?;
                invoiceLine.Version = command.Parameters["@NewVerCol"].Value as byte[];
                foreach (TaxableItem taxableItem in invoiceLine.TaxableItems)
                    _taxableItemDao.SaveOrUpdate(taxableItem,transaction);
                //invoiceLine.Id = command.Parameters["@NewId"].Value as int?;
                //invoiceLine.Version = command.Parameters["@NewVerCol"].Value as byte[];
            }
        }

        private DbCommand CreateInsertCommand(InvoiceLine invoiceLine)
        {
            DbCommand insertCommand = _connection.CreateCommand("[dbo].[InsertInvoiceLine]",CommandType.StoredProcedure);
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@Description", parameterValue:invoiceLine.Description));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@ItemType", parameterValue: invoiceLine.ItemType));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@ItemCode", parameterValue: invoiceLine.ItemCode));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@UnitType", parameterValue: invoiceLine.UnitType));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@Quantity", parameterValue: invoiceLine.Quantity, dbType:DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@SalesTotal", parameterValue: invoiceLine.SalesTotal, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@CurrencySold", parameterValue: invoiceLine.UnitValue.CurrencySold,dbType:DbType.String));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@AmountEGP", parameterValue: invoiceLine.UnitValue.AmountEGP, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@AmountSold", parameterValue: invoiceLine.UnitValue.AmountSold, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@InternalCode", parameterValue: invoiceLine.InternalCode, dbType: DbType.String));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@ItemsDiscount", parameterValue: invoiceLine.ItemsDiscount, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@CurrencyExchangeRate", parameterValue: invoiceLine.UnitValue.CurrencyExchangeRate, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@NetTotal", parameterValue: invoiceLine.NetTotal, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@TotalTaxableFees", parameterValue: invoiceLine.TotalTaxableFees, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@ValueDifference", parameterValue: invoiceLine.ValueDifference, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DocumentId", parameterValue: invoiceLine.DocumentId, dbType: DbType.Int32));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@Total", parameterValue: invoiceLine.Total, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DiscountRate", parameterValue: invoiceLine.Discount.Rate, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DiscountAmount", parameterValue: invoiceLine.Discount.Amount, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@LineId",  dbType: DbType.Int32,direction:ParameterDirection.Output));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@VerCol",  dbType: DbType.Binary, direction: ParameterDirection.Output));
            insertCommand.Parameters["@VerCol"].Size = 8;
            insertCommand.Connection = _connection;
            return insertCommand;
        }
    }
}

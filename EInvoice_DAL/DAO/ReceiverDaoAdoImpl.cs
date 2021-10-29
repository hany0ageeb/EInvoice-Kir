using EInvoice.Model;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Collections.Generic;
using System;

namespace EInvoice.DAL.DAO
{
    public class ReceiverDaoAdoImpl : IReceiverDao
    {
        private DbConnection _connection;
        public ReceiverDaoAdoImpl(DbConnection connection)
        {
            _connection = connection;
        }
        public IList<Receiver> Find()
        {
            IList<Receiver> receivers = new List<Receiver>();
            DbCommand selectCommand = _connection.CreateCommand();
            selectCommand.CommandType = CommandType.Text;
            selectCommand.CommandText = "SELECT [CustomerId]" +
                "      ,[Type]" +
                "      ,[Id]" +
                "      ,[Name]" +
                ",[Country]" +
                ",[Governate]" +
                  ",[RegionCity]" +
                  ",[Street]" +
                  ",[BuildingNumber]" +
                  ",[PostalCode]" +
                  ",[Floor]" +
                  ",[Room]" +
                  ",[Landmark]" +
                  ",[AdditionalInformation]" +
                  ",[VerCol]" +
            "FROM [dbo].[Customer]";
            DbDataAdapter adapter = DbProviderFactories.GetFactory(_connection).CreateDataAdapter();
            adapter.SelectCommand = selectCommand;
            DataTable table = new DataTable("Customer");
            adapter.Fill(table);
            var recs = table.AsEnumerable();
            foreach(DataRow row in recs)
            {
                Receiver receiver = new Receiver();
                receiver.Id = row.Field<string>("Id");
                receiver.Name = row.Field<string>("Name");
                receiver.Type = row.Field<string>("Type").ToReceiverType();
                receiver.Version = row.Field<byte[]>("VerCol");
                receiver.Address = new ReceiverAddress();
                receiver.Address.AdditionalInformation = row.Field<string>("AdditionalInformation");
                receiver.Address.BuildingNumber = row.Field<string>("BuildingNumber");
                receiver.Address.Country = row.Field<string>("Country");
                receiver.Address.Floor = row.Field<string>("Floor");
                receiver.Address.Room = row.Field<string>("Room");
                receiver.Address.Landmark = row.Field<string>("Landmark");
                receiver.Address.PostalCode = row.Field<string>("PostalCode");
                receiver.Address.Street = row.Field<string>("Street");
                receiver.Address.RegionCity = row.Field<string>("RegionCity");
                receiver.Address.Governate = row.Field<string>("Governate");
                receiver.InternalId = row.Field<int?>("CustomerId");
                receivers.Add(receiver);
            }
            return receivers;
        }
        public void Insert(Receiver receiver)
        {
            DbCommand command = _connection.CreateCommand("[dbo].[InsertCustomer]", CommandType.StoredProcedure);
            command.Parameters.Add(command.CreateParameter(parameterName: "@Type", parameterValue: receiver.Type.ToString()));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Id", parameterValue: receiver.Id));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Name", parameterValue: receiver.Name));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Country", parameterValue: receiver.Address?.Country));
            command.Parameters.Add(command.CreateParameter(parameterName: "@BuildingNumber", parameterValue: receiver.Address?.BuildingNumber));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Room", parameterValue: receiver.Address?.Room));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Floor", parameterValue: receiver.Address?.Floor));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Street", parameterValue: receiver.Address?.Street));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Landmark", parameterValue: receiver.Address?.Landmark));
            command.Parameters.Add(command.CreateParameter(parameterName: "@AdditionalInformation", parameterValue: receiver.Address?.AdditionalInformation));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Governate", parameterValue: receiver.Address?.Governate));
            command.Parameters.Add(command.CreateParameter(parameterName: "@RegionCity", parameterValue: receiver.Address?.RegionCity));
            command.Parameters.Add(command.CreateParameter(parameterName: "@PostalCode", parameterValue: receiver.Address?.PostalCode));
            command.Parameters.Add(command.CreateParameter(parameterName: "@CustomerId",direction:ParameterDirection.Output,dbType:DbType.Int32));
            command.Parameters.Add(command.CreateParameter(parameterName: "@VerCol", direction: ParameterDirection.Output,dbType:DbType.Binary));
            command.Parameters["@VerCol"].Size = 8;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            if (command.ExecuteNonQuery() == 1)
            {
                receiver.Version = command.Parameters["@VerCol"].Value as byte[];
                receiver.InternalId = command.Parameters["@CustomerId"].Value as int?;
            }
        }
        public IList<Receiver> Find(Issuer issuer)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetTaxpayerCustomer]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@TaxpayerId", parameterValue: issuer.Id, dbType: DbType.String));
            DbDataAdapter adapter = DbProviderFactories.GetFactory(_connection).CreateDataAdapter();
            adapter.SelectCommand = selectCommand;
            DataTable table = new DataTable();
            adapter.Fill(table);
            var receiversRows = (from rec in table.AsEnumerable() select rec).OrderBy((r) => { return r.Field<string>("Name"); });
            IList<Receiver> receivers = new List<Receiver>();
            foreach(var reciverRow in receiversRows)
            {
                receivers.Add(new Receiver()
                {
                    Id = reciverRow.Field<string>("Id"),
                    Name = reciverRow.Field<string>("Name"),
                    InternalId = reciverRow.Field<int?>("CustomerId"),
                    Type = reciverRow.Field<string>("Type").ToReceiverType(),
                    Version = reciverRow.Field<byte[]>("VerCol"),
                    Address = new ReceiverAddress()
                    {
                        AdditionalInformation = reciverRow.Field<string>("AdditionalInformation"),
                        BuildingNumber = reciverRow.Field<string>("BuildingNumber"),
                        Country = reciverRow.Field<string>("Country"),
                        Floor = reciverRow.Field<string>("Floor"),
                        Governate = reciverRow.Field<string>("Governate"),
                        Landmark = reciverRow.Field<string>("Landmark"),
                        PostalCode = reciverRow.Field<string>("PostalCode"),
                        RegionCity = reciverRow.Field<string>("RegionCity"),
                        Room = reciverRow.Field<string>("Room"),
                        Street = reciverRow.Field<string>("Street")
                    }
                });
            }
            return receivers;
        }
        public int? FindReceiverId(string name)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetCustomerIdByName]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@Name", parameterValue: name));
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@FoundId", direction: ParameterDirection.Output, dbType: DbType.Int32));
            selectCommand.Connection = _connection;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            selectCommand.ExecuteNonQuery();
            _connection.Close();
            return selectCommand.Parameters["@FoundId"].Value as int?;
        }
        public IList<Receiver> Find(string Id)
        {
            DbCommand command = _connection.CreateCommand();
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[GetCustomerById]";
            command.Parameters.Add(command.CreateParameter(parameterName:"@Id",parameterValue:Id,dbType:DbType.String));
            IList<Receiver> receivers = new List<Receiver>();
            Dictionary<string, string> propertyColumnMappings = new Dictionary<string, string>()
            {
                { "Receiver.InternalId","CustomerId"},{"Receiver.Type","Type"},{"Receiver.Id","Id"},{"Receiver.Name","Name"},{"","Country"},{"ReceiverAddress.BuildingNumber","BuildingNumber"},
                {"ReceiverAddress.Room","Room"},{"ReceiverAddress.Floor","Floor"},{ "ReceiverAddress.Street","Street"},{"ReceiverAddress.Landmark","Landmark"},
                {"ReceiverAddress.Governate","Governate"},{"ReceiverAddress.RegionCity","RegionCity"},{"ReceiverAddress.PostalCode","PostalCode"},{"ReceiverAddress.AdditionalInformation","AdditionalInformation"},{"Receiver.Version","VerCol"}
            };
            using (_connection)
            {
                try 
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            receivers.Add(reader.ReadReceiver(propertyColumnMappings));
                        }
                    }
                }
                finally
                {

                }
            }
            return receivers;
        }
    }
}

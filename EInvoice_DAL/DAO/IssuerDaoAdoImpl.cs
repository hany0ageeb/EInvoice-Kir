using EInvoice.Model;
using System;
using System.Data.Common;
using System.Collections.Generic;

namespace EInvoice.DAL.DAO
{
    public class IssuerDaoAdoImpl : IIssuerDao
    {
        private readonly DbConnection _connection;
        public IssuerDaoAdoImpl(DbConnection _conn)
        {
            _connection = _conn;
        }
        public void Insert(Issuer issuer)
        {
            throw new NotImplementedException();
        }        
        public IList<Issuer> Find()
        {
            IList<Issuer> issuers = new List<Issuer>();
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetAllTaxpayers]", System.Data.CommandType.StoredProcedure);
            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = selectCommand.ExecuteReader();
            Dictionary<string, string> propertyColumnMappings = new Dictionary<string, string>()
            {
                {"Issuer.Id","Id" },
                { "Issuer.Name","Name"},
                {"Issuer.Type","Type" },
                { "Issuer.Version","VerCol"},
                {"IssuerAddress.AdditionalInformation","AdditionalInformation" },
                {"IssuerAddress.BranchId","BranchId" },
                {"IssuerAddress.BuildingNumber","BuildingNumber" },
                {"IssuerAddress.Country","Country" },
                {"IssuerAddress.Floor","Floor" },
                {"IssuerAddress.Governate","Governate" },
                { "IssuerAddress.Landmark","Landmark"},
                { "IssuerAddress.PostalCode","PostalCode"},
                { "IssuerAddress.RegionCity","RegionCity"},
                { "IssuerAddress.Room","Room"},
                { "IssuerAddress.Street","Street"}
            };
            while (reader.Read())
            {
                issuers.Add(reader.ReadIssuer(propertyColumnMappings));
            }
            reader.Close();
            _connection.Close();
            return issuers;
        }
        public Issuer Find(string id)
        {
            Issuer issuer = null;
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetTaxPayerById]", System.Data.CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@taxPayerId",parameterValue:id,dbType:System.Data.DbType.String));
            selectCommand.Connection = _connection;
            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = selectCommand.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                issuer = new Issuer
                {
                    Id = id,
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Type = reader.GetString(reader.GetOrdinal("Type")),
                    Version = reader.GetFieldValue<byte[]>(reader.GetOrdinal("VerCol")),
                    Address = new IssuerAddress()
                    {
                        AdditionalInformation = reader.IsDBNull(reader.GetOrdinal("AdditionalInformation")) ? "" : reader.GetString(reader.GetOrdinal("AdditionalInformation")),
                        BranchId = reader.IsDBNull(reader.GetOrdinal("BranchId"))?"":reader.GetString(reader.GetOrdinal("BranchId")),
                        BuildingNumber = reader.IsDBNull(reader.GetOrdinal("BuildingNumber")) ?"":reader.GetString(reader.GetOrdinal("BuildingNumber")),
                        Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? "" : reader.GetString(reader.GetOrdinal("Country")),
                        Floor = reader.IsDBNull(reader.GetOrdinal("Floor")) ? "" : reader.GetString(reader.GetOrdinal("Floor")),
                        Governate = reader.IsDBNull(reader.GetOrdinal("Governate")) ? "" : reader.GetString(reader.GetOrdinal("Governate")),
                        Landmark = reader.IsDBNull(reader.GetOrdinal("Landmark")) ? "" : reader.GetString(reader.GetOrdinal("Landmark")),
                        Street = reader.IsDBNull(reader.GetOrdinal("Street")) ? "" : reader.GetString(reader.GetOrdinal("Street")),
                        Room = reader.IsDBNull(reader.GetOrdinal("Room")) ? "" : reader.GetString(reader.GetOrdinal("Room")),
                        PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? "" : reader.GetString(reader.GetOrdinal("PostalCode")),
                        RegionCity = reader.IsDBNull(reader.GetOrdinal("RegionCity")) ? "" : reader.GetString(reader.GetOrdinal("RegionCity")),
                    }
                };
            }
            reader.Close();
            _connection.Close();
            return issuer;
        }
    }
}

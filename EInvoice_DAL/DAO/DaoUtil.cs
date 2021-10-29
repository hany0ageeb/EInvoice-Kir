using System.Data.Common;
using System.Data;
namespace EInvoice.DAL.DAO
{
    public static class DaoUtil
    {
        private static DbTransaction transaction;
        public static DbParameter CreateParameter(this DbCommand command,string parameterName,ParameterDirection direction = ParameterDirection.Input,object parameterValue = null,DbType dbType = DbType.String)
        {
            DbParameter para = command.CreateParameter();
            para.ParameterName = parameterName;
            para.Direction = direction ;
            para.Value = parameterValue;
            para.DbType = dbType;
            if (para.DbType == DbType.Decimal)
            {
                para.Precision = 28;
                para.Scale = 5;
            }
            return para;
        }
        public static DbTransaction StartTransaction(this DbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
            if(transaction == null)
                transaction = connection.BeginTransaction();
            return transaction;
        }
        public static void CommitTransaction(this DbConnection connection)
        {
            if (transaction != null)
            {
                transaction.Commit();
                transaction = null;
            }
        }
        public static void RollbackTransaction(this DbConnection connection)
        {
            if (transaction != null)
            {
                transaction.Rollback();
                transaction = null;
            }
        }
        public static DbCommand CreateCommand(this DbConnection connection,string commandText,CommandType commandType = CommandType.Text)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Connection = connection;
            return command;
        }
    }
}

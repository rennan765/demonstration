using _4oito6.Demonstration.Data.Connection.Bulk;
using _4oito6.Demonstration.Data.Connection.MsSql.Bulk;
using System.Data.SqlClient;

namespace _4oito6.Demonstration.Data.Connection.MsSql
{
    public class MsSqlAsyncDbConnection : AsyncDbConnection, IMsSqlAsyncDbConnection
    {
        public MsSqlAsyncDbConnection(SqlConnection connection)
            : base(connection)
        {
        }

        public IBulkOperation GetBulkOperation(string tableName, int commandTimeout = 0)
        {
            return new MsSqlBulkOperation
            (
                conn: this,
                tableName: tableName,
                commandTimeout: commandTimeout
            );
        }
    }
}
using _4oito6.Demonstration.Data.Connection.Bulk;
using _4oito6.Demonstration.Data.Connection.MySql.Bulk;
using MySqlConnector;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Data.Connection.MySql
{
    [ExcludeFromCodeCoverage]
    public class MySqlAsyncDbConnection : AsyncDbConnection, IMySqlAsyncDbConnection
    {
        public MySqlAsyncDbConnection(MySqlConnection connection)
            : base(connection)
        {
        }

        public IBulkOperation GetBulkOperation(string tableName, int commandTimeout = 0)
        {
            return new MySqlBulkOperation
            (
                conn: this,
                tableName: tableName,
                commandTimeout: commandTimeout
            );
        }
    }
}
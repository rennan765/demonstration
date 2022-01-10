using _4oito6.Demonstration.Data.Connection.Bulk;
using _4oito6.Demonstration.Data.Connection.MySql.Bulk;
using MySqlConnector;

namespace _4oito6.Demonstration.Data.Connection.MySql
{
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
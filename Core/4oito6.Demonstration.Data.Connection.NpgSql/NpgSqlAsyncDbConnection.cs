using _4oito6.Demonstration.Data.Connection.Bulk;
using _4oito6.Demonstration.Data.Connection.NpgSql.Bulk;
using Npgsql;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Data.Connection.NpgSql
{
    [ExcludeFromCodeCoverage]
    public class NpgSqlAsyncDbConnection : AsyncDbConnection, INpgSqlAsyncDbConnection
    {
        public NpgSqlAsyncDbConnection(NpgsqlConnection connection)
            : base(connection)
        {
        }

        public IBulkOperation GetBulkOperation(string command, int commandTimeout = 0)
        {
            return new NpgSqlBulkOperation(this, command, commandTimeout);
        }
    }
}
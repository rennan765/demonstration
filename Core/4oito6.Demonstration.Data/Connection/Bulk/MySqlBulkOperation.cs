using MySqlConnector;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Data.Connection.Bulk
{
    public class MySqlBulkOperation : BulkOperation, IBulkOperation
    {
        private readonly MySqlBulkCopy _bulkCopy;

        public MySqlBulkOperation(IMySqlAsyncDbConnection conn, string tableName, int commandTimeout = 0)
            : base(tableName)
        {
            _bulkCopy = new MySqlBulkCopy((MySqlConnection)conn.Connection, (MySqlTransaction)conn.Transaction)
            {
                DestinationTableName = TableName
            };

            if (commandTimeout > 0)
            {
                _bulkCopy.BulkCopyTimeout = commandTimeout;
            }
        }

        public override async Task<int> BulkInsertAsync()
        {
            var result = await _bulkCopy.WriteToServerAsync(Table).ConfigureAwait(false);
            return result.RowsInserted;
        }
    }
}
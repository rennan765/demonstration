using System.Data.SqlClient;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Data.Connection.Bulk
{
    public class MsSqlBulkOperation : BulkOperation, IBulkOperation
    {
        private readonly SqlBulkCopy _bulkCopy;

        public MsSqlBulkOperation(IMySqlAsyncDbConnection conn, string tableName, int commandTimeout = 0)
            : base(tableName)
        {
            _bulkCopy = new SqlBulkCopy
            (
                connection: (SqlConnection)conn.Connection,
                copyOptions: SqlBulkCopyOptions.Default,
                externalTransaction: (SqlTransaction)conn.Transaction
            )
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
            _bulkCopy.BatchSize = Table.Rows.Count;
            await _bulkCopy.WriteToServerAsync(Table).ConfigureAwait(false);
            return _bulkCopy.BatchSize;
        }
    }
}
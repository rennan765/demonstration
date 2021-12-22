using _4oito6.Demonstration.Commons;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Data.Connection.Bulk
{
    public class MySqlBulkOperation : DisposableObject, IBulkOperation
    {
        private readonly string _tableName;
        private readonly MySqlBulkCopy _bulkCopy;

        public MySqlBulkOperation(IMySqlAsyncDbConnection conn, string tableName, int commandTimeout = 0)
            : base(new IDisposable[0])
        {
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));

            _bulkCopy = new MySqlBulkCopy((MySqlConnection)conn.Connection, (MySqlTransaction)conn.Transaction)
            {
                DestinationTableName = _tableName
            };

            if (commandTimeout > 0)
            {
                _bulkCopy.BulkCopyTimeout = commandTimeout;
            }

            Table = new DataTable
            {
                TableName = _tableName
            };
        }

        public DataTable Table { get; private set; }

        public async Task<int> BulkInsertAsync()
        {
            var result = await _bulkCopy.WriteToServerAsync(Table).ConfigureAwait(false);
            return result.RowsInserted;
        }

        public void AddRow(Dictionary<string, object> row)
        {
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            if (Table is null)
            {
                Table = new DataTable
                {
                    TableName = _tableName
                };
            }

            var dataRow = Table.NewRow();
            row.Select(p => dataRow[p.Key] = p.Value);
            Table.Rows.Add(dataRow);
        }

        public override void Dispose()
        {
            base.Dispose();
            Table?.Dispose();
            Table = null;
        }
    }
}
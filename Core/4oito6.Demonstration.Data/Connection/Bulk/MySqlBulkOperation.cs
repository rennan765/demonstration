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
        private readonly IAsyncDbConnection _conn;
        private readonly string _tableName;
        private readonly MySqlBulkCopy _bulkCopy;

        public MySqlBulkOperation(IAsyncDbConnection conn, string tableName)
            : base(new IDisposable[] { conn })
        {
            _conn = conn ?? throw new ArgumentNullException(nameof(conn));
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));

            _bulkCopy = new MySqlBulkCopy((MySqlConnection)_conn.Connection, (MySqlTransaction)_conn.Transaction)
            {
                DestinationTableName = _tableName
            };

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

        public void SetRow(Dictionary<string, object> row)
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
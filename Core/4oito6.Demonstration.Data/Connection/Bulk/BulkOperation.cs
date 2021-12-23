using _4oito6.Demonstration.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Data.Connection.Bulk
{
    public abstract class BulkOperation : DisposableObject, IBulkOperation
    {
        public BulkOperation(string tableName) : base(new IDisposable[0])
        {
            TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));

            Table = new DataTable
            {
                TableName = TableName
            };
        }

        protected string TableName { get; set; }

        protected DataTable Table { get; set; }

        public virtual void AddRow(Dictionary<string, object> row)
        {
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            if (Table is null)
            {
                Table = new DataTable
                {
                    TableName = TableName
                };
            }

            var dataRow = Table.NewRow();
            foreach (var pair in row)
            {
                if (!Table.Columns.Contains(pair.Key))
                {
                    Table.Columns.Add(pair.Key, pair.Value.GetType());
                }

                dataRow[pair.Key] = pair.Value;
            }

            Table.Rows.Add(dataRow);
        }

        public virtual Task AddRowAsync(Dictionary<string, object> row)
        {
            AddRow(row);
            return Task.CompletedTask;
        }

        public abstract Task<int> BulkInsertAsync();

        public override void Dispose()
        {
            base.Dispose();
            Table?.Dispose();
            Table = null;
        }
    }
}
using _4oito6.Demonstration.Data.Connection.Bulk;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Data.Connection.NpgSql.Bulk
{
    [ExcludeFromCodeCoverage]
    public class NpgSqlBulkOperation : BulkOperation, IBulkOperation
    {
        private NpgsqlBinaryImporter _binaryImporter;

        public NpgSqlBulkOperation(INpgSqlAsyncDbConnection connection, string command, int commandTimeout = 0)
            : base(command)
        {
            _binaryImporter = ((NpgsqlConnection)connection.Connection).BeginBinaryImport(Command);
            _binaryImporter.Timeout = TimeSpan.FromSeconds(commandTimeout);
        }

        public string Command => TableName;

        public override void AddRow(Dictionary<string, object> row)
        {
            base.AddRow(row);
            _binaryImporter.StartRow();

            foreach (var value in row.Select(p => p.Value))
            {
                if (value is null)
                {
                    _binaryImporter.WriteNull();
                    continue;
                }

                _binaryImporter.Write(value);
            }
        }

        public override async Task AddRowAsync(Dictionary<string, object> row)
        {
            await base.AddRowAsync(row).ConfigureAwait(false);
            await _binaryImporter.StartRowAsync().ConfigureAwait(false);

            foreach (var value in row.Select(p => p.Value))
            {
                if (value is null)
                {
                    await _binaryImporter.WriteNullAsync().ConfigureAwait(false);
                    continue;
                }

                await _binaryImporter.WriteAsync(value).ConfigureAwait(false);
            }
        }

        public override async Task<int> BulkInsertAsync()
        {
            var result = await _binaryImporter.CompleteAsync().ConfigureAwait(false);
            await _binaryImporter.CloseAsync().ConfigureAwait(false);
            return (int)result;
        }

        public override void Dispose()
        {
            base.Dispose();
            _binaryImporter?.Dispose();
            _binaryImporter = null;
        }
    }
}
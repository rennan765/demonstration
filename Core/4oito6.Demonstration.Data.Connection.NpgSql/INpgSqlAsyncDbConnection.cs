using _4oito6.Demonstration.Data.Connection.Bulk;

namespace _4oito6.Demonstration.Data.Connection.NpgSql
{
    public interface INpgSqlAsyncDbConnection : IAsyncDbConnection
    {
        /// <summary>
        /// Start MySql Bulk Operation
        /// </summary>
        /// <param name="command">SQL COPY command.</param>
        /// <param name="commandTimeout">Command Timeout. Optional, default 0. If 0, it will use the default MySql Command Timeout</param>
        /// <returns></returns>
        IBulkOperation GetBulkOperation(string command, int commandTimeout = 0);
    }
}
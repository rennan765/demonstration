using _4oito6.Demonstration.Data.Connection.Bulk;

namespace _4oito6.Demonstration.Data.Connection.MySql
{
    public interface IMySqlAsyncDbConnection : IAsyncDbConnection
    {
        /// <summary>
        /// Start MySql Bulk Operation
        /// </summary>
        /// <param name="tableName">Destination table</param>
        /// <param name="commandTimeout">Command Timeout. Optional, default 0. If 0, it will use the default MySql Command Timeout</param>
        /// <returns></returns>
        IBulkOperation GetBulkOperation(string tableName, int commandTimeout = 0);
    }
}
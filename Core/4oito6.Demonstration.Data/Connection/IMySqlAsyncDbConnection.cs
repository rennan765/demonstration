using _4oito6.Demonstration.Data.Connection.Bulk;

namespace _4oito6.Demonstration.Data.Connection
{
    public interface IMySqlAsyncDbConnection : IAsyncDbConnection
    {
        IBulkOperation GetBulkOperation();
    }
}
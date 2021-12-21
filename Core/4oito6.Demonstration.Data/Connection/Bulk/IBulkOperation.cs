using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Data.Connection.Bulk
{
    public interface IBulkOperation : IDisposable
    {
        void SetRow(Dictionary<string, object> row);

        Task<int> BulkInsertAsync();
    }
}
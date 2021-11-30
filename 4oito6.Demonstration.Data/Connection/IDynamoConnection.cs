using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Data.Connection
{
    public interface IDynamoConnection<T> : IDisposable where T : class
    {
        Task<Document> CreateAsync(Document document);

        Task<T> CreateAsync(T document);

        Task<Document> UpdateAsync(Document document);

        Task<T> UpdateAsync(T document);

        Task DeleteAsync(string partitionKey);

        Task DeleteAsync(Document document);

        Task DeleteAsync(T document);

        Task<IEnumerable<T>> FindAllAsync();

        Task<IEnumerable<T>> FindByDateTicksAsync(string dateProperty, long startTicks, long endTicks);
    }
}
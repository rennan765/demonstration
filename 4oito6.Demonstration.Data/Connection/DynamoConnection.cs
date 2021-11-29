using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Data.Model.Dynamo;
using _4oito6.Demonstration.Data.Model.Dynamo.Base;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Data.Connection
{
    public class DynamoConnection<T> : DisposableObject, IDynamoConnection<T> where T : DynamoEntityDto, new()
    {
        private readonly IAmazonDynamoDB _conn;
        private readonly Table _table;

        public DynamoConnection(IAmazonDynamoDB conn) : base(new IDisposable[] { conn })
        {
            _conn = conn ?? throw new ArgumentNullException(nameof(conn));
            _table = Table.LoadTable(_conn, new T().TableName);
        }

        public Task<Document> CreateAsync(Document document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            return _table.PutItemAsync(document);
        }

        public async Task<T> CreateAsync(T document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var result = await CreateAsync(document.ToDocument()).ConfigureAwait(false);

            return result?.ToEntity<T>();
        }

        public Task DeleteAsync(string partitionKey)
        {
            return _table.DeleteItemAsync(partitionKey, new DeleteItemOperationConfig());
        }

        public Task DeleteAsync(Document document)
        {
            return _table.DeleteItemAsync(document, new DeleteItemOperationConfig());
        }

        public Task DeleteAsync(T document)
        {
            return DeleteAsync(document.ToDocument());
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            return (await _table.Scan(default(Expression)).GetRemainingAsync().ConfigureAwait(false))
                .Select(document => document.ToEntity<T>())
                .ToList();
        }

        public async Task<IEnumerable<T>> FindByDateTicksAsync(string dateProperty, long startTicks, long endTicks)
        {
            var scanFilter = new ScanFilter();
            var filterValues = new DynamoDBEntry[] { startTicks, endTicks };

            scanFilter.AddCondition(dateProperty, ScanOperator.Between, filterValues);
            var search = _table.Scan(scanFilter);

            var documents = new List<Document>();
            do
            {
                documents.AddRange(await search.GetNextSetAsync().ConfigureAwait(false));
            }
            while (search.IsDone);

            return documents
                .Select(document => document.ToEntity<T>())
                .ToList();
        }

        public Task<Document> UpdateAsync(Document document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var config = new UpdateItemOperationConfig
            {
                ReturnValues = ReturnValues.AllNewAttributes
            };

            return _table.UpdateItemAsync(document, config);
        }

        public async Task<T> UpdateAsync(T document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var result = await UpdateAsync(document.ToDocument()).ConfigureAwait(false);

            return result?.ToEntity<T>();
        }
    }
}
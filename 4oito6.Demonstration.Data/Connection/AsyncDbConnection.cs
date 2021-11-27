using _4oito6.Demonstration.Commons;
using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Data.Connection
{
    public class AsyncDbConnection : DisposableObject, IAsyncDbConnection
    {
        private readonly IDbConnection _conn;

        public AsyncDbConnection(IDbConnection conn)
            : base(new IDisposable[] { conn })
        {
            _conn = conn ?? throw new ArgumentNullException(nameof(conn));
        }

        public IDbTransaction Transaction { get; private set; }

        public string ConnectionString => _conn.ConnectionString;

        public int ConnectionTimeOut => _conn.ConnectionTimeout;

        public ConnectionState State => _conn.State;

        public string Database => _conn.Database;

        public Task<IEnumerable<dynamic>> QueryAsync(CommandDefinition command)
            => _conn.QueryAsync(command);

        public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
            => _conn.QueryAsync<T>(command);

        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters)
            => _conn.QueryAsync(sql, parameters);

        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters, IDbTransaction transaction)
            => _conn.QueryAsync(sql: sql, param: parameters, transaction: transaction);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters, IDbTransaction transaction)
            => _conn.QueryAsync<T>(sql: sql, param: parameters, transaction: transaction);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters)
            => _conn.QueryAsync<T>(sql: sql, param: parameters);

        public Task<int> ExecuteAsync(CommandDefinition command)
            => _conn.ExecuteAsync(command);

        public Task<int> InsertAsync<T>(T entity, IDbTransaction transaction) where T : class
        {
            return _conn.InsertAsync<T>(entity, transaction);
        }

        public Task<bool> UpdateAsync<T>(T entity, IDbTransaction transaction) where T : class
        {
            return _conn.UpdateAsync<T>(entity, transaction);
        }

        public Task<bool> DeleteAsync<T>(T entity, IDbTransaction transaction) where T : class
        {
            return _conn.DeleteAsync<T>(entity, transaction);
        }

        public IDbTransaction BeginTransaction() => _conn.BeginTransaction();

        public IDbTransaction BeginTransaction(IsolationLevel level) => _conn.BeginTransaction(level);

        public void ChangeDatabase(string databaseName)
        {
            _conn.ChangeDatabase(databaseName);
        }

        public void Open()
        {
            _conn.Open();
        }

        public void Close()
        {
            _conn.Close();
        }

        public void CreateCommand()
        {
            _conn.CreateCommand();
        }
    }
}
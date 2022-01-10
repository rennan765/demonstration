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
        public AsyncDbConnection(IDbConnection connection)
            : base(new IDisposable[] { connection })
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public IDbConnection Connection { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public string ConnectionString => Connection.ConnectionString;

        public int ConnectionTimeOut => Connection.ConnectionTimeout;

        public ConnectionState State => Connection.State;

        public string Database => Connection.Database;

        public Task<IEnumerable<dynamic>> QueryAsync(CommandDefinition command)
            => Connection.QueryAsync(command);

        public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
            => Connection.QueryAsync<T>(command);

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn = "Id")
            => Connection.QueryAsync(command, map, splitOn);

        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters)
            => Connection.QueryAsync(sql, parameters);

        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters, IDbTransaction transaction)
            => Connection.QueryAsync(sql: sql, param: parameters, transaction: transaction);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters, IDbTransaction transaction)
            => Connection.QueryAsync<T>(sql: sql, param: parameters, transaction: transaction);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters)
            => Connection.QueryAsync<T>(sql: sql, param: parameters);

        public Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction transaction, int? commandTimeout = null) where T : class
            => Connection.GetAllAsync<T>(transaction, commandTimeout);

        public Task<int> ExecuteAsync(CommandDefinition command)
            => Connection.ExecuteAsync(command);

        public Task<int> InsertAsync<T>(T entity, IDbTransaction transaction) where T : class
        {
            return Connection.InsertAsync(entity, transaction);
        }

        public Task<bool> UpdateAsync<T>(T entity, IDbTransaction transaction) where T : class
        {
            return Connection.UpdateAsync(entity, transaction);
        }

        public Task<bool> DeleteAsync<T>(T entity, IDbTransaction transaction) where T : class
        {
            return Connection.DeleteAsync(entity, transaction);
        }

        public IDbTransaction BeginTransaction()
        {
            Transaction = Connection.BeginTransaction();
            return Transaction;
        }

        public IDbTransaction BeginTransaction(IsolationLevel level)
        {
            Transaction = Connection.BeginTransaction(level);
            return Transaction;
        }

        public void ChangeDatabase(string databaseName)
        {
            Connection.ChangeDatabase(databaseName);
        }

        public void Open()
        {
            Connection.Open();
        }

        public void Close()
        {
            Connection.Close();
        }

        public void CreateCommand()
        {
            Connection.CreateCommand();
        }
    }
}
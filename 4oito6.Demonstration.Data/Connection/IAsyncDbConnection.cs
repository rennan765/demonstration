using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Data.Connection
{
    /// <summary>
    /// SQL Connection wrapper, used to encapsulate any SQL Connection type
    /// </summary>
    public interface IAsyncDbConnection : IDisposable
    {
        IDbTransaction Transaction { get; }

        string ConnectionString { get; }

        int ConnectionTimeOut { get; }

        string Database { get; }

        ConnectionState State { get; }

        Task<IEnumerable<dynamic>> QueryAsync(CommandDefinition command);

        Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command);

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn = "Id");

        Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters);

        Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters, IDbTransaction transaction);

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters);

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters, IDbTransaction transaction);

        Task<int> InsertAsync<T>(T entity, IDbTransaction transaction) where T : class;

        Task<bool> UpdateAsync<T>(T entity, IDbTransaction transaction) where T : class;

        Task<bool> DeleteAsync<T>(T entity, IDbTransaction transaction) where T : class;

        Task<int> ExecuteAsync(CommandDefinition command);

        IDbTransaction BeginTransaction();

        IDbTransaction BeginTransaction(IsolationLevel level);

        void ChangeDatabase(string databaseName);

        void Open();

        void Close();

        void CreateCommand();
    }
}
﻿using _4oito6.Demonstration.Data.Connection;
using _4oito6.Demonstration.Domain.Data.Transaction;
using _4oito6.Demonstration.Domain.Data.Transaction.Enuns;
using _4oito6.Demonstration.Domain.Data.Transaction.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace _4oito6.Demonstration.Data.Transaction
{
    public class BaseUnitOfWork : IBaseUnitOfWork
    {
        private readonly Dictionary<DataSource, IAsyncDbConnection> _connections;
        private readonly Dictionary<DataSource, IDbTransaction> _transactions;
        private bool disposedValue;

        protected BaseUnitOfWork()
        {
            _connections = new Dictionary<DataSource, IAsyncDbConnection>();
            _transactions = new Dictionary<DataSource, IDbTransaction>();
        }

        public void CloseConnections()
        {
            foreach (var conn in _connections.Values)
            {
                conn.Close();
            }
        }

        public void Commit()
        {
            foreach (var transaction in _transactions.Values)
            {
                transaction.Commit();
            }
        }

        public void NotifyDataOperation(DataOperation dataOperation)
        {
            if (_connections.ContainsKey(dataOperation.DataSource))
            {
                _connections[dataOperation.DataSource].Open();
            }

            if (_connections.ContainsKey(dataOperation.DataSource) && dataOperation.OperationType == OperationType.Write && !_transactions.ContainsKey(dataOperation.DataSource))
            {
                _transactions.Add(dataOperation.DataSource, _connections[dataOperation.DataSource].BeginTransaction());
            }
        }

        public void Rollback()
        {
            foreach (var transaction in _transactions.Values)
            {
                transaction.Rollback();
            }
        }

        protected void Attach(IAsyncDbConnection connection, DataSource dataSource)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (!_connections.ContainsKey(dataSource))
            {
                _connections.Add(dataSource, connection);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var transaction in _transactions.Values)
                    {
                        transaction.Rollback();
                        transaction.Dispose();
                    }

                    _transactions.Clear();

                    foreach (var conn in _connections.Values)
                    {
                        conn.Close();
                        conn.Dispose();
                    }

                    _connections.Clear();
                }

                disposedValue = true;
            }
        }

        ~BaseUnitOfWork()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
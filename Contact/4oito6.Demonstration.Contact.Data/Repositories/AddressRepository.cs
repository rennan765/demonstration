using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Contact.Domain.Data.Repositories;
using _4oito6.Demonstration.Data.Connection;
using _4oito6.Demonstration.Data.Model;
using _4oito6.Demonstration.Domain.Data.Transaction;
using _4oito6.Demonstration.Domain.Data.Transaction.Model;
using _4oito6.Demonstration.Domain.Model.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Data.Repositories
{
    public class AddressRepository : DisposableObject, IAddressRepository
    {
        private readonly IAsyncDbConnection _conn;
        private readonly IDataOperationHandler _handler;

        public static string DeleteWithoutPerson { get; private set; }

        public static string GetByAddress { get; private set; }

        static AddressRepository()
        {
            var addressProperties = typeof(AddressDto).GetProperties().Select(p => p.Name);

            DeleteWithoutPerson = $@"
            DELETE FROM tb_address A
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_person P
                              WHERE P.addressid = A.addressid);
            ";

            GetByAddress = $@"
            SELECT
                {string.Join(",", addressProperties.Select(p => $"A.{p}"))}
            FROM tb_address A
            WHERE {string.Join(" AND ", addressProperties.Where(p => p != nameof(AddressDto.addressid)).Select(p => $"A.{p} = @{p}"))}
            ORDER BY {nameof(AddressDto.addressid)}
            LIMIT 1;";
        }

        public AddressRepository(IAsyncDbConnection conn, IDataOperationHandler handler)
            : base(new IDisposable[] { conn })
        {
            _conn = conn ?? throw new ArgumentNullException(nameof(conn));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task DeleteWithoutPersonAsync()
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseWrite);

            var command = new CommandDefinition
            (
                commandText: DeleteWithoutPerson,
                transaction: _conn.Transaction,
                commandTimeout: (int)TimeSpan.FromMinutes(2).TotalSeconds
            );

            return _conn.ExecuteAsync(command);
        }

        public async Task<Address> GetByAddressAsync(Address address)
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            var command = new CommandDefinition
            (
                commandText: GetByAddress,
                parameters: address.ToAddress(),
                transaction: _conn.Transaction
            );

            return (await _conn.QueryAsync<AddressDto>(command).ConfigureAwait(false))
                .FirstOrDefault()?.ToAddress();
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            return (await _conn
                .GetAllAsync<AddressDto>
                (
                    transaction: _conn.Transaction,
                    commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
                ).ConfigureAwait(false))
                .Select(dto => dto.ToAddress())
                .ToList();
        }

        public Task CloneAsync(IEnumerable<Address> addresses)
        {
            throw new NotImplementedException();
        }
    }
}
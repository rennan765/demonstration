using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Contact.Data.Model;
using _4oito6.Demonstration.Contact.Domain.Data.Repositories;
using _4oito6.Demonstration.Data.Connection;
using _4oito6.Demonstration.Data.Connection.MySql;
using _4oito6.Demonstration.Data.Model;
using _4oito6.Demonstration.Domain.Data.Transaction;
using _4oito6.Demonstration.Domain.Data.Transaction.Model;
using _4oito6.Demonstration.Domain.Model.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class AddressRepository : DisposableObject, IAddressRepository
    {
        private readonly IAsyncDbConnection _relationalConn;
        private readonly IMySqlAsyncDbConnection _cloneConn;
        private readonly IDataOperationHandler _handler;

        public static string DeleteWithoutPerson { get; private set; }

        public static string GetByAddress { get; private set; }

        public static string TemporaryTableName { get; private set; }

        public static string DropTemporaryTable { get; private set; }

        public static string CreateTemporaryTable { get; private set; }

        public static string MaintainFromTemporaryTable { get; private set; }

        static AddressRepository()
        {
            var properties = typeof(AddressDto).GetProperties();
            var propertiesWithoutId = properties.Where(n => !n.Name.Equals(nameof(AddressDto.addressid)));

            DeleteWithoutPerson = $@"
            DELETE FROM tb_address A
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_person P
                              WHERE P.addressid = A.addressid);
            ";

            GetByAddress = $@"
            SELECT
                {string.Join(",", properties.Select(p => $"A.{p.Name}"))}
            FROM tb_address A
            WHERE {string.Join(" AND ", properties.Where(p => p.Name != nameof(AddressDto.addressid)).Select(p => $"A.{p.Name} = @{p.Name}"))}
            ORDER BY {nameof(AddressDto.addressid)}
            LIMIT 1;";

            TemporaryTableName = @"temp_address";

            DropTemporaryTable = $@"DROP TABLE IF EXISTS {TemporaryTableName};";

            CreateTemporaryTable = $@"{DropTemporaryTable}

            CREATE TEMPORARY TABLE {TemporaryTableName}
            (
                {nameof(AddressDto.addressid)} INT NOT NULL,
                {nameof(AddressDto.city)} VARCHAR(30) NOT NULL,
                {nameof(AddressDto.complement)} VARCHAR(20) NULL,
                {nameof(AddressDto.district)} VARCHAR(30) NOT NULL,
                {nameof(AddressDto.number)} VARCHAR(8) NULL,
                {nameof(AddressDto.postalcode)} CHAR(8) NOT NULL,
                {nameof(AddressDto.state)} CHAR(2) NOT NULL,
                {nameof(AddressDto.street)} VARCHAR(250) NOT NULL
            );
            ";

            MaintainFromTemporaryTable = $@"
            DELETE A FROM tb_address A
            WHERE NOT EXISTS (SELECT 1 FROM {TemporaryTableName} TEMP WHERE TEMP.{nameof(AddressDto.addressid)} = A.{nameof(AddressDto.addressid)});

            UPDATE tb_address A
            INNER JOIN {TemporaryTableName} TEMP ON TEMP.{nameof(AddressDto.addressid)} = A.{nameof(AddressDto.addressid)}
            SET {string.Join(",", propertiesWithoutId.Select(p => $"A.{p.Name} = TEMP.{p.Name}"))}
            WHERE {string.Join(" AND ", propertiesWithoutId.Select(p => $"TEMP.{p.Name} IS NOT NULL"))};

            INSERT INTO tb_address ({string.Join(",", properties.Select(p => p.Name))})
            SELECT {string.Join(",", properties.Select(p => p.Name))}
            FROM {TemporaryTableName} TEMP
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_address A
                              WHERE A.{nameof(AddressDto.addressid)} = TEMP.{nameof(AddressDto.addressid)});
            ";
        }

        public AddressRepository
        (
            IAsyncDbConnection relationalConn,
            IMySqlAsyncDbConnection cloneConn,
            IDataOperationHandler handler
        )
            : base(new IDisposable[] { relationalConn, cloneConn })
        {
            _relationalConn = relationalConn ?? throw new ArgumentNullException(nameof(relationalConn));
            _cloneConn = cloneConn ?? throw new ArgumentNullException(nameof(cloneConn));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task DeleteWithoutPersonAsync()
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseWrite);

            var command = new CommandDefinition
            (
                commandText: DeleteWithoutPerson,
                transaction: _relationalConn.Transaction,
                commandTimeout: (int)TimeSpan.FromMinutes(2).TotalSeconds
            );

            return _relationalConn.ExecuteAsync(command);
        }

        public async Task<Address> GetByAddressAsync(Address address)
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            var command = new CommandDefinition
            (
                commandText: GetByAddress,
                parameters: address.ToAddressDto(),
                transaction: _relationalConn.Transaction
            );

            return (await _relationalConn.QueryAsync<AddressDto>(command).ConfigureAwait(false))
                .FirstOrDefault()?.ToAddress();
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            return (await _relationalConn
                .GetAllAsync<AddressDto>
                (
                    transaction: _relationalConn.Transaction,
                    commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
                ).ConfigureAwait(false))
                .Select(dto => dto.ToAddress())
                .ToList();
        }

        public async Task CloneAsync(IEnumerable<Address> addresses)
        {
            if (addresses is null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }

            if (!addresses.Any())
            {
                return;
            }

            _handler.NotifyDataOperation(DataOperation.CloneDatabaseWrite);

            //creating temp table:
            var command = new CommandDefinition
            (
                commandText: CreateTemporaryTable,
                transaction: _cloneConn.Transaction
            );

            await _cloneConn.ExecuteAsync(command).ConfigureAwait(false);

            //var inserting
            using var bulkOperation = _cloneConn
                .GetBulkOperation
                (
                    tableName: TemporaryTableName,
                    commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
                );

            foreach (var dto in addresses.Select(dto => dto.ToAddressDto()))
            {
                bulkOperation.AddRow(dto.ToBulkDictionary());
            }

            await bulkOperation.BulkInsertAsync().ConfigureAwait(false);

            //maintain:
            command = new CommandDefinition
            (
                commandText: MaintainFromTemporaryTable,
                transaction: _cloneConn.Transaction,
                commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
            );

            await _cloneConn.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
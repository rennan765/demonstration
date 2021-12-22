using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Contact.Data.Model;
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
    public class PhoneRepository : DisposableObject, IPhoneRepository
    {
        private readonly IAsyncDbConnection _relationalConn;
        private readonly IMySqlAsyncDbConnection _cloneConn;
        private readonly IDataOperationHandler _handler;

        public static string GetByNumber { get; private set; }

        public static string DeleteWithoutPerson { get; private set; }

        public static string TemporaryTableName { get; private set; }

        public static string DropTemporaryTable { get; private set; }

        public static string CreateTemporaryTable { get; private set; }

        public static string MaintainFromTemporaryTable { get; private set; }

        static PhoneRepository()
        {
            var properties = typeof(PhoneDto).GetProperties();

            GetByNumber = $@"
            WITH Phones ({nameof(PhoneDto.type)}, {nameof(PhoneDto.code)}, {nameof(PhoneDto.number)}) AS ({{0}})
            SELECT P.{nameof(PhoneDto.phoneid)}, P.{nameof(PhoneDto.type)}, P.{nameof(PhoneDto.code)}, P.{nameof(PhoneDto.number)}
            FROM tb_phone P
            INNER JOIN Phones TEMP
	            ON TEMP.{nameof(PhoneDto.type)} = P.{nameof(PhoneDto.type)}
	            AND TEMP.{nameof(PhoneDto.code)} = P.{nameof(PhoneDto.code)}
	            AND TEMP.{nameof(PhoneDto.number)} = P.{nameof(PhoneDto.number)}
            INNER JOIN LATERAL (SELECT P2.*
				   	            FROM tb_phone P2
				                WHERE P2.{nameof(PhoneDto.type)} = p.{nameof(PhoneDto.type)}
					            AND P2.{nameof(PhoneDto.code)} = p.{nameof(PhoneDto.code)}
					            AND P2.{nameof(PhoneDto.number)} = p.{nameof(PhoneDto.number)}
				                ORDER BY P2.{nameof(PhoneDto.phoneid)} ASC
				                LIMIT 1) AS orderedphones ON orderedphones.{nameof(PhoneDto.phoneid)} = P.{nameof(PhoneDto.phoneid)}
            WHERE EXISTS (SELECT 1
			              FROM Phones TEMP
			              WHERE TEMP.{nameof(PhoneDto.type)} = P.{nameof(PhoneDto.type)}
			              AND TEMP.{nameof(PhoneDto.code)} = P.{nameof(PhoneDto.code)}
			              AND TEMP.{nameof(PhoneDto.number)} = P.{nameof(PhoneDto.number)});
            ";

            DeleteWithoutPerson = $@"
            DELETE FROM tb_phone P
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_person_phone PP
                              WHERE PP.{nameof(PhoneDto.phoneid)} = P.{nameof(PhoneDto.phoneid)});
            ";

            TemporaryTableName = @"temp_phone";

            DropTemporaryTable = $@"DROP TABLE IF EXISTS {TemporaryTableName};";

            CreateTemporaryTable = $@"{DropTemporaryTable}

            CREATE TEMPORARY TABLE {TemporaryTableName}
            (
                {nameof(PhoneDto.phoneid)} INT NOT NULL,
                {nameof(PhoneDto.code)} CHAR(2) NOT NULL,
                {nameof(PhoneDto.number)} varchar(9) NOT NULL
            );
            ";

            MaintainFromTemporaryTable = $@"
            DELETE P FROM tb_phone P
            WHERE NOT EXISTS (SELECT 1 FROM {TemporaryTableName} TEMP WHERE TEMP.{nameof(PhoneDto.phoneid)} = P.{nameof(PhoneDto.phoneid)});

            UPDATE tb_phone P
            INNER JOIN {TemporaryTableName} TEMP ON TEMP.{nameof(PhoneDto.phoneid)} = P.{nameof(PhoneDto.phoneid)}
            SET P.{nameof(PhoneDto.code)} = TEMP.{nameof(PhoneDto.code)},
	            P.{nameof(PhoneDto.number)} = TEMP.{nameof(PhoneDto.number)}
            WHERE TEMP.{nameof(PhoneDto.code)} IS NOT NULL AND TEMP.{nameof(PhoneDto.number)} IS NOT NULL;

            INSERT INTO tb_phone ({string.Join(",", properties.Select(p => p.Name))})
            SELECT {string.Join(",", properties.Select(p => p.Name))}
            FROM {TemporaryTableName} TEMP
            WHER NOT EXISTS (SELECT 1
                             FROM tb_phone P
                             WHERE p.{nameof(PhoneDto.phoneid)} = TEMP.{nameof(PhoneDto.phoneid)});
            ";
        }

        public PhoneRepository
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

        public async Task<IEnumerable<Phone>> GetByNumberAsync(IEnumerable<Phone> phones)
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            var i = 0;
            var whereClauses = new List<string>();
            var parameters = new Dictionary<string, object>();

            foreach (var phone in phones.Select(p => p.ToPhoneDto()))
            {
                var typeName = $"@{nameof(PhoneDto.type)}{i}";
                parameters.Add(typeName, phone.type);

                var codeName = $"@{nameof(PhoneDto.code)}{i}";
                parameters.Add(codeName, phone.code);

                var numberName = $"@{nameof(PhoneDto.number)}{i}";
                parameters.Add(numberName, phone.number);

                whereClauses.Add($"SELECT {typeName}, {codeName}, {numberName}");
                i++;
            }

            var command = new CommandDefinition
            (
                commandText: string.Format(GetByNumber, string.Join(" UNION ", whereClauses)),
                parameters: parameters,
                transaction: _relationalConn.Transaction
            );

            whereClauses.Clear();

            return (await _relationalConn.QueryAsync<PhoneDto>(command).ConfigureAwait(false))
                .Select(dto => dto.ToPhone())
                .ToList();
        }

        public async Task<IEnumerable<Phone>> GetAllAsync()
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            return (await _relationalConn
                .GetAllAsync<PhoneDto>
                (
                    transaction: _relationalConn.Transaction,
                    commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
                ).ConfigureAwait(false))
                .Select(dto => dto.ToPhone())
                .ToList();
        }

        public async Task CloneAsync(IEnumerable<Phone> phones)
        {
            if (phones is null)
            {
                throw new ArgumentNullException(nameof(phones));
            }

            if (!phones.Any())
            {
                return;
            }

            _handler.NotifyDataOperation(DataOperation.CloneDatabaseWrite);

            //creating temp table:
            var command = new CommandDefinition
            (
                commandText: CreateTemporaryTable,
                transaction: _relationalConn.Transaction
            );

            await _relationalConn.ExecuteAsync(command).ConfigureAwait(false);

            //var inserting
            using var bulkOperation = _cloneConn
                .GetBulkOperation
                (
                    tableName: TemporaryTableName,
                    commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
                );

            phones.Select(p => p.ToPhoneDto().ToBulkDictionary());
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
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
    public class PhoneRepository : DisposableObject, IPhoneRepository
    {
        private readonly IAsyncDbConnection _conn;
        private readonly IDataOperationHandler _handler;

        public static string GetByNumber { get; private set; }

        public static string DeleteWithoutPerson { get; private set; }

        static PhoneRepository()
        {
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
        }

        public PhoneRepository(IAsyncDbConnection conn, IDataOperationHandler handler)
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
                transaction: _conn.Transaction
            );

            whereClauses.Clear();

            return (await _conn.QueryAsync<PhoneDto>(command).ConfigureAwait(false))
                .Select(dto => dto.ToPhone())
                .ToList();
        }

        public async Task<IEnumerable<Phone>> GetAllAsync()
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            return (await _conn
                .GetAllAsync<PhoneDto>
                (
                    transaction: _conn.Transaction,
                    commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
                ).ConfigureAwait(false))
                .Select(dto => dto.ToPhone())
                .ToList();
        }

        public Task CloneAsync(IEnumerable<Phone> phones)
        {
            throw new NotImplementedException();
        }
    }
}
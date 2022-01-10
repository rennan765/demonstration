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
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Data.Repositories
{
    public class PersonRepository : DisposableObject, IPersonRepository
    {
        private readonly IAsyncDbConnection _relationalConn;
        private readonly IMySqlAsyncDbConnection _cloneConn;
        private readonly IDataOperationHandler _handler;

        public static string Get { get; private set; }

        public static string GetById { get; private set; }

        public static string MaintainPersonPhone { get; private set; }

        public static string TemporaryTableName { get; private set; }

        public static string DropTemporaryTable { get; private set; }

        public static string CreateTemporaryTable { get; private set; }

        public static string MaintainFromTemporaryTable { get; private set; }

        public static string PersonPhoneTemporaryTableName { get; private set; }

        public static string DropPersonPhoneTemporaryTable { get; private set; }

        public static string CreatePersonPhoneTemporaryTable { get; private set; }

        public static string MaintainPersonPhoneFromTemporaryTable { get; private set; }

        static PersonRepository()
        {
            var personProperties = typeof(PersonDto).GetProperties().Select(p => p.Name).OrderBy(p => p);
            var personPropertiesWithoutId = personProperties.Where(n => !n.Equals(nameof(PersonDto.personid)));

            var addressProperties = typeof(AddressDto).GetProperties().Select(p => p.Name);
            var phoneProperties = typeof(PhoneDto).GetProperties().Select(p => p.Name);

            var personPhoneProperties = typeof(PersonPhoneDto).GetProperties().Select(p => p.Name)
                .Where(n => !n.Equals(nameof(PersonPhoneDto.personphoneid))).OrderBy(p => p);

            Get = $@"
            SELECT
                {string.Join(",", personProperties.Select(p => $"p.{p}"))},
                {string.Join(",", addressProperties.Select(a => $"a.{a}"))},
                {string.Join(",", phoneProperties.Select(ph => $"ph.{ph}"))}
            FROM tb_person p
            LEFT JOIN tb_address a ON a.{nameof(AddressDto.addressid)} = p.{nameof(PersonDto.addressid)}
            INNER JOIN tb_person_phone pp ON pp.{nameof(PersonPhoneDto.personid)} = p.{nameof(PersonDto.personid)}
            INNER JOIN tb_phone ph ON ph.{nameof(PhoneDto.phoneid)} = pp.{nameof(PersonPhoneDto.phoneid)}
            ";

            GetById = $@"{Get}
            WHERE p.{nameof(PersonDto.personid)} = @{nameof(PersonDto.personid)};
            ";

            MaintainPersonPhone = $@"
            WITH Phones ({nameof(PersonPhoneDto.phoneid)}) AS ({{0}})
            INSERT INTO tb_person_phone ({nameof(PersonPhoneDto.personid)}, {nameof(PersonPhoneDto.phoneid)})
            SELECT @{nameof(PersonPhoneDto.personid)}, TEMP.{nameof(PersonPhoneDto.phoneid)}
            FROM Phones TEMP
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_person_phone pp
                              WHERE pp.{nameof(PersonPhoneDto.personid)} = @{nameof(PersonPhoneDto.personid)}
                              AND pp.{nameof(PersonPhoneDto.phoneid)} = TEMP.{nameof(PersonPhoneDto.phoneid)});

            WITH Phones ({nameof(PersonPhoneDto.phoneid)}) AS ({{0}})
            DELETE FROM tb_person_phone pp1
            WHERE pp1.{nameof(PersonPhoneDto.personid)} = @{nameof(PersonPhoneDto.personid)}
            AND EXISTS (SELECT 1
                        FROM tb_person_phone pp2
                        WHERE pp2.{nameof(PersonPhoneDto.personphoneid)} = pp1.{nameof(PersonPhoneDto.personphoneid)}
                        AND NOT EXISTS (SELECT 1
                                        FROM Phones TEMP
                                        WHERE TEMP.{nameof(PersonPhoneDto.phoneid)} = pp2.{nameof(PersonPhoneDto.phoneid)}));
            ";

            TemporaryTableName = @"temp_person";

            DropTemporaryTable = $@"DROP TABLE IF EXISTS {TemporaryTableName};";

            CreateTemporaryTable = $@"{DropTemporaryTable}

            CREATE TEMPORARY TABLE {TemporaryTableName}
            (
                {nameof(PersonDto.addressid)} INT NULL,
                {nameof(PersonDto.birthdate)} DATE NOT NULL,
                {nameof(PersonDto.document)} CHAR(11) NOT NULL,
	            {nameof(PersonDto.email)} VARCHAR(150) NOT NULL,
	            {nameof(PersonDto.gender)} INT NOT NULL,
                {nameof(PersonDto.mainphoneid)} INT NOT NULL,
                {nameof(PersonDto.name)} VARCHAR(80) NOT NULL,
                {nameof(PersonDto.personid)} INT NOT NULL
            );
            ";

            MaintainFromTemporaryTable = $@"
            DELETE PP FROM tb_person_phone PP
            WHERE NOT EXISTS (SELECT 1 FROM {TemporaryTableName} TEMP WHERE TEMP.{nameof(PersonDto.personid)} = PP.{nameof(PersonDto.personid)});

            DELETE P FROM tb_person P
            WHERE NOT EXISTS (SELECT 1 FROM {TemporaryTableName} TEMP WHERE TEMP.{nameof(PersonDto.personid)} = P.{nameof(PersonDto.personid)});

            UPDATE tb_person P
            INNER JOIN {TemporaryTableName} TEMP ON TEMP.{nameof(PersonDto.personid)} = P.{nameof(PersonDto.personid)}
            SET {string.Join(",", personPropertiesWithoutId.Select(p => $"P.{p} = TEMP.{p}"))}
            WHERE {string.Join(" AND ", personPropertiesWithoutId.Select(p => $"TEMP.{p} IS NOT NULL"))};

            INSERT INTO tb_person ({string.Join(",", personProperties.Select(p => p))})
            SELECT {string.Join(",", personProperties.Select(p => p))}
            FROM {TemporaryTableName} TEMP
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_person P
                              WHERE P.{nameof(PersonDto.personid)} = TEMP.{nameof(PersonDto.personid)});
            ";

            PersonPhoneTemporaryTableName = @"temp_person_phone";

            DropPersonPhoneTemporaryTable = $@"DROP TABLE IF EXISTS {PersonPhoneTemporaryTableName};";

            CreatePersonPhoneTemporaryTable = $@"{DropPersonPhoneTemporaryTable}

            CREATE TEMPORARY TABLE {PersonPhoneTemporaryTableName}
            (
                {nameof(PersonPhoneDto.personid)} INT NOT NULL,
                {nameof(PersonPhoneDto.personphoneid)} INT NULL,
	            {nameof(PersonPhoneDto.phoneid)} INT NOT NULL
            );
            ";

            MaintainPersonPhoneFromTemporaryTable = $@"
            DELETE PP FROM tb_person_phone PP
            WHERE NOT EXISTS (SELECT 1
                              FROM {PersonPhoneTemporaryTableName} TEMP
                              WHERE {string.Join(" AND ", personPhoneProperties.Select(p => $"TEMP.{p} = PP.{p}"))});

            INSERT INTO tb_person_phone ({string.Join(",", personPhoneProperties.Select(p => p))})
            SELECT {string.Join(",", personPhoneProperties.Select(p => p))}
            FROM {PersonPhoneTemporaryTableName} TEMP
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_person_phone P
                              WHERE {string.Join(" AND ", personPhoneProperties.Select(p => $"P.{p} = TEMP.{p}"))});
            ";
        }

        public PersonRepository
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

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            var command = new CommandDefinition
            (
                commandText: Get,
                transaction: _relationalConn.Transaction,
                commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
            );

            return (await _relationalConn
                .QueryAsync<CompletePersonDto, AddressDto, PhoneDto, CompletePersonDto>
                (
                    command: command,
                    map: PersonDataMapper.MapPersonDtoProperties,
                    splitOn: $"{nameof(PersonDto.personid)}, {nameof(AddressDto.addressid)}, {nameof(PhoneDto.phoneid)}"
                )
                .ConfigureAwait(false))
                .GroupBy(p => p.personid)
                .ToDictionary(p => p.Key, p => p.ToList())
                .Select(p => p.Value.ToPerson())
                .ToList();
        }

        public async Task<Person> GetByIdAsync(int id)
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            var command = new CommandDefinition
            (
                commandText: GetById,
                parameters: new Dictionary<string, object>
                {
                    { $"{nameof(PersonDto.personid)}", id }
                },
                transaction: _relationalConn.Transaction
            );

            return (await _relationalConn
                .QueryAsync<CompletePersonDto, AddressDto, PhoneDto, CompletePersonDto>
                (
                    command: command,
                    map: PersonDataMapper.MapPersonDtoProperties,
                    splitOn: $"{nameof(PersonDto.personid)}, {nameof(AddressDto.addressid)}, {nameof(PhoneDto.phoneid)}"
                )
                .ConfigureAwait(false))?.ToPerson();
        }

        public async Task UpdateContactInformationAsync(Person person)
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseWrite);

            //updating person data:
            await _relationalConn.UpdateAsync(person.ToPersonDto(), _relationalConn.Transaction)
                .ConfigureAwait(false);

            //updating personphone data:
            var i = 0;
            var parameters = person.Phones
                .ToDictionary(_ => $"@{nameof(PersonPhoneDto.phoneid)}{++i}", p => (object)p.Id);

            var cteClause = string.Join(" UNION ", parameters.Keys.Select(p => $"SELECT {p}"));
            parameters.Add($"@{nameof(PersonPhoneDto.personid)}", person.Id);

            var command = new CommandDefinition
            (
                commandText: string.Format(MaintainPersonPhone, cteClause),
                parameters: parameters,
                transaction: _relationalConn.Transaction,
                commandTimeout: (int)TimeSpan.FromMinutes(1).TotalSeconds
            );

            await _relationalConn.ExecuteAsync(command).ConfigureAwait(false);
        }

        public async Task CloneAsync(IEnumerable<Person> persons)
        {
            if (persons is null)
            {
                throw new ArgumentNullException(nameof(persons));
            }

            if (!persons.Any())
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

            //inserting:
            using var bulkOperation = _cloneConn
                .GetBulkOperation
                (
                    tableName: TemporaryTableName,
                    commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
                );

            foreach (var dto in persons.Select(dto => dto.ToPersonDto()))
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

            //updating personphone data:
            //creating temp table:
            var dtos = new List<PersonPhoneDto>();
            foreach (var person in persons)
            {
                dtos.AddRange(person.Phones.Select(p => new PersonPhoneDto
                {
                    personid = person.Id,
                    phoneid = p.Id
                }));
            }

            command = new CommandDefinition
            (
                commandText: CreatePersonPhoneTemporaryTable,
                transaction: _cloneConn.Transaction
            );

            await _cloneConn.ExecuteAsync(command).ConfigureAwait(false);

            //var inserting
            using var personPhoneBulkOperation = _cloneConn
                .GetBulkOperation
                (
                    tableName: PersonPhoneTemporaryTableName,
                    commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
                );

            foreach (var dto in dtos)
            {
                personPhoneBulkOperation.AddRow(dto.ToBulkDictionary());
            }

            await personPhoneBulkOperation.BulkInsertAsync().ConfigureAwait(false);

            //maintain:
            command = new CommandDefinition
            (
                commandText: MaintainPersonPhoneFromTemporaryTable,
                transaction: _cloneConn.Transaction,
                commandTimeout: (int)TimeSpan.FromMinutes(15).TotalSeconds
            );

            await _cloneConn.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
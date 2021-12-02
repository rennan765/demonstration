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
    public class PersonRepository : DisposableObject, IPersonRepository
    {
        private readonly IAsyncDbConnection _conn;
        private readonly IDataOperationHandler _handler;

        public static string GetById { get; private set; }
        public static string MaintainPersonPhone { get; private set; }

        static PersonRepository()
        {
            var personProperties = typeof(PersonDto).GetProperties().Select(p => p.Name);
            var addressProperties = typeof(AddressDto).GetProperties().Select(p => p.Name);
            var phoneProperties = typeof(PhoneDto).GetProperties().Select(p => p.Name);

            GetById = $@"
            SELECT
                {string.Join(",", personProperties.Select(p => $"p.{p}"))},
                {string.Join(",", addressProperties.Select(a => $"a.{a}"))},
                {string.Join(",", phoneProperties.Select(ph => $"ph.{ph}"))}
            FROM tb_person p
            LEFT JOIN tb_address a ON a.{nameof(AddressDto.addressid)} = p.{nameof(PersonDto.addressid)}
            INNER JOIN tb_person_phone pp ON pp.{nameof(PersonPhoneDto.personid)} = p.{nameof(PersonDto.personid)}
            INNER JOIN tb_phone ph ON ph.{nameof(PhoneDto.phoneid)} = pp.{nameof(PersonPhoneDto.phoneid)}
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
            WHERE EXISTS (SELECT 1
                          FROM tb_person_phone pp2
                          WHERE pp2.{nameof(PersonPhoneDto.personphoneid)} = pp1.{nameof(PersonPhoneDto.personphoneid)}
                          AND NOT EXISTS (SELECT 1
                                          FROM Phones TEMP
                                          WHERE TEMP.{nameof(PersonPhoneDto.personid)} = @{nameof(PersonPhoneDto.personid)}
                                          AND TEMP.{nameof(PersonPhoneDto.phoneid)} = pp2.{nameof(PersonPhoneDto.phoneid)}));
            ";
        }

        public PersonRepository(IAsyncDbConnection conn, IDataOperationHandler handler)
            : base(new IDisposable[] { conn })
        {
            _conn = conn ?? throw new ArgumentNullException(nameof(conn));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
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
                transaction: _conn.Transaction
            );

            return (await _conn
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
            await _conn.UpdateAsync(person.ToPersonDto(), _conn.Transaction)
                .ConfigureAwait(false);

            //updating personphone data:
            var i = 0;
            var parameters = person.Phones
                .ToDictionary(_ => $"@{nameof(PersonPhoneDto.phoneid)}{++i}", p => (object)p.Id);

            var whereClause = string.Join(",", parameters.Keys);
            parameters.Add($"{nameof(PersonPhoneDto.personid)}", person.Id);

            var command = new CommandDefinition
            (
                commandText: string.Format(MaintainPersonPhone, whereClause),
                parameters: parameters,
                transaction: _conn.Transaction,
                commandTimeout: (int)TimeSpan.FromMinutes(1).TotalSeconds
            );

            await _conn.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
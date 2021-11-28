using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Data.Connection;
using _4oito6.Demonstration.Data.Model;
using _4oito6.Demonstration.Domain.Data.Transaction;
using _4oito6.Demonstration.Domain.Data.Transaction.Model;
using _4oito6.Demonstration.Person.Domain.Data.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Person.Data.Repositories
{
    using _4oito6.Demonstration.Domain.Model.Entities;

    public class PersonRepository : DisposableObject, IPersonRepository
    {
        private readonly IAsyncDbConnection _conn;
        private readonly IDataOperationHandler _handler;

        public static string Get { get; private set; }
        public static string GetById { get; private set; }

        public static string GetByEmail { get; private set; }
        public static string GetByEmailOrDocument { get; private set; }

        public static string GetPhonesByPersonId { get; private set; }
        public static string MaintainPersonPhone { get; private set; }

        static PersonRepository()
        {
            var personProperties = typeof(PersonDto).GetProperties().Select(p => p.Name);
            var addressProperties = typeof(AddressDto).GetProperties().Select(p => p.Name);
            var phoneProperties = typeof(PhoneDto).GetProperties().Select(p => p.Name);

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
            WHERE p.{nameof(PersonDto.personid)} = @{nameof(PersonDto.personid)}";

            GetByEmail = $@"{Get}
            WHERE p.{nameof(PersonDto.email)} = @{nameof(PersonDto.email)}
            ";

            GetByEmailOrDocument = $@"{GetByEmail}
            OR p.{nameof(PersonDto.document)} = @{nameof(PersonDto.document)}
            ";

            GetPhonesByPersonId = $@"
            SELECT
                {string.Join(",", phoneProperties.Select(ph => $"ph.{ph}"))}
            FROM tb_phone ph
            WHERE EXISTS (SELECT 1
                          FROM tb_person_phone pp
                          WHERE pp.{nameof(PersonPhoneDto.phoneid)} = ph.{nameof(PhoneDto.phoneid)}
                          AND pp.{nameof(PersonPhoneDto.personid)} = @{nameof(PersonDto.personid)})
            ";

            MaintainPersonPhone = $@"
            WITH Phones ({nameof(PhoneDto.type)}, {nameof(PhoneDto.code)}, {nameof(PhoneDto.number)}) ({{0}})
            INSERT INTO tb_phone ({nameof(PhoneDto.type)}, {nameof(PhoneDto.code)}, {nameof(PhoneDto.number)})
            SELECT ({nameof(PhoneDto.type)}, {nameof(PhoneDto.code)}, {nameof(PhoneDto.number)})
            FROM Phone TEMP
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_phone p
                              WHERE p.{nameof(PhoneDto.type)} = TEMP.{nameof(PhoneDto.type)}
                              AND p.{nameof(PhoneDto.code)} = TEMP.{nameof(PhoneDto.code)}
                              AND p.{nameof(PhoneDto.number)} = TEMP.{nameof(PhoneDto.number)});

            WITH Phones ({nameof(PhoneDto.type)}, {nameof(PhoneDto.code)}, {nameof(PhoneDto.number)}) ({{0}})
            INSERT INTO tb_person_phone ({nameof(PersonPhoneDto.personid)}, {nameof(PersonPhoneDto.phoneid)})
            SELECT @({nameof(PersonPhoneDto.personid)}, p.({nameof(PhoneDto.phoneid)}
            FROM tb_phone p
            WHERE NOT EXISTS (SELECT 1
                              FROM Phones TEMP
                              WHERE p.{nameof(PhoneDto.type)} = TEMP.{nameof(PhoneDto.type)}
                              AND p.{nameof(PhoneDto.code)} = TEMP.{nameof(PhoneDto.code)}
                              AND p.{nameof(PhoneDto.number)} = TEMP.{nameof(PhoneDto.number)});

            WITH Phones ({nameof(PhoneDto.type)}, {nameof(PhoneDto.code)}, {nameof(PhoneDto.number)}) ({{0}})
            DELETE FROM tb_person_phone pp1
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_person_phone pp2
                              INNER JOIN tb_phone p ON p.{nameof(PhoneDto.phoneid)} = pp2.{nameof(PersonPhoneDto.phoneid)}
                              WHERE pp2.{nameof(PersonPhoneDto.personphoneid)} = pp1.{nameof(PersonPhoneDto.personphoneid)}
                              AND EXISTS (SELECT 1
                                          FROM Phones TEMP
                                          WHERE p.{nameof(PhoneDto.type)} = TEMP.{nameof(PhoneDto.type)}
                                          AND p.{nameof(PhoneDto.code)} = TEMP.{nameof(PhoneDto.code)}
                                          AND p.{nameof(PhoneDto.number)} = TEMP.{nameof(PhoneDto.number)}));
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

        public async Task<Person> GetByEmailAsync(string email)
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            var command = new CommandDefinition
            (
                commandText: GetByEmail,
                parameters: new Dictionary<string, object>
                {
                    { $"{nameof(PersonDto.email)}", email }
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

        public async Task<Person> GetByEmailOrDocumentAsync(string email, string document)
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseRead);

            var command = new CommandDefinition
            (
                commandText: GetByEmailOrDocument,
                parameters: new Dictionary<string, object>
                {
                    { $"{nameof(PersonDto.email)}", email },
                    { $"{nameof(PersonDto.document)}", document }
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

        public async Task<Person> InsertAsync(Person person)
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseWrite);
            var personDto = person.ToPersonDto();

            //inserting address:
            AddressDto? addressDto = null;
            if (person.Address != null)
            {
                addressDto = person.Address.ToAddress();
                addressDto.addressid = await _conn.InsertAsync(addressDto, _conn.Transaction).ConfigureAwait(false);
                personDto.addressid = addressDto.addressid;
            }

            //inserting phones
            var phoneDtos = person.Phones.ToDictionary(x => x.ToString(), x => x.ToPhoneDto());
            foreach (var dto in phoneDtos)
            {
                dto.Value.phoneid = await _conn.InsertAsync(dto.Value, _conn.Transaction).ConfigureAwait(false);
            }

            personDto.mainphoneid = phoneDtos[person.MainPhone.ToString()].phoneid;

            //inserting person:
            personDto.personid = await _conn.InsertAsync(personDto, _conn.Transaction).ConfigureAwait(false);

            //inserting phone join
            foreach (var phoneDto in phoneDtos.Values)
            {
                await _conn
                    .InsertAsync(new PersonPhoneDto
                    {
                        personid = personDto.personid,
                        phoneid = phoneDto.phoneid
                    }, _conn.Transaction)
                    .ConfigureAwait(false);
            }

            return personDto.ToPerson(addressDto, phoneDtos.Values);
        }

        public async Task<Person> UpdateAsync(Person person)
        {
            _handler.NotifyDataOperation(DataOperation.RelationalDatabaseWrite);
            var personDto = person.ToPersonDto();

            //inserting address:
            AddressDto? addressDto = null;
            if (person.Address != null)
            {
                addressDto = person.Address.ToAddress();
                addressDto.addressid = await _conn.InsertAsync(addressDto, _conn.Transaction).ConfigureAwait(false);
                personDto.addressid = addressDto.addressid;
            }

            //maintain phones:
            var i = 0;
            var ctlClause = string.Empty;
            var parameters = new Dictionary<string, object>();

            foreach (var dto in person.Phones.Select(dto => dto.ToPhoneDto()))
            {
                string typeParameterName = $"@{nameof(PhoneDto.type)}{i}";
                string codeParameterName = $"@{nameof(PhoneDto.code)}{i}";
                string numberParameterName = $"@{nameof(PhoneDto.number)}{i}";

                parameters.Add(typeParameterName, dto.type);
                parameters.Add(codeParameterName, dto.code);
                parameters.Add(numberParameterName, dto.number);

                var clause = $"SELECT {typeParameterName}, {codeParameterName}, {numberParameterName}";
                ctlClause = ctlClause == string.Empty ? clause : $"{ctlClause} UNION {clause}";

                i++;
            }

            parameters.Add($"@{nameof(PersonPhoneDto.personid)}", personDto.personid);

            var command = new CommandDefinition
            (
                commandText: string.Format(MaintainPersonPhone, ctlClause),
                parameters: parameters,
                transaction: _conn.Transaction
            );

            await _conn.ExecuteAsync(command).ConfigureAwait(false);

            parameters = new Dictionary<string, object>
            {
                { $"@{nameof(PersonDto.personid)})", personDto.personid }
            };

            command = new CommandDefinition
            (
                commandText: GetPhonesByPersonId,
                parameters: parameters,
                transaction: _conn.Transaction
            );

            var phoneDtos = (await _conn.QueryAsync<PhoneDto>(command).ConfigureAwait(false))
                .ToDictionary(x => $"{x.type}|{x.code}|{x.number}");

            var mainPhoneDto = person.MainPhone.ToPhoneDto();
            personDto.mainphoneid = phoneDtos[$"{mainPhoneDto.type}|{mainPhoneDto.code}|{mainPhoneDto.number}"].phoneid;

            //updating person:
            await _conn.UpdateAsync(personDto, _conn.Transaction).ConfigureAwait(false);

            return personDto.ToPerson(addressDto, phoneDtos.Values);
        }
    }
}
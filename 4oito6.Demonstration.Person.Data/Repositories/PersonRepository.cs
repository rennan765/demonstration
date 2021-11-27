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
            LEFT JOIN tb_address a ON a.{nameof(AddressDto.Id)} = p.{nameof(PersonDto.AddressId)}
            INNER JOIN tb_person_phone pp ON pp.{nameof(PersonPhoneDto.PersonId)} = p.{nameof(PersonDto.Id)}
            INNER JOIN tb_phone ph ON ph.{nameof(PhoneDto.Id)} = pp.{nameof(PersonPhoneDto.PhoneId)}
            ";

            GetById = $@"{Get}
            WHERE p.{nameof(PersonDto.Id)} = @{nameof(PersonDto.Id)}";

            GetByEmailOrDocument = $@"{Get}
            WHERE p.{nameof(PersonDto.Email)} = COALESCE(@{nameof(PersonDto.Email)}, p.{nameof(PersonDto.Email)})
            OR p.{nameof(PersonDto.Document)} = COALESCE(@{nameof(PersonDto.Document)}, p.{nameof(PersonDto.Document)})
            ";

            GetPhonesByPersonId = $@"
            SELECT
                {string.Join(",", phoneProperties.Select(ph => $"ph.{ph}"))}
            FROM tb_phone ph
            WHERE EXISTS (SELECT 1
                          FROM tb_person_phone pp
                          WHERE pp.{nameof(PersonPhoneDto.PhoneId)} = ph.{nameof(PhoneDto.Id)}
                          AND pp.{nameof(PersonPhoneDto.PersonId)} = @{nameof(PersonDto.Id)})
            ";

            MaintainPersonPhone = $@"
            WITH Phones ({nameof(PhoneDto.Type)}, {nameof(PhoneDto.Code)}, {nameof(PhoneDto.Number)}) ({{0}})
            INSERT INTO tb_phone ({nameof(PhoneDto.Type)}, {nameof(PhoneDto.Code)}, {nameof(PhoneDto.Number)})
            SELECT ({nameof(PhoneDto.Type)}, {nameof(PhoneDto.Code)}, {nameof(PhoneDto.Number)})
            FROM Phone TEMP
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_phone p
                              WHERE p.{nameof(PhoneDto.Type)} = TEMP.{nameof(PhoneDto.Type)}
                              AND p.{nameof(PhoneDto.Code)} = TEMP.{nameof(PhoneDto.Code)}
                              AND p.{nameof(PhoneDto.Number)} = TEMP.{nameof(PhoneDto.Number)});

            WITH Phones ({nameof(PhoneDto.Type)}, {nameof(PhoneDto.Code)}, {nameof(PhoneDto.Number)}) ({{0}})
            INSERT INTO tb_person_phone ({nameof(PersonPhoneDto.PersonId)}, {nameof(PersonPhoneDto.PhoneId)})
            SELECT @({nameof(PersonPhoneDto.PersonId)}, p.({nameof(PhoneDto.Id)}
            FROM tb_phone p
            WHERE NOT EXISTS (SELECT 1
                              FROM Phones TEMP
                              WHERE p.{nameof(PhoneDto.Type)} = TEMP.{nameof(PhoneDto.Type)}
                              AND p.{nameof(PhoneDto.Code)} = TEMP.{nameof(PhoneDto.Code)}
                              AND p.{nameof(PhoneDto.Number)} = TEMP.{nameof(PhoneDto.Number)});

            WITH Phones ({nameof(PhoneDto.Type)}, {nameof(PhoneDto.Code)}, {nameof(PhoneDto.Number)}) ({{0}})
            DELETE FROM tb_person_phone pp1
            WHERE NOT EXISTS (SELECT 1
                              FROM tb_person_phone pp2
                              INNER JOIN tb_phone p ON p.{nameof(PhoneDto.Id)} = pp2.{nameof(PersonPhoneDto.PhoneId)}
                              WHERE pp2.{nameof(PersonPhoneDto.Id)} = pp1.{nameof(PersonPhoneDto.Id)}
                              AND EXISTS (SELECT 1
                                          FROM Phones TEMP
                                          WHERE p.{nameof(PhoneDto.Type)} = TEMP.{nameof(PhoneDto.Type)}
                                          AND p.{nameof(PhoneDto.Code)} = TEMP.{nameof(PhoneDto.Code)}
                                          AND p.{nameof(PhoneDto.Number)} = TEMP.{nameof(PhoneDto.Number)}));
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
                    { $"{nameof(PersonDto.Id)}", id }
                },
                transaction: _conn.Transaction
            );

            return (await _conn
                .QueryAsync<CompletePersonDto, AddressDto, PhoneDto, CompletePersonDto>
                (
                    command: command,
                    map: PersonDataMapper.MapPersonDtoProperties,
                    splitOn: $"{nameof(PersonDto.Id)}, {nameof(PersonDto.AddressId)}, {nameof(PersonDto.MainPhoneId)}"
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
                    { $"{nameof(PersonDto.Email)}", email },
                    { $"{nameof(PersonDto.Document)}", document }
                },
                transaction: _conn.Transaction
            );

            return (await _conn
                .QueryAsync<CompletePersonDto, AddressDto, PhoneDto, CompletePersonDto>
                (
                    command: command,
                    map: PersonDataMapper.MapPersonDtoProperties,
                    splitOn: $"{nameof(PersonDto.Id)}, {nameof(PersonDto.AddressId)}, {nameof(PersonDto.MainPhoneId)}"
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
                addressDto.Id = await _conn.InsertAsync(addressDto, _conn.Transaction).ConfigureAwait(false);
                personDto.AddressId = addressDto.Id;
            }

            //inserting phones
            var phoneDtos = person.Phones.ToDictionary(x => x.ToString(), x => x.ToPhoneDto());
            foreach (var dto in phoneDtos)
            {
                dto.Value.Id = await _conn.InsertAsync(dto.Value, _conn.Transaction).ConfigureAwait(false);
            }

            personDto.MainPhoneId = phoneDtos[person.MainPhone.ToString()].Id;

            //inserting person:
            personDto.Id = await _conn.InsertAsync(personDto, _conn.Transaction).ConfigureAwait(false);

            //inserting phone join
            foreach (var phoneDto in phoneDtos.Values)
            {
                await _conn
                    .InsertAsync(new PersonPhoneDto
                    {
                        PersonId = personDto.Id,
                        PhoneId = phoneDto.Id
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
                addressDto.Id = await _conn.InsertAsync(addressDto, _conn.Transaction).ConfigureAwait(false);
                personDto.AddressId = addressDto.Id;
            }

            //maintain phones:
            var i = 0;
            var ctlClause = string.Empty;
            var parameters = new Dictionary<string, object>();

            foreach (var dto in person.Phones.Select(dto => dto.ToPhoneDto()))
            {
                string typeParameterName = $"@{nameof(PhoneDto.Type)}{i}";
                string codeParameterName = $"@{nameof(PhoneDto.Code)}{i}";
                string numberParameterName = $"@{nameof(PhoneDto.Number)}{i}";

                parameters.Add(typeParameterName, dto.Type);
                parameters.Add(codeParameterName, dto.Code);
                parameters.Add(numberParameterName, dto.Number);

                var clause = $"SELECT {typeParameterName}, {codeParameterName}, {numberParameterName}";
                ctlClause = ctlClause == string.Empty ? clause : $"{ctlClause} UNION {clause}";

                i++;
            }

            parameters.Add($"@{nameof(PersonPhoneDto.PersonId)}", personDto.Id);

            var command = new CommandDefinition
            (
                commandText: string.Format(MaintainPersonPhone, ctlClause),
                parameters: parameters,
                transaction: _conn.Transaction
            );

            await _conn.ExecuteAsync(command).ConfigureAwait(false);

            parameters = new Dictionary<string, object>
            {
                { $"@{nameof(PersonDto.Id)})", personDto.Id }
            };

            command = new CommandDefinition
            (
                commandText: GetPhonesByPersonId,
                parameters: parameters,
                transaction: _conn.Transaction
            );

            var phoneDtos = (await _conn.QueryAsync<PhoneDto>(command).ConfigureAwait(false))
                .ToDictionary(x => $"{x.Type}|{x.Code}|{x.Number}");

            var mainPhoneDto = person.MainPhone.ToPhoneDto();
            personDto.MainPhoneId = phoneDtos[$"{mainPhoneDto.Type}|{mainPhoneDto.Code}|{mainPhoneDto.Number}"].Id;

            //updating person:
            await _conn.UpdateAsync(personDto, _conn.Transaction).ConfigureAwait(false);

            return personDto.ToPerson(addressDto, phoneDtos.Values);
        }
    }
}
using _4oito6.Demonstration.Contact.Data.Repositories;
using _4oito6.Demonstration.Contact.Domain.Data.Repositories;
using _4oito6.Demonstration.Contact.Domain.Data.Transaction;
using _4oito6.Demonstration.Data.Connection;
using _4oito6.Demonstration.Data.Connection.MySql;
using _4oito6.Demonstration.Data.Transaction;
using _4oito6.Demonstration.Domain.Data.Transaction.Enuns;
using System;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Contact.Data.Transaction
{
    [ExcludeFromCodeCoverage]
    public class ContactUnitOfWork : BaseUnitOfWork, IContactUnitOfWork
    {
        private readonly IAsyncDbConnection _relationalDatabase;
        private readonly IMySqlAsyncDbConnection _cloneDatabase;

        private IPersonRepository _person;
        private IPhoneRepository _phone;
        private IAddressRepository _address;

        public ContactUnitOfWork
        (
            IAsyncDbConnection relationalDatabase,
            IMySqlAsyncDbConnection cloneDatabase
        ) : base()
        {
            _relationalDatabase = relationalDatabase ?? throw new ArgumentNullException(nameof(relationalDatabase));
            Attach(relationalDatabase, DataSource.RelationalDatabase);

            _cloneDatabase = cloneDatabase ?? throw new ArgumentNullException(nameof(cloneDatabase));
            Attach(cloneDatabase, DataSource.CloneDatabase);
        }

        public IPersonRepository Person => _person ??= new PersonRepository(_relationalDatabase, _cloneDatabase, this);

        public IPhoneRepository Phone => _phone ??= new PhoneRepository(_relationalDatabase, _cloneDatabase, this);

        public IAddressRepository Address => _address ??= new AddressRepository(_relationalDatabase, _cloneDatabase, this);

        public override void Dispose()
        {
            base.Dispose();

            _person?.Dispose();
            _person = null;

            _phone?.Dispose();
            _phone = null;

            _address?.Dispose();
            _address = null;
        }
    }
}
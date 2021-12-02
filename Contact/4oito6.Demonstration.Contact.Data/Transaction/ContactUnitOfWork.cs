using _4oito6.Demonstration.Contact.Data.Repositories;
using _4oito6.Demonstration.Contact.Domain.Data.Repositories;
using _4oito6.Demonstration.Contact.Domain.Data.Transaction;
using _4oito6.Demonstration.Data.Connection;
using _4oito6.Demonstration.Data.Transaction;
using _4oito6.Demonstration.Domain.Data.Transaction.Enuns;
using System;

namespace _4oito6.Demonstration.Contact.Data.Transaction
{
    public class ContactUnitOfWork : BaseUnitOfWork, IContactUnitOfWork
    {
        private readonly IAsyncDbConnection _conn;

        private IPersonRepository _person;
        private IPhoneRepository _phone;
        private IAddressRepository _address;

        public ContactUnitOfWork(IAsyncDbConnection conn) : base()
        {
            _conn = conn ?? throw new ArgumentNullException(nameof(conn));
            Attach(conn, DataSource.RelationalDatabase);
        }

        public IPersonRepository Person => _person ??= new PersonRepository(_conn, this);

        public IPhoneRepository Phone => _phone ??= new PhoneRepository(_conn, this);

        public IAddressRepository Address => _address ??= new AddressRepository(_conn, this);

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
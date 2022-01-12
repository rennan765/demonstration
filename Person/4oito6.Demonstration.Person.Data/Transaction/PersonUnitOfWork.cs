using _4oito6.Demonstration.Data.Connection;
using _4oito6.Demonstration.Data.Transaction;
using _4oito6.Demonstration.Domain.Data.Transaction.Enuns;
using _4oito6.Demonstration.Person.Data.Repositories;
using _4oito6.Demonstration.Person.Domain.Data.Repositories;
using _4oito6.Demonstration.Person.Domain.Data.Repositories.NonTransactional;
using _4oito6.Demonstration.Person.Domain.Data.Transaction;
using System;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Person.Data.Transaction
{
    [ExcludeFromCodeCoverage]
    public class PersonUnitOfWork : BaseUnitOfWork, IPersonUnitOfWork
    {
        private IAsyncDbConnection _relationalDatabase;

        private IPersonRepository _person;

        public PersonUnitOfWork(IAsyncDbConnection relationalDatabase, IPersonNonTransactionRepositoryRoot nonTransactional)
        {
            _relationalDatabase = relationalDatabase ?? throw new ArgumentNullException(nameof(relationalDatabase));
            Attach(relationalDatabase, DataSource.RelationalDatabase);

            NonTransactional = nonTransactional ?? throw new ArgumentNullException(nameof(nonTransactional));
        }

        public IPersonRepository Person => _person ??= new PersonRepository(_relationalDatabase, this, NonTransactional.PersonQueue);

        public IPersonNonTransactionRepositoryRoot NonTransactional { get; private set; }

        public override void Dispose()
        {
            base.Dispose();

            _relationalDatabase?.Dispose();
            _relationalDatabase = null;

            _person?.Dispose();
            _person = null;

            NonTransactional?.Dispose();
            NonTransactional = null;
        }
    }
}
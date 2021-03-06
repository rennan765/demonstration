using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Person.Domain.Data.Repositories.NonTransactional;
using _4oito6.Demonstration.SQS.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Person.Data.Repositories.NonTransactional
{
    [ExcludeFromCodeCoverage]
    public class PersonNonTransactionRepositoryRoot : DisposableObject, IPersonNonTransactionRepositoryRoot
    {
        public PersonNonTransactionRepositoryRoot(ISQSHelper personQueue)
            : base(new IDisposable[] { personQueue })
        {
            PersonQueue = personQueue ?? throw new ArgumentNullException(nameof(personQueue));
        }

        public ISQSHelper PersonQueue { get; private set; }
    }
}
using _4oito6.Demonstration.Domain.Data.Repositories.NonTransactional;
using _4oito6.Demonstration.SQS.Interfaces;

namespace _4oito6.Demonstration.Person.Domain.Data.Repositories.NonTransactional
{
    public interface IPersonNonTransactionRepositoryRoot : INonTransactionRepositoryRootBase
    {
        ISQSHelper PersonQueue { get; }
    }
}
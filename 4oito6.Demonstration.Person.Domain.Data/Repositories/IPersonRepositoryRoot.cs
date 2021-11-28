using _4oito6.Demonstration.Domain.Data.Repositories;

namespace _4oito6.Demonstration.Person.Domain.Data.Repositories
{
    public interface IPersonRepositoryRoot : IBaseRepositoryRoot
    {
        IPersonRepository Person { get; }
    }
}
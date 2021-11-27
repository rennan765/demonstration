using _4oito6.Demonstration.Domain.Data.Repositories;

namespace _4oito6.Demonstration.Domain.Data.Transaction
{
    public interface IBaseUnitOfWork : IBaseRepositoryRoot, IDataOperationHandler
    {
        void Commit();

        void Rollback();

        void CloseConnections();
    }
}
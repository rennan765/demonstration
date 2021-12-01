using _4oito6.Demonstration.Domain.Data.Transaction.Model;

namespace _4oito6.Demonstration.Domain.Data.Transaction
{
    public interface IDataOperationHandler
    {
        void NotifyDataOperation(DataOperation dataOperation);
    }
}
using _4oito6.Demonstration.Domain.Data.Transaction.Enuns;

namespace _4oito6.Demonstration.Domain.Data.Transaction.Model
{
    public class DataOperation
    {
        public static DataOperation RelationalDatabaseRead { get; private set; }
        public static DataOperation RelationalDatabaseWrite { get; private set; }

        public DataOperation()
        {
            RelationalDatabaseRead = new DataOperation(DataSource.RelationalDatabase, OperationType.Read);
            RelationalDatabaseWrite = new DataOperation(DataSource.RelationalDatabase, OperationType.Write);
        }

        public DataOperation(DataSource dataSource, OperationType operationType)
        {
            DataSource = dataSource;
            OperationType = operationType;
        }

        public DataSource DataSource { get; private set; }
        public OperationType OperationType { get; private set; }
    }
}
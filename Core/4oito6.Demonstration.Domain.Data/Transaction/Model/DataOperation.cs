using _4oito6.Demonstration.Domain.Data.Transaction.Enuns;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Domain.Data.Transaction.Model
{
    [ExcludeFromCodeCoverage]
    public class DataOperation
    {
        public static DataOperation RelationalDatabaseRead { get; private set; }
        public static DataOperation RelationalDatabaseWrite { get; private set; }

        public static DataOperation CloneDatabaseRead { get; private set; }
        public static DataOperation CloneDatabaseWrite { get; private set; }

        static DataOperation()
        {
            RelationalDatabaseRead = new DataOperation(DataSource.RelationalDatabase, OperationType.Read);
            RelationalDatabaseWrite = new DataOperation(DataSource.RelationalDatabase, OperationType.Write);

            CloneDatabaseRead = new DataOperation(DataSource.CloneDatabase, OperationType.Read);
            CloneDatabaseWrite = new DataOperation(DataSource.CloneDatabase, OperationType.Write);
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
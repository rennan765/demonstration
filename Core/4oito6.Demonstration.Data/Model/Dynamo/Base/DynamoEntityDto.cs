using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Data.Model.Dynamo.Base
{
    [ExcludeFromCodeCoverage]
    public abstract class DynamoEntityDto
    {
        public abstract string TableName { get; }

        public abstract string Namespace { get; }
    }
}
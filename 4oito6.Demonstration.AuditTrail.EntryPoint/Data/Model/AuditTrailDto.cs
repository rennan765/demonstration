using _4oito6.Demonstration.Data.Model.Dynamo.Base;
using System;

namespace _4oito6.Demonstration.AuditTrail.EntryPoint.Data.Model
{
    public class AuditTrailDto : DynamoEntityDto
    {
        public override string TableName => "AuditTrail";

        public override string Namespace { get; }

        public string Id { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }

        public string AdditionalInformation { get; set; }

        public long DateTicks { get; set; }
    }
}
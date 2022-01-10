using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.AuditTrail.Sender.Arguments
{
    [ExcludeFromCodeCoverage]
    public class AuditTrailRequest
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string AdditionalInformation { get; set; }
    }
}
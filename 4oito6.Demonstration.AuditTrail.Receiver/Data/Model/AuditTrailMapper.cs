using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;

namespace _4oito6.Demonstration.AuditTrail.Receiver.Data.Model
{
    public static class AuditTrailMapper
    {
        public static AuditTrailDto ToAuditTrailDto(this AuditTrailMessage message)
        {
            return new AuditTrailDto
            {
                AdditionalInformation = message.AdditionalInformation,
                Code = message.Code,
                Date = message.Date,
                DateTicks = message.DateTicks,
                Id = message.Id.ToString(),
                Message = message.Message
            };
        }
    }
}
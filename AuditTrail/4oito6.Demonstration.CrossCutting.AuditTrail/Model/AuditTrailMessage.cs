using Newtonsoft.Json;
using System;

namespace _4oito6.Demonstration.CrossCutting.AuditTrail.Model
{
    public class AuditTrailMessage : ICloneable
    {
        private class AdditionalInformationDto
        {
            public string Message { get; set; }
            public string StackTrace { get; set; }
            public AdditionalInformationDto InnerAdditionalInformation { get; set; }
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string AdditionalInformation { get; set; }

        public long DateTicks => Date.Ticks;

        /// <summary>
        /// Simple constructor
        /// </summary>
        public AuditTrailMessage()
        {
            Id = Guid.NewGuid();
            Date = DateTime.UtcNow;
        }

        /// <summary>
        /// Basic constructor, auto filling date
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="additionalInformation"></param>
        public AuditTrailMessage(string code, string message, string additionalInformation = null) : this()
        {
            Code = code;
            Message = message;
            AdditionalInformation = additionalInformation;
        }

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <param name="additionalInformation"></param>
        public AuditTrailMessage(string code, string message, DateTime date, string additionalInformation = null)
            : this(code, message, additionalInformation)
        {
            Code = code;
            Message = message;
            Date = date;
            AdditionalInformation = additionalInformation;
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <param name="additionalInformation"></param>
        public AuditTrailMessage(Guid id, string code, string message, DateTime date, string additionalInformation = null)
            : this(code, message, date, additionalInformation)
        {
            Id = id;
        }

        private AdditionalInformationDto CreateAdditionalInformationDto(Exception ex)
        {
            var dto = new AdditionalInformationDto
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace
            };

            if (ex.InnerException != null)
            {
                dto.InnerAdditionalInformation = CreateAdditionalInformationDto(ex.InnerException);
            }

            return dto;
        }

        public void SetException(Exception ex)
        {
            if (ex is null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            AdditionalInformation = JsonConvert.SerializeObject(CreateAdditionalInformationDto(ex));
        }

        public object Clone()
        {
            return new AuditTrailMessage(Id, Code, Message, Date, AdditionalInformation);
        }

        public bool Match(AuditTrailMessage message, bool isConsiderDate = false)
        {
            var result = message.Code == Code &&
                message.Message == Message &&
                message.AdditionalInformation == AdditionalInformation;

            if (isConsiderDate)
            {
                return result && DateTime.Compare(message.Date, Date) == 0;
            }

            return result;
        }
    }
}
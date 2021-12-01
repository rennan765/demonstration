using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.CrossCutting.AuditTrail
{
    public class AuditTrailSender : DisposableObject, IAuditTrailSender
    {
        private readonly IAmazonSQS _sqs;
        private readonly string _queue;

        public AuditTrailSender(IAmazonSQS sqs, string queue) : base(new IDisposable[] { sqs })
        {
            _sqs = sqs ?? throw new ArgumentNullException(nameof(sqs));
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        public Task SendAsync(AuditTrailMessage message)
        {
            var request = new SendMessageRequest
            {
                DelaySeconds = 5,
                MessageBody = JsonConvert.SerializeObject(message),
                QueueUrl = _queue
            };

            return _sqs.SendMessageAsync(request);
        }

        public Task SendAsync(string code, string message, string additionalInformation = null)
        {
            return SendAsync(new AuditTrailMessage(code, message, additionalInformation));
        }
    }
}
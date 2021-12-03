using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.SQS.Interfaces;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.SQS
{
    public class SQSHelper : DisposableObject, ISQSHelper
    {
        private readonly IAmazonSQS _sqs;
        private readonly string _queue;

        public SQSHelper(IAmazonSQS sqs, string queue)
            : base(new IDisposable[] { sqs })
        {
            _sqs = sqs ?? throw new ArgumentNullException(nameof(sqs));
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        public async Task<T> GetAsync<T>()
        {
            var request = new ReceiveMessageRequest
            {
                MaxNumberOfMessages = 1,
                QueueUrl = _queue
            };

            var response = await _sqs.ReceiveMessageAsync(request).ConfigureAwait(false);
            var body = response?.Messages.FirstOrDefault()?.Body;

            if (string.IsNullOrEmpty(body))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(body);
        }
    }
}
using _4oito6.Demonstration.AuditTrail.Receiver;
using _4oito6.Demonstration.AuditTrail.Receiver.Application;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace QuatroOito6.Demonstration.AuditTrail.Receiver
{
    [ExcludeFromCodeCoverage]
    public class Function
    {
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
            }
        }

        private Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            context.Logger.LogLine($"Processed message {message.Body}");

            var auditMessage = JsonConvert.DeserializeObject<AuditTrailMessage>(message.Body);

            if (auditMessage is null)
            {
                return Task.CompletedTask;
            }

            using var appService = IoC.Provider.GetService<IAuditTrailAppServices>();
            return appService.ProcessMessageAsync(auditMessage);
        }
    }
}
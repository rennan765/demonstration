using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Text.Json;

namespace _4oito6.Demonstration.Person.EntryPoint.Middleware
{
    [ExcludeFromCodeCoverage]
    public class ExceptionHandlerMiddleware : DisposableObject
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IAuditTrailSender _auditTrail;

        public ExceptionHandlerMiddleware
        (
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IAuditTrailSender auditTrail
        ) : base(new IDisposable[] { auditTrail })
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _auditTrail = auditTrail ?? throw new ArgumentNullException(nameof(auditTrail));
        }

        private async Task TreatExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex.ToString());

            var message = string.Empty;
#if DEBUG
            message = ex.Message;
#else
            message = HttpStatusCode.InternalServerError.ToString();
#endif

            var autitTrailMessage = new AuditTrailMessage
            (
                code: "PERSON_EX_01",
                message: message
            );

            autitTrailMessage.SetException(ex);
            await _auditTrail.SendAsync(autitTrailMessage).ConfigureAwait(false);

            using var writer = new StreamWriter(context.Response.Body, Encoding.UTF8, 2048, true);
            var response = HttpStatusCode.InternalServerError;

            context.Response.StatusCode = (int)response;
            context.Response.Headers.Add("Content-Type", "application/json");

            await writer.WriteAsync(JsonSerializer.Serialize(response)).ConfigureAwait(false);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await TreatExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }
    }
}
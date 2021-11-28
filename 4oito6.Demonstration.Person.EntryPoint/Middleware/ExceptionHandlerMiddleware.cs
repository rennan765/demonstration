using System.Net;
using System.Text;
using System.Text.Json;

namespace _4oito6.Demonstration.Person.EntryPoint.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
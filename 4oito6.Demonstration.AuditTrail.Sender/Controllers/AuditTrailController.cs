using _4oito6.Demonstration.AuditTrail.Sender.Arguments;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace _4oito6.Demonstration.AuditTrail.Sender.Controllers
{
    [ApiController]
    [Route("audittrail")]
    public class AuditTrailController : Controller
    {
        private IAuditTrailSender _sender;

        public AuditTrailController(IAuditTrailSender sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost("send")]
        public async Task<IActionResult> SendAsync([FromBody] AuditTrailRequest request)
        {
            try
            {
                await _sender
                    .SendAsync(request.Code, request.Message, request.AdditionalInformation)
                    .ConfigureAwait(false);

                return Ok();
            }
            catch (Exception ex)
            {
                var message = new AuditTrailMessage
                (
                    code: "AUDITTRAIL_EX01",
                    message: "Erro ao adicionar trilha de auditoria via POST."
                );

                message.SetException(ex);
                await _sender.SendAsync(message).ConfigureAwait(false);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _sender?.Dispose();
                _sender = null;
            }
        }
    }
}

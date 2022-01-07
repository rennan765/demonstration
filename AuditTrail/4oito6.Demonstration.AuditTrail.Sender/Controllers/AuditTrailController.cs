using _4oito6.Demonstration.AuditTrail.Receiver.Application;
using _4oito6.Demonstration.AuditTrail.Sender.Arguments;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace _4oito6.Demonstration.AuditTrail.Sender.Controllers
{
    [ApiController]
    [Route("audittrail")]
    public class AuditTrailController : Controller
    {
        private IAuditTrailAppServices _appService;

        public AuditTrailController(IAuditTrailAppServices sender)
        {
            _appService = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost("send")]
        public async Task<IActionResult> SendAsync([FromBody] AuditTrailRequest request)
        {
            await _appService
                .SendAsync(request.Code, request.Message, request.AdditionalInformation)
                .ConfigureAwait(false);

            return StatusCode((int)_appService.HttpStatusCode);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _appService?.Dispose();
                _appService = null;
            }
        }
    }
}
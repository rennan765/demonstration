using _4oito6.Demonstration.Application.Interfaces;
using _4oito6.Demonstration.Application.Model;
using _4oito6.Demonstration.Person.Application.Interfaces;
using _4oito6.Demonstration.Person.Application.Model.Person;
using _4oito6.Demonstration.Person.EntryPoint.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace _4oito6.Demonstration.Person.EntryPoint.Controllers
{
    [ApiController]
    [Route("person")]
    public class PersonController : MainController
    {
        private readonly IPersonAppServices _appServices;

        public PersonController(IPersonAppServices appServices)
            : base(new IAppServiceBase[] { appServices })
        {
            _appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        }

        /// <summary>
        /// Create new person.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<Notification>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] PersonRequest request)
        {
            var response = await _appServices.CreateAsync(request).ConfigureAwait(false);
            return Result(response);
        }

        /// <summary>
        /// Update an existing person.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<Notification>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] PersonRequest request)
        {
            var response = await _appServices.UpdateAsync(id, request).ConfigureAwait(false);
            return Result(response);
        }
    }
}
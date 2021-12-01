using _4oito6.Demonstration.Application.Interfaces;
using _4oito6.Demonstration.Application.Model;
using _4oito6.Demonstration.Config.Model;
using _4oito6.Demonstration.Person.Application.Interfaces;
using _4oito6.Demonstration.Person.Application.Model.Person;
using _4oito6.Demonstration.Person.EntryPoint.Controllers.Base;
using _4oito6.Demonstration.Person.EntryPoint.IoC.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;

namespace _4oito6.Demonstration.Person.EntryPoint.Controllers
{
    [ApiController]
    [Route("person")]
    public class PersonController : MainController
    {
        private readonly TokenConfig _tokenConfig;
        private readonly IPersonAppServices _appServices;

        public PersonController(IPersonAppServices appServices, IHttpContextAccessor httpContextAccessor, IPersonConfig config)
            : base(httpContextAccessor, new IAppServiceBase[] { appServices })
        {
            _appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            _tokenConfig = config?.TokenConfig ?? throw new ArgumentNullException(nameof(config.TokenConfig));
        }

        [NonAction]
        public object GenerateToken(PersonResponse model)
        {
            var createDate = DateTime.UtcNow;
            var expirationDate = createDate + TimeSpan.FromSeconds(_tokenConfig.TokenTime);

            var identity = new ClaimsIdentity
            (
                new GenericIdentity(model.Id.ToString(), "Id"),
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim(typeof(PersonResponse).ToString(), JsonConvert.SerializeObject(model))
                }
            );

            HttpContextAccessor.HttpContext.User.AddIdentity(identity);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken
            (
                new SecurityTokenDescriptor
                {
                    Issuer = _tokenConfig.Issuer,
                    Audience = _tokenConfig.Audience,
                    SigningCredentials = _tokenConfig.SigningCredentials,
                    Subject = identity,
                    NotBefore = createDate,
                    Expires = expirationDate
                }
            );

            return handler.WriteToken(securityToken);
        }

        /// <summary>
        /// Get information by logged user
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var person = GetToken();
            var response = await _appServices.GetByIdAsync(person.Id).ConfigureAwait(false);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
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
        /// Log using an e-mail
        /// </summary>
        /// <param name="email"></param>
        /// <returns>JWT token</returns>
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<Notification>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] string email)
        {
            var response = await _appServices
                .GetByEmailAsync(email)
                .ConfigureAwait(false);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(GenerateToken(response));
        }

        /// <summary>
        /// Update current logged person.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<Notification>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] PersonRequest request)
        {
            var person = GetToken();
            var response = await _appServices.UpdateAsync(person.Id, request).ConfigureAwait(false);
            return Result(response);
        }
    }
}
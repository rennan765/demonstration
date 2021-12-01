using _4oito6.Demonstration.Application.Interfaces;
using _4oito6.Demonstration.Person.Application.Model.Person;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace _4oito6.Demonstration.Person.EntryPoint.Controllers.Base
{
    public class MainController : Controller
    {
        private readonly List<IAppServiceBase> _appServices;

        public MainController(IHttpContextAccessor httpContextAccessor, IEnumerable<IAppServiceBase> appServices)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _appServices = new List<IAppServiceBase>(appServices ?? throw new ArgumentNullException(nameof(appServices)));
        }

        protected IHttpContextAccessor HttpContextAccessor { get; private set; }

        [NonAction]
        protected ObjectResult Result(object? value)
        {
            if (_appServices.Any(app => !app.IsValid))
            {
                return StatusCode(_appServices.Max(app => (int)app.HttpStatusCode), _appServices.SelectMany(app => app.Notifications));
            }

            return StatusCode(_appServices.Min(app => (int)app.HttpStatusCode), value);
        }

        [NonAction]
        protected PersonResponse GetToken()
        {
            if (HttpContextAccessor.HttpContext.Items[typeof(PersonResponse).ToString()] is null)
            {
                var user = HttpContextAccessor.HttpContext.User.FindFirst(typeof(PersonResponse).ToString());

                if (user != null)
                {
                    if (HttpContextAccessor.HttpContext.Items[typeof(PersonResponse).ToString()] is null)
                    {
                        HttpContextAccessor.HttpContext.Items[typeof(PersonResponse).ToString()] = JsonConvert.DeserializeObject<PersonResponse>(user.Value);
                    }
                }
            }

            return (PersonResponse)HttpContextAccessor
                .HttpContext.Items[typeof(PersonResponse).ToString()];
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_appServices != null)
                {
                    _appServices.ForEach(app => app.Dispose());
                    _appServices.Clear();
                }
            }
        }
    }
}
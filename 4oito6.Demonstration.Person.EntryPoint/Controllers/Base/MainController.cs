using _4oito6.Demonstration.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace _4oito6.Demonstration.Person.EntryPoint.Controllers.Base
{
    public class MainController : Controller
    {
        private readonly List<IAppServiceBase> _appServices;

        public MainController(IEnumerable<IAppServiceBase> appServices)
        {
            _appServices = new List<IAppServiceBase>(appServices ?? throw new ArgumentNullException(nameof(appServices)));
        }

        [NonAction]
        protected ObjectResult Result(object? value)
        {
            if (_appServices.Any(app => !app.IsValid))
            {
                return StatusCode(_appServices.Max(app => (int)app.HttpStatusCode), _appServices.SelectMany(app => app.Notifications));
            }

            return StatusCode(_appServices.Min(app => (int)app.HttpStatusCode), value);
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
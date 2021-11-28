using _4oito6.Demonstration.Application.Interfaces;
using _4oito6.Demonstration.Application.Model;
using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Domain.Model.Entities.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace _4oito6.Demonstration.Application
{
    public abstract class AppServiceBase : DisposableObject, IAppServiceBase
    {
        private readonly List<Notification> _notifications;

        protected AppServiceBase(ILogger logger, IEnumerable<IDisposable> composition)
            : base(composition)
        {
            _notifications = new List<Notification>();
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            IsValid = true;
            HttpStatusCode = HttpStatusCode.OK;
        }

        public IEnumerable<Notification> Notifications => _notifications;

        protected ILogger Logger { get; private set; }

        public HttpStatusCode HttpStatusCode { get; private set; }

        public bool IsValid { get; private set; }

        protected void HandleException(Exception ex)
        {
            HttpStatusCode = HttpStatusCode.InternalServerError;
            _notifications.Add(new Notification("HttpStatusCode", HttpStatusCode.InternalServerError.ToString()));
            IsValid = false;
            Logger.LogError(ex.ToString());
        }

        protected void Notify(EntityBase entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (!entity.IsValid)
            {
                IsValid = false;

                _notifications.AddRange(entity.ValidationResults.
                    SelectMany(r => r.Errors)
                    .Select(vf => new Notification(vf.PropertyName, vf.ErrorMessage)));
            }
        }
    }
}
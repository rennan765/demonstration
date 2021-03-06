using _4oito6.Demonstration.Application.Interfaces;
using _4oito6.Demonstration.Application.Model;
using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using _4oito6.Demonstration.Domain.Model.Entities.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Application
{
    [ExcludeFromCodeCoverage]
    public abstract class AppServiceBase : DisposableObject, IAppServiceBase
    {
        private static DateTime? _lastHealthCheck;
        private static object _healthCheckSyncRoot;

        private readonly List<Notification> _notifications;

        static AppServiceBase()
        {
            _healthCheckSyncRoot = new object();
        }

        protected AppServiceBase
        (
            ILogger logger,
            IAuditTrailSender auditTrail,
            IEnumerable<IDisposable> composition
        ) : base(composition)
        {
            _notifications = new List<Notification>();
            IsValid = true;
            HttpStatusCode = HttpStatusCode.OK;

            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            AuditTrail = auditTrail ?? throw new ArgumentNullException(nameof(auditTrail));
        }

        public IEnumerable<Notification> Notifications => _notifications;

        protected ILogger Logger { get; private set; }

        public virtual IAuditTrailSender AuditTrail { get; private set; }

        public HttpStatusCode HttpStatusCode { get; protected set; }

        public bool IsValid { get; private set; }

        public virtual Task HandleExceptionAsync(Exception ex, string code = null)
        {
            HttpStatusCode = HttpStatusCode.InternalServerError;
            _notifications.Add(new Notification("HttpStatusCode", HttpStatusCode.InternalServerError.ToString()));

            IsValid = false;
            Logger.LogError(ex.ToString());

            var message = new AuditTrailMessage(code ?? "DEMONSTRATION_EX_01", ex.Message);
            message.SetException(ex);
            return AuditTrail.SendAsync(message);
        }

        public virtual void Notify(EntityBase entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (!entity.IsValid)
            {
                IsValid = false;
                HttpStatusCode = HttpStatusCode.BadRequest;

                _notifications.AddRange(entity.ValidationResults.
                    SelectMany(r => r.Errors)
                    .Select(vf => new Notification(vf.PropertyName, vf.ErrorMessage)));
            }
        }

        public virtual void HandleEmptyQueue(string message = "Empty queue.")
        {
            var now = DateTime.UtcNow;

            if (_lastHealthCheck is null || now.Subtract(_lastHealthCheck.Value).TotalHours > 1)
            {
                lock (_healthCheckSyncRoot)
                {
                    if (_lastHealthCheck is null || now.Subtract(_lastHealthCheck.Value).TotalHours > 1)
                    {
                        _lastHealthCheck = now;
                        Logger.LogInformation(message);
                    }
                }
            }
        }
    }
}
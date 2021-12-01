using _4oito6.Demonstration.Application.Model;
using System;
using System.Collections.Generic;
using System.Net;

namespace _4oito6.Demonstration.Application.Interfaces
{
    public interface IAppServiceBase : IDisposable
    {
        public bool IsValid { get; }
        IEnumerable<Notification> Notifications { get; }
        HttpStatusCode HttpStatusCode { get; }
    }
}
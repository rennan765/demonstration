using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Domain.Services.Interfaces
{
    public interface ICloningServices : IDisposable
    {
        Task CloneAsync();
    }
}
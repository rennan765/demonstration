using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.SQS.Interfaces
{
    public interface ISQSHelper : IDisposable
    {
        Task<T> GetAsync<T>();

        Task SendAsync<T>(T request);
    }
}
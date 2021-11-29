using _4oito6.Demonstration.AuditTrail.Receiver.Data.Model;
using _4oito6.Demonstration.AuditTrail.Receiver.Domain.Data;
using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using _4oito6.Demonstration.Data.Connection;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.AuditTrail.Receiver.Data
{
    public class AuditTrailRepository : DisposableObject, IAuditTrailRepository
    {
        private readonly IDynamoConnection<AuditTrailDto> _conn;

        public AuditTrailRepository(IDynamoConnection<AuditTrailDto> conn)
            : base(new IDisposable[] { conn })
        {
            _conn = conn ?? throw new ArgumentNullException(nameof(conn));
        }

        public Task InsertAsync(AuditTrailMessage message)
            => _conn.CreateAsync(message.ToAuditTrailDto());
    }
}
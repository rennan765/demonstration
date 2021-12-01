using System;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Commons
{
    public class DisposableObject : IDisposable
    {
        private bool _disposedValue;
        private readonly List<IDisposable> _composition;

        public DisposableObject(IEnumerable<IDisposable> composition)
        {
            _composition = (composition ?? throw new ArgumentNullException(nameof(composition))).ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _composition.ForEach(x => x.Dispose());
                    _composition.Clear();
                }

                _disposedValue = true;
            }
        }

        ~DisposableObject()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: false);
        }

        public virtual void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
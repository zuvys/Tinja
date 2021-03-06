﻿using System;
using Tinja.Abstractions.Injection;

namespace Tinja.Extensions.DependencyInjection
{
    public class ServiceProviderAdapter : IServiceProvider, IDisposable
    {
        private bool _disposed;

        private readonly IServiceResolver _resolver;

        public ServiceProviderAdapter(IServiceResolver resolver)
        {
            _resolver = resolver;
        }

        public virtual object GetService(Type serviceType)
        {
            return _resolver.ResolveService(serviceType);
        }

        ~ServiceProviderAdapter()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispoing)
        {
            if (!dispoing || _disposed)
            {
                return;
            }

            lock (this)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _resolver.Dispose();
                }
            }
        }
    }
}

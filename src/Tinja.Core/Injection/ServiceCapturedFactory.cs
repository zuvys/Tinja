﻿using System;
using Tinja.Abstractions.Injection;

namespace Tinja.Core.Injection
{
    internal class ServiceCapturedFactory : IServiceCapturedFactory
    {
        internal ServiceLifeScope Scope { get; }

        internal ServiceCapturedFactory(ServiceLifeScope scope)
        {
            Scope = scope;
        }

        public object CreateCapturedService(Func<IServiceResolver, object> factory)
        {
            if (Scope.IsDisposed)
            {
                throw new InvalidOperationException($"this scope is disposed!");
            }

            var service = factory(Scope.ServiceResolver);
            if (service is IDisposable disposable)
            {
                Scope.DisposableServices.Add(disposable);
            }

            return service;
        }

        public object CreateCapturedService(int serviceId, Func<IServiceResolver, object> factory)
        {
            if (Scope.IsDisposed)
            {
                throw new InvalidOperationException($"this scope is disposed!");
            }

            if (Scope.ResolvedServices.TryGetValue(serviceId, out var service))
            {
                return service;
            }

            lock (Scope.ResolvedServices)
            {
                if (Scope.ResolvedServices.TryGetValue(serviceId, out service))
                {
                    return service;
                }

                service = Scope.ResolvedServices[serviceId] = factory(Scope.ServiceResolver);

                if (service is IDisposable disposable)
                {
                    Scope.DisposableServices.Add(disposable);
                }

                return service;
            }
        }
    }
}
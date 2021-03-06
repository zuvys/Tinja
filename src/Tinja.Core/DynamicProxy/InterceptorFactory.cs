﻿using System;
using Tinja.Abstractions.DynamicProxy;
using Tinja.Abstractions.Extensions;
using Tinja.Abstractions.Injection;

namespace Tinja.Core.DynamicProxy
{
    [DisableProxy]
    public class InterceptorFactory : IInterceptorFactory
    {
        private readonly IServiceResolver _serviceResolver;

        public InterceptorFactory(IServiceResolver serviceResolver)
        {
            _serviceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));
        }

        public IInterceptor Create(Type interceptorType)
        {
            if (interceptorType == null)
            {
                throw new ArgumentNullException(nameof(interceptorType));
            }

            return _serviceResolver.ResolveServiceRequired<IInterceptor>(interceptorType);
        }
    }
}

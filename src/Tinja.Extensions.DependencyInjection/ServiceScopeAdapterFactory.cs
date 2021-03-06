﻿using Microsoft.Extensions.DependencyInjection;
using System;
using Tinja.Abstractions.Extensions;
using Tinja.Abstractions.Injection;
using Tinja.Core;

namespace Tinja.Extensions.DependencyInjection
{
    public class ServiceScopeAdapterFactory : IServiceScopeFactory
    {
        private readonly IServiceResolver _serviceResolver;

        public ServiceScopeAdapterFactory(IServiceResolver serviceResolver)
        {
            _serviceResolver = serviceResolver;
        }

        public IServiceScope CreateScope()
        {
            return new ServiceScopeAdapter(_serviceResolver.CreateScope().ServiceResolver.ResolveService<IServiceProvider>());
        }
    }
}

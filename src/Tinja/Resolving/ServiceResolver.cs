﻿using System;
using Tinja.ServiceLife;
using Tinja.Resolving.Activation;
using Tinja.Resolving.Context;
using Tinja.Resolving.Dependency.Builder;

namespace Tinja.Resolving
{
    public class ServiceResolver : IServiceResolver
    {
        /// <summary>
        /// <see cref="IServiceLifeScope"/>
        /// </summary>
        public IServiceLifeScope LifeScope { get; }

        /// <summary>
        /// <see cref="IResolvingContextBuilder"/>
        /// </summary>
        internal IResolvingContextBuilder ResolvingContextBuilder { get; }

        /// <summary>
        /// <see cref="IServiceActivatorProvider"/>
        /// </summary>
        internal IServiceActivatorProvider ServiceActivatorProvider { get; }

        static Func<IServiceResolver, IServiceLifeScope, object> DefaultFacotry = (resolver, scope) => null;

        public ServiceResolver(IResolvingContextBuilder builder, IServiceLifeScopeFactory scopeFactory)
        {
            LifeScope = scopeFactory.Create(this);
            ResolvingContextBuilder = builder;
            ServiceActivatorProvider = this.Resolve<IServiceActivatorProvider>();
        }

        internal ServiceResolver(IServiceResolver root)
        {
            LifeScope = root.Resolve<IServiceLifeScopeFactory>().Create(this, root.LifeScope);
            ResolvingContextBuilder = root.Resolve<IResolvingContextBuilder>();
            ServiceActivatorProvider = root.Resolve<IServiceActivatorProvider>();
        }

        public object Resolve(Type serviceType)
        {
            return GetFactory(serviceType)(this, LifeScope);
        }

        protected virtual Func<IServiceResolver, IServiceLifeScope, object> GetFactory(Type serviceType)
        {
            var factory = ServiceActivatorProvider?.Get(serviceType);
            if (factory != null)
            {
                return factory;
            }

            var context = ResolvingContextBuilder.BuildResolvingContext(serviceType);
            if (context == null)
            {
                return DefaultFacotry;
            }

            if (context.Component.ImplementionFactory != null)
            {
                return (resolver, scope) =>
                {
                    return scope.ApplyServiceLifeStyle(
                        context,
                        scopeResolver => context.Component.ImplementionFactory(scopeResolver)
                    );
                };
            }

            var chain = new ServiceDependencyBuilder(ResolvingContextBuilder).BuildDependChain(context);
            if (chain == null)
            {
                return DefaultFacotry;
            }

            return ServiceActivatorProvider.Get(chain) ?? DefaultFacotry;
        }

        public void Dispose()
        {
            LifeScope.Dispose();
        }
    }
}

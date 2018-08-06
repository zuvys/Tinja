﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tinja.Abstractions.DynamicProxy.Metadatas;
using Tinja.Abstractions.Extensions;

namespace Tinja.Core.DynamicProxy.Metadatas
{
    public class InterceptorMetadataCollector : IInterceptorMetadataCollector
    {
        protected ConcurrentDictionary<MemberInfo, IEnumerable<InterceptorMetadata>> Cache { get; }

        public InterceptorMetadataCollector()
        {
            Cache = new ConcurrentDictionary<MemberInfo, IEnumerable<InterceptorMetadata>>();
        }

        public IEnumerable<InterceptorMetadata> Collect(MemberMetadata metadata)
        {
            return Cache.GetOrAdd(metadata.Member, key => Collect(metadata.Member, metadata.InterfaceInherits.Select(item => item.Member).ToArray()));
        }

        protected virtual IEnumerable<InterceptorMetadata> Collect(MemberInfo memberInfo, MemberInfo[] implementsInterfaces)
        {
            foreach (var attr in memberInfo.DeclaringType.GetInterceptorAttributes())
            {
                yield return new InterceptorMetadata(attr.Order, attr.InterceptorType, memberInfo);
            }

            foreach (var attr in memberInfo.GetInterceptorAttributes())
            {
                yield return new InterceptorMetadata(attr.Order, attr.InterceptorType, memberInfo);
            }

            foreach (var implementsInterface in implementsInterfaces)
            {
                foreach (var attr in implementsInterface.GetInterceptorAttributes())
                {
                    yield return new InterceptorMetadata(attr.Order, attr.InterceptorType, memberInfo);
                }
            }

            foreach (var implementsInterface in implementsInterfaces.Select(item => item.DeclaringType))
            {
                foreach (var attr in implementsInterface.GetInterceptorAttributes())
                {
                    yield return new InterceptorMetadata(attr.Order, attr.InterceptorType, memberInfo);
                }
            }
        }
    }
}

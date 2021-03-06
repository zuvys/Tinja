﻿using System.Collections.Generic;
using Tinja.Abstractions.DynamicProxy.Metadatas;

namespace ConsoleApp
{
    public class InterceptorMetadataCollector : IInterceptorMetadataCollector
    {
        public IEnumerable<InterceptorMetadata> Collect(MemberMetadata metadata)
        {
            if (metadata.Member.Name == "GetString")
            {
                yield return new InterceptorMetadata(typeof(UserServiceInterceptor2), metadata.Member, 1024);
            }
        }
    }
}

﻿using System;
using System.Reflection;

namespace Tinja.Abstractions.Injection.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ValuerProviderAttribute : Attribute
    {
        public abstract object GetValue(IServiceResolver serviceResolver, ConstructorInfo constructorInfo, ParameterInfo parameterInfo);
    }
}

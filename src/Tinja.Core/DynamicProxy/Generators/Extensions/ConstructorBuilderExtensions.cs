﻿using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Tinja.Abstractions.DynamicProxy.Registrations;
using Tinja.Abstractions.Extensions;
using Tinja.Core.Injection;

namespace Tinja.Core.DynamicProxy.Generators.Extensions
{
    internal static class ConstructorBuilderExtensions
    {
        internal static ConstructorBuilder DefineParameters(this ConstructorBuilder builder, ParameterInfo[] parameterInfos, int paramterCount)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (parameterInfos == null)
            {
                throw new ArgumentNullException(nameof(parameterInfos));
            }

            if (parameterInfos.Length == 0)
            {
                return builder;
            }

            for (var i = 0; i < parameterInfos.Length; i++)
            {
                var parameter = builder.DefineParameter(i + 1, parameterInfos[i].Attributes, parameterInfos[i].Name);
                if (parameterInfos[i].HasDefaultValue)
                {
                    parameter.SetConstant(parameterInfos[i].DefaultValue);
                }

                parameter.SetCustomAttributes(parameterInfos[i]);
            }

            for (var i = parameterInfos.Length; i < paramterCount; i++)
            {
                builder.DefineParameter(i + 1, ParameterAttributes.None, "parameter" + (i + 1));
            }

            return builder;
        }

        internal static ConstructorBuilder SetCustomAttributes(this ConstructorBuilder builder, ConstructorInfo constructorInfo)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (constructorInfo == null)
            {
                throw new ArgumentNullException(nameof(constructorInfo));
            }

            foreach (var customAttriute in constructorInfo
                .CustomAttributes
                .Where(item => item.AttributeType.IsNotType<InjectAttribute>() && item.AttributeType.IsNotType<InterceptorAttribute>()))
            {
                var attrBuilder = GeneratorUtils.CreateCustomAttribute(customAttriute);
                if (attrBuilder != null)
                {
                    builder.SetCustomAttribute(attrBuilder);
                }
            }

            return builder;
        }
    }
}
